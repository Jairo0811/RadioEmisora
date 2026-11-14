<p align="center">
  <img src="docs/RadioEmisoraRD.png" width="720" alt="Logo de RadioEmisora RD">
</p>

<p align="center">
  Aplicación de escritorio para escuchar emisoras dominicanas mediante streaming en vivo.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/ITLA-SOF--010-0057B8?style=for-the-badge" alt="ITLA SOF-010">
</p>

<p align="center">
  <a href="https://github.com/Jairo0811/RadioEmisora/actions/workflows/ci.yml"><img src="https://github.com/Jairo0811/RadioEmisora/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="https://github.com/Jairo0811/RadioEmisora/releases"><img src="https://img.shields.io/github/v/release/Jairo0811/RadioEmisora?display_name=tag&sort=semver" alt="Última versión"></a>
  <img src="https://img.shields.io/badge/versión-3.1.1-2EA44F" alt="Versión 3.1.1">
  <img src="https://img.shields.io/badge/estado-completado%20·%20mantenimiento-6A35FF" alt="Estado: completado, en mantenimiento">
  <img src="https://img.shields.io/badge/plataforma-Windows-0078D4?logo=windows&logoColor=white" alt="Windows">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10">
  <img src="https://img.shields.io/badge/arquitectura-MVVM-6A35FF" alt="MVVM">
</p>

<p align="center">
  <strong>WPF · C# · .NET 10 · XAML · MVVM · Streaming en vivo · JSON · MSTest · GitHub Actions</strong>
</p>

> ⭐ Modernización profesional del proyecto final de **Diseño Centrado en el Usuario (SOF-010)**, desarrollado originalmente en el ITLA durante el período **2018-C1**.
>
> Proyecto académico independiente desarrollado originalmente como trabajo final del ITLA. No es una aplicación oficial del Instituto Tecnológico de Las Américas (ITLA).

## 🎬 Demostración

<p align="center">
  <img src="docs/media/demo.gif" width="900" alt="Demostración animada de RadioEmisora RD">
</p>

| 🏠 Dashboard | ▶️ Reproductor |
|---|---|
| ![Dashboard](docs/media/dashboard.png) | ![Reproductor](docs/media/reproductor.png) |

| ⭐ Favoritos | 🔎 Búsqueda |
|---|---|
| ![Favoritos](docs/media/favoritos.png) | ![Búsqueda](docs/media/busqueda.png) |

| 🕘 Historial | ℹ️ Acerca de |
|---|---|
| ![Historial](docs/media/historial.png) | ![Acerca de](docs/media/acerca-de.png) |

Las imágenes se generan desde la aplicación WPF real mediante el flujo de integración continua; no son mockups.

## 📖 Descripción

RadioEmisora RD permite explorar, buscar y escuchar un catálogo de emisoras mediante transmisión en línea. 

La versión 3.1 conserva el concepto, la navegación y la identidad visual de la reconstrucción WPF 3.0, y añade la capa final de estabilidad necesaria para presentarla en un portafolio profesional. La versión **3.1.1** es la release final de mantenimiento: instalador para Windows, endurecimiento de seguridad, pulido de accesibilidad y documentación sincronizada.

La aplicación funciona sin cuentas, base de datos ni servicios en la nube. Favoritos, historial, volumen, última emisora y configuración se almacenan localmente en JSON.

## ✨ Funcionalidades

### 📻 Reproducción robusta

- Reproducir, pausar, continuar y detener.
- Cambio rápido de emisora sin reproducciones simultáneas.
- Validación asíncrona del stream antes de abrir `MediaPlayer`.
- Timeout configurable sin bloquear la interfaz.
- Reconexión automática después de fallos temporales o pérdida de Internet.
- Cancelación segura de conexiones anteriores.
- Liberación explícita de eventos, streams, clientes HTTP, timers y `MediaPlayer`.
- Mensaje amigable y opción de reintentar cuando una emisora no está disponible.

### 🚦 Estados visibles

El reproductor refleja siete estados consistentes en el hero, el panel de reproducción y las tarjetas:

| Estado | Significado |
|---|---|
| ⏹️ Detenido | No hay reproducción activa |
| 🔄 Conectando | Se está comprobando el endpoint |
| ⏳ Buffering | El motor prepara el audio |
| ▶️ Reproduciendo | El stream está sonando |
| ⏸️ Pausado | La reproducción está suspendida |
| ♻️ Reconectando | Se intenta recuperar una conexión perdida |
| ❌ Error | El stream falló y puede reintentarse |

### 🗂️ Organización y persistencia

- Búsqueda en tiempo real sin distinción de mayúsculas ni acentos.
- Filtros: Todas, Favoritas, FM, AM y Online.
- Favoritos persistentes.
- Historial de las diez emisoras más recientes.
- Restauración de la última emisora seleccionada.
- Volumen persistente con escritura diferida para evitar acceso excesivo al disco.
- Guardado atómico, copia de respaldo y recuperación automática de JSON corrupto.

### 🌐 Catálogo actualizable

El catálogo base vive en [`catalog/emisoras.json`](catalog/emisoras.json). Al iniciar, la aplicación:

1. Carga siempre el catálogo incluido en la distribución.
2. Prefiere una copia local válida más reciente, si existe.
3. Consulta en segundo plano el JSON remoto alojado en GitHub.
4. Valida versión, tamaño, identificadores, duplicados, logos, colores y que cada stream sea una URL HTTPS pública.
5. Guarda la nueva versión de forma atómica.
6. Mantiene el catálogo local si no hay Internet, ocurre un timeout o el remoto es inválido.

La disponibilidad de un stream depende de su emisora o proveedor. Un fallo individual nunca bloquea el resto de la aplicación.

## 📻 Emisoras incluidas

| Emisora | Frecuencia | Categoría |
|---|---:|---|
| Mortal | 104.9 FM | Urbano / Hip Hop |
| Cima 100 | 100.5 FM | Pop / Baladas |
| Fuego 90 | 90.1 FM | Salsa / Merengue |
| Escándalo 102 | 102.5 FM | Tropical / Variada |
| Disco 106 | 106.1 FM | Pop / Disco |
| Radio Disney | 97.3 FM | Pop / Juvenil |
| Radio Popular | 950 AM | Noticias / Informativa |
| Z 101 | 101.3 FM | Noticias / Opinión |
| Primera FM | 88.1 FM | Variada |
| Alofoke FM | 99.3 FM | Urbano / Entretenimiento |
| Independencia FM | 93.3 FM | Variada |
| La Mega | 97.9 FM | Urbano / Tropical |

## 🧱 Stack tecnológico

### 🖥️ Aplicación de escritorio

<p>
  <img src="https://skillicons.dev/icons?i=cs,dotnet,visualstudio" alt="C#, .NET y Visual Studio" />
</p>

- C#
- .NET 10
- WPF
- XAML
- Visual Studio 2026

### 🏗️ Arquitectura y experiencia de usuario

<p>
  <img src="https://img.shields.io/badge/MVVM-Arquitectura-6A35FF?style=flat-square" alt="MVVM">
  <img src="https://img.shields.io/badge/WPF-Desktop-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="WPF">
  <img src="https://img.shields.io/badge/XAML-UI-0C54C2?style=flat-square" alt="XAML">
</p>

- Arquitectura MVVM.
- Data binding y comandos reutilizables.
- UserControls modulares.
- Temas y estilos compartidos.
- Navegación por teclado y diseño adaptable.

### 📡 Streaming y persistencia

<p>
  <img src="https://img.shields.io/badge/Streaming-HTTPS-1E88E5?style=flat-square" alt="Streaming HTTPS">
  <img src="https://img.shields.io/badge/JSON-Persistencia-000000?style=flat-square&logo=json&logoColor=white" alt="JSON">
</p>

