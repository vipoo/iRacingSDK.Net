using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample
{
    public partial class LogMessages : Form
    {
        private Thread operation;
        public LogMessages()
        {
            InitializeComponent();
        }

        public TextBox TraceMessage
        {
            get
            {
                return this.TraceMessageTextBox;
            }
        }

        private void LogMessages_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (operation != null)
                operation.Abort();

            operation = null;

            e.Cancel = true;
            this.Hide();
        }

        public void StartOperation(Action action)
        {
            if (operation != null)
                operation.Abort();

            operation = new Thread(() => action());
            operation.Start();
            TraceMessage.Text = "";
            this.Show();
        }

        private void LogMessages_Resize(object sender, EventArgs e)
        {
            TraceMessage.Width = this.Width - TraceMessage.Left * 3;
            TraceMessage.Height = this.Height - TraceMessage.Top - TraceMessage.Left*4;
            CancelButton.Left = this.Width - CancelButton.Width - TraceMessage.Left * 3;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (operation != null)
                operation.Abort();

            operation = null;
            this.Close();
        }

        private void LogMessages_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (operation != null)
                operation.Abort();
        }
    }

    public class MyListener : TraceListener
    {
        static TextBox textBox;
        static SynchronizationContext context;

        public MyListener(TextBox textBox)
        {
            context = SynchronizationContext.Current;
            MyListener.textBox = textBox;
        }

        public static void Clear()
        {
            context.Post(ignore =>
            {
                textBox.Text = "";
            }, null);
        }

        public override void Write(string message, string category)
        {
            if (category == "DEBUG")
                return;

            WriteInfo(string.Format("{0} {1}", category, message));
        }

        public override void WriteLine(string message, string category)
        {
            if (category == "DEBUG")
                return;
            
            WriteInfoLine(string.Format("{0} {1}", category, message));
        }

        public void WriteInfo(string message)
        {
            context.Post(ignore =>
            {
                textBox.Text = textBox.Text + message;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }, null);
        }

        public void WriteInfoLine(string message)
        {
            WriteInfo(message);
            WriteInfo("\r\n");
        }

        public override void Write(string message)
        {
            WriteInfo(message);
        }

        public override void WriteLine(string message)
        {
            WriteInfoLine(message);
        }
    }

}
