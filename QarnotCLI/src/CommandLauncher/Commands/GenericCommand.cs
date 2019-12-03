namespace QarnotCLI
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICommand<T1, T2>
    {
        Task<T2> ExecuteAsync(T1 tObj, IConfiguration iconfig, CancellationToken ct);
    }

    public class GenericCommand<T1, T2> : ICommand<T1, T2>
                                                where T1 : T2
    {
        public virtual Task<T2> ExecuteAsync(T1 obj, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("print info");
            return Task.FromResult<T2>(obj);
        }
    }

    public class GenericCollectionCommand<T1, T2> : ICommand<T1, T2>
                                             where T2 : class, ICollectionCommandValue
    {
        public GenericCollectionCommand(IModelMapper<T1> listObj)
        {
            this.ListObj = listObj;
        }

        private IModelMapper<T1> ListObj { get; set; }

        public virtual Task<T2> ExecuteAsync(T1 obj, IConfiguration iconfig = null, CancellationToken ct = default(CancellationToken))
        {
            CLILogs.Debug("print list");
            T2 objToPrint = ListObj.CreateObj(obj) as T2;
            return Task.FromResult(objToPrint);
        }
    }
}