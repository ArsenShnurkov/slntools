using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools
{
    public class SectionHashList
        : KeyedCollection<string, Section>
    {
        public SectionHashList()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public SectionHashList(IEnumerable<Section> items)
            : this()
        {
            AddRange(items);
        }

        protected override string GetKeyForItem(Section item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, Section item)
        {
            // Add a clone of the item instead of the item itself
            base.InsertItem(index, new Section(item));
        }

        protected override void SetItem(int index, Section item)
        {
            // Add a clone of the item instead of the item itself
            base.SetItem(index, new Section(item));
        }

        public void AddRange(IEnumerable<Section> items)
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
