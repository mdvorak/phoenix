using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Configuration;

namespace PhoenixLauncher.Data
{
    public enum LaunchEventType
    {
        Run,
        RunOnce
    }

    public enum LaunchEventAction
    {
        Execute,
        ExecuteAndWait
    }

    public struct LaunchEvent
    {
        public LaunchEventType Type;
        public LaunchEventAction Action;
        public string Command;

        public static LaunchEvent[] LoadList(ISettings settings, params object[] path)
        {
            ElementInfo[] elements = settings.EnumarateElements(path + "Event");
            List<LaunchEvent> list = new List<LaunchEvent>();

            for (int i = 0; i < elements.Length; i++)
            {
                try
                {
                    LaunchEvent e = new LaunchEvent();

                    e.Type = (LaunchEventType)Enum.Parse(typeof(LaunchEventType), elements[i].Attributes["Type"].ToString(), true);
                    e.Action = (LaunchEventAction)Enum.Parse(typeof(LaunchEventAction), elements[i].Attributes["Action"].ToString(), true);
                    e.Command = elements[i].Attributes["Command"].ToString();

                    list.Add(e);
                }
                catch { }
            }

            return list.ToArray();
        }
    }
}
