using System;

namespace MetaSpecTools.Merge
{
    public class EmptyElement
        : Element
    {
        public EmptyElement(ElementIdentifier identifier)
            : base(identifier)
        {
        }

        public override Difference CompareTo(Element oldElement)
        {
            if (oldElement == null)
                throw new ArgumentNullException("oldElement");
            if (!oldElement.Identifier.Equals(this.Identifier))
                throw new MergeException("Cannot compare elements that does not share the same identifier.");

            if (oldElement is ValueElement)
            {
                return new ValueDifference(
                            this.Identifier,
                            OperationOnParent.Removed,
                            ((ValueElement)oldElement).Value,
                            null);
            }
            else if (oldElement is NodeElement)
            {
                return new NodeDifference(
                            this.Identifier,
                            OperationOnParent.Removed,
                            null);
            }
            else
            {
                throw new MergeException(string.Format("Cannot compare a {0} to a {1}.", oldElement.GetType().Name, this.GetType().Name));
            }
        }

        public override Element Apply(Difference difference)
        {
            if (difference == null)
                throw new ArgumentNullException("difference");
            if (!difference.Identifier.Equals(this.Identifier))
                throw new MergeException("Cannot apply a difference that does not share the same identifier with the element.");

            if (difference is ValueDifference)
            {
                if (difference.OperationOnParent == OperationOnParent.Removed)
                    throw new MergeException("Cannot apply a 'remove' difference on a ValueElement.");
                return new ValueElement(
                            this.Identifier,
                            ((ValueDifference)difference).NewValue);
            }
            else if (difference is NodeDifference)
            {
                var emptyNodeElement = new NodeElement(this.Identifier, null);
                return emptyNodeElement.Apply(difference);
            }
            else
            {
                throw new MergeException(string.Format("Cannot apply a {0} on a {1}.", difference.GetType().Name, this.GetType().Name));
            }
        }
    }
}
