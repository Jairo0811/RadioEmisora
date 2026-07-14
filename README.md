# 📻 RadioEmisora RD

<p align="center">
  <img src="RadioEmisoraRD/RadioEmisoraRD/Assets/RadioEmisoraRD.ico" width="180" alt="RadioEmisora RD Logo">
</p>

<p align="center">
  <img src="https://skillicons.dev/icons?i=cs,dotnet,visualstudio,git,github" />
</p>

<p align="center">
    <img src="https://img.shields.io/badge/ITLA-2018--C1-0057B8?style=for-the-badge"/>
  <img src="https://img.shields.io/badge/Versión-3.0.0-success?style=for-the-badge"/>
  <img src="https://img.shields.io/badge/Plataforma-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white"/>
  <img src="https://img.shields.io/badge/Arquitectura-MVVM-6A35FF?style=for-the-badge"/>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
</p>

<p align="center">
  <strong>WPF · MVVM · Streaming en vivo · Persistencia JSON</strong>
</p>

> ⭐ Proyecto de modernización de software legado desarrollado originalmente como proyecto final universitario en 2018 y reconstruido completamente en 2026.

---

## 📖 Descripción

**RadioEmisora RD** es una aplicación de escritorio para Windows que permite escuchar emisoras de radio mediante transmisión en línea.

El proyecto nació en **2018** como trabajo final de la asignatura **Diseño Centrado en el Usuario (SOF-010)** del **Instituto Tecnológico de Las Américas (ITLA)**. La versión original fue desarrollada con **Windows Forms**, .NET Framework y el control de Windows Media Player.

En **2026**, el proyecto fue recuperado y reconstruido desde cero conservando su concepto original. La nueva versión utiliza **WPF**, arquitectura **MVVM**, componentes reutilizables, persistencia local mediante JSON y streams reales de emisoras.

---

## 📌 Información del proyecto

| Información | Detalle |
|-------------|---------|
| 👨‍🎓 Estudiante | Francis Jairo Matías Rosario |
| 🆔 Matrícula | 2015-2984 |
| 📖 Asignatura | Diseño Centrado en el Usuario (SOF-010) |
| 👨‍🏫 Profesor | Juan Martínez López |
| 🏫 Institución | Instituto Tecnológico de Las Américas (ITLA) |
| 📅 Período Académico | 2018-C1 |
| 🎯 Tipo de proyecto | Proyecto Final |
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
- Sidebar interactivo.
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
- Pausar, continuar y detener.
- Cambio de emisora desde la lista.
- Control de volumen.
- Indicador visual de reproducción.
- Ecualizador visual animado.
- Estado dinámico del stream.

### 🔎 Organización

- Búsqueda en tiempo real por nombre, frecuencia, categoría, provincia o conglomerado.
- Filtros por:
  - Todas.
  - Favoritas.
  - FM.
  - AM.
  - Online.
- Sistema de favoritos.
- Historial reciente.
- Restauración de la última emisora seleccionada.

### 💾 Persistencia

- Volumen.
- Emisoras favoritas.
- Historial de reproducción.
- Última emisora seleccionada.
- Configuración almacenada localmente en JSON.

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
| `Esc` | Detener reproducción |
| `Ctrl + F` | Enfocar el buscador |
| `Ctrl + R` | Actualizar el catálogo |
| `Ctrl + Q` | Cerrar la aplicación |

---

## 📻 Emisoras incluidas

- Mortal 104.9 FM.
- Cima 100.5 FM.
- Fuego 90.1 FM.
- Escándalo 102.5 FM.
- Disco 106.1 FM.
- Radio Disney 97.3 FM.
- Radio Popular 950 AM.
- Z 101.3 FM.
- Primera 88.1 FM.
- Alofoke 99.3 FM.
- Independencia 93.3 FM.
- La Mega 97.9 FM.

> Los streams pertenecen a sus respectivas emisoras y proveedores. Su disponibilidad puede variar según el servidor de origen.

---

## 🧰 Tecnologías utilizadas

<p align="center">
  <img src="https://skillicons.dev/icons?i=cs,dotnet,visualstudio,git,github" />
</p>

- C#.
- .NET 10.
- WPF.
- MVVM.
- `System.Windows.Media.MediaPlayer`.
- JSON.
- Visual Studio 2022.
- Git.
- GitHub.

---

## 🏗️ Arquitectura

La aplicación separa responsabilidades mediante MVVM y una estructura modular:

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

- **Models:** entidades y configuración de la aplicación.
- **Services:** reproducción, emisoras, favoritos y persistencia.
- **ViewModels:** estado de la interfaz, comandos y lógica de presentación.
- **Controls:** componentes visuales reutilizables.
- **Themes:** estilos, colores, botones y tarjetas compartidas.
- **Helpers:** utilidades como `RelayCommand`.

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
- **RadioEmisora2.0 (Prototipo):** primer intento de modernización.
- **RadioEmisoraRD:** versión final reconstruida en WPF y MVVM.

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
- Visual Studio 2022.
- SDK de .NET 10.
- Conexión a Internet para reproducir los streams.

### Pasos

1. Clonar el repositorio:

```bash
git clone https://github.com/Jairo0811/RadioEmisora.git
```

2. Abrir la solución o el proyecto ubicado en:

```text
RadioEmisoraRD/RadioEmisoraRD/
```

3. Restaurar dependencias y compilar.

4. Ejecutar con:

```text
F5
```

También puede ejecutarse desde la terminal:

```bash
dotnet run --project RadioEmisoraRD/RadioEmisoraRD/RadioEmisoraRD.csproj
```

---

## 👨‍💻 Autor

**Francis Jairo Matías Rosario**

🎓 Tecnólogo en Desarrollo de Software — ITLA  
🎓 Estudiante de Ingeniería de Software — UNAPEC

---

## 📜 Licencia

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
