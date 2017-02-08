using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools.Merge
{
    public class ElementHashList : KeyedCollection<ElementIdentifier, Element>
    {
        public ElementHashList()
        {
        }

        public ElementHashList(IEnumerable<Element> items)
        {
            AddRange(items);
        }

        protected override ElementIdentifier GetKeyForItem(Element item)
        {
            return item.Identifier;
        }

        public void AddRange(IEnumerable<Element> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        public void AddOrUpdate(Element item)
        {
            var existingItem = (Contains(GetKeyForItem(item))) ? this[GetKeyForItem(item)] : null;
            if (existingItem == null)
            {
                Add(item);
            }
            else
            {
                // If the item already exist in the list, we put the new version in the same spot.
                SetItem(IndexOf(existingItem), item);
            }
        }
    }
}
