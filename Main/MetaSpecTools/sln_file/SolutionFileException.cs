using System;

namespace MetaSpecTools
{
    class SolutionFileException
        : Exception
    {
        public SolutionFileException(string message)
            : base(message)
        {
        }

        public SolutionFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
