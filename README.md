# 📻 RadioEmisora RD

<p align="center">
  <img src="RadioEmisoraRD/RadioEmisoraRD/Assets/RadioEmisoraRD.ico" width="420" alt="Logo de RadioEmisora RD">
</p>

<p align="center">
  <img src="https://img.shields.io/badge/ITLA-2018--C1-0057B8?style=for-the-badge" alt="ITLA 2018-C1">
</p>

<p align="center">
  <strong>Aplicación de escritorio para escuchar emisoras de radio mediante streaming en vivo</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Estado-Finalizado-success?style=for-the-badge" alt="Estado finalizado">
  <img src="https://img.shields.io/badge/Versión-3.0.0-2EA44F?style=for-the-badge" alt="Versión 3.0.0">
  <img src="https://img.shields.io/badge/Plataforma-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white" alt="Windows">
  <img src="https://img.shields.io/badge/Arquitectura-MVVM-6A35FF?style=for-the-badge" alt="Arquitectura MVVM">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10">
</p>

<p align="center">
  <strong>WPF · MVVM · Streaming en vivo · Persistencia JSON</strong>
</p>

> ⭐ Proyecto de modernización de software legado desarrollado originalmente como proyecto final universitario en 2018 y reconstruido completamente en 2026.

---

## 📖 Descripción

**RadioEmisora RD** es una aplicación de escritorio para Windows que permite escuchar emisoras de radio mediante transmisión en línea.

El proyecto nació en **2018** como trabajo final de la asignatura **Diseño Centrado en el Usuario (SOF-010)** del **Instituto Tecnológico de Las Américas (ITLA)**. La versión original fue desarrollada con **Windows Forms**, **.NET Framework** y el control ActiveX de Windows Media Player.

En **2026**, el proyecto fue recuperado y reconstruido desde cero conservando su concepto original. La versión 3.0 utiliza **WPF**, arquitectura **MVVM**, componentes reutilizables, persistencia local mediante JSON y streams reales de emisoras.

El repositorio conserva las distintas etapas del proyecto para documentar su evolución técnica desde una aplicación académica tradicional hasta una solución de escritorio moderna y modular.

---

## 📌 Información académica

| Información | Detalle |
|-------------|---------|
| 👨‍🎓 Estudiante | Francis Jairo Matías Rosario |
| 🆔 Matrícula | 2015-2984 |
| 📖 Asignatura | Diseño Centrado en el Usuario (SOF-010) |
| 👨‍🏫 Profesor | Juan Martínez López |
| 🏫 Institución | Instituto Tecnológico de Las Américas (ITLA) |
| 📅 Período académico | 2018-C1 |
| 🎯 Tipo de proyecto | Proyecto final |
| 🛠️ Modernización | 2026 |

---

## 🚀 Evolución del proyecto

### 📅 Versión original — 2018

- Windows Forms.
- .NET Framework.
- Windows Media Player ActiveX.
- Interfaz clásica de escritorio.
- Lógica concentrada en un único formulario.
- Selección de emisoras mediante estructuras condicionales.

### 📅 RadioEmisora RD 3.0 — 2026

- WPF sobre .NET 10.
- Arquitectura MVVM.
- UserControls reutilizables.
- Dashboard inicial.
- Sidebar interactiva.
- Hero dinámico por emisora.
- Reproductor moderno.
- Persistencia mediante JSON.
- Streams reales.
- Búsqueda, filtros, favoritos e historial.
- Navegación mediante teclado.
- Diseño adaptable a diferentes resoluciones.

---

## ✨ Funcionalidades

### 📻 Reproducción

- Reproducción de emisoras en vivo.
- Controles para reproducir, pausar, continuar y detener.
- Cambio de emisora desde la lista.
- Control de volumen.
- Indicador visual de reproducción.
- Ecualizador visual animado.
- Estado dinámico del stream.

### 🔎 Búsqueda y organización

- Búsqueda en tiempo real por nombre, frecuencia, categoría, provincia o conglomerado.
- Filtros por emisoras:
  - Todas.
  - Favoritas.
  - FM.
  - AM.
  - Online.
