namespace Reminder_WPF.Services
{
    public interface IAPIManager
    {
        string BasePath { get; }

        string GetToken();
    }
}