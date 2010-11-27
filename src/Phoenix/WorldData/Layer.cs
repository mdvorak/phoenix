using System;

namespace Phoenix.WorldData
{
    /// <summary>
    /// Character layers
    /// </summary>
    public enum Layer : byte
    {
        /// <summary>
        /// Object is not in layer
        /// </summary>
        None = 0x0,

        /// <summary>
        /// One handed weapon
        /// </summary>
        RightHand = 0x01,

        /// <summary>
        /// Two handed weapon, shield, or misc.
        /// </summary>
        LeftHand = 0x02,

        /// <summary>
        /// Shoes
        /// </summary>
        Shoes = 0x03,

        /// <summary>
        /// Pants
        /// </summary>
        Pants = 0x04,

        /// <summary>
        /// Shirt
        /// </summary>
        Shirt = 0x05,

        /// <summary>
        /// Helm/Hat
        /// </summary>
        Hat = 0x06,

        /// <summary>
        /// Gloves
        /// </summary>
        Gloves = 0x07,

        /// <summary>
        /// Ring
        /// </summary>
        Ring = 0x08,

        /// <summary>
        /// Unused
        /// </summary>
        Unused = 0x09,

        /// <summary>
        /// Neck
        /// </summary>
        Neck = 0x0A,

        /// <summary>
        /// Hair
        /// </summary>
        Hair = 0x0B,

        /// <summary>
        /// Waist (half apron)
        /// </summary>
        Waist = 0x0C,

        /// <summary>
        /// Torso (inner) (chest armor)
        /// </summary>
        InnerTorso = 0x0D,

        /// <summary>
        /// Bracelet
        /// </summary>
        Bracelet = 0x0E,

        /// <summary>
        /// Unused (backpack, but backpacks go to 0x15)
        /// </summary>
        Unused2 = 0x0F,

        /// <summary>
        /// Facial Hair
        /// </summary>
        FacialHair = 0x10,

        /// <summary>
        /// Torso (middle) (sircoat, tunic, full apron, sash)
        /// </summary>
        MiddleTorso = 0x11,

        /// <summary>
        /// Earrings
        /// </summary>
        Earrings = 0x12,

        /// <summary>
        /// Arms
        /// </summary>
        Arms = 0x13,

        /// <summary>
        /// Back (cloak)
        /// </summary>
        Cloak = 0x14,

        /// <summary>
        /// Backpack
        /// </summary>
        Backpack = 0x15,

        /// <summary>
        /// Torso (outer) (robe)
        /// </summary>
        OuterTorso = 0x16,

        /// <summary>
        /// Legs (outer) (skirt/kilt)
        /// </summary>
        OuterLegs = 0x17,

        /// <summary>
        /// Legs (inner) (leg armor)
        /// </summary>
        InnerLegs = 0x18,

        /// <summary>
        /// Mount (horse, ostard, etc)
        /// </summary>
        Mount = 0x19,

        /// <summary>
        /// NPC Buy Restock container
        /// </summary>
        NPCBuyContainer = 0x1A,

        /// <summary>
        /// NPC Buy no restock container
        /// </summary>
        NPCContainer = 0x1B,

        /// <summary>
        /// NPC Sell container
        /// </summary>
        NPCSellContainer = 0x1C,

        /// <summary>
        /// PC Bank Box
        /// </summary>
        Bank = 0x1D
    }
}
