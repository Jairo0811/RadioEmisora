# Arquitectura de RadioEmisora RD

RadioEmisora RD utiliza **WPF + MVVM** sobre .NET 10. La aplicación separa presentación, estado de interfaz, reproducción, catálogo, persistencia y conectividad para mantener una experiencia de escritorio estable aun cuando un stream o el catálogo remoto fallen.

## Vista general

```mermaid
flowchart LR
    User["Usuario"] --> Views["WPF Views / UserControls"]
    Views --> VM["MainViewModel"]

    VM --> Commands["Commands / Navegación / Filtros"]
    VM --> Player["MediaPlayerService"]
    VM --> Catalog["RadioCatalogService"]
    VM --> Favorites["FavoriteService"]
    VM --> History["HistoryService"]
    VM --> Config["ConfigService"]

    Player --> Media["WPF MediaPlayer"]
    Player --> Streams["Streams HTTP / HTTPS"]

    Catalog --> Bundled["catalog/emisoras.json"]
    Catalog --> Remote["Catálogo remoto en GitHub"]
    Catalog --> LocalCatalog["Copia local validada"]

    Favorites --> JSON[("JSON local")]
    History --> JSON
    Config --> JSON
    LocalCatalog --> JSON

    Logs["Logging"] --> Files["Logs locales"]
    Player --> Logs
    Catalog --> Logs
```

La interfaz no conoce detalles de HTTP, archivos o `MediaPlayer`. `MainViewModel` coordina estado y comandos, mientras los servicios encapsulan infraestructura y efectos secundarios.

## Capas y responsabilidades

```mermaid
flowchart TD
    UI["Views / Controls / Themes"] --> Presentation["ViewModels / Commands"]
    Presentation --> Services["Services"]
    Services --> Audio["Audio / MediaPlayer"]
    Services --> Network["HTTP / Catálogo remoto"]
    Services --> Persistence["JSON atómico + backups"]
    Services --> Logging["Logging"]
```

| Área | Responsabilidad |
|---|---|
| Views / Controls | Presentación, layout, accesibilidad y bindings |
| ViewModels | Estado observable, navegación, búsqueda, filtros y comandos |
| Services | Audio, red, catálogo, persistencia, historial y logging |
| Models | Emisoras, configuración, catálogo y estados |
| Helpers | Comandos síncronos/asíncronos y utilidades reutilizables |
| Themes | Colores, estilos, tarjetas, botones y foco de teclado |

## Máquina de estados del reproductor

```mermaid
stateDiagram-v2
    [*] --> Detenido
    Detenido --> Conectando: Reproducir
    Conectando --> Buffering: endpoint válido
    Conectando --> Error: fallo / timeout
    Buffering --> Reproduciendo: MediaOpened
    Buffering --> Error: MediaFailed
    Reproduciendo --> Pausado: Pausar
    Pausado --> Reproduciendo: Continuar
    Reproduciendo --> Reconectando: fallo temporal
    Reconectando --> Reproduciendo: recuperación
    Reconectando --> Error: reintentos agotados
    Error --> Conectando: Reintentar
    Reproduciendo --> Detenido: Detener
    Pausado --> Detenido: Detener
    Error --> Detenido: Limpiar / detener
```

## Cambio de emisora

```mermaid
sequenceDiagram
    participant U as Usuario
    participant VM as MainViewModel
    participant P as MediaPlayerService
    participant H as HTTP
    participant M as WPF MediaPlayer

    U->>VM: selecciona emisora
    VM->>P: PlayAsync(emisora)
    P->>P: cancelar reproducción anterior
    P->>H: validar stream con timeout
    H-->>P: endpoint disponible
    P->>M: abrir stream
    M-->>P: MediaOpened / MediaFailed
    P-->>VM: actualizar estado
    VM-->>U: feedback visual
```

La cancelación de conexiones anteriores evita reproducciones simultáneas y condiciones de carrera durante cambios rápidos de emisora.

## Catálogo actualizable

```mermaid
flowchart LR
    Start["Inicio"] --> Bundled["Catálogo incluido"]
    Bundled --> Local["Copia local válida"]
    Local --> Current["Catálogo activo"]
    Current --> RemoteCheck["Consulta remota en segundo plano"]
    RemoteCheck --> Validate["Validar versión / IDs / URLs / logos"]
    Validate -->|válido y más reciente| Save["Guardado atómico"]
    Validate -->|inválido / offline| Keep["Mantener catálogo actual"]
    Save --> Current
    Keep --> Current
```

## Persistencia local

La aplicación utiliza archivos JSON por usuario para configuración, favoritos, historial y catálogo actualizado. Las escrituras críticas se realizan de forma atómica y mantienen respaldo para recuperación ante corrupción.

```text
%APPDATA%/RadioEmisoraRD/
├── config.json
├── config.json.bak
├── favoritos.json
├── favoritos.json.bak
├── catalogo.json
└── Logs/
```

El modo portable redirige esta persistencia a una carpeta `Data` junto a la aplicación.

## Build y calidad

```mermaid
flowchart LR
    Source["WPF / .NET 10"] --> Build["dotnet build · warnings as errors"]
    Tests["MSTest"] --> CI["GitHub Actions"]
    Build --> CI
    CI --> Publish["Build portable / autocontenido"]
    CI --> Screens["Capturas reales"]
    Publish --> Release["GitHub Releases"]
```

## Criterio de evolución

La arquitectura MVVM actual es suficiente para una aplicación de escritorio de un solo proceso. Nuevas capacidades deben añadirse como servicios y contratos antes de introducir dependencias directamente en los ViewModels o controles.
