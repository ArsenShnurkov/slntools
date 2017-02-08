using System;

namespace MetaSpecTools.Merge
{
    public class ElementIdentifier
    {
        private readonly string r_name;
        private readonly string r_friendlyName;

        public ElementIdentifier(string name)
            : this(name, null)
        {
        }

        public ElementIdentifier(string name, string friendlyName)
        {
            r_name = name;
            r_friendlyName = friendlyName ?? name;
        }

        public string Name { get { return r_name; } }
        public string FriendlyName { get { return r_friendlyName; } }

        public EmptyElement CreateEmptyElement()
        {
            return new EmptyElement(this);
        }

        public override bool Equals(object obj)
        {
            var objAsElementIdentifier = obj as ElementIdentifier;
            if (objAsElementIdentifier == null)
                return false;

            return (string.Compare(this.Name, objAsElementIdentifier.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.Name);
        }

        public override string ToString()
        {
            return this.FriendlyName;
        }
    }
}
