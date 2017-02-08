namespace MetaSpecTools
{
    public class PropertyLine
    {
        private readonly string r_name;
        private string m_value;

        public PropertyLine(PropertyLine original)
            : this(original.Name, original.Value)
        {
        }

        public PropertyLine(string name, string value)
        {
            r_name = name;
            m_value = value;
        }

        public string Name
        {
            get { return r_name; }
        }
        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", this.Name, this.Value);
        }
    }
}
