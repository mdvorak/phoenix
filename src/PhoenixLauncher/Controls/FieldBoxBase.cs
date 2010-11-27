using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PhoenixLauncher.Controls
{
    public enum ErrorType
    {
        None,
        Warning,
        Error
    }

    [DesignTimeVisible(false)]
    public class FieldBoxBase : UserControl
    {
        private string errString = null;

        public FieldBoxBase()
        {
        }

        public virtual string Value
        {
            get { throw new Exception("The method or operation is not implemented."); }
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public virtual ErrorType Check()
        {
            return ErrorType.None;
        }

        public string ErrorString
        {
            get { return errString; }
            protected set { errString = value; }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FieldBoxBase
            // 
            this.Name = "FieldBoxBase";
            this.ResumeLayout(false);

        }
    }
}
