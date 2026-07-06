using System.Windows;

namespace OverlayWidgets.Services;

public interface IWindowOverlayService
{
    void Configure(Window window);
    void ApplyMode(Window window, bool isEditMode);
    bool RegisterEditModeHotkey(Window window);
    void UnregisterEditModeHotkey(Window window);
}
