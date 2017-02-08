using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools
{
    public class PropertyLineHashList
        : KeyedCollection<string, PropertyLine>
    {
        public PropertyLineHashList()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public PropertyLineHashList(IEnumerable<PropertyLine> items)
            : this()
        {
            AddRange(items);
        }

        protected override string GetKeyForItem(PropertyLine item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, PropertyLine item)
        {
            var existingItem = (Contains(GetKeyForItem(item))) ? this[GetKeyForItem(item)] : null;

            if (existingItem == null)
            {
                // Add a clone of the item instead of the item itself
                base.InsertItem(index, new PropertyLine(item));
            }
            else if (item.Value != existingItem.Value)
            {
                throw new SolutionFileException(
                            string.Format("Trying to add a new property line '{0}={1}' when there is already a line with the same key and the value '{2}' in the collection.",
                                item.Name,
                                item.Value,
                                existingItem.Value));
            }
            else
            {
                // Nothing to do, the item provided is a duplicate of an item already present in the collection.
            }
        }

        protected override void SetItem(int index, PropertyLine item)
        {
            // Add a clone of the item instead of the item itself
            base.SetItem(index, new PropertyLine(item));
        }

        public void AddRange(IEnumerable<PropertyLine> items)
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
