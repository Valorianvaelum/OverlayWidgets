using OverlayWidgets.Models;

namespace OverlayWidgets.Services;

public interface ISettingsService
{
    AppSettings Load();
    void Save(AppSettings settings);
}
