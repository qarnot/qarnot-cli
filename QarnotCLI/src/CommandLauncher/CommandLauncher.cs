namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using QarnotSDK;

    /// <summary>
    /// This class Interface manages the creation of the connection, the elements
    /// and send it to a class who will call the elements functions.
    /// </summary>
    public interface ICommandLauncher
    {
        Task<string> RunAndPrintCommandAsync(IConfiguration config, IPrinter printer, CancellationToken ct = default(CancellationToken));
    }

    /// <summary>
    /// Retrieve and take action in one SDK element and print the result information.
    /// Used for SDK elements outside list.
    /// </summary>
    /// <typeparam name="T1">The first generic type parameter: SDK Object to manipulate.</typeparam>
    /// <typeparam name="T2">The second generic type parameter: Return value Object.</typeparam>
    public class CommandGenericOneElem<T1, T2> : ICommandLauncher
    {
        private readonly IQElementRetriever<T1> QElementRetriever;

        private readonly ICommand<T1, T2> ElementLauncher;

        private readonly IResultFormatter Formatter;

        private readonly IConnectionWrapper API;

        public CommandGenericOneElem(IQElementRetriever<T1> qelementRetriever, ICommand<T1, T2> getInfo, IResultFormatter formatter, IConnectionWrapper api = null)
        {
            this.QElementRetriever = qelementRetriever;
            this.ElementLauncher = getInfo;
            this.Formatter = formatter;
            this.API = api ?? new ConnectionWrapper();
        }

        public virtual async Task<string> RunAndPrintCommandAsync(IConfiguration configuration, IPrinter printer, CancellationToken ct = default(CancellationToken))
        {
            Connection connect = this.API.CreateConnection(configuration);

            T1 tElem = await this.QElementRetriever.RetrieveAsync(configuration, connect, ct);

            T2 valueToPrint = await this.ElementLauncher.ExecuteAsync(tElem, configuration, ct);

            string returnString = this.Formatter.Format<T2>(valueToPrint);

            if (printer != null)
            {
                printer.Print(returnString);
            }

            return returnString;
        }
    }

    public class CreateCommandLauncher : ICommandLauncher
    {
        private readonly ApiObjectCreator.IApiObjectCreator Creator;

        private readonly IResultFormatter FormatToString;

        private readonly IConnectionWrapper API;

        public CreateCommandLauncher(ApiObjectCreator.IApiObjectCreator createElem, IResultFormatter format, IConnectionWrapper api = null)
        {
            this.Creator = createElem;
            this.FormatToString = format;
            this.API = api ?? new ConnectionWrapper();
        }

        public virtual async Task<string> RunAndPrintCommandAsync(IConfiguration config, IPrinter printer, CancellationToken ct = default(CancellationToken))
        {
            Connection connect = this.API.CreateConnection(config);

            CommandValues.GenericInfoCommandValue info = await this.Creator.Create(config, connect, ct);
            string returnString = FormatToString.Format(info);

            if (printer != null)
            {
                printer.Print(returnString);
            }

            return returnString;
        }
    }

    public class CommandAll : ICommandLauncher
    {
        private readonly Dictionary<string, ICommandLauncher> CommandToLaunch;

        public CommandAll(Dictionary<string, ICommandLauncher> dict)
        {
            CommandToLaunch = dict;
        }

        private string ConcatString(string name, string ret)
        {
            if (!string.IsNullOrEmpty(ret))
            {
                return name + Environment.NewLine
                    + ret
                    + Environment.NewLine + Environment.NewLine;
            }

            return string.Empty;
        }

        public virtual async Task<string> RunAndPrintCommandAsync(IConfiguration iconfig, IPrinter printer, CancellationToken ct = default(CancellationToken))
        {
            string returnString = string.Empty;
            Dictionary<string, Task<string>> retDict = new Dictionary<string, Task<string>>();

            foreach (var kvp in this.CommandToLaunch)
            {
                CLILogs.Debug("Command All launch for " + kvp.Key);
                retDict[kvp.Key] = kvp.Value.RunAndPrintCommandAsync(iconfig, null, ct);
            }

            foreach (var asyncKVP in retDict)
            {
                string launchString = this.ConcatString(asyncKVP.Key, await asyncKVP.Value);
                returnString += launchString;
                if (printer != null)
                {
                    printer.Print(launchString);
                }
            }

            return returnString;
        }
    }

    /// <summary>
    /// Retrieve, manage a list of SDK element and print the result information.
    /// Used for SDK Objects list.
    /// </summary>
    /// <typeparam name="T1">The first generic type parameter: QObject.</typeparam>
    /// <typeparam name="T2">The second generic type parameter: Return value Object.</typeparam>
    public class CommandGeneric<T1, T2> : ICommandLauncher
                                        where T2 : class
    {
        private readonly IConnectionWrapper API;

        private readonly IQCollectionRetriever<T1> QCollectionRetriever;

        private readonly ICommand<T1, T2> OneElementLauncher;

        private readonly IResultFormatter FormatToString;

        public CommandGeneric(IQCollectionRetriever<T1> qcollectionRetriever, ICommand<T1, T2> oneInfo, IResultFormatter format, IConnectionWrapper api = null)
        {
            QCollectionRetriever = qcollectionRetriever;
            OneElementLauncher = oneInfo;
            FormatToString = format;
            this.API = api ?? new ConnectionWrapper();
        }

        public virtual async Task<string> RunAndPrintCommandAsync(IConfiguration config, IPrinter printer, CancellationToken ct = default(CancellationToken))
        {
            Connection connect = this.API.CreateConnection(config);

            List<T2> listToPrint = null;
            List<T1> listOfT = await this.QCollectionRetriever.RetrieveAsync(config, connect, ct);

            List<Task<T2>> taskList = listOfT.Select(elem => this.OneElementLauncher.ExecuteAsync(elem, config, ct)).ToList();

            var waitTaskResult = Task.WhenAll(taskList);
            waitTaskResult.Wait();
            listToPrint = waitTaskResult.Result.Where(result => result != null).ToList();

            string returnString = this.FormatToString.FormatCollection(listToPrint);

            if (printer != null)
            {
                printer.Print(returnString);
            }

            return returnString;
        }
    }
}