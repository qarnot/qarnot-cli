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
            pool.ElasticMinimumTotalSlots = config.ElasticMinimumTotalSlots == default(int) ? pool.ElasticMinimumTotalSlots : config.ElasticMinimumTotalSlots;
            pool.ElasticMaximumTotalSlots = config.ElasticMaximumTotalSlots == default(int) ? pool.ElasticMaximumTotalSlots : config.ElasticMaximumTotalSlots;
            pool.ElasticMinimumIdlingSlots = config.ElasticMinimumIdlingSlots == default(int) ? pool.ElasticMinimumIdlingSlots : config.ElasticMinimumIdlingSlots;
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