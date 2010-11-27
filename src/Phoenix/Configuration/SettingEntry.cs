using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;


namespace Phoenix.Configuration
{
    /// <summary>
    /// Basic class for settings entries.
    /// </summary>
    /// <typeparam name="T">Any type supported by Settings class.</typeparam>
    public abstract class SettingEntry<T>
    {
        /// <summary>
        /// Value itself
        /// </summary>
        protected T value;
        /// <summary>
        /// Default value.
        /// </summary>
        protected T defaultValue;
        /// <summary>
        /// Attribute name.
        /// </summary>
        protected string attribute;
        /// <summary>
        /// Element path.
        /// </summary>
        protected object[] path;

        private DefaultPublicEvent changed;
        private DefaultPublicEvent saving;
        /// <summary>
        /// Raised when setting value was changed.
        /// </summary>
        public event EventHandler Changed
        {
            add { changed.AddHandler(value); }
            remove { changed.RemoveHandler(value); }
        }
        /// <summary>
        /// Raised when setting is being saved.
        /// </summary>
        public event EventHandler Saving
        {
            add { saving.AddHandler(value); }
            remove { saving.RemoveHandler(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingEntry(ISettings settings, T defaultValue, string attribute, params object[] path)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            settings.Loaded += new EventHandler(settings_Loaded);
            settings.Saving += new EventHandler(settings_Saving);

            this.path = path;
            this.attribute = attribute;
            this.defaultValue = defaultValue;
            this.value = defaultValue;

            changed = new DefaultPublicEvent();
            saving = new DefaultPublicEvent();
        }

        /// <summary>
        /// Element path.
        /// </summary>
        /// <value></value>
        public object[] Path
        {
            get { return path; }
        }

        /// <summary>
        /// Attribute name.
        /// </summary>
        /// <value></value>
        public string Attribute
        {
            get { return attribute; }
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public abstract T Value { get; set; }

        /// <summary>
        /// Sets value without invoking changed event. Use carrefully.
        /// </summary>
        /// <param name="val">New setting value.</param>
        public virtual void SetValue(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Sets default value.
        /// </summary>
        public void Reset()
        {
            Value = defaultValue;
        }

        /// <summary>
        /// Raises Changed event.
        /// </summary>
        /// <param name="e">Additional event argumetns.</param>
        protected virtual void OnChanged(EventArgs e)
        {
            changed.Invoke(this, e);
        }

        /// <summary>
        /// Raises Changing event.
        /// </summary>
        /// <param name="e">Additional event argumetns.</param>
        protected virtual void OnSaving(EventArgs e)
        {
            saving.Invoke(this, e);
        }

        /// <summary>
        /// ISettings.Loaded handler. Value should be loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected abstract void settings_Loaded(object sender, EventArgs e);

        /// <summary>
        /// Settings.Saving handler. Value should be stored here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected virtual void settings_Saving(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            OnSaving(e);
            settings.SetAttribute(value, attribute, path);
        }

        /// <summary>
        /// Returns string representation of Value.
        /// </summary>
        /// <returns>Human-readable value.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Default operator to T type.
        /// </summary>
        public static implicit operator T(SettingEntry<T> obj)
        {
            return obj.Value;
        }
    }


    /// <summary>
    /// SettingEntry implementation for String.
    /// </summary>
    public class SettingStringEntry : SettingEntry<String>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingStringEntry(ISettings settings, string defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override String Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            Value = settings.GetAttribute(defaultValue, attribute, path);
        }
    }

    /// <summary>
    /// SettingEntry implementation for Int32.
    /// </summary>
    public class SettingInt32Entry : SettingEntry<Int32>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingInt32Entry(ISettings settings, int defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override Int32 Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            Value = settings.GetAttribute(defaultValue, attribute, path);
        }
    }

    /// <summary>
    /// SettingEntry implementation for UInt16.
    /// </summary>
    public class SettingUInt16Entry : SettingEntry<UInt16>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingUInt16Entry(ISettings settings, ushort defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override UInt16 Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            Value = (ushort)settings.GetAttribute(defaultValue, attribute, path);
        }
    }

    /// <summary>
    /// SettingEntry implementation for float.
    /// </summary>
    public class SettingSingleEntry : SettingEntry<Single>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingSingleEntry(ISettings settings, Single defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override Single Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            Value = settings.GetAttribute(defaultValue, attribute, path);
        }
    }

    /// <summary>
    /// SettingEntry implementation for Boolean.
    /// </summary>
    public class SettingBoolEntry : SettingEntry<Boolean>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingBoolEntry(ISettings settings, bool defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override bool Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            Value = settings.GetAttribute(defaultValue, attribute, path);
        }
    }

    /// <summary>
    /// SettingEntry implementation for Enum type.
    /// </summary>
    public class SettingEnumEntry<T> : SettingEntry<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingEnumEntry(ISettings settings, T defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override T Value
        {
            get { return this.value; }
            set
            {
                IComparable compare = (IComparable)this.value;

                if (compare.CompareTo(value) != 0)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            Value = settings.GetEnumAttribute<T>(defaultValue, attribute, path);
        }
    }

    /// <summary>
    /// SettingEntry implementation for Point.
    /// </summary>
    public class SettingPointEntry : SettingEntry<Point>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingPointEntry(ISettings settings, Point defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override Point Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            string str = settings.GetAttribute(defaultValue.ToString(), attribute, path);
            Value = ParsePoint(str);
        }


        /// <summary>
        /// To speedup Size string parsing regex is precompiled.
        /// </summary>
        private static Regex xRegex = new Regex(@"\bX\s*=\s*(?<X>-?\d+)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static Regex yRegex = new Regex(@"\bY\s*=\s*(?<Y>-?\d+)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Finds Point object in string.
        /// </summary>
        /// <param name="text">Input string.</param>
        /// <returns>Point object containing read value.</returns>
        public static Point ParsePoint(string text)
        {
            Match xMatch = xRegex.Match(text);
            Match yMatch = yRegex.Match(text);

            if (xMatch == null || !xMatch.Success || yMatch == null || !yMatch.Success)
                throw new FormatException("Invalid string format for Point type.");

            Point pt = new Point();

            pt.X = Int32.Parse(xMatch.Groups["X"].Value);
            pt.Y = Int32.Parse(yMatch.Groups["Y"].Value);

            return pt;
        }
    }

    /// <summary>
    /// SettingEntry implementation for Size.
    /// </summary>
    public class SettingSizeEntry : SettingEntry<Size>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settings">ISettings which will be object assigned to.</param>
        /// <param name="defaultValue">Default value used when settings entry is missing.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="path">Element path.</param>
        public SettingSizeEntry(ISettings settings, Size defaultValue, string attribute, params object[] path)
            : base(settings, defaultValue, attribute, path)
        {
        }

        /// <summary>
        /// Gets or sets value for this setting. If changed Changed event is raised.
        /// </summary>
        /// <value>Type depending on inherited class.</value>
        public override Size Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ISettings.Loaded handler. Value is loaded here.
        /// </summary>
        /// <param name="sender">Settings caller target.</param>
        /// <param name="e">Event parameters.</param>
        protected override void settings_Loaded(object sender, EventArgs e)
        {
            ISettings settings = (ISettings)sender;
            string str = settings.GetAttribute(defaultValue.ToString(), attribute, path);
            Value = ParseSize(str);
        }


        /// <summary>
        /// To speedup Size string parsing regex is precompiled.
        /// </summary>
        private static Regex sizeParseRegex = new Regex(@"(\bWidth\s*=\s*(?<Width>\d+)\b|\bHeight\s*=\s*(?<Height>\d+)\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Finds Size object in string.
        /// </summary>
        /// <param name="text">Input string.</param>
        /// <returns>Size object containing read value.</returns>
        public static Size ParseSize(string text)
        {
            MatchCollection matches = sizeParseRegex.Matches(text);
            if (matches == null) throw new FormatException("Invalid string format for Size type.");

            Size size = new Size();

            foreach (Match match in matches)
            {
                if (match.Groups["Width"].Success)
                    size.Width = Int32.Parse(match.Groups["Width"].Value);
                if (match.Groups["Height"].Success)
                    size.Height = Int32.Parse(match.Groups["Height"].Value);
            }

            return size;
        }
    }
}
