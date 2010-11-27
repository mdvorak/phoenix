using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Phoenix.WorldData;

namespace Phoenix.Gui.Controls
{
    delegate void InsertTextDelegate(object sender, string text);

    partial class CommandBuilderDialog : Form
    {
        public event InsertTextDelegate InsertText;

        public CommandBuilderDialog()
        {
            InitializeComponent();
        }

        protected virtual void OnInsertText(string text)
        {
            DelegateInvoker.Invoke(InsertText, this, text);
        }

        private void serialButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SerialCallback));
        }

        private void SerialCallback(object unused)
        {
            UO.Print("Select object:");
            Serial serial = UIManager.TargetObject();

            if (serial.IsValid)
                OnInsertText(serial.ToString() + " ");
            else
                UO.PrintWarning("Invalid object selected.");
        }

        private void graphicButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(GraphicCallback));
        }

        private void GraphicCallback(object unused)
        {
            UO.Print("Select object:");
            UOObject obj = World.GetObject(UIManager.TargetObject());

            if (obj.Exist) {
                Graphic graphic;

                if (obj is UOItem)
                    graphic = ((UOItem)obj).Graphic;
                else
                    graphic = ((UOCharacter)obj).Model;

                OnInsertText(graphic.ToString() + " ");
            }
            else
                UO.PrintWarning("Invalid object selected.");
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ColorCallback));
        }

        private void ColorCallback(object unused)
        {
            UO.Print("Select object:");
            UOObject obj = World.GetObject(UIManager.TargetObject());

            if (obj.Exist) {
                OnInsertText(obj.Color.ToString() + " ");
            }
            else
                UO.PrintWarning("Invalid object selected.");
        }

        private void graphicColorButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(GraphicColorCallback));
        }

        private void GraphicColorCallback(object unused)
        {
            UO.Print("Select object:");
            UOObject obj = World.GetObject(UIManager.TargetObject());

            if (obj.Exist) {
                Graphic graphic;

                if (obj is UOItem)
                    graphic = ((UOItem)obj).Graphic;
                else
                    graphic = ((UOCharacter)obj).Model;

                OnInsertText(String.Format("{0} {1} ", graphic, obj.Color));
            }
            else
                UO.PrintWarning("Invalid object selected.");
        }
    }
}