# Troubleshooting de RadioEmisora RD

Esta guía cubre los problemas más comunes de la versión 3.1.0.

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
- Revisa `CatalogUrl` en `config.json`; debe ser una URL HTTPS.
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

## SmartScreen muestra una advertencia

Los ZIP publicados no están firmados con un certificado comercial. Descarga únicamente desde la página Releases del repositorio y compara:

```powershell
Get-FileHash .\RadioEmisoraRD-win-x64.zip -Algorithm SHA256
```

El resultado debe coincidir con `SHA256SUMS.txt`.

## Diagnóstico para desarrolladores

```powershell
dotnet --info
dotnet restore .\RadioEmisoraRD\RadioEmisoraRD.slnx
dotnet build .\RadioEmisoraRD\RadioEmisoraRD.slnx -c Release --warnaserror
dotnet test .\RadioEmisoraRD\RadioEmisoraRD.slnx -c Release --logger trx
```

Requisitos de compilación:

- SDK de .NET 10.
- Windows 10 1809 o superior.
- Visual Studio con desarrollo de escritorio .NET cuando se use la interfaz gráfica.

## Reportar un problema

Incluye:

- Versión de RadioEmisora RD.
- Versión de Windows.
- Emisora afectada.
- Pasos para reproducir.
- Fragmento mínimo relevante del log, eliminando cualquier ruta o dato que prefieras mantener privado.

No publiques el archivo de configuración completo si contiene una URL de catálogo privada personalizada.
