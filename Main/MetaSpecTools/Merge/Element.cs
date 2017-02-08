using System;

namespace MetaSpecTools.Merge
{
    public abstract class Element
    {
        private readonly ElementIdentifier r_identifier;

        protected Element(ElementIdentifier identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");

            r_identifier = identifier;
        }

        public ElementIdentifier Identifier { get { return r_identifier; } }

        public abstract Difference CompareTo(Element oldElement);
        public abstract Element Apply(Difference difference);
    }
}