- Sistema de favoritos.
- Historial reciente.
- Restauración de la última emisora seleccionada.

### 💾 Persistencia local

La aplicación conserva automáticamente:

- Volumen.
- Emisoras favoritas.
- Historial de reproducción.
- Última emisora seleccionada.
- Configuración local en formato JSON.

### 🎨 Interfaz

- Dashboard de bienvenida.
- Hero dinámico con logo, frecuencia, categoría, ubicación y grupo radial.
- Sidebar moderna.
- Tarjetas de emisoras con estado, logo y favoritos.
- Tema oscuro.
- Toasts informativos.
- Ventana personalizada **Acerca de**.
- Diseño modular mediante UserControls.

---

## ⌨️ Atajos de teclado

| Atajo | Acción |
|-------|--------|
| `Enter` | Reproducir la emisora seleccionada |
| `Doble clic` | Reproducir una emisora |
| `Espacio` | Reproducir, pausar o continuar |
| `Esc` | Detener la reproducción |
| `Ctrl + F` | Enfocar el buscador |
| `Ctrl + R` | Actualizar el catálogo |
| `Ctrl + Q` | Cerrar la aplicación |

---

## 📻 Emisoras incluidas

| Emisora | Frecuencia |
|---------|------------|
| Mortal | 104.9 FM |
| Cima 100 | 100.5 FM |
| Fuego | 90.1 FM |
| Escándalo | 102.5 FM |
| Disco | 106.1 FM |
| Radio Disney | 97.3 FM |
| Radio Popular | 950 AM |
| Z 101 | 101.3 FM |
| Primera FM | 88.1 FM |
| Alofoke FM | 99.3 FM |
| Independencia FM | 93.3 FM |
| La Mega | 97.9 FM |

> Los streams pertenecen a sus respectivas emisoras y proveedores. Su disponibilidad puede variar según el servidor de origen.

---

## 🧰 Stack tecnológico

### 🖥️ Aplicación de escritorio

<p>
  <img src="https://skillicons.dev/icons?i=cs,dotnet" alt="C# y .NET">
</p>

<p>
  <img src="https://img.shields.io/badge/WPF-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="WPF">
  <img src="https://img.shields.io/badge/XAML-0C54C2?style=flat-square&logo=xml&logoColor=white" alt="XAML">
  <img src="https://img.shields.io/badge/MVVM-Arquitectura-6C2DC7?style=flat-square" alt="Arquitectura MVVM">
</p>

- **C#** como lenguaje principal.
- **WPF** sobre .NET 10.
- **XAML** para la composición visual.
- UserControls reutilizables.
- Temas y estilos compartidos.
- Diseño adaptable para Windows.

### ⚙️ Arquitectura y lógica de aplicación

- Arquitectura MVVM.
- Separación mediante Models, ViewModels, Services, Controls y Helpers.
- Comandos reutilizables mediante `RelayCommand`.
- Data binding para sincronizar interfaz y estado.
- Servicios especializados para reproducción, emisoras, favoritos y persistencia.

### 📻 Reproducción y persistencia

- `System.Windows.Media.MediaPlayer` para reproducir streams.
- Gestión centralizada del estado del reproductor.
- JSON para configuración y datos locales.
- Persistencia sin dependencia de una base de datos externa.

### 🧰 Herramientas de desarrollo

<p>
  <img src="https://skillicons.dev/icons?i=visualstudio,git,github" alt="Visual Studio, Git y GitHub">
</p>

- Visual Studio 2022.
- SDK de .NET 10.
- Git para control de versiones.
- GitHub para alojamiento y documentación.

---

## 🏗️ Arquitectura

La versión 3.0 separa responsabilidades mediante MVVM y una estructura modular:

