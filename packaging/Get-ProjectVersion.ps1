[CmdletBinding()]
param(
    # Tag de Git (por ejemplo v3.1.1). Si se indica, debe coincidir con la versión del proyecto.
    [string]$Tag = "",

    # Exige que CHANGELOG y RELEASE_NOTES documenten la versión (se usa al publicar).
    [switch]$RequireReleaseDocs
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $root "RadioEmisoraRD/RadioEmisoraRD/RadioEmisoraRD.csproj"
$manifestPath = Join-Path $root "RadioEmisoraRD/RadioEmisoraRD/app.manifest"
$installerPath = Join-Path $root "packaging/RadioEmisoraRD.iss"

[xml]$project = Get-Content -LiteralPath $projectPath -Raw
$version = @($project.Project.PropertyGroup |
    Where-Object { $_.PSObject.Properties.Name -contains "Version" } |
    ForEach-Object { [string]$_.Version })[0]

if ($version -notmatch '^\d+\.\d+\.\d+$') {
    throw "La versión del proyecto no es válida: '$version'."
}

$problems = [System.Collections.Generic.List[string]]::new()

[xml]$manifest = Get-Content -LiteralPath $manifestPath -Raw
$manifestVersion = [string]$manifest.assembly.assemblyIdentity.version
if ($manifestVersion -ne "$version.0") {
    $problems.Add("app.manifest declara $manifestVersion; se esperaba $version.0.")
}

$installer = Get-Content -LiteralPath $installerPath -Raw
$installerMatch = [regex]::Match($installer, '#define\s+MyAppVersion\s+"([^"]+)"')
if (-not $installerMatch.Success -or $installerMatch.Groups[1].Value -ne $version) {
    $problems.Add("RadioEmisoraRD.iss declara '$($installerMatch.Groups[1].Value)'; se esperaba $version.")
}

if (-not [string]::IsNullOrWhiteSpace($Tag) -and $Tag.TrimStart("v") -ne $version) {
    $problems.Add("El tag $Tag no coincide con la versión del proyecto $version.")
}

if ($RequireReleaseDocs) {
    $changelog = Get-Content -LiteralPath (Join-Path $root "CHANGELOG.md") -Raw
    if ($changelog -notmatch "(?m)^## \[$([regex]::Escape($version))\] - \d{4}-\d{2}-\d{2}") {
        $problems.Add("CHANGELOG.md no contiene una sección fechada para $version.")
    }

    $releaseNotesTitle = Get-Content -LiteralPath (Join-Path $root "RELEASE_NOTES.md") -TotalCount 1
    if ($releaseNotesTitle -notmatch [regex]::Escape($version)) {
        $problems.Add("RELEASE_NOTES.md no corresponde a la versión $version.")
    }
}

if ($problems.Count -gt 0) {
    $problems | ForEach-Object { Write-Error $_ -ErrorAction Continue }
    throw "Las versiones del proyecto no son coherentes."
}

Write-Output $version
