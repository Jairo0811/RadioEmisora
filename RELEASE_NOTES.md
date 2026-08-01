# RadioEmisora RD 3.1.0

RadioEmisora RD 3.1.0 es la versión final de estabilización del proyecto. Mantiene el concepto, la interfaz y el alcance original, y fortalece la reproducción en vivo, la recuperación ante fallos y la persistencia local.

## Puntos destacados

- Reproducción asíncrona con timeout, cancelación y cambio rápido de emisora.
- Reconexión automática ante caídas temporales del stream o de Internet.
- Siete estados visibles del reproductor y mensajes de error recuperables.
- Catálogo remoto versionado con validación y fallback local.
- JSON atómico con copias de respaldo y recuperación de archivos corruptos.
- Logs locales para diagnóstico sin recopilar datos personales.
- Suite automatizada y compilación Release sin advertencias en GitHub Actions.
- Paquetes portable y autocontenido `win-x64`, ambos con verificación SHA-256.

## Descargas

- `RadioEmisoraRD-portable.zip`: requiere el Desktop Runtime de .NET 10 y guarda sus datos dentro de la carpeta del programa.
- `RadioEmisoraRD-win-x64.zip`: incluye el runtime para Windows x64 y no requiere instalar .NET por separado.
- `SHA256SUMS.txt`: sumas para comprobar la integridad de ambas descargas.

MSIX no se distribuye en esta versión porque requiere una identidad y un certificado de firma del publicador. Los paquetes ZIP evitan presentar un instalador sin firma como si fuera confiable.
