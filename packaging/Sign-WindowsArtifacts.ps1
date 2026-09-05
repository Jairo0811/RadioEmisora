[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$CertificatePath,

    [Parameter(Mandatory = $true)]
    [string]$Password,

    [Parameter(Mandatory = $true)]
    [string[]]$Paths,

    [string]$TimestampUrl = "http://timestamp.digicert.com"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if (-not (Test-Path -LiteralPath $CertificatePath -PathType Leaf)) {
    throw "No se encontró el certificado PFX: $CertificatePath"
}

$missingArtifacts = @($Paths | Where-Object { -not (Test-Path -LiteralPath $_ -PathType Leaf) })
if ($missingArtifacts.Count -gt 0) {
    throw "No se encontraron los artefactos a firmar: $($missingArtifacts -join ', ')"
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
    Write-Host "Firmando $resolvedArtifact"

    & $signTool.FullName sign `
        /fd SHA256 `
        /f $CertificatePath `
        /p $Password `
        /tr $TimestampUrl `
        /td SHA256 `
        $resolvedArtifact

    if ($LASTEXITCODE -ne 0) {
        throw "signtool falló al firmar $resolvedArtifact"
    }

    & $signTool.FullName verify /pa /v $resolvedArtifact
    if ($LASTEXITCODE -ne 0) {
        throw "La verificación Authenticode falló para $resolvedArtifact"
    }
}

Write-Host "Firma Authenticode completada y verificada."