- `System.Windows.Media.MediaPlayer`.
- Validación asíncrona de streams.
- Reconexión automática.
- Catálogo local y remoto.
- Persistencia JSON con respaldo y recuperación.

### 🧪 Calidad e infraestructura

<p>
  <img src="https://skillicons.dev/icons?i=git,github,githubactions" alt="Git, GitHub y GitHub Actions" />
</p>

- MSTest.
- GitHub Actions.
- Compilación con advertencias tratadas como errores.
- NuGet Audit de dependencias.
- Publicación portable, autocontenida e instalador Inno Setup.
- Generación automatizada de capturas y artefactos.

## 🏗️ Arquitectura

La solución mantiene MVVM y separa la vista, el estado de presentación y la infraestructura:

```mermaid
flowchart TD
    V["Views y UserControls"] --> VM["MainViewModel"]
    VM --> P["MediaPlayerService"]
    VM --> C["RadioCatalogService"]
    VM --> J["Config y Favorite services"]
    P --> E["WPF MediaPlayer"]
    C --> L["Catálogo local / remoto"]
    J --> D["JSON atómico + respaldo"]
```

### 📦 Responsabilidades

| Capa | Responsabilidad |
|---|---|
| Models | Emisoras, configuración, catálogo y estados |
| ViewModels | Comandos, navegación, filtros y estado observable |
| Services | Audio, red, catálogo, persistencia, historial y logging |
| Controls | Dashboard, hero, reproductor, sidebar, tarjetas y toast |
| Helpers | Comandos síncronos y asíncronos reutilizables |
| Themes | Colores, tarjetas, botones y foco de teclado |

### 📁 Estructura actual

```text
RadioEmisora/
├── .github/workflows/            # CI y publicación
├── catalog/emisoras.json         # Fuente versionada del catálogo
├── docs/                          # Logo, capturas y documentación
├── packaging/                    # Instalador Inno Setup, firma, versión y pruebas del Setup
├── RadioEmisoraRD/
│   ├── RadioEmisoraRD/            # Aplicación WPF
│   │   ├── Assets/
│   │   ├── Controls/
│   │   ├── Helpers/
│   │   ├── Models/
│   │   ├── Properties/PublishProfiles/
│   │   ├── Services/
│   │   ├── Themes/
│   │   └── ViewModels/
│   ├── RadioEmisoraRD.Tests/      # Suite MSTest
│   └── RadioEmisoraRD.slnx
├── CHANGELOG.md
├── PROJECT_STATUS.md
├── RELEASE_NOTES.md
├── SECURITY.md
├── THIRD_PARTY_NOTICES.md
└── README.md
```

Las versiones Windows Forms de 2018 y el prototipo intermedio permanecen documentados en el historial de Git, pero no forman parte del árbol activo de la versión 3.1.

## 📋 Requisitos

### ▶️ Para ejecutar

- **Plataforma recomendada:** Windows 11 x64.
- **Compatibilidad técnica:** Windows 10 versión 1809 o superior (x64). Windows 10 llegó al fin de su soporte general de Microsoft el 14 de octubre de 2025; para un entorno soportado se recomienda Windows 11.
- Conexión a Internet para escuchar emisoras.
- Instalador y ZIP autocontenido: no requieren instalar .NET.
- ZIP portable dependiente del framework: Desktop Runtime de .NET 10.

### 🛠️ Para desarrollar

- Visual Studio 2026 con la carga de trabajo **Desarrollo de escritorio de .NET** (Visual Studio 2022 no admite oficialmente `net10.0`), o el SDK con cualquier editor.
- SDK de .NET 10.
- Git.

No existen paquetes de producción externos; la aplicación utiliza las bibliotecas de .NET y WPF. Las pruebas usan MSTest 4.3.3.

## 🚀 Instalación

