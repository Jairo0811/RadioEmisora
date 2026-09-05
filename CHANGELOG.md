# Changelog

Todos los cambios relevantes de RadioEmisora RD se documentan en este archivo.

El formato sigue [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el proyecto utiliza versionado semántico.

## [Unreleased]

### Infraestructura

- Se añadió un instalador profesional para Windows x64 generado con Inno Setup 6.
- El instalador usa instalación por usuario, menú Inicio, acceso directo opcional y desinstalador registrado.
- CI compila y valida `RadioEmisoraRD-Setup-win-x64.exe` junto con los builds existentes.
- El flujo de release publica el instalador además de los ZIP portable y autocontenido.
- Se añadió soporte opcional de firma Authenticode SHA-256 con timestamp y verificación mediante certificado PFX almacenado en GitHub Secrets.
- Los hashes SHA-256 de release incluyen ahora el instalador.

### Documentación

- Se añadió la licencia MIT para el código fuente del proyecto.
- Se documentaron por separado las marcas, logos, streams y demás contenido perteneciente a terceros.
- Se añadió una política de seguridad con alcance de soporte para la línea 3.1.x.
- Se declaró formalmente el proyecto como completado y en modo mantenimiento.
- Se definieron los cambios futuros aceptados y las funcionalidades deliberadamente fuera de alcance.
- Se documentó el proceso de distribución, instalación y firma de Windows.

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

[Unreleased]: https://github.com/Jairo0811/RadioEmisora/compare/v3.1.0...HEAD
[3.1.0]: https://github.com/Jairo0811/RadioEmisora/compare/v3.0.0...v3.1.0