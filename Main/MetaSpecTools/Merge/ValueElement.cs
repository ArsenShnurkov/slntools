using System;

namespace MetaSpecTools.Merge
{
    public class ValueElement : Element
    {
        private readonly string r_value;

        public ValueElement(
                    ElementIdentifier identifier,
                    string value)
            : base(identifier)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            r_value = value;
        }

        public string Value { get { return r_value; } }

        public override Difference CompareTo(Element oldElement)
        {
            if (oldElement == null)
                throw new ArgumentNullException("oldElement");
            if (!oldElement.Identifier.Equals(this.Identifier))
                throw new MergeException("Cannot compare elements that does not share the same identifier.");

            if (oldElement is EmptyElement)
            {
                return new ValueDifference(
                            this.Identifier,
                            OperationOnParent.Added,
                            null,
                            r_value);
            }
            else if (oldElement is ValueElement)
            {
                var oldValue = ((ValueElement)oldElement).Value;
                if (oldValue != r_value)
                {
                    return new ValueDifference(
                                this.Identifier,
                                OperationOnParent.Modified,
                                oldValue,
                                r_value);
                }
                else
                {
                    return null;
                }
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
                return new ValueElement(
                            this.Identifier,
                            ((ValueDifference)difference).NewValue);
            }
            else
            {
                throw new MergeException(string.Format("Cannot apply a {0} on a {1}.", difference.GetType().Name, this.GetType().Name));
            }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", this.Identifier, r_value);
        }
    }
}
