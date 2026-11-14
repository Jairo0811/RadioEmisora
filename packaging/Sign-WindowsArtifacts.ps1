[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$CertificatePath,

    # Puede omitirse para PFX sin contraseña. En CI se pasa desde un secreto, nunca desde el repositorio.
    [string]$Password = "",

    [Parameter(Mandatory = $true)]
    [string[]]$Paths,

    # Servidor RFC 3161. HTTP es el transporte estándar de los TSA: la respuesta va firmada.
    [string]$TimestampUrl = "http://timestamp.digicert.com",

    [int]$TimestampAttempts = 3
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if (-not (Test-Path -LiteralPath $CertificatePath -PathType Leaf)) {
    throw "No se encontró el certificado PFX indicado."
}

$missingArtifacts = @($Paths | Where-Object { -not (Test-Path -LiteralPath $_ -PathType Leaf) })
if ($missingArtifacts.Count -gt 0) {
    throw "No se encontraron los artefactos a firmar: $($missingArtifacts -join ', ')"
}

# Comprobaciones previas: el certificado debe servir para firma de código y no ser autofirmado.
# Un certificado autofirmado no aporta confianza real a usuarios externos.
$certificate = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
    $CertificatePath,
    $Password,
    [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::EphemeralKeySet)
try {
    if (-not $certificate.HasPrivateKey) {
        throw "El PFX no contiene la clave privada."
    }
    if ($certificate.Subject -eq $certificate.Issuer) {
        throw "Se rechazó un certificado autofirmado: no es válido para distribuir RadioEmisora RD."
    }
    $codeSigningOid = "1.3.6.1.5.5.7.3.3"
    $ekus = @($certificate.Extensions |
        Where-Object { $_ -is [System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension] } |
        ForEach-Object { $_.EnhancedKeyUsages } |
        ForEach-Object { $_.Value })
    if ($ekus.Count -gt 0 -and $ekus -notcontains $codeSigningOid) {
        throw "El certificado no está autorizado para firma de código."
    }
    if ($certificate.NotAfter -lt (Get-Date)) {
        throw "El certificado de firma está vencido."
    }
    Write-Host "Certificado: $($certificate.Subject) (vence $($certificate.NotAfter.ToString('yyyy-MM-dd')))"
}
finally {
    $certificate.Dispose()
}

$windowsKitsRoot = Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\bin"
$signTool = Get-ChildItem -Path $windowsKitsRoot -Filter signtool.exe -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match '\\x64\\signtool\.exe$' } |
    Sort-Object FullName -Descending |
    Select-Object -First 1

if (-not $signTool) {
    throw "No se encontró signtool.exe en Windows Kits."
}

foreach ($artifact in $Paths) {
    $resolvedArtifact = (Resolve-Path -LiteralPath $artifact).Path
    Write-Host "Firmando $(Split-Path -Leaf $resolvedArtifact)"

    $signArguments = @("sign", "/fd", "SHA256", "/f", $CertificatePath)
    if (-not [string]::IsNullOrEmpty($Password)) {
        $signArguments += @("/p", $Password)
    }
    $signArguments += @("/tr", $TimestampUrl, "/td", "SHA256", $resolvedArtifact)

    $signed = $false
    for ($attempt = 1; $attempt -le $TimestampAttempts -and -not $signed; $attempt++) {
        # signtool no imprime la contraseña; su salida solo se muestra si falla, para diagnosticar.
        $output = & $signTool.FullName @signArguments 2>&1 | Out-String
        if ($LASTEXITCODE -eq 0) {
            $signed = $true
        }
        else {
            Write-Warning "signtool falló (intento $attempt): $($output.Trim())"
            if ($attempt -lt $TimestampAttempts) {
                Start-Sleep -Seconds (5 * $attempt)
            }
        }
    }

    if (-not $signed) {
        throw "signtool falló al firmar $(Split-Path -Leaf $resolvedArtifact)."
    }

    & $signTool.FullName verify /pa /tw $resolvedArtifact | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "La verificación Authenticode falló para $(Split-Path -Leaf $resolvedArtifact)."
    }

    $signature = Get-AuthenticodeSignature -LiteralPath $resolvedArtifact
    if ($signature.Status -ne [System.Management.Automation.SignatureStatus]::Valid) {
        throw "Firma inválida en $(Split-Path -Leaf $resolvedArtifact): $($signature.StatusMessage)"
    }
    if ($null -eq $signature.TimeStamperCertificate) {
        throw "La firma de $(Split-Path -Leaf $resolvedArtifact) no incluye timestamp."
    }
}

Write-Host "Firma Authenticode completada y verificada ($($Paths.Count) archivo(s))."
