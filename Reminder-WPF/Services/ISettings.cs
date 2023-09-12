using System.Threading.Tasks;

namespace Reminder_WPF.Services;

public interface ISettings
{
    bool HideMainWindowOnStartup { get; set; }
    bool RunAppOnWindowsStartup { get; set; }

    Task Load(string savePath);
    Task Save(string savePath);
}