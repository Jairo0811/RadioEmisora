# Changelog

Todos los cambios relevantes de RadioEmisora RD se documentan en este archivo.

El formato sigue [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el proyecto utiliza versionado semántico.

## [Unreleased]

Proyecto en modo mantenimiento: solo se aceptan correcciones de seguridad, estabilidad, datos, catálogo, compatibilidad, accesibilidad y documentación.

## [3.1.1] - 2026-11-14

Versión de mantenimiento final. Cierra la línea 3.1.x como release de portafolio congelada.

### Seguridad

- Política de destinos de red (`NetworkUriPolicy`) para el catálogo remoto y los streams: solo HTTPS hacia hosts públicos, sin credenciales en la URL, sin fragmentos y con longitud máxima de 2048 caracteres.
- Bloqueo de loopback, redes privadas RFC 1918, link-local, CGNAT, ULA, multicast, rangos de documentación, NAT64/6to4 con IPv4 interna y nombres locales (`localhost`, `.local`, `.internal`, `.lan`, `.home.arpa`, nombres de una sola etiqueta).
- La IP se valida después de resolver DNS y es la misma a la que se conecta el socket, lo que impide eludir la política con DNS rebinding.
- Redirecciones seguidas manualmente (máximo 5) validando cada salto, en lugar de desactivarlas: los streams legítimos que redirigen siguen funcionando.
- Un salto de redirección `http://` se eleva a `https://` antes de validarlo, como hacen los navegadores con el audio de contenido mixto: Mortal, Primera FM e Independencia FM (radiojar) redirigen a HTTP pero sirven el mismo destino por HTTPS. Nunca se abre una conexión HTTP.
- El log describe el destino de una redirección bloqueada (esquema, host y puerto, sin ruta ni tokens).
- Catálogo remoto limitado a 512 KB incluso sin `Content-Length` o con compresión; JSON con profundidad máxima de 16.
- Límites de longitud y rechazo de caracteres de control en catálogo, configuración, favoritos e historial; IDs, nombres de logo (`[a-z0-9-].png`) y colores validados con anclaje estricto (`\z`).
- El reproductor no reintenta destinos bloqueados ni respuestas que no son audio.
- Logs con rotación por tamaño (2 MB), retención de 10 archivos, una línea por evento y sin stack traces para fallos esperados de streams.
- NuGet Audit para dependencias directas y transitivas; vulnerabilidades moderadas, altas y críticas bloquean la compilación.

### Distribución

- Instalador `RadioEmisoraRD-Setup-win-x64.exe` (Inno Setup 6): por usuario, sin privilegios de administrador, menú Inicio, acceso directo opcional, desinstalador, actualización in situ y licencia/avisos de terceros incluidos.
- La desinstalación conserva intencionadamente favoritos, historial y preferencias de `%APPDATA%\RadioEmisoraRD`.
- Fuente única de versión en el `.csproj`, validada contra `app.manifest`, el instalador, el tag y la documentación de la release.
- Prueba de humo automática del instalador: instalación limpia en una ruta con espacios y Unicode, ejecución, actualización, desinstalación y reinstalación.
- `SHA256SUMS.txt` cubre el instalador y ambos ZIP, y se verifica antes de publicar.
- Firma Authenticode opcional: SHA-256, timestamp RFC 3161 con reintentos, verificación posterior, rechazo de certificados autofirmados o vencidos y eliminación del PFX temporal.

### Integración continua

- `actions/checkout@v7`, `actions/setup-dotnet@v6`, `actions/upload-artifact@v7` y `actions/download-artifact@v8`.
- Mínimo privilegio: CI y build de release con `contents: read`; solo el job que crea la GitHub Release usa `contents: write`.
- Smoke test informativo de los streams reales del catálogo.

### Interfaz y accesibilidad

- Icono de audífonos de la barra lateral en cyan (antes casi invisible sobre el fondo oscuro).
- Barras de desplazamiento oscuras con estados hover y arrastre.
- Distinción clara entre emisora seleccionada (anillo cyan) y emisora sonando (borde violeta); el hero indica qué emisora suena realmente y el botón principal pasa a «Cambiar».
- Al cambiar de filtro o búsqueda se conserva la emisora seleccionada si sigue visible; sin selección, el hero ya no muestra «REPRODUCIENDO» junto a «Selecciona FM» e indica qué emisora suena.
- Filtro activo resaltado y anunciado a lectores de pantalla; nombres accesibles por tarjeta, foco de teclado visible, estados anunciados y pasos de volumen de 5 %/10 %.
- «Acerca de»: sin matrícula, con aviso de proyecto académico no oficial, versión leída del ensamblado y contenido desplazable con escalado alto.
- Cifras del dashboard que se recortaban a la mitad con el tamaño por defecto de la ventana.
- ★ de favorita y ▶ de reproducción visibles en las tarjetas (la cuadrícula tenía un ancho fijo mayor que la barra lateral).
- Capturas de portafolio regeneradas y sin bandas vacías del marco de la ventana.

### Assets

- Logos sobredimensionados reducidos sin alterar su diseño (`lamega.png`: 1440 px → 700 px).

### Documentación

- README con el instalador como opción recomendada, Windows 11 como plataforma recomendada y estado final del proyecto.
- Licencia MIT, avisos de terceros (incluido el logo e identidad del ITLA), política de seguridad y estado del proyecto.

## [3.1.0] - 2026-08-01

### Agregado

- Estados explícitos del reproductor: Detenido, Conectando, Buffering, Reproduciendo, Pausado, Reconectando y Error.
- Validación previa de streams, timeout configurable y reconexión automática.
- Actualización remota versionada del catálogo con fallback local transparente.
- Logging diario en la carpeta de datos de la aplicación.
- Manejo global de excepciones recuperables.
- Pruebas automatizadas de servicios, persistencia, favoritos, historial, búsqueda, catálogo y reproductor.
- Flujos de CI y publicación para builds portables y autocontenidos de Windows.
- Generación automatizada de capturas reales y GIF de portafolio.

### Cambiado

- Persistencia JSON atómica con respaldo, normalización y recuperación de corrupción.
- Reproductor desacoplado de WPF para facilitar pruebas y liberar correctamente sus recursos.
- Búsqueda sin distinción de mayúsculas ni acentos.
- Escritura de volumen diferida para reducir operaciones de disco.
- Catálogo movido a JSON mantenible y URLs temporales reemplazadas por endpoints estables.
- Metadatos e identidad de versión actualizados a 3.1.0.

### Corregido

- Suscripciones de timers, eventos de ventana y `MediaPlayer` que no se liberaban.
- Posibles reproducciones simultáneas al cambiar rápidamente de emisora.
- Bloqueos de UI durante la comprobación de streams.
- Handler de doble clic de emisoras que existía, pero no estaba enlazado en XAML.
- Desbordamiento de los botones inferiores en el ancho mínimo de la ventana.
- Interferencia del atajo Espacio con cajas de texto, botones y control de volumen.
- Inconsistencias del README respecto a la estructura actual del repositorio.

## [3.0.0] - 2026

### Agregado

- Reconstrucción de la aplicación con WPF, MVVM, dashboard, favoritos, historial, búsqueda y persistencia JSON.

[Unreleased]: https://github.com/Jairo0811/RadioEmisora/compare/v3.1.1...HEAD
[3.1.1]: https://github.com/Jairo0811/RadioEmisora/compare/v3.1.0...v3.1.1
[3.1.0]: https://github.com/Jairo0811/RadioEmisora/compare/v3.0.0...v3.1.0
