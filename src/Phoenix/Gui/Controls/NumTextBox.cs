using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Controls
{
    [ToolboxBitmap(typeof(TextBox))]
    public class NumTextBox : TextBox
    {
        private int value;
        private int minValue;
        private int maxValue;
        private bool showHexPrefix;
        private string formatString;

        public NumTextBox()
        {
            value = 0;
            minValue = 0;
            maxValue = 100;
            showHexPrefix = false;
            formatString = "";
        }

        private void UpdateValue(int value)
        {
            int newValue = Math.Max(Math.Min(value, maxValue), minValue);

            if (showHexPrefix)
            {
                Text = "0x" + newValue.ToString(formatString);
            }
            else
            {
                Text = newValue.ToString(formatString);
            }

            if (this.value != newValue)
            {
                this.value = newValue;
                OnValueChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        [Browsable(false)]
        public override bool Multiline
        {
            get { return base.Multiline; }
            set { base.Multiline = false; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Value of this control.")]
        [DefaultValue(0)]
        public int Value
        {
            get { return this.value; }
            set
            {
                UpdateValue(value);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Minimal value of this control.")]
        [DefaultValue(0)]
        public int MinValue
        {
            get { return minValue; }
            set
            {
                if (minValue != value)
                {
                    minValue = value;
                    maxValue = Math.Max(minValue, maxValue);
                    UpdateValue(this.value);
                    OnMinValueChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximal value of this control.")]
        [DefaultValue(100)]
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue != value)
                {
                    maxValue = value;
                    minValue = Math.Min(minValue, maxValue);
                    UpdateValue(this.value);
                    OnMaxValueChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("There will be always inserted '0x' before number.")]
        [DefaultValue(false)]
        public bool ShowHexPrefix
        {
            get { return showHexPrefix; }
            set
            {
                if (showHexPrefix != value)
                {
                    showHexPrefix = value;
                    UpdateValue(this.value);
                    OnShowHexPrefixChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Format string used to convert Value to string. See \"Numeric Format Strings\" for details")]
        [DefaultValue("")]
        public string FormatString
        {
            get { return formatString; }
            set
            {
                if (formatString != value)
                {
                    formatString = value;
                    UpdateValue(this.value);
                    OnFormatStringChanged(EventArgs.Empty);
                }
            }
        }

        protected int ParseInt32(string text)
        {
            if (text.StartsWith("0x"))
                return Int32.Parse(text.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
            else
                return Int32.Parse(text);
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            try
            {
                Value = ParseInt32(Text);
            }
            catch (Exception)
            {
                UpdateValue(Value);
            }

            base.OnValidating(e);
        }

        protected void Validate()
        {
            CancelEventArgs e = new CancelEventArgs();
            OnValidating(e);
            if (!e.Cancel)
            {
                OnValidated(EventArgs.Empty);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Validate();
            }
        }

        #region Events

        public event EventHandler ValueChanged;
        public event EventHandler MinValueChanged;
        public event EventHandler MaxValueChanged;
        public event EventHandler ShowHexPrefixChanged;
        public event EventHandler FormatStringChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            SyncEvent.Invoke(ValueChanged, this, e);
        }

        protected virtual void OnMinValueChanged(EventArgs e)
        {
            SyncEvent.Invoke(MinValueChanged, this, e);
        }

        protected virtual void OnMaxValueChanged(EventArgs e)
        {
            SyncEvent.Invoke(MaxValueChanged, this, e);
        }

        protected virtual void OnShowHexPrefixChanged(EventArgs e)
        {
            SyncEvent.Invoke(ShowHexPrefixChanged, this, e);
        }

        protected virtual void OnFormatStringChanged(EventArgs e)
        {
            SyncEvent.Invoke(FormatStringChanged, this, e);
        }

        #endregion
    }
}
