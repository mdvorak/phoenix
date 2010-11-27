using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PhoenixLauncher
{
    static class Safe
    {
        private delegate void SetTextDelegate(Control control, string text);
        public static void SetText(Control control, string text)
        {
            if (control.InvokeRequired)
                control.Invoke(new SetTextDelegate(SetText), control, text);
            else
                control.Text = text;
        }

        private delegate void AppendTextDelegate(TextBoxBase control, string text);
        public static void AppendText(TextBoxBase control, string text)
        {
            if (control.InvokeRequired)
                control.Invoke(new AppendTextDelegate(AppendText), control, text);
            else
                control.AppendText(text);
        }

        private delegate void SetEnabledDelegate(Control control, bool value);
        public static void SetEnabled(Control control, bool value)
        {
            if (control.InvokeRequired)
                control.Invoke(new SetEnabledDelegate(SetEnabled), control, value);
            else
                control.Enabled = value;
        }

        private delegate void SetSelectionColorDelegate(RichTextBox control, System.Drawing.Color color);
        public static void SetSelectionColor(RichTextBox control, System.Drawing.Color color)
        {
            if (control.InvokeRequired)
                control.Invoke(new SetSelectionColorDelegate(SetSelectionColor), control, color);
            else
                control.SelectionColor = color;
        }

        private delegate void ResetTextDelegate(Control control);
        public static void ResetText(Control control)
        {
            if (control.InvokeRequired)
                control.Invoke(new ResetTextDelegate(ResetText), control);
            else
                control.ResetText();
        }

        private delegate void CloseDelegate(Form form);
        public static void Close(Form form)
        {
            if (form.InvokeRequired)
                form.Invoke(new CloseDelegate(Close), form);
            else
                form.Close();
        }

        private delegate void SetValueDelegate(ProgressBar control, int value);
        public static void SetValue(ProgressBar control, int value)
        {
            if (control.InvokeRequired)
                control.Invoke(new SetValueDelegate(SetValue), control, value);
            else
                control.Value = value;
        }

        private delegate void ShowHideDelegate(Control control);
        public static void Show(Control control)
        {
            if (control.InvokeRequired)
                control.Invoke(new ShowHideDelegate(Show), control);
            else
                control.Show();
        }

        public static void Hide(Control control)
        {
            if (control.InvokeRequired)
                control.Invoke(new ShowHideDelegate(Hide), control);
            else
                control.Hide();
        }
    }
}
