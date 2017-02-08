using System;

namespace MetaSpecTools.Merge
{
    public class ValueConflict : Conflict
    {
        private readonly OperationOnParent r_operationOnParent;
        private readonly string r_oldValue;
        private readonly string r_newValueInSourceBranch;
        private readonly string r_newValueInDestinationBranch;

        public ValueConflict(
                    ElementIdentifier identifier,
                    OperationOnParent operationOnParent,
                    string commonAncestrorValue,
                    string newValueInSourceBranch,
                    string newValueInDestinationBranch)
            : base(identifier)
        {
            if (!Enum.IsDefined(operationOnParent.GetType(), operationOnParent))
                throw new ArgumentOutOfRangeException("operationOnParent", operationOnParent.ToString(), "Invalid value");

            r_operationOnParent = operationOnParent;
            r_oldValue = commonAncestrorValue;
            r_newValueInSourceBranch = newValueInSourceBranch;
            r_newValueInDestinationBranch = newValueInDestinationBranch;
        }

        public OperationOnParent OperationOnParent { get { return r_operationOnParent; } }
        public string OldValue { get { return r_oldValue; } }
        public string NewValueInSourceBranch { get { return r_newValueInSourceBranch; } }
        public string NewValueInDestinationBranch { get { return r_newValueInDestinationBranch; } }

        public override Difference Resolve(
                    ConflictContext context,
                    OperationTypeConflictResolver operationTypeConflictResolver,
                    ValueConflictResolver valueConflictResolver)
        {
            var resolvedValue = valueConflictResolver(context.CreateSubcontext(this), r_newValueInSourceBranch, r_newValueInDestinationBranch);
            return resolvedValue == null
                        ? null
                        : new ValueDifference(this.Identifier, this.OperationOnParent, r_oldValue, resolvedValue);
        }

        public override string ToString()
        {
            return string.Format("Both branches {0} {1} but with different values: Source = \"{2}\" and Destination = \"{3}\".",
                        this.OperationOnParent.ToString().ToLower(),
                        this.Identifier,
                        this.NewValueInSourceBranch,
                        this.NewValueInDestinationBranch);
        }
    }
}
