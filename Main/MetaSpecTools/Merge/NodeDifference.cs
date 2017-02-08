using System;
using System.Collections.Generic;

namespace MetaSpecTools.Merge
{
    public delegate bool ShouldBeRemovedHandler(
                Difference difference);

    public class NodeDifference : Difference
    {
        private DifferenceHashList m_subdifferences;

        public NodeDifference(
                    ElementIdentifier identifier,
                    OperationOnParent operationOnParent,
                    IEnumerable<Difference> subdifferences)
            : base(identifier, operationOnParent)
        {
            m_subdifferences = new DifferenceHashList(subdifferences);
        }

        public DifferenceHashList Subdifferences
        {
            get { return m_subdifferences; }
        }

        public override Conflict CompareTo(Difference destinationDifference)
        {
            if (destinationDifference == null)
                throw new ArgumentNullException("destinationDifference");
            if (!destinationDifference.Identifier.Equals(this.Identifier))
                throw new MergeException("Cannot compare differences that does not share the same identifier.");

            var source = this;
            var destination = destinationDifference as NodeDifference;
            if (destination == null)
                throw new MergeException(string.Format("Cannot compare a {0} to a {1}.", destinationDifference.GetType().Name, this.GetType().Name));

            if (source.OperationOnParent != destination.OperationOnParent)
            {
                return new OperationTypeConflict(
                            source,
                            destination);
            }
            else
            {
                var subconflicts = new List<Conflict>();
                var acceptedSubdifferences = new DifferenceHashList();
                foreach (var destinationSubdifference in destination.Subdifferences)
                {
                    // Add all the destinationBranchDifferences to the acceptedSubdifferences (they might be removed from the list later).
                    acceptedSubdifferences.Add(destinationSubdifference);
                }
                foreach (var sourceSubdifference in source.Subdifferences)
                {
                    if (!acceptedSubdifferences.Contains(sourceSubdifference.Identifier))
                    {
                        // This is a new difference that is not present in the destination branch, add the difference to the acceptedSubdifferences.
                        acceptedSubdifferences.Add(sourceSubdifference);
                    }
                    else
                    {
                        // There is a difference in both branch for the same identifier, see if there is a conflict or not
                        var destinationSubdifference = acceptedSubdifferences[sourceSubdifference.Identifier];
                        var conflict = sourceSubdifference.CompareTo(destinationSubdifference);
                        if (conflict != null)
                        {
                            var nodeConflict = conflict as NodeConflict;
                            if ((nodeConflict != null) && (nodeConflict.Subconflicts.Count == 0))
                            {
                                acceptedSubdifferences.Remove(sourceSubdifference.Identifier);
                                acceptedSubdifferences.Add(
                                            new NodeDifference(
                                                nodeConflict.Identifier,
                                                nodeConflict.OperationOnParent,
                                                nodeConflict.AcceptedSubdifferences));
                            }
                            else
                            {
                                acceptedSubdifferences.Remove(sourceSubdifference.Identifier);
                                subconflicts.Add(conflict);
                            }
                        }
                    }
                }

                return new NodeConflict(
                            source.Identifier,
                            source.OperationOnParent,
                            acceptedSubdifferences,
                            subconflicts);
            }
        }

        public void Remove(ShouldBeRemovedHandler shouldBeRemovedHandler)
        {
            var filteredSubdifferences = new DifferenceHashList();
            foreach (var subdifference in m_subdifferences)
            {
                if (shouldBeRemovedHandler(subdifference))
                {
                    // Do nothing
                }
                else if (subdifference is NodeDifference)
                {
                    var nodeSubdifference = subdifference as NodeDifference;
                    var nbSubdiffsBefore = nodeSubdifference.Subdifferences.Count;
                    nodeSubdifference.Remove(shouldBeRemovedHandler);
                    if ((nodeSubdifference.Subdifferences.Count > 0) || (nbSubdiffsBefore == 0))
                    {
                        filteredSubdifferences.Add(subdifference);
                    }
                }
                else
                {
                    filteredSubdifferences.Add(subdifference);
                }
            }
            m_subdifferences = filteredSubdifferences;
        }

        public override string ToString()
        {
            return string.Format("{0} has been {1}.", this.Identifier, this.OperationOnParent.ToString().ToLower());
        }
    }
}
