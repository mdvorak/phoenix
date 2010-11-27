using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MulLib;

namespace Phoenix
{
    public static class DataFiles
    {
        private static TileData tiledata;
        private static Hues hues;
        private static RadarCol radarCol;
        private static Skills skills;
        private static Art art;
        private static Multi multi;

        internal static event EventHandler Loaded;

        /// <summary>
        /// Called by Phoenix.Initialize().
        /// </summary>
        /// <param name="param">Client directory.</param>
        internal static void Load(object param)
        {
            string dir = param.ToString();

            try {
                Trace.WriteLine("Loading ultima data files started..", "MulLib");

                tiledata = TileData.Load(Path.Combine(dir, "tiledata.mul"));
                hues = Hues.Load(Path.Combine(dir, "hues.mul"));
                radarCol = RadarCol.Load(Path.Combine(dir, "radarcol.mul"));
                skills = Skills.Load(Path.Combine(dir, "skills.idx"), Path.Combine(dir, "skills.mul"));
                art = Art.Load(Path.Combine(dir, "artidx.mul"), Path.Combine(dir, "art.mul"), MulFileAccessMode.ReadOnly);
                multi = Multi.Load(Path.Combine(dir, "multi.idx"), Path.Combine(dir, "multi.mul"), MulFileAccessMode.ReadOnly);

                Trace.WriteLine("Loading ultima data files finished.", "MulLib");
            }
            catch (Exception e) {
                string msg = String.Format("Unable to load ultima data files. Program will be terminated. Exception:\r\n{0}", e);
                Trace.WriteLine(msg, "MulLib");
                MessageBox.Show(msg, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Core.Terminate();
            }

            try {
                SyncEvent.Invoke(Loaded, null, EventArgs.Empty);
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled exception in DataFiles.Loaded event. Exception:\r\n" + e.ToString(), "MulLib");
            }
        }

        internal static void Dispose()
        {
            if (tiledata != null) tiledata.Dispose();
            if (hues != null) hues.Dispose();
            if (radarCol != null) radarCol.Dispose();
            if (skills != null) skills.Dispose();
            if (art != null) art.Dispose();
        }

        public static TileData Tiledata
        {
            get { return tiledata; }
        }

        public static Hues Hues
        {
            get { return hues; }
        }

        public static RadarCol RadarCol
        {
            get { return radarCol; }
        }

        public static Skills Skills
        {
            get { return skills; }
        }

        public static Art Art
        {
            get { return art; }
        }

        public static Multi Multi
        {
            get { return multi; }
        }
    }
}
