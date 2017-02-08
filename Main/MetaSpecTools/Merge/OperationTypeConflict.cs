namespace MetaSpecTools.Merge
{
    public class OperationTypeConflict : Conflict
    {
        private readonly Difference r_differenceInSourceBranch;
        private readonly Difference r_differenceInDestinationBranch;

        public OperationTypeConflict(
                    Difference differenceInSourceBranch,
                    Difference differenceInDestinationBranch)
            : base(differenceInSourceBranch.Identifier)
        {
            r_differenceInSourceBranch = differenceInSourceBranch;
            r_differenceInDestinationBranch = differenceInDestinationBranch;
        }

        public Difference DifferenceInSourceBranch { get { return r_differenceInSourceBranch; } }
        public Difference DifferenceInDestinationBranch { get { return r_differenceInDestinationBranch; } }

        public override Difference Resolve(
                    ConflictContext context,
                    OperationTypeConflictResolver operationTypeConflictResolver,
                    ValueConflictResolver valueConflictResolver)
        {
            return operationTypeConflictResolver(context.CreateSubcontext(this), r_differenceInSourceBranch, r_differenceInDestinationBranch);
        }

        public override string ToString()
        {
            return string.Format("{0} was {1} in the source branch but it was {2} in the destination branch.",
                        this.Identifier,
                        this.DifferenceInSourceBranch.OperationOnParent.ToString().ToLower(),
                        this.DifferenceInDestinationBranch.OperationOnParent.ToString().ToLower());
        }
    }
}
