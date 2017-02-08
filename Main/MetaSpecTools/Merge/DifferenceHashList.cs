using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools.Merge
{
    public class DifferenceHashList : KeyedCollection<ElementIdentifier, Difference>
    {
        public DifferenceHashList()
        {
        }

        public DifferenceHashList(IEnumerable<Difference> items)
        {
            AddRange(items);
        }

        protected override ElementIdentifier GetKeyForItem(Difference item)
        {
            return item.Identifier;
        }

        public void AddRange(IEnumerable<Difference> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }
    }
}
