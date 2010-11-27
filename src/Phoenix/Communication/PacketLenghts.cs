using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication
{
    static class PacketLenghts
    {
        /// <summary>
        /// Dynamic packet means that packet size follows id byte and it is sent as ushort.
        /// </summary>
        public const int Dynamic = 0x8000;
        private static int[] lenghts;

        /// <summary>
        /// Gets packet lenght.
        /// </summary>
        /// <param name="id">Packet id.</param>
        /// <returns>Returns packet lenght or Dynamic. If lenght is zero packet lenght is unknown or stream is corrupted.</returns>
        public static int GetLenght(byte id)
        {
            return lenghts[id];
        }

        static PacketLenghts()
        {
            lenghts = new int[256];

            lenghts[0x00] = 0x0068; // Character Creation
            lenghts[0x01] = 0x0005; // Logout
            lenghts[0x02] = 0x0007; // Request Movement
            lenghts[0x03] = Dynamic; // Speech
            lenghts[0x04] = 0x0002; // Request God Mode
            lenghts[0x05] = 0x0005; // Attack
            lenghts[0x06] = 0x0005; // Double Click
            lenghts[0x07] = 0x0007; // Take Object
            lenghts[0x08] = 0x000E; // Drop Object
            lenghts[0x09] = 0x0005; // Single Click
            lenghts[0x0A] = 0x000B; // Edit
            lenghts[0x0B] = 0x010A; // Edit Area
            lenghts[0x0C] = Dynamic; // Tile Data
            lenghts[0x0D] = 0x0003; // NPC Data
            lenghts[0x0E] = Dynamic; // Edit Template Data
            lenghts[0x0F] = 0x003D; // Paperdoll Old
            lenghts[0x10] = 0x00D7; // Hue Data
            lenghts[0x11] = Dynamic; // Mobile Stats
            lenghts[0x12] = Dynamic; // God Command
            lenghts[0x13] = 0x000A; // Equip Item Request
            lenghts[0x14] = 0x0006; // Change Elevation
            lenghts[0x15] = 0x0009; // Follow
            lenghts[0x16] = 0x0001; // Request Script Names
            lenghts[0x17] = Dynamic; // Script Tree Command
            lenghts[0x18] = Dynamic; // Script Attach
            lenghts[0x19] = Dynamic; // NPC Conversation Data
            lenghts[0x1A] = Dynamic; // Show Item
            lenghts[0x1B] = 0x0025; // Login Confirm
            lenghts[0x1C] = Dynamic; // Text
            lenghts[0x1D] = 0x0005; // Destroy
            lenghts[0x1E] = 0x0004; // Animate
            lenghts[0x1F] = 0x0008; // Explode
            lenghts[0x20] = 0x0013; // Teleport
            lenghts[0x21] = 0x0008; // Block Movement
            lenghts[0x22] = 0x0003; // Accept Movement/Resync Request
            lenghts[0x23] = 0x001A; // Drag Item
            lenghts[0x24] = 0x0007; // Open Container
            lenghts[0x25] = 0x0014; // Object to Object
            lenghts[0x26] = 0x0005; // Old Client
            lenghts[0x27] = 0x0002; // Get Item Failed
            lenghts[0x28] = 0x0005; // Drop Item Failed
            lenghts[0x29] = 0x0001; // Drop Item OK
            lenghts[0x2A] = 0x0005; // Blood
            lenghts[0x2B] = 0x0002; // God Mode
            lenghts[0x2C] = 0x0002; // Death
            lenghts[0x2D] = 0x0011; // Health
            lenghts[0x2E] = 0x000F; // Equip Item
            lenghts[0x2F] = 0x000A; // Swing
            lenghts[0x30] = 0x0005; // Attack OK
            lenghts[0x31] = 0x0001; // Attack End
            lenghts[0x32] = 0x0002; // Hack Mover
            lenghts[0x33] = 0x0002; // Group
            lenghts[0x34] = 0x000A; // Client Query
            lenghts[0x35] = 0x028D; // Resource Type
            lenghts[0x36] = Dynamic; // Resource Tile Data
            lenghts[0x37] = 0x0008; // Move Object
            lenghts[0x38] = 0x0007; // Follow Move
            lenghts[0x39] = 0x0009; // Groups
            lenghts[0x3A] = Dynamic; // Skills
            lenghts[0x3B] = Dynamic; // Accept Offer
            lenghts[0x3C] = Dynamic; // Container Contents
            lenghts[0x3D] = 0x0002; // Ship
            lenghts[0x3E] = 0x0025; // Versions
            lenghts[0x3F] = Dynamic; // Update Statics
            lenghts[0x40] = 0x00C9; // Update Terrain
            lenghts[0x41] = Dynamic; // Update Tiledata
            lenghts[0x42] = Dynamic; // Update Art
            lenghts[0x43] = 0x0229; // Update Anim
            lenghts[0x44] = 0x02C9; // Update Hues
            lenghts[0x45] = 0x0005; // Ver OK
            lenghts[0x46] = Dynamic; // New Art
            lenghts[0x47] = 0x000B; // New Terrain
            lenghts[0x48] = 0x0049; // New Anim
            lenghts[0x49] = 0x005D; // New Hues
            lenghts[0x4A] = 0x0005; // Destroy Art
            lenghts[0x4B] = 0x0009; // Check Ver
            lenghts[0x4C] = Dynamic; // Script Names
            lenghts[0x4D] = Dynamic; // Script File
            lenghts[0x4E] = 0x0006; // Light Change
            lenghts[0x4F] = 0x0002; // Sunlight
            lenghts[0x50] = Dynamic; // Board Header
            lenghts[0x51] = Dynamic; // Board Message
            lenghts[0x52] = Dynamic; // Post Message
            lenghts[0x53] = 0x0002; // Login Reject
            lenghts[0x54] = 0x000C; // Sound
            lenghts[0x55] = 0x0001; // Login Complete
            lenghts[0x56] = 0x000B; // Map Command
            lenghts[0x57] = 0x006E; // Update Regions
            lenghts[0x58] = 0x006A; // New Region
            lenghts[0x59] = Dynamic; // New Context FX
            lenghts[0x5A] = Dynamic; // Update Context FX
            lenghts[0x5B] = 0x0004; // Game Time
            lenghts[0x5C] = 0x0002; // Restart Ver
            lenghts[0x5D] = 0x0049; // Pre Login
            lenghts[0x5E] = Dynamic; // Server List
            lenghts[0x5F] = 0x0031; // Add Server
            lenghts[0x60] = 0x0005; // Server Remove
            lenghts[0x61] = 0x0009; // Destroy Static
            lenghts[0x62] = 0x000F; // Move Static
            lenghts[0x63] = 0x000D; // Area Load
            lenghts[0x64] = 0x0001; // Area Load Request
            lenghts[0x65] = 0x0004; // Weather Change
            lenghts[0x66] = Dynamic; // Book Contents
            lenghts[0x67] = 0x0015; // Simple Edit
            lenghts[0x68] = Dynamic; // Script LS Attach
            lenghts[0x69] = Dynamic; // Friends
            lenghts[0x6A] = 0x0003; // Friend Notify
            lenghts[0x6B] = 0x0009; // Key Use
            lenghts[0x6C] = 0x0013; // Target
            lenghts[0x6D] = 0x0003; // Music
            lenghts[0x6E] = 0x000E; // Animation
            lenghts[0x6F] = Dynamic; // Trade
            lenghts[0x70] = 0x001C; // Effect
            lenghts[0x71] = Dynamic; // Bulletin Board
            lenghts[0x72] = 0x0005; // Combat
            lenghts[0x73] = 0x0002; // Ping
            lenghts[0x74] = Dynamic; // Shop Data
            lenghts[0x75] = 0x0023; // Rename MOB
            lenghts[0x76] = 0x0010; // Server Change
            lenghts[0x77] = 0x0011; // Naked MOB
            lenghts[0x78] = Dynamic; // Equipped MOB
            lenghts[0x79] = 0x0009; // Resource Query
            lenghts[0x7A] = Dynamic; // Resource Data
            lenghts[0x7B] = 0x0002; // Sequence
            lenghts[0x7C] = Dynamic; // Object Picker
            lenghts[0x7D] = 0x000D; // Picked Object
            lenghts[0x7E] = 0x0002; // God View Query
            lenghts[0x7F] = Dynamic; // God View Data
            lenghts[0x80] = 0x003E; // Account Login Request
            lenghts[0x81] = Dynamic; // Account Login OK
            lenghts[0x82] = 0x0002; // Account Login Failed
            lenghts[0x83] = 0x0027; // Account Delete Character
            lenghts[0x84] = 0x0045; // Change Character Password
            lenghts[0x85] = 0x0002; // Delete Character Failed
            lenghts[0x86] = Dynamic; // All Characters
            lenghts[0x87] = Dynamic; // Send Resources
            lenghts[0x88] = 0x0042; // Open Paperdoll
            lenghts[0x89] = Dynamic; // Corpse Equipment
            lenghts[0x8A] = Dynamic; // Trigger Edit
            lenghts[0x8B] = Dynamic; // Display Sign
            lenghts[0x8C] = 0x000B; // Server Redirect
            lenghts[0x8D] = Dynamic; // Unused3
            lenghts[0x8E] = Dynamic; // Move Character
            lenghts[0x8F] = Dynamic; // Unused4
            lenghts[0x90] = 0x0013; // Open Course Gump
            lenghts[0x91] = 0x0041; // Post Login
            lenghts[0x92] = Dynamic; // Update Multi
            lenghts[0x93] = 0x0063; // Book Header
            lenghts[0x94] = Dynamic; // Update Skill
            lenghts[0x95] = 0x0009; // Hue Picker
            lenghts[0x96] = Dynamic; // Game Central Monitor
            lenghts[0x97] = 0x0002; // Move Player
            lenghts[0x98] = Dynamic; // MOB Name
            lenghts[0x99] = 0x001A; // Target Multi
            lenghts[0x9A] = Dynamic; // Text Entry
            lenghts[0x9B] = 0x0102; // Request Assistance
            lenghts[0x9C] = 0x0135; // Assist Request
            lenghts[0x9D] = 0x0033; // GM Single
            lenghts[0x9E] = Dynamic; // Shop Sell
            lenghts[0x9F] = Dynamic; // Shop Offer
            lenghts[0xA0] = 0x0003; // Server Select
            lenghts[0xA1] = 0x0009; // HP Health
            lenghts[0xA2] = 0x0009; // Mana Health
            lenghts[0xA3] = 0x0009; // Fat Health
            lenghts[0xA4] = 0x0095; // Hardware Info
            lenghts[0xA5] = Dynamic; // Web Browser
            lenghts[0xA6] = Dynamic; // Message
            lenghts[0xA7] = 0x0004; // Request Tip
            lenghts[0xA8] = Dynamic; // Server List
            lenghts[0xA9] = Dynamic; // Character List
            lenghts[0xAA] = 0x0005; // Current Target
            lenghts[0xAB] = Dynamic; // String Query
            lenghts[0xAC] = Dynamic; // String Response
            lenghts[0xAD] = Dynamic; // Speech Unicode
            lenghts[0xAE] = Dynamic; // Text Unicode
            lenghts[0xAF] = 0x000D; // Death Animation
            lenghts[0xB0] = Dynamic; // Generic Gump
            lenghts[0xB1] = Dynamic; // Generic Gump Trigger
            lenghts[0xB2] = Dynamic; // Chat Message
            lenghts[0xB3] = Dynamic; // Chat Text
            lenghts[0xB4] = Dynamic; // Target Object List
            lenghts[0xB5] = 0x0040; // Open Chat
            lenghts[0xB6] = 0x0009; // Help Request
            lenghts[0xB7] = Dynamic; // Help Text
            lenghts[0xB8] = Dynamic; // Character Profile
            lenghts[0xB9] = 0x0003; // Features
            lenghts[0xBA] = 0x0006; // Pointer
            lenghts[0xBB] = 0x0009; // Account ID
            lenghts[0xBC] = 0x0003; // Game Season
            lenghts[0xBD] = Dynamic; // Client Version
            lenghts[0xBE] = Dynamic; // Assist Version
            lenghts[0xBF] = Dynamic; // Generic Command
            lenghts[0xC0] = 0x0024; // Hued FX
            lenghts[0xC1] = Dynamic; // Localized Text
            lenghts[0xC2] = Dynamic; // Unicode Text Entry
            lenghts[0xC3] = Dynamic; // Global Queue
            lenghts[0xC4] = 0x0006; // Semivisible
            lenghts[0xC5] = 0x00CB; // Invalid Map
            lenghts[0xC6] = 0x0001; // Invalid Map Enable
            lenghts[0xC7] = 0x0031; // Particle Effect
            lenghts[0xC8] = 0x0002; // Change Update Range
            lenghts[0xC9] = 0x0006; // Trip Time
            lenghts[0xCA] = 0x0006; // UTrip Time
            lenghts[0xCB] = 0x0007; // Global Queue Count
            lenghts[0xCC] = Dynamic; // Localized Text Plus String
            lenghts[0xCD] = 0x0001; // Unknown God Packet
            lenghts[0xCE] = Dynamic; // IGR Client
            lenghts[0xCF] = 0x004E; // IGR Login
            lenghts[0xD0] = Dynamic; // IGR Configuration
            lenghts[0xD1] = 0x0002; // IGR Logout
            lenghts[0xD2] = 0x0019; // Update Mobile
            lenghts[0xD3] = Dynamic; // Show Mobile
            lenghts[0xD4] = Dynamic; // Book Info
            lenghts[0xD5] = Dynamic; // Unknown Client Packet
            lenghts[0xD6] = Dynamic; // Mega Cliloc
            lenghts[0xD7] = Dynamic; // AOS Command
            lenghts[0xD8] = Dynamic; // Custom House
            lenghts[0xD9] = 0x010C; // Metrics
            lenghts[0xDA] = Dynamic; // Mahjong
            lenghts[0xDB] = Dynamic; // Character Transfer Log
            lenghts[0xDC] = 0x0009; // Equipment Description
            lenghts[0xDD] = Dynamic; // Compressed Gump
            lenghts[0xDE] = Dynamic; // Update Mobile Status
            lenghts[0xDF] = Dynamic; // Buff/Debuff System
            lenghts[0xE0] = Dynamic; // Bug Report (KR)
            lenghts[0xE1] = 0x0009; // Character List Update (KR)
            lenghts[0xE2] = 0x000A; // Mobile status/Animation update (KR)
            lenghts[0xF0] = Dynamic;
        }
    }
}
