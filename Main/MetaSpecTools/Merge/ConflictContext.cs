using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools.Merge
{
    public class ConflictContext
    {
        private readonly List<Conflict> r_conflicts;

        public ConflictContext()
        {
            r_conflicts = new List<Conflict>();
        }
        private ConflictContext(ConflictContext context, Conflict subconflict)
        {
            r_conflicts = new List<Conflict>(context.r_conflicts) {subconflict};
        }

        public ReadOnlyCollection<Conflict> HierachyZoom
        {
            get { return r_conflicts.AsReadOnly(); }
        }

        public ConflictContext CreateSubcontext(Conflict subconflict)
        {
            return new ConflictContext(this, subconflict);
        }
    }
}
