namespace QarnotCLI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class CommandLineWrapError : CommandLine.Error
    {
        public CommandLineWrapError()
            : base(CommandLine.ErrorType.HelpRequestedError, true)
        {
        }

        public CommandLineWrapError(CommandLine.ErrorType error)
            : base(error, true)
        {
        }
    }
}