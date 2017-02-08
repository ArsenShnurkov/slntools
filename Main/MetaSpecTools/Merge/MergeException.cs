using System;

namespace MetaSpecTools.Merge
{
    class MergeException
        : Exception
    {
        public MergeException(string message)
            : base(message)
        {
        }

        public MergeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
