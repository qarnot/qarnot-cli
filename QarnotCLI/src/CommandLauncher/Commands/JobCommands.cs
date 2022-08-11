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
            var config = iconfig as DefaultRunConfiguration;

            if (job == default)
            {
                var message = "Abortion ignored. Job not found";
                throw new System.Exception(message);
            }

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
            var config = iconfig as DefaultRunConfiguration;

            if (job == default)
            {
                var message = "Deletion ignored. Job not found";
                throw new System.Exception(message);
            }
            await job.DeleteAsync(cancellationToken:ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = job.Uuid.ToString(),
                Message = "Job delete  ",
            };
        }
    }
}