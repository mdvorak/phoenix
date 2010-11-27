using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class LayersCollection : ItemsCollection
    {
        public LayersCollection(uint serial)
            : base(serial, false)
        {
        }

        public LayersCollection(uint serial, bool searchSubContainers)
            : base(serial, searchSubContainers)
        {
        }

        public UOItem this[Layer layer]
        {
            get { return this[(byte)layer]; }
        }

        public UOItem this[byte layer]
        {
            get { return World.GetItem(World.GetRealCharacter(Container).Layers[layer]); }
        }
    }
}
