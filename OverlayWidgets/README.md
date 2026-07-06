# OverlayWidgets

Aplicacion nativa para Windows hecha con C# y WPF. Muestra widgets flotantes sobre el escritorio u otras ventanas, con modo edicion para moverlos, redimensionarlos y persistir su configuracion local en JSON.

## Requisitos

- Windows 10 19041 o superior.
- .NET 8 Desktop SDK.
- Runtime de escritorio de Windows incluido con el SDK.

## Instalar .NET 8 SDK

Descargar e instalar el .NET 8 SDK desde:

```txt
https://dotnet.microsoft.com/download/dotnet/8.0
```

Validar instalacion:

```powershell
dotnet --info
```

## Ejecutar

```powershell
cd OverlayWidgets
dotnet build
dotnet run
```

## Uso

- La app inicia en modo edicion por defecto.
- En modo edicion los widgets pueden moverse arrastrandolos.
- En modo edicion los widgets pueden redimensionarse desde la esquina inferior derecha `↘`.
- El panel `Widgets` permite activar o desactivar widgets disponibles.
- El boton `BLOQUEAR HUD` cambia a modo normal: la ventana queda arriba y evita capturar clics.
- `Ctrl+Shift+O` alterna entre modo edicion y modo normal aunque el overlay este bloqueado.
- Si otra aplicacion ya usa `Ctrl+Shift+O`, OverlayWidgets sigue funcionando y muestra un aviso en modo edicion.
- Al cerrar, se guarda la posicion, tamano y estado activo/inactivo de cada widget en `%LOCALAPPDATA%\OverlayWidgets\settings.json`.

## Archivos locales

- Configuracion: `%LOCALAPPDATA%\OverlayWidgets\settings.json`
- Backups de configuracion corrupta: `%LOCALAPPDATA%\OverlayWidgets\settings.corrupt.yyyymmdd-HHmmss.json`
- Logs: `%LOCALAPPDATA%\OverlayWidgets\logs\yyyy-MM-dd.log`

Si `settings.json` no existe, esta incompleto o no contiene widgets nuevos agregados por la app, se completa con valores por defecto. Si esta corrupto, se crea un backup con timestamp y se restaura una configuracion segura.

## Capa 4: tema Tactical Mecha HUD

- Rediseño visual del overlay con estetica HUD futurista inspirada en interfaces tacticas, mecha y shooters sci-fi, sin copiar interfaces propietarias.
- Nuevo chrome de widgets con fondo oscuro, bordes neón, acentos cian/magenta/ambar y encabezado tecnico.
- Rediseño del panel de control de modo edicion como consola HUD.
- Rediseño del selector de widgets como panel `LOADOUT` con toggles personalizados.
- Rediseño visual de `ClockWidget` como panel cronometro/telemetria.
- Rediseño visual de `MediaWidget` como modulo de sesion multimedia.
- Mejor integracion visual del handle `↘` de resize dentro del lenguaje HUD.

## Capa 3: resize interactivo

- Cada widget muestra un handle `↘` en la esquina inferior derecha durante el modo edicion.
- El handle permite cambiar ancho y alto arrastrando con el mouse.
- El tamano queda limitado por los minimos definidos en el host del widget.
- El widget se mantiene dentro del area visible del overlay.
- El ancho y alto se guardan en `settings.json` al cerrar la app.

## Capa 2: selector de widgets

- Cada widget disponible se muestra como opcion en el panel `Widgets` durante el modo edicion.
- Al desactivar un widget, desaparece del overlay pero conserva su ultima posicion y tamano conocidos.
- Al reactivarlo, vuelve a aparecer usando su configuracion guardada.
- El estado activo/inactivo se persiste en `settings.json` mediante la propiedad `isEnabled`.

## Blindaje actual del MVP

- Overlay preparado para cubrir el escritorio virtual completo en configuraciones multi-monitor.
- Estilos Win32 del overlay aplicados con una ruta compatible con procesos de 32 y 64 bits.
- Registro defensivo del atajo global `Ctrl+Shift+O`.
- Desregistro del atajo global al cerrar la app.
- Hook Win32 removido durante el cierre de la ventana.
- Logging local tolerante a errores.
- Widget multimedia tolerante a sesiones inexistentes, metadatos incompletos y errores de refresco.
- Proteccion para que los widgets queden dentro del area visible del overlay.

## Validacion automatica

El repositorio incluye un workflow de GitHub Actions en `.github/workflows/dotnet-desktop.yml` que ejecuta:

```powershell
dotnet restore OverlayWidgets/OverlayWidgets.csproj
dotnet build OverlayWidgets/OverlayWidgets.csproj --configuration Release --no-restore
```

El workflow corre en `windows-latest` porque la aplicacion es WPF.

## Estructura

- `Views/`: ventanas y controles WPF.
- `ViewModels/`: estado observable, comandos y coordinacion de UI.
- `Services/`: configuracion, overlay, registro de widgets y multimedia.
- `Widgets/`: contrato comun y widgets disponibles.
- `Models/`: modelos de configuracion y datos.

## Widgets incluidos

- `ClockWidget`: hora y fecha actual.
- `MediaWidget`: informacion basica de la sesion multimedia actual de Windows cuando esta disponible.
