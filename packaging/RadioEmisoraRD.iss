#ifndef MyAppVersion
#define MyAppVersion "3.1.0"
#endif

#define MyAppName "RadioEmisora RD"
#define MyAppPublisher "Jairo Matías"
#define MyAppURL "https://github.com/Jairo0811/RadioEmisora"
#define MyAppExeName "RadioEmisoraRD.exe"
#define RepoRoot SourcePath + "..\"
#define PublishDir RepoRoot + "artifacts\self-contained"
#define AppIcon RepoRoot + "RadioEmisoraRD\RadioEmisoraRD\Assets\RadioEmisoraRD.ico"
#define LicenseFile RepoRoot + "LICENSE"
#define ThirdPartyNotices RepoRoot + "THIRD_PARTY_NOTICES.md"

[Setup]
AppId={{5F0CE6D9-7F4A-4A2B-9F3A-5DB9559F62B7}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={localappdata}\Programs\RadioEmisora RD
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
OutputDir={#RepoRoot}artifacts
OutputBaseFilename=RadioEmisoraRD-Setup-win-x64
SetupIconFile={#AppIcon}
UninstallDisplayIcon={app}\{#MyAppExeName}
LicenseFile={#LicenseFile}
InfoBeforeFile={#ThirdPartyNotices}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no
DisableDirPage=auto
DisableReadyPage=no
AllowNoIcons=yes
UsePreviousAppDir=yes
UsePreviousGroup=yes
VersionInfoVersion={#MyAppVersion}.0
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Instalador de {#MyAppName}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoCopyright=Copyright © 2018-2026 Francis Jairo Matías Rosario

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el escritorio"; GroupDescription: "Accesos directos adicionales:"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir {#MyAppName}"; Flags: nowait postinstall skipifsilent
