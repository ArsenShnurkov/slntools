using System;

namespace MetaSpecTools.Merge
{
    public abstract class Difference
    {
        private readonly ElementIdentifier r_identifier;
        private readonly OperationOnParent r_operationOnParent;

        protected Difference(
                    ElementIdentifier identifier,
                    OperationOnParent operationOnParent)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (! Enum.IsDefined(operationOnParent.GetType(), operationOnParent))
                throw new ArgumentOutOfRangeException("operationOnParent", operationOnParent, "Invalid value");
            r_identifier = identifier;
            r_operationOnParent = operationOnParent;
        }

        public ElementIdentifier Identifier { get { return r_identifier; } }
        public OperationOnParent OperationOnParent { get { return r_operationOnParent; } }

        public abstract Conflict CompareTo(Difference destinationDifference);
    }
}
