# OverlayWidgets

Aplicacion nativa para Windows hecha con C# y WPF. Muestra widgets flotantes sobre el escritorio u otras ventanas, con modo edicion para moverlos y persistencia local en JSON.

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
- El boton `Bloquear` cambia a modo normal: la ventana queda arriba y evita capturar clics.
- `Ctrl+Shift+O` alterna entre modo edicion y modo normal aunque el overlay este bloqueado.
- Si otra aplicacion ya usa `Ctrl+Shift+O`, OverlayWidgets sigue funcionando y muestra un aviso en modo edicion.
- Al cerrar, se guarda la posicion y tamano de cada widget en `%LOCALAPPDATA%\OverlayWidgets\settings.json`.

## Archivos locales

- Configuracion: `%LOCALAPPDATA%\OverlayWidgets\settings.json`
- Backups de configuracion corrupta: `%LOCALAPPDATA%\OverlayWidgets\settings.corrupt.yyyymmdd-HHmmss.json`
- Logs: `%LOCALAPPDATA%\OverlayWidgets\logs\yyyy-MM-dd.log`

Si `settings.json` no existe, esta incompleto o no contiene widgets nuevos agregados por la app, se completa con valores por defecto. Si esta corrupto, se crea un backup con timestamp y se restaura una configuracion segura.

## Estructura

- `Views/`: ventanas y controles WPF.
- `ViewModels/`: estado observable, comandos y coordinacion de UI.
- `Services/`: configuracion, overlay, registro de widgets y multimedia.
- `Widgets/`: contrato comun y widgets disponibles.
- `Models/`: modelos de configuracion y datos.

## Widgets incluidos

- `ClockWidget`: hora y fecha actual.
- `MediaWidget`: informacion basica de la sesion multimedia actual de Windows cuando esta disponible.
