namespace Test
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.gantt1 = new TrainingSchedule.Gantt();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // gantt1
            // 
            this.gantt1.AthleteOrTeamName = null;
            this.gantt1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider1.SetHelpString(this.gantt1, "F1 provides help to the users, it\'s highly recommended implement this tool the wa" +
        "y you like by using the help provider control ");
            this.gantt1.Items = ((System.Collections.ObjectModel.ObservableCollection<TrainingSchedule.TrainingItem>)(resources.GetObject("gantt1.Items")));
            this.gantt1.Location = new System.Drawing.Point(0, 0);
            this.gantt1.Name = "gantt1";
            this.helpProvider1.SetShowHelp(this.gantt1, true);
            this.gantt1.Size = new System.Drawing.Size(700, 296);
            this.gantt1.TabIndex = 0;
            this.gantt1.Text = "gantt1";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 296);
            this.Controls.Add(this.gantt1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private TrainingSchedule.Gantt gantt1;
        private System.Windows.Forms.HelpProvider helpProvider1;


    }
}