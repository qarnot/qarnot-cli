namespace QarnotCLI
{
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class WaitTaskCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Wait");
            await task.WaitAsync(ct:ct);
            var info = new CommandValues.GenericInfoCommandValue();
            info.Uuid = task.Uuid.ToString();
            info.Message = "Task Wait end status : " + task.State;
            return info;
        }
    }

    public class AbortTaskCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Abort");
            await task.AbortAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = task.Uuid.ToString(),
                Message = "Task abort ",
            };
        }
    }

    public class DeleteTaskCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Delete");
            await task.DeleteAsync(cancellationToken: ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = task.Uuid.ToString(),
                Message = "Task delete ",
            };
        }
    }

    public class UpdateTaskResourcesCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Update Storage");
            await task.UpdateResourcesAsync(cancellationToken: ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = task.Uuid.ToString(),
                Message = "Task resources updated",
            };
        }
    }
}