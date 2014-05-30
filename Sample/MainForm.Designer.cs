namespace Sample
{
    partial class MainForm
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
            this.EventSampleButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EventSampleButton
            // 
            this.EventSampleButton.Location = new System.Drawing.Point(12, 12);
            this.EventSampleButton.Name = "EventSampleButton";
            this.EventSampleButton.Size = new System.Drawing.Size(136, 46);
            this.EventSampleButton.TabIndex = 0;
            this.EventSampleButton.Text = "Event Sample";
            this.EventSampleButton.UseVisualStyleBackColor = true;
            this.EventSampleButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 46);
            this.button1.TabIndex = 1;
            this.button1.Text = "Fastest Lap Sample";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 138);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(136, 46);
            this.button2.TabIndex = 2;
            this.button2.Text = "iRacingInstance access";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.iRacingInstance_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1337, 445);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.EventSampleButton);
            this.Name = "MainForm";
            this.Text = "Main";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button EventSampleButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}