# Distribución profesional para Windows

RadioEmisora RD dispone de tres formatos de distribución para Windows x64:

| Artefacto | Uso recomendado |
|---|---|
| `RadioEmisoraRD-Setup-win-x64.exe` | Instalación normal para usuarios finales |
| `RadioEmisoraRD-win-x64.zip` | Ejecución autocontenida sin instalador |
| `RadioEmisoraRD-portable.zip` | Uso portable; requiere .NET Desktop Runtime 10 |

## Instalador

El instalador se genera con Inno Setup 6 a partir del build autocontenido de Windows.

Características:

- instalación por usuario en `%LOCALAPPDATA%\Programs\RadioEmisora RD`;
- no requiere privilegios de administrador;
- entrada en el menú Inicio;
- acceso directo de escritorio opcional;
- desinstalador registrado en Windows;
- icono y metadatos del producto;
- licencia mostrada durante la instalación;
- aviso de contenido y servicios de terceros;
- compresión LZMA2;
- comprobación automática del instalador en CI;
- hash SHA-256 publicado junto a cada release.

La definición se encuentra en [`packaging/RadioEmisoraRD.iss`](../packaging/RadioEmisoraRD.iss).

## Firma Authenticode

El flujo de release admite firma Authenticode de:

1. `artifacts/portable/RadioEmisoraRD.exe`;
2. `artifacts/self-contained/RadioEmisoraRD.exe`;
3. `artifacts/RadioEmisoraRD-Setup-win-x64.exe`.

La firma utiliza SHA-256, timestamp RFC 3161 y `signtool.exe`. Después de firmar, cada artefacto se verifica con la política Authenticode de Windows antes de continuar la publicación.

### GitHub Secrets requeridos

La firma se activa únicamente cuando el repositorio tiene configurados estos secretos:

- `WINDOWS_SIGNING_CERT_BASE64`: contenido Base64 de un certificado de firma de código en formato PFX/P12;
- `WINDOWS_SIGNING_CERT_PASSWORD`: contraseña del certificado PFX/P12.

Ejemplo local para convertir un PFX a Base64 en PowerShell:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("codesigning.pfx")) | Set-Clipboard
```

El certificado privado **nunca debe añadirse al repositorio**. El workflow lo reconstruye temporalmente en el runner, firma los artefactos y lo elimina al finalizar.

## Certificado recomendado

Para que Windows pueda establecer confianza real en el publicador, el certificado debe proceder de una autoridad certificadora de confianza y estar autorizado para firma de código.

No se genera ni se publica un certificado autofirmado porque una firma autofirmada no elimina de forma fiable las advertencias de SmartScreen para usuarios externos y puede dar una falsa impresión de confianza.

## Comportamiento sin certificado

Si los secretos de firma no están configurados, el workflow continúa generando correctamente el instalador, los ZIP y los hashes SHA-256, pero muestra explícitamente que los binarios se publican sin firma Authenticode.

Esto permite mantener builds reproducibles de portafolio sin almacenar material criptográfico privado en GitHub.

## Compilar el instalador localmente

Primero genera el build autocontenido:

```powershell
dotnet publish .\RadioEmisoraRD\RadioEmisoraRD\RadioEmisoraRD.csproj `
  -c Release -p:PublishProfile=SelfContained -o .\artifacts\self-contained
```

Con Inno Setup 6 instalado:

```powershell
& "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe" .\packaging\RadioEmisoraRD.iss
```

El resultado será:

```text
artifacts\RadioEmisoraRD-Setup-win-x64.exe
```

## Firmar manualmente

Con un certificado PFX de confianza:

```powershell
.\packaging\Sign-WindowsArtifacts.ps1 `
  -CertificatePath .\codesigning.pfx `
  -Password "<contraseña>" `
  -Paths @(
    ".\artifacts\self-contained\RadioEmisoraRD.exe",
    ".\artifacts\RadioEmisoraRD-Setup-win-x64.exe"
  )
```

No guardes la contraseña ni el PFX en archivos versionados.
