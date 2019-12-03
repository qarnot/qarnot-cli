namespace QarnotCLI
{
    using System;

    /// <summary>
    /// The Exceptions send by the parser.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        public ParseException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ParseException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        public ParseException(string exceptionMessage)
            : base(exceptionMessage)
        {
        }
    }

    /// <summary>
    /// The Exceptions send when a version is ask.
    /// </summary>
    public class ParseVersionException : ParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseVersionException"/> class.
        /// </summary>
        public ParseVersionException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseVersionException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ParseVersionException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseVersionException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        public ParseVersionException(string exceptionMessage)
            : base(exceptionMessage)
        {
        }
    }

    /// <summary>
    /// The Exceptions send when the help is ask.
    /// </summary>
    public class ParseHelpException : ParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseHelpException"/> class.
        /// </summary>
        public ParseHelpException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseHelpException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ParseHelpException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseHelpException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        public ParseHelpException(string exceptionMessage)
            : base(exceptionMessage)
        {
        }
    }
}