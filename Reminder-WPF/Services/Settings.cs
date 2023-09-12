using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reminder_WPF.Services;

public class Settings : ISettings
{

    public bool HideMainWindowOnStartup { get; set; } = false;
    public bool RunAppOnWindowsStartup { get; set; } = false;

    public async Task Load(string savePath)
    {
        if (File.Exists(savePath))
        {
            using var fs = new FileStream(savePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fs);
            string s =  await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(s))
            {
                Settings? settings = JsonSerializer.Deserialize<Settings>(s);
                if (settings != null)
                {
                    this.RunAppOnWindowsStartup = settings.RunAppOnWindowsStartup;
                    this.HideMainWindowOnStartup = settings.HideMainWindowOnStartup;
                }
            }
        }
    }

    public async Task Save(string savePath)
    {
        if (!File.Exists(savePath))
        {
            FileInfo fi = new FileInfo(savePath);
            Directory.CreateDirectory(fi.Directory!.FullName);
        }

        using var fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new StreamWriter(fs);
        var j = JsonSerializer.Serialize<Settings>(this);
        await writer.WriteAsync(j);
        await writer.FlushAsync();
        writer.Close();
    }
}
