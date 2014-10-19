namespace Sample
{
    partial class LogMessages
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TraceMessageTextBox = new System.Windows.Forms.TextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TraceMessageTextBox
            // 
            this.TraceMessageTextBox.BackColor = System.Drawing.Color.Black;
            this.TraceMessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TraceMessageTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TraceMessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TraceMessageTextBox.ForeColor = System.Drawing.Color.Lime;
            this.TraceMessageTextBox.Location = new System.Drawing.Point(12, 47);
            this.TraceMessageTextBox.MaxLength = 1040000;
            this.TraceMessageTextBox.Multiline = true;
            this.TraceMessageTextBox.Name = "TraceMessageTextBox";
            this.TraceMessageTextBox.ReadOnly = true;
            this.TraceMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TraceMessageTextBox.Size = new System.Drawing.Size(1105, 469);
            this.TraceMessageTextBox.TabIndex = 5;
            this.TraceMessageTextBox.TabStop = false;
            this.TraceMessageTextBox.WordWrap = false;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(1042, 12);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // LogMessages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1129, 528);
            this.Controls.Add(this.TraceMessageTextBox);
            this.Controls.Add(this.CancelButton);
            this.DoubleBuffered = true;
            this.Name = "LogMessages";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "LogMessages";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogMessages_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogMessages_FormClosed);
            this.Resize += new System.EventHandler(this.LogMessages_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TraceMessageTextBox;
        private new System.Windows.Forms.Button CancelButton;
    }
}