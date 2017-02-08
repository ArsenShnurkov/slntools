namespace MetaSpecTools.Merge
{
    public delegate string ValueConflictResolver(
                ConflictContext context,
                string valueInSourceBranch,
                string valueInDestinationBranch);
    public delegate Difference OperationTypeConflictResolver(
                ConflictContext context,
                Difference differenceInSourceBranch,
                Difference differenceInDestinationBranch);

    public abstract class Conflict
    {
        public static NodeConflict Merge(
                    NodeElement commonAncestrorElement,
                    NodeElement elementInSourceBranch,
                    NodeElement elementInDestinationBranch)
        {
            NodeDifference ignoreA, ignoreB;
            return Merge(commonAncestrorElement, elementInSourceBranch, elementInDestinationBranch, out ignoreA, out ignoreB);
        }

        public static NodeConflict Merge(
                    NodeElement commonAncestrorElement,
                    NodeElement elementInSourceBranch,
                    NodeElement elementInDestinationBranch,
                    out NodeDifference differenceInSourceBranch,
                    out NodeDifference differenceInDestinationBranch)
        {
            differenceInSourceBranch = (NodeDifference) elementInSourceBranch.CompareTo(commonAncestrorElement)
                            ?? new NodeDifference(new ElementIdentifier("SolutionFile"), OperationOnParent.Modified, null);

            differenceInDestinationBranch = (NodeDifference) elementInDestinationBranch.CompareTo(commonAncestrorElement)
                            ?? new NodeDifference(new ElementIdentifier("SolutionFile"), OperationOnParent.Modified, null);

            return (NodeConflict)differenceInSourceBranch.CompareTo(differenceInDestinationBranch);
        }


        private readonly ElementIdentifier r_identifier;

        protected Conflict(ElementIdentifier identifier)
        {
            r_identifier = identifier;
        }

        public ElementIdentifier Identifier { get { return r_identifier; } }

        public Difference Resolve(
                    OperationTypeConflictResolver operationTypeConflictResolver,
                    ValueConflictResolver valueConflictResolver)
        {
            return Resolve(new ConflictContext(), operationTypeConflictResolver, valueConflictResolver);
        }

        public abstract Difference Resolve(
                        ConflictContext context,
                        OperationTypeConflictResolver operationTypeConflictResolver,
                        ValueConflictResolver valueConflictResolver);
    }
}