Todas las descargas están en [Releases](https://github.com/Jairo0811/RadioEmisora/releases), junto con `SHA256SUMS.txt` para verificar su integridad.

### 📥 Opción recomendada: instalador

1. Descarga `RadioEmisoraRD-Setup-win-x64.exe`.
2. Ejecútalo y sigue el asistente.

El instalador:

- instala por usuario en `%LOCALAPPDATA%\Programs\RadioEmisora RD`, sin privilegios de administrador;
- no requiere instalar .NET por separado;
- crea el acceso en el menú Inicio y, opcionalmente, en el escritorio;
- registra un desinstalador en **Aplicaciones instaladas**;
- actualiza una instalación anterior sin duplicarla;
- al desinstalar, elimina el programa y conserva favoritos, historial y preferencias en `%APPDATA%\RadioEmisoraRD`.

### 🗜️ Alternativa sin instalación

1. Descarga `RadioEmisoraRD-win-x64.zip` (incluye el runtime de .NET).
2. Extrae el ZIP completo y ejecuta `RadioEmisoraRD.exe`.

### 🎒 Portable dependiente del runtime

1. Instala el [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Descarga `RadioEmisoraRD-portable.zip`, extráelo y ejecuta `RadioEmisoraRD.exe`.
3. Los datos se guardan en la carpeta `Data` junto al programa.

### ✔️ Verificación y SmartScreen

```powershell
Get-FileHash .\RadioEmisoraRD-Setup-win-x64.exe -Algorithm SHA256
```

El resultado debe coincidir con `SHA256SUMS.txt`. Si los paquetes de una release no están firmados con Authenticode, Windows SmartScreen puede mostrar una advertencia; ejecútalos solo si el hash coincide. Detalles en [Distribución para Windows](docs/WINDOWS_DISTRIBUTION.md).

### 💻 Desde el código fuente

```powershell
git clone https://github.com/Jairo0811/RadioEmisora.git
cd RadioEmisora
dotnet restore .\RadioEmisoraRD\RadioEmisoraRD.slnx
dotnet run --project .\RadioEmisoraRD\RadioEmisoraRD\RadioEmisoraRD.csproj
```

## 🧪 Compilación, pruebas y publicación

```powershell
# Compilar sin advertencias
dotnet build .\RadioEmisoraRD\RadioEmisoraRD.slnx -c Release --warnaserror

# Ejecutar toda la suite
dotnet test .\RadioEmisoraRD\RadioEmisoraRD.slnx -c Release

# Publicación portable (requiere .NET 10 Desktop Runtime)
dotnet publish .\RadioEmisoraRD\RadioEmisoraRD\RadioEmisoraRD.csproj `
  -c Release -p:PublishProfile=Portable

# Publicación autocontenida para Windows x64
dotnet publish .\RadioEmisoraRD\RadioEmisoraRD\RadioEmisoraRD.csproj `
  -c Release -p:PublishProfile=SelfContained
```

El flujo de release genera el instalador (Inno Setup), los ZIP portable y autocontenido y `SHA256SUMS.txt`. El instalador se compila con `packaging/Build-Installer.ps1`. No se publica MSIX porque requiere identidad de publicador y certificado de firma.

## ✅ Pruebas

La suite cubre:

- Configuración y normalización de valores.
- Recuperación de JSON corrupto.
- Guardado y deduplicación de favoritos.
- Orden y capacidad del historial.
- Búsqueda y filtros.
- Validación y fallback del catálogo.
- Actualización remota por versión.
- Máquina de estados del reproductor.
- Conexión, reintentos, error, pausa, continuación y cierre.
- Seguridad: destinos locales, privados y link-local, credenciales en URLs, redirecciones inseguras, catálogos enormes o profundamente anidados, IDs, colores y logos inválidos, favoritos e historial malformados y rotación de logs.
- Integridad del catálogo incluido y de sus logos.

GitHub Actions compila en `windows-latest`, trata advertencias como errores, audita dependencias NuGet, ejecuta la suite, publica los builds portable y autocontenido, compila el instalador y lo prueba (instalación, ejecución, actualización, desinstalación y reinstalación), y genera las capturas reales del README.

## 🔐 Datos locales y privacidad

Modo normal:

```text
%APPDATA%\RadioEmisoraRD\
├── config.json            # Preferencias, volumen, última emisora e historial
├── config.json.bak
├── favoritos.json
├── favoritos.json.bak
├── catalogo.json          # Catálogo remoto validado
└── Logs\radioemisorard-AAAAmmdd.log   # Rotación a 2 MB, máximo 10 archivos
```

Para modo portable, coloca `portable.flag` junto al ejecutable o define `RADIOEMISORARD_PORTABLE=1`. Los datos se guardarán en `Data` dentro de la carpeta de la aplicación.

RadioEmisora RD no crea cuentas, no guarda contraseñas, no recopila telemetría ni analytics y no necesita backend. Favoritos e historial nunca salen del equipo. Sus únicas conexiones son la descarga del catálogo y los streams que el usuario reproduce.

## 🛡️ Seguridad

El catálogo remoto y las URLs de streaming se tratan como datos no confiables:

- Solo se aceptan destinos HTTPS públicos; se bloquean loopback, redes privadas, link-local, nombres locales y URLs con credenciales.
- La IP se valida después de resolver DNS (protección frente a DNS rebinding) y cada redirección se comprueba antes de seguirla; un salto `http://` se eleva a `https://`, nunca se abre una conexión HTTP.
- Límites de tamaño y profundidad para el catálogo y los JSON locales; validación estricta de textos, IDs, colores y nombres de logos.
- Auditoría NuGet en cada compilación y workflows con permisos mínimos.

Consulta la [política de seguridad](SECURITY.md) para el modelo de amenazas y cómo reportar vulnerabilidades.

## ⌨️ Atajos de teclado

| Atajo | Acción |
|---|---|
| `Enter` | Reproducir la emisora seleccionada |
| Doble clic | Reproducir una emisora |
| `Espacio` | Reproducir, pausar o continuar fuera de controles editables |
| `Esc` | Detener |
| `Ctrl + F` | Enfocar el buscador |
| `Ctrl + R` | Consultar el catálogo remoto |
| `Ctrl + Q` | Cerrar la aplicación |

## 🧰 Solución de problemas

| Problema | Acción recomendada |
|---|---|
| Una emisora no reproduce | Espera la reconexión automática y pulsa **Reintentar**. El resto del catálogo sigue disponible. |
| Todas las emisoras fallan | Verifica Internet, firewall, proxy corporativo y fecha/hora de Windows. |
| El catálogo no se actualiza | La aplicación usa automáticamente la copia local; prueba `Ctrl + R` más tarde. |
| Favoritos o volumen no se guardan | Comprueba permisos de `%APPDATA%\RadioEmisoraRD` o utiliza modo portable en una carpeta con escritura. |
| JSON corrupto | La aplicación preserva el archivo como `*.corrupt-fecha.json` y recupera el respaldo. |
| SmartScreen bloquea el EXE | Descarga desde Releases, valida SHA-256 y usa **Más información → Ejecutar de todas formas** solo si coincide. |
| Se repite un error | Revisa el log diario en la carpeta `Logs`. |

Consulta la [guía completa de troubleshooting](docs/TROUBLESHOOTING.md) para diagnóstico avanzado.

## 📈 Evolución

| Versión | Tecnología | Alcance |
|---|---|---|
| 2018 | Windows Forms, .NET Framework, ActiveX WMP | Proyecto final original |
| 2.0 | Windows Forms modernizado | Prototipo de recuperación |
| 3.0 | WPF, .NET 10, MVVM | Reconstrucción funcional |
| 3.1 | WPF, MVVM, pruebas, CI y streaming resiliente | Estabilización final para portafolio |
| 3.1.1 | Instalador, hardening, accesibilidad y CI con mínimo privilegio | Release final de mantenimiento |

Consulta el [changelog](CHANGELOG.md) y las [notas de la versión 3.1.1](RELEASE_NOTES.md).

## ✅ Estado del proyecto

**Completado · Portfolio-ready · Solo mantenimiento.** La línea estable es 3.1.x y la release final es **3.1.1**. El alcance funcional está congelado: solo se aceptan correcciones de seguridad, crashes, pérdida de datos, mantenimiento de streams/catálogo, compatibilidad, accesibilidad y documentación. Más detalles en [PROJECT_STATUS.md](PROJECT_STATUS.md).

## 🎓 Información académica

| Información | Detalle |
|---|---|
| 👨‍🎓 Estudiante | Francis Jairo Matías Rosario |
| 🆔 Matrícula | 2015-2984 |
| 📖 Asignatura | Diseño Centrado en el Usuario (SOF-010) |
| 👨‍🏫 Profesor | Juan Martínez López |
| 🏫 Institución | Instituto Tecnológico de Las Américas (ITLA) |
| 📅 Período | 2018-C1 |
| 📁 Tipo | Proyecto final |
| 🛠️ Modernización | 2026 |

## 🧭 Continuidad académica

**RadioEmisora RD** representa el primer capítulo de una continuidad académica desarrollada con el profesor **Juan Martínez López** en el Instituto Tecnológico de Las Américas (ITLA). La relación entre los proyectos es académica y formativa: no comparten dominio funcional ni código, sino que documentan la evolución entre dos asignaturas impartidas por el mismo docente y sus respectivos proyectos finales.

La secuencia comenzó en **2018-C1** con **RadioEmisora**, proyecto final de **Diseño Centrado en el Usuario (SOF-010)**. Su enfoque principal estuvo en la experiencia de usuario, la interacción y el diseño de una aplicación de escritorio orientada al consumo de radio por Internet. La reconstrucción moderna **RadioEmisora RD** conserva ese origen y lo lleva a una implementación WPF contemporánea.

Dos cuatrimestres después, en **2018-C3**, la continuidad avanzó con **GestorAdministrativo**, proyecto final de **Administración de Proyectos de Software (SOF-013)**. El nuevo trabajo amplió la perspectiva desde el diseño de interacción hacia la planificación, organización y gestión integral de procesos de software. Ese proyecto académico fue posteriormente reconstruido y modernizado como [**AdminGest**](https://github.com/Jairo0811/AdminGest).

| Orden | Proyecto actual | Proyecto académico original | Asignatura | Profesor | Período |
|---:|---|---|---|---|---|
| 1 | **RadioEmisora RD** | RadioEmisora | Diseño Centrado en el Usuario (SOF-010) | Juan Martínez López | 2018-C1 |
| 2 | [**AdminGest**](https://github.com/Jairo0811/AdminGest) | GestorAdministrativo | Administración de Proyectos de Software (SOF-013) | Juan Martínez López | 2018-C3 |

Vista como trayectoria académica, esta secuencia muestra una progresión desde el **diseño centrado en las personas y la experiencia de usuario** hacia la **gestión de proyectos y procesos de software**. Las modernizaciones de 2026 preservan esa historia y, al mismo tiempo, incorporan arquitectura, pruebas, seguridad y prácticas de ingeniería actuales para convertir ambos trabajos en proyectos profesionales de portafolio.

## 👨‍💻 Autor

**Francis Jairo Matías Rosario — Jairo Matías**

- 🎓 Tecnólogo en Desarrollo de Software — ITLA.
- 🎓 Estudiante de Ingeniería de Software — UNAPEC.

## 📄 Licencia y avisos de terceros

El código fuente se distribuye bajo la [licencia MIT](LICENSE). La licencia **no** cubre el logo ni el nombre del ITLA, ni los nombres, logos, marcas y transmisiones de las emisoras, grupos de medios o proveedores de streaming, que pertenecen a sus respectivos titulares. Consulta [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

## 📡 Aviso sobre streams

Los nombres, logos y transmisiones pertenecen a sus respectivas emisoras y proveedores. Este repositorio no aloja audio ni garantiza la disponibilidad permanente de endpoints externos. El catálogo puede actualizarse sin distribuir una nueva versión de la aplicación.

