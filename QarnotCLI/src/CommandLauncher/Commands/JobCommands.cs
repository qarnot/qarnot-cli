namespace QarnotCLI
{
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class AbortJobCommand : ICommand<QJob, CommandValues.GenericInfoCommandValue>
    {
        private readonly IModelMapper<QJob> ListObj;

        public AbortJobCommand(IModelMapper<QJob> listObj)
        {
            this.ListObj = listObj;
        }

        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QJob job, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("command job : terminate");
            if (job.State == QJobStates.Active)
            {
                await job.TerminateAsync(ct);
                return new CommandValues.GenericInfoCommandValue()
                {
                    Uuid = job.Uuid.ToString(),
                    Message = "Job terminate  ",
                };
            }

            return null;
        }
    }

    public class DeleteJobCommand : ICommand<QJob, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QJob job, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("command job : delete");
            await job.DeleteAsync(cancellationToken:ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = job.Uuid.ToString(),
                Message = "Job delete  ",
            };
        }
    }
}