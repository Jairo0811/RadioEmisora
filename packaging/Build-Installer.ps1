[CmdletBinding()]
param(
    # Versión a estampar en el instalador. Por defecto se lee del proyecto.
    [string]$Version = ""
)

# Compila RadioEmisoraRD-Setup-win-x64.exe con Inno Setup 6 a partir de
# artifacts/self-contained (generado previamente con el perfil SelfContained).

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = & (Join-Path $PSScriptRoot "Get-ProjectVersion.ps1")
}

$publishDir = Join-Path $root "artifacts/self-contained"
if (-not (Test-Path -LiteralPath (Join-Path $publishDir "RadioEmisoraRD.exe") -PathType Leaf)) {
    throw "Falta artifacts/self-contained/RadioEmisoraRD.exe. Ejecuta antes la publicación SelfContained."
}

# Inno Setup muestra correctamente UTF-8 solo con BOM: se generan copias para el asistente
# y para la carpeta instalada sin modificar los archivos del repositorio.
$docsDir = Join-Path $root "artifacts/installer-docs"
New-Item -ItemType Directory -Path $docsDir -Force | Out-Null
$utf8WithBom = [System.Text.UTF8Encoding]::new($true)
foreach ($document in @(
        @{ Source = "LICENSE"; Target = "LICENSE.txt" },
        @{ Source = "THIRD_PARTY_NOTICES.md"; Target = "THIRD_PARTY_NOTICES.txt" })) {
    $text = [System.IO.File]::ReadAllText((Join-Path $root $document.Source))
    [System.IO.File]::WriteAllText((Join-Path $docsDir $document.Target), $text, $utf8WithBom)
}

$iscc = Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"
if (-not (Test-Path -LiteralPath $iscc -PathType Leaf)) {
    throw "No se encontró Inno Setup 6 en '$iscc'."
}

& $iscc "/DMyAppVersion=$Version" (Join-Path $root "packaging/RadioEmisoraRD.iss")
if ($LASTEXITCODE -ne 0) {
    throw "La compilación del instalador falló (código $LASTEXITCODE)."
}

$setup = Join-Path $root "artifacts/RadioEmisoraRD-Setup-win-x64.exe"
if (-not (Test-Path -LiteralPath $setup -PathType Leaf)) {
    throw "El instalador esperado no fue generado."
}

$setupVersion = (Get-Item -LiteralPath $setup).VersionInfo.ProductVersion
if ($setupVersion -ne $Version) {
    throw "El instalador declara la versión '$setupVersion'; se esperaba $Version."
}

Write-Host "Instalador generado: $setup ($Version)"
