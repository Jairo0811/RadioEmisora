# Troubleshooting de RadioEmisora RD

Esta guía cubre los problemas más comunes de la versión 3.1.1.

## Ubicación de datos y logs

En modo normal:

```text
%APPDATA%\RadioEmisoraRD
```

En modo portable:

```text
<carpeta-del-programa>\Data
```

El log diario está en `Logs\radioemisorard-AAAAMMDD.log`. No contiene contraseñas ni historial de navegación; registra transiciones relevantes y detalles técnicos de excepciones.

## Una emisora no reproduce

1. Espera a que finalicen los intentos de reconexión.
2. Selecciona otra emisora para confirmar que la aplicación mantiene conectividad.
3. Regresa a la emisora y pulsa **Reintentar**.
4. Comprueba la misma estación en su sitio oficial.
5. Revisa el log para distinguir timeout, respuesta HTTP inválida o formato no compatible.

Los servidores de radio pueden cambiar, limitar conexiones geográficas o permanecer temporalmente fuera de servicio.

## Ninguna emisora reproduce

1. Confirma que Windows tiene Internet.
2. Desactiva temporalmente VPN o proxy para descartar una política de red.
3. Permite `RadioEmisoraRD.exe` en el firewall o antivirus.
4. Verifica que la fecha, hora y zona horaria sean correctas; TLS puede fallar con un reloj incorrecto.
5. Reinicia la aplicación.

## El catálogo remoto no se actualiza

La aplicación nunca depende del servidor remoto para iniciar. Si la consulta falla, conserva la última copia válida.

- Usa `Ctrl + R` para reintentar.
- Comprueba acceso a `raw.githubusercontent.com`.
- Revisa `CatalogUrl` en `config.json`; debe ser una URL HTTPS pública. Las direcciones locales, privadas o link-local y las URLs con credenciales se rechazan.
- Si la configuración contiene una URL inválida, RadioEmisora RD restaura automáticamente la URL oficial.

## Configuración o favoritos dañados

La persistencia utiliza escrituras temporales y respaldos `*.bak`. Si un JSON no se puede deserializar:

1. Se conserva como `nombre.corrupt-AAAAMMDDHHmmssfff.json`.
2. Se intenta recuperar `nombre.json.bak`.
3. Si tampoco es válido, se crea una configuración segura por defecto.

Para restablecer manualmente, cierra la aplicación y mueve fuera de la carpeta `config.json` o `favoritos.json`. No elimines archivos si necesitas analizarlos o recuperar información.

## No se pueden guardar datos

- Confirma que el usuario tenga escritura en `%APPDATA%\RadioEmisoraRD`.
- Si ejecutas desde una memoria USB o carpeta protegida, evita `Program Files` para modo portable.
- Comprueba espacio libre y protección contra ransomware de Windows Defender.
- Busca `UnauthorizedAccessException` o `IOException` en el log.

## Radio Popular 950 AM no reproduce

Desde noviembre de 2026 el endpoint del proveedor de Radio Popular responde `503 Service Unavailable`. Es un problema externo: la aplicación muestra el error y permite reintentar; el resto del catálogo no se ve afectado.

## SmartScreen muestra una advertencia

Si el instalador o los ZIP de una release no están firmados con Authenticode (no hay un certificado de firma de código configurado), SmartScreen puede advertir al ejecutarlos. Descarga únicamente desde la página Releases del repositorio y compara:

```powershell
Get-FileHash .\RadioEmisoraRD-Setup-win-x64.exe -Algorithm SHA256
```

El resultado debe coincidir con `SHA256SUMS.txt`. Si coincide, usa **Más información → Ejecutar de todas formas**.

## Desinstalar o reinstalar

Desinstala desde **Configuración → Aplicaciones → Aplicaciones instaladas → RadioEmisora RD**. La desinstalación conserva `%APPDATA%\RadioEmisoraRD` (favoritos, historial, preferencias y logs) para que una reinstalación los recupere. Para empezar de cero, elimina esa carpeta manualmente después de desinstalar.

## Diagnóstico para desarrolladores

```powershell
dotnet --info
dotnet restore .\RadioEmisoraRD\RadioEmisoraRD.slnx
dotnet build .\RadioEmisoraRD\RadioEmisoraRD.slnx -c Release --warnaserror
dotnet test .\RadioEmisoraRD\RadioEmisoraRD.slnx -c Release --logger trx
```

Requisitos de compilación:

- SDK de .NET 10.
- Windows 11 x64 recomendado (Windows 10 1809 o superior sigue siendo técnicamente compatible).
- Visual Studio 2026 con desarrollo de escritorio .NET cuando se use el IDE (Visual Studio 2022 no admite oficialmente `net10.0`).

## Reportar un problema

Incluye:

- Versión de RadioEmisora RD.
- Versión de Windows.
- Emisora afectada.
- Pasos para reproducir.
- Fragmento mínimo relevante del log, eliminando cualquier ruta o dato que prefieras mantener privado.

No publiques el archivo de configuración completo si contiene una URL de catálogo privada personalizada.
