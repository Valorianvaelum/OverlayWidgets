using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OverlayWidgets.Services;

public sealed class WindowOverlayService : IWindowOverlayService
{
    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x00000020;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;
    private const int EditModeHotkeyId = 9001;
    private const uint ModShift = 0x0004;
    private const uint ModControl = 0x0002;
    private const uint KeyO = 0x4F;
    private readonly ILoggerService _logger;
    private bool _isHotkeyRegistered;

    public WindowOverlayService(ILoggerService logger)
    {
        _logger = logger;
    }

    public void Configure(Window window)
    {
        window.WindowStyle = WindowStyle.None;
        window.AllowsTransparency = true;
        window.Background = System.Windows.Media.Brushes.Transparent;
        window.Topmost = true;
        window.ShowInTaskbar = true;
        window.ResizeMode = ResizeMode.NoResize;
        window.Left = 0;
        window.Top = 0;
        window.Width = SystemParameters.PrimaryScreenWidth;
        window.Height = SystemParameters.PrimaryScreenHeight;
    }

    public void ApplyMode(Window window, bool isEditMode)
    {
        window.Topmost = true;

        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        var extendedStyle = GetWindowLong(handle, GwlExStyle);
        extendedStyle |= WsExToolWindow;

        if (isEditMode)
        {
            extendedStyle &= ~WsExTransparent;
            extendedStyle &= ~WsExNoActivate;
        }
        else
        {
            extendedStyle |= WsExTransparent | WsExNoActivate;
        }

        SetWindowLong(handle, GwlExStyle, extendedStyle);
    }

    public bool RegisterEditModeHotkey(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            _logger.Warning("Could not register Ctrl+Shift+O hotkey because window handle is not available.");
            return false;
        }

        _isHotkeyRegistered = RegisterHotKey(handle, EditModeHotkeyId, ModControl | ModShift, KeyO);
        if (!_isHotkeyRegistered)
        {
            var errorCode = Marshal.GetLastWin32Error();
            _logger.Warning($"Could not register Ctrl+Shift+O hotkey. Win32 error: {errorCode}.");
            return false;
        }

        _logger.Info("Ctrl+Shift+O hotkey registered.");
        return true;
    }

    public void UnregisterEditModeHotkey(Window window)
    {
        if (!_isHotkeyRegistered)
        {
            return;
        }

        var handle = new WindowInteropHelper(window).Handle;
        if (handle != IntPtr.Zero)
        {
            if (!UnregisterHotKey(handle, EditModeHotkeyId))
            {
                var errorCode = Marshal.GetLastWin32Error();
                _logger.Warning($"Could not unregister Ctrl+Shift+O hotkey. Win32 error: {errorCode}.");
            }
            else
            {
                _logger.Info("Ctrl+Shift+O hotkey unregistered.");
            }
        }

        _isHotkeyRegistered = false;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint virtualKey);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hwnd, int id);
}
