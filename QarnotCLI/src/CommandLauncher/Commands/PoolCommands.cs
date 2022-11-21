using System;
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
            var config = iconfig as DefaultRunConfiguration;

            if (pool == default)
            {
                var message = "Deletion ignored. Pool not found";
                throw new System.Exception(message);
            }

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
            var config = iconfig as DefaultRunConfiguration;

            if (pool == default)
            {
                var message = "Resources update failed. Pool not found";
                throw new System.Exception(message);
            }

            await pool.UpdateResourcesAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = "Pool resources updated",
            };
        }
    }

    public class UpdatePoolConstantCommand : ICommand<QPool, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QPool pool, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("Command pool : Update Constant");
            var config = iconfig as ConstantUpdateConfiguration;

            if (pool == default)
            {
                var message = "Constant update failed. Pool not found";
                throw new System.Exception(message);
            }

            pool.SetConstant(config.ConstantName, config.ConstantValue);
            await pool.CommitAsync(cancellationToken: ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = String.Format("Pool constant {0} updated", config.ConstantName),
            };
        }
    }

    public class SetPoolElasticSettingsCommand : ICommand<QPool, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QPool pool, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            PoolSetElasticSettingsConfiguration config = iconfig as PoolSetElasticSettingsConfiguration;
            CLILogs.Debug("Command pool : Pool name " + pool?.Name ?? config?.Name);
            CLILogs.Debug("Command pool : Set Pool Elastic info");

            if (pool == default)
            {
                var message = "Elastic settings update failed. Pool not found";
                throw new System.Exception(message);
            }

            pool.ElasticMinimumTotalSlots = config.ElasticMinimumTotalSlots ?? pool.ElasticMinimumTotalSlots;
            pool.ElasticMaximumTotalSlots = config.ElasticMaximumTotalSlots ?? pool.ElasticMaximumTotalSlots;
            pool.ElasticMinimumIdlingSlots = config.ElasticMinimumIdlingSlots ?? pool.ElasticMinimumIdlingSlots;
            pool.ElasticResizePeriod = config.ElasticResizePeriod ?? pool.ElasticResizePeriod;
            pool.ElasticResizeFactor = config.ElasticResizeFactor ?? pool.ElasticResizeFactor;
            pool.ElasticMinimumIdlingTime = config.ElasticMinimumIdlingTime ?? pool.ElasticMinimumIdlingTime;
            await pool.CommitAsync(ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = "Pool set ",
            };
        }
    }


    public class SetPoolScalingCommand : ICommand<QPool, CommandValues.GenericInfoCommandValue>
    {
        public virtual async Task<CommandValues.GenericInfoCommandValue> ExecuteAsync(QPool pool, IConfiguration iconfig, CancellationToken ct = default(CancellationToken))
        {
            PoolSetScalingConfiguration config = iconfig as PoolSetScalingConfiguration;
            CLILogs.Debug("Command pool : Pool name " + pool?.Name ?? config?.Name);
            CLILogs.Debug("Command pool : Set pool scaling");

            if (pool == default)
            {
                var message = "Pool scaling update failed. Pool not found";
                throw new System.Exception(message);
            }

            await pool.UpdateScalingAsync(config.Scaling, ct);
            return new CommandValues.GenericInfoCommandValue()
            {
                Uuid = pool.Uuid.ToString(),
                Message = "Update pool scaling ",
            };
        }
    }
}
