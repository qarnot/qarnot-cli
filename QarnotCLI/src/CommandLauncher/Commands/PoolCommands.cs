namespace QarnotCLI
{
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    public class DeletePoolCommand : ICommand<QPool, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QPool pool, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command pool : delete");
            await pool.DeleteAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = "Pool delete ",
            };
        }
    }

    public class UpdatePoolResourcesCommand : ICommand<QPool, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QPool pool, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command pool : Update Storage");
            await pool.UpdateResourcesAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = "Pool resources updated",
            };
        }
    }

    public class SetPoolElasticSettingsCommand : ICommand<QPool, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QPool pool, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command pool : Pool name " + pool.Name);
            PoolSetElasticSettingsConfiguration config = iconfig as PoolSetElasticSettingsConfiguration;
            CLILogs.Debug("Command pool : Set Pool Elastic info");
            pool.ElasticMinimumTotalNodes = config.ElasticMinimumTotalNodes == default(int) ? pool.ElasticMinimumTotalNodes : config.ElasticMinimumTotalNodes;
            pool.ElasticMaximumTotalNodes = config.ElasticMaximumTotalNodes == default(int) ? pool.ElasticMaximumTotalNodes : config.ElasticMaximumTotalNodes;
            pool.ElasticMinimumIdlingNodes = config.ElasticMinimumIdlingNodes == default(int) ? pool.ElasticMinimumIdlingNodes : config.ElasticMinimumIdlingNodes;
            pool.ElasticResizePeriod = config.ElasticResizePeriod == default(int) ? pool.ElasticResizePeriod : config.ElasticResizePeriod;
            pool.ElasticResizeFactor = config.ElasticResizeFactor == default(int) ? pool.ElasticResizeFactor : config.ElasticResizeFactor;
            pool.ElasticMinimumIdlingTime = config.ElasticMinimumIdlingTime == default(int) ? pool.ElasticMinimumIdlingTime : config.ElasticMinimumIdlingTime;
            await pool.CommitAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = "Pool set ",
            };
        }
    }
}