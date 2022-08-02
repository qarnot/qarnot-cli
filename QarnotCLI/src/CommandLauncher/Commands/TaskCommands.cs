namespace QarnotCLI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class WaitTaskCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Wait");
            var config = iconfig as StdConfiguration;
            bool end = false;
            while (end == false)
            {
                if (config.Stdout)
                {
                    var stdmessage = await task.FreshStdoutAsync(ct);
                    if (!string.IsNullOrEmpty(stdmessage))
                    {
                        CLILogs.Info("Stdout:" + Environment.NewLine + stdmessage.Replace("\\n", Environment.NewLine));
                    }
                }
                if (config.Stderr)
                {
                    var stdmessage = await task.FreshStderrAsync(ct);
                    if (!string.IsNullOrEmpty(stdmessage))
                    {
                        CLILogs.Info("Stderr:" + Environment.NewLine + stdmessage.Replace("\\n", Environment.NewLine));
                    }
                }
                end = await task.WaitAsync(taskTimeoutSeconds:2,ct:ct);
            }

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

    public class StdoutTaskCommand : ICommand<QTask, string>
    {
        public virtual async Task<string> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Print Stdout");
            var config = iconfig as StdConfiguration;

            string message = "";
            if (config.Fresh)
            {
                message = await task.FreshStdoutAsync(ct);
            }
            else
            {
                message = await task.StdoutAsync(ct);
            }

            return string.Join(Environment.NewLine, message.Split("\\n", StringSplitOptions.RemoveEmptyEntries));
        }
    }

    public class StderrTaskCommand : ICommand<QTask, string>
    {
        public virtual async Task<string> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Print Stderr");
            var config = iconfig as StdConfiguration;

            string message = "";
            if (config.Fresh)
            {
                message = await task.FreshStderrAsync(ct);
            }
            else
            {
                message = await task.StderrAsync(ct);
            }

            return string.Join(Environment.NewLine, message.Split("\\n", StringSplitOptions.RemoveEmptyEntries));
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

    public class UpdateTaskConstantCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Update Constant");
            var config = iconfig as ConstantUpdateConfiguration;
            task.SetConstant(config.ConstantName, config.ConstantValue);
            await task.CommitAsync(cancellationToken: ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = task.Uuid.ToString(),
                Message = String.Format("Task constant {0} updated", config.ConstantName),
            };
        }
    }

    public class SnapshotTaskCommand : ICommand<QTask, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QTask task, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command Task : Snapshot Task");
            var config = iconfig as SnapshotConfiguration;

            task.SnapshotWhitelist = config.Whitelist;
            task.SnapshotBlacklist = config.Blacklist;

            if (config.SnapshotPeriodicSec > 0)
            {
                await task.TriggerPeriodicSnapshotAsync(config.SnapshotPeriodicSec, config.Whitelist, config.Blacklist, cancellationToken: ct);
            }
            else
            {
                await task.TriggerSnapshotAsync(config.Whitelist, config.Blacklist, cancellationToken: ct);
            }

            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = task.Uuid.ToString(),
                Message = "Task Snapshot",
            };
        }
    }
}
