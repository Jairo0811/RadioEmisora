[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$InstallerPath,

    [Parameter(Mandatory = $true)]
    [string]$ExpectedVersion
)

# Prueba de humo del instalador en un runner limpio:
# instalación silenciosa en una ruta con espacios y Unicode, ejecución de la app instalada,
# actualización sobre la instalación existente, desinstalación (conservando %APPDATA%)
# y reinstalación.

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$appId = "{5F0CE6D9-7F4A-4A2B-9F3A-5DB9559F62B7}_is1"
$uninstallKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\$appId"
$installDir = Join-Path $env:RUNNER_TEMP "Ruta con espacios ñ\RadioEmisora RD"
$exePath = Join-Path $installDir "RadioEmisoraRD.exe"
$userDataDir = Join-Path $env:APPDATA "RadioEmisoraRD"
$userMarker = Join-Path $userDataDir "favoritos.json"
$installer = (Resolve-Path -LiteralPath $InstallerPath).Path

function Invoke-Installer {
    param([string]$LogName)

    $log = Join-Path $env:RUNNER_TEMP $LogName
    $process = Start-Process -FilePath $installer -Wait -PassThru -ArgumentList @(
        "/VERYSILENT", "/SUPPRESSMSGBOXES", "/NORESTART", "/SP-",
        "/DIR=`"$installDir`"", "/LOG=`"$log`""
    )
    if ($process.ExitCode -ne 0) {
        Get-Content -LiteralPath $log -ErrorAction SilentlyContinue | Select-Object -Last 40
        throw "El instalador terminó con código $($process.ExitCode)."
    }
}

function Assert-Installed {
    if (-not (Test-Path -LiteralPath $exePath -PathType Leaf)) {
        throw "No se encontró la aplicación instalada en '$exePath'."
    }

    $fileVersion = (Get-Item -LiteralPath $exePath).VersionInfo.ProductVersion
    if (-not $fileVersion.StartsWith($ExpectedVersion)) {
        throw "El ejecutable instalado informa la versión '$fileVersion'; se esperaba $ExpectedVersion."
    }

    $entry = Get-ItemProperty -LiteralPath $uninstallKey -ErrorAction Stop
    if ($entry.DisplayVersion -ne $ExpectedVersion) {
        throw "Apps instaladas muestra '$($entry.DisplayVersion)'; se esperaba $ExpectedVersion."
    }
    if ($entry.Publisher -ne "Jairo Matías") {
        throw "Publisher inesperado: '$($entry.Publisher)'."
    }

    $startMenu = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs\RadioEmisora RD\RadioEmisora RD.lnk"
    if (-not (Test-Path -LiteralPath $startMenu)) {
        throw "No se creó el acceso directo del menú Inicio."
    }
}

function Test-InstalledApplication {
    $captureDir = Join-Path $env:RUNNER_TEMP "installed-app-check"
    Remove-Item -LiteralPath $captureDir -Recurse -Force -ErrorAction SilentlyContinue
    $process = Start-Process -FilePath $exePath -PassThru -ArgumentList @("--capture-docs", "`"$captureDir`"")
    if (-not $process.WaitForExit(120000)) {
        $process.Kill()
        throw "La aplicación instalada no terminó a tiempo."
    }
    if ($process.ExitCode -ne 0 -or -not (Test-Path -LiteralPath (Join-Path $captureDir "dashboard.png"))) {
        throw "La aplicación instalada no se ejecutó correctamente (código $($process.ExitCode))."
    }
}

function Invoke-Uninstaller {
    $uninstaller = Join-Path $installDir "unins000.exe"
    $process = Start-Process -FilePath $uninstaller -Wait -PassThru -ArgumentList @(
        "/VERYSILENT", "/SUPPRESSMSGBOXES", "/NORESTART"
    )
    if ($process.ExitCode -ne 0) {
        throw "El desinstalador terminó con código $($process.ExitCode)."
    }

    # El desinstalador de Inno Setup se relanza desde %TEMP%: se espera a que termine.
    $deadline = (Get-Date).AddSeconds(90)
    while ((Test-Path -LiteralPath $exePath) -or (Test-Path -LiteralPath $uninstallKey)) {
        if ((Get-Date) -gt $deadline) {
            throw "La desinstalación no eliminó la aplicación o su registro."
        }
        Start-Sleep -Milliseconds 500
    }
}

Write-Host "1/5 Instalación limpia en '$installDir'"
Invoke-Installer -LogName "setup-clean.log"
Assert-Installed

Write-Host "2/5 Ejecución de la aplicación instalada"
Test-InstalledApplication

Write-Host "3/5 Actualización sobre la instalación existente"
New-Item -ItemType Directory -Path $userDataDir -Force | Out-Null
'["mortal-1049"]' | Set-Content -LiteralPath $userMarker -Encoding utf8
Invoke-Installer -LogName "setup-upgrade.log"
Assert-Installed
$entries = @(Get-ChildItem "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall" |
    Where-Object { $_.PSChildName -eq $appId })
if ($entries.Count -ne 1) {
    throw "Se esperaba una sola entrada de desinstalación y hay $($entries.Count)."
}

Write-Host "4/5 Desinstalación"
Invoke-Uninstaller
if (-not (Test-Path -LiteralPath $userMarker)) {
    throw "La desinstalación eliminó los datos del usuario en %APPDATA%."
}

Write-Host "5/5 Reinstalación"
Invoke-Installer -LogName "setup-reinstall.log"
Assert-Installed
Invoke-Uninstaller

Write-Host "Instalador verificado: instalación, ejecución, actualización, desinstalación y reinstalación."
