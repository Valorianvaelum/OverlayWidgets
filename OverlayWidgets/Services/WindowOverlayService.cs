using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OverlayWidgets.Services;

public sealed class WindowOverlayService : IWindowOverlayService
{
    private const int GwlExStyle = -20;
    private const long WsExTransparent = 0x00000020;
    private const long WsExToolWindow = 0x00000080;
    private const long WsExNoActivate = 0x08000000;
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
        window.WindowStartupLocation = WindowStartupLocation.Manual;

        // The overlay covers the full virtual desktop so widgets remain usable with multi-monitor setups,
        // including monitors placed to the left or above the primary screen.
        window.Left = SystemParameters.VirtualScreenLeft;
        window.Top = SystemParameters.VirtualScreenTop;
        window.Width = SystemParameters.VirtualScreenWidth;
        window.Height = SystemParameters.VirtualScreenHeight;
    }

    public void ApplyMode(Window window, bool isEditMode)
    {
        window.Topmost = true;

        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            _logger.Warning("Could not apply overlay mode because window handle is not available.");
            return;
        }

        var extendedStyle = GetWindowLongPtr(handle, GwlExStyle).ToInt64();
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

        var previousStyle = SetWindowLongPtr(handle, GwlExStyle, new IntPtr(extendedStyle));
        if (previousStyle == IntPtr.Zero)
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
            {
                _logger.Warning($"Could not update overlay extended style. Win32 error: {errorCode}.");
            }
        }
    }

    public bool RegisterEditModeHotkey(Window window)
    {
        if (_isHotkeyRegistered)
        {
            return true;
        }

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
        else
        {
            _logger.Warning("Could not unregister Ctrl+Shift+O hotkey because window handle is not available.");
        }

        _isHotkeyRegistered = false;
    }

    private static IntPtr GetWindowLongPtr(IntPtr handle, int index)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(handle, index)
            : new IntPtr(GetWindowLong32(handle, index));
    }

    private static IntPtr SetWindowLongPtr(IntPtr handle, int index, IntPtr newStyle)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(handle, index, newStyle)
            : new IntPtr(SetWindowLong32(handle, index, newStyle.ToInt32()));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
    private static extern int GetWindowLong32(IntPtr handle, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int SetWindowLong32(IntPtr handle, int index, int newStyle);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr handle, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr handle, int index, IntPtr newStyle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint virtualKey);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hwnd, int id);
}
