using Quartz;
using Reminder_WPF.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    public interface IReminderManager
    {
        string Name { get; }

        Task AddReminder(Reminder item);
        Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default);
        Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default);
        Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default);
        Task RemoveReminder(Reminder item);
        Task UpdateReminder(Reminder item);
    }
}