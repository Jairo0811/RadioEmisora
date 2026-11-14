# RadioEmisora RD 3.1.1

**Final Portfolio Release · Maintenance Mode**

RadioEmisora RD 3.1.1 es la versión de mantenimiento que cierra el proyecto. No añade funcionalidades nuevas: incorpora el instalador para Windows, endurece el manejo de datos externos, pule la interfaz y sincroniza la documentación con el código. A partir de esta versión el proyecto queda congelado funcionalmente.

> Proyecto académico independiente desarrollado originalmente como trabajo final del ITLA. No es una aplicación oficial del Instituto Tecnológico de Las Américas (ITLA).

## Descargas

| Archivo | Recomendado para |
|---|---|
| `RadioEmisoraRD-Setup-win-x64.exe` | **Opción recomendada.** Instalador por usuario: no requiere administrador ni instalar .NET, crea acceso en el menú Inicio, ofrece acceso directo de escritorio opcional y registra un desinstalador. |
| `RadioEmisoraRD-win-x64.zip` | Uso sin instalación. Incluye el runtime de .NET; basta con extraerlo y ejecutar `RadioEmisoraRD.exe`. |
| `RadioEmisoraRD-portable.zip` | Modo portable dependiente del runtime: requiere .NET 10 Desktop Runtime y guarda sus datos en la carpeta `Data` del programa. |
| `SHA256SUMS.txt` | Hashes SHA-256 de los tres paquetes. |

Para verificar una descarga en PowerShell:

```powershell
Get-FileHash .\RadioEmisoraRD-Setup-win-x64.exe -Algorithm SHA256
```

Compara el resultado con la línea correspondiente de `SHA256SUMS.txt`.

**Plataforma recomendada:** Windows 11 x64. La aplicación sigue siendo técnicamente compatible con Windows 10 versión 1809 o posterior, pero Windows 10 llegó al fin de su soporte general de Microsoft el 14 de octubre de 2025.

## Novedades

### Instalador y distribución

- Nuevo instalador Windows x64 generado con Inno Setup 6, probado automáticamente en CI: instalación limpia, ejecución, actualización sobre una instalación anterior, desinstalación y reinstalación.
- Al desinstalar se conservan favoritos, historial y preferencias en `%APPDATA%\RadioEmisoraRD`, por si reinstalas más adelante.
- Versión única (3.1.1) validada entre proyecto, manifiesto, instalador, tag y documentación.
- Hashes SHA-256 de los tres paquetes, verificados antes de publicar la release.

### Seguridad y privacidad

- El catálogo remoto y los streams solo pueden apuntar a destinos HTTPS públicos: se bloquean direcciones locales, privadas y link-local, URLs con credenciales y redirecciones hacia la red interna, incluso si el DNS cambia entre solicitudes. Si una emisora redirige a HTTP, la aplicación usa el mismo destino por HTTPS y nunca abre conexiones sin cifrar.
- Límites de tamaño y profundidad para el catálogo y los archivos JSON locales; validación estricta de identificadores, logos, colores y textos.
- Logs locales con rotación y retención limitada, sin datos personales ni volcados innecesarios.
- Auditoría de dependencias NuGet en cada compilación.
- Sin cambios en privacidad: no hay cuentas, contraseñas, telemetría, analytics ni backend. Favoritos e historial nunca salen del equipo.

### Interfaz y accesibilidad

- Mayor contraste del icono de la barra lateral y barras de desplazamiento integradas en el tema oscuro.
- Queda claro qué emisora está seleccionada y cuál está sonando cuando no coinciden.
- Las cifras del dashboard ya no se recortan y el ★ de favorita y el ▶ de reproducción vuelven a verse en las tarjetas.
- Mejoras para teclado y lectores de pantalla: foco visible, nombres accesibles, filtro activo anunciado y ajuste fino del volumen.
- La ventana «Acerca de» incluye el aviso de proyecto académico no oficial y ya no muestra datos de matrícula.

### Integración continua

- Acciones de GitHub actualizadas y permisos mínimos: solo el paso que crea la release puede escribir en el repositorio.

## Firma digital y SmartScreen

El flujo de release admite firma Authenticode (SHA-256 con timestamp) cuando se configura un certificado de firma de código de una autoridad reconocida. Si los paquetes de esta release no están firmados, Windows SmartScreen puede mostrar una advertencia al ejecutarlos. Descarga solo desde la página oficial de Releases de este repositorio, comprueba el hash SHA-256 y, si coincide, usa **Más información → Ejecutar de todas formas**.

## Problema conocido

En la verificación de la release, 11 de las 12 emisoras del catálogo respondieron con audio. El endpoint del proveedor de **Radio Popular 950 AM** devuelve `503 Service Unavailable`; no se encontró un endpoint oficial alternativo verificable, por lo que la emisora se mantiene en el catálogo y la aplicación muestra su estado normal de «no disponible / reintentar». El catálogo puede corregirse más adelante sin publicar una nueva versión de la aplicación.

## Alcance

Los nombres, logos y transmisiones de las emisoras pertenecen a sus respectivos titulares y no están cubiertos por la licencia MIT del código. Consulta `THIRD_PARTY_NOTICES.md`.

Historial completo de cambios: [CHANGELOG.md](https://github.com/Jairo0811/RadioEmisora/blob/master/CHANGELOG.md).