```text
RadioEmisoraRD/
├── Assets/
│   ├── logos/
│   ├── itla.png
│   └── RadioEmisoraRD.ico
│
├── Controls/
│   ├── Dashboard/
│   ├── Hero/
│   ├── Player/
│   ├── Sidebar/
│   ├── StationCard/
│   └── Toast/
│
├── Helpers/
├── Models/
├── Services/
├── Themes/
├── ViewModels/
│
├── AboutWindow.xaml
├── App.xaml
├── MainWindow.xaml
└── RadioEmisoraRD.csproj
```

### Responsabilidades principales

| Componente | Responsabilidad |
|------------|-----------------|
| **Models** | Entidades y configuración de la aplicación |
| **Services** | Reproducción, catálogo, favoritos y persistencia |
| **ViewModels** | Estado de la interfaz, comandos y lógica de presentación |
| **Controls** | Componentes visuales reutilizables |
| **Themes** | Colores, estilos, botones y tarjetas compartidas |
| **Helpers** | Utilidades reutilizables como `RelayCommand` |

---

## 📂 Estructura del repositorio

```text
RadioEmisora/
├── ProyectoFinalDCU.Legacy/
├── RadioEmisora2.0 (Prototipo)/
├── RadioEmisoraRD/
├── .gitignore
└── README.md
```

- **ProyectoFinalDCU.Legacy:** versión original en Windows Forms, preservada como referencia histórica.
- **RadioEmisora2.0 (Prototipo):** etapa intermedia y primer intento de modernización.
- **RadioEmisoraRD:** versión final reconstruida con WPF y MVVM.

---

## 📊 Comparativa

| Característica | Proyecto 2018 | RadioEmisora RD 3.0 |
|----------------|:-------------:|:-------------------:|
| Windows Forms | ✅ | ❌ |
| WPF | ❌ | ✅ |
| MVVM | ❌ | ✅ |
| Arquitectura modular | ❌ | ✅ |
| Streams reales | ⚠️ | ✅ |
| Dashboard | ❌ | ✅ |
| Favoritos | ❌ | ✅ |
| Historial | ❌ | ✅ |
| Persistencia JSON | ❌ | ✅ |
| Búsqueda y filtros | ❌ | ✅ |
| Atajos de teclado | ❌ | ✅ |
| Interfaz adaptable | ❌ | ✅ |

---

## ▶️ Ejecución

### Requisitos

- Windows 10 o superior.
- Visual Studio 2022 o una herramienta compatible con proyectos WPF.
- SDK de .NET 10.
- Conexión a Internet para reproducir los streams.

### Pasos

1. Clonar el repositorio:

```bash
git clone https://github.com/Jairo0811/RadioEmisora.git
```

2. Acceder al proyecto moderno:

```text
RadioEmisoraRD/RadioEmisoraRD/
```

3. Restaurar las dependencias y compilar la solución.

4. Ejecutar desde Visual Studio con `F5` o desde la terminal:

```bash
dotnet run --project RadioEmisoraRD/RadioEmisoraRD/RadioEmisoraRD.csproj
```

---

## 👨‍💻 Autor

**Francis Jairo Matías Rosario**

🎓 Tecnólogo en Desarrollo de Software — ITLA  
🎓 Estudiante de Ingeniería de Software — UNAPEC

---

## 📜 Licencia y uso académico

Este proyecto fue desarrollado originalmente con fines **académicos** para el ITLA.

La modernización realizada en **2026** tiene como objetivo preservar el proyecto, documentar su evolución y demostrar conocimientos en **WPF**, **MVVM**, arquitectura modular y modernización de software legado.

Los nombres, marcas, logotipos y transmisiones de las emisoras pertenecen a sus respectivos propietarios.

---

## 🙌 Agradecimientos

- 🏫 Instituto Tecnológico de Las Américas (ITLA).
- 👨‍🏫 Prof. Juan Martínez López.
- 📻 Emisoras y proveedores de streaming incluidos en el proyecto.

---

> **Proyecto original de 2018 reconstruido en 2026 para conservar su valor histórico y demostrar la evolución técnica desde Windows Forms hasta una aplicación moderna en WPF con arquitectura MVVM.**
