namespace Test
{
    partial class Form1
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
            this.calendar1 = new TrainingSchedule.Calendar();
            this.SuspendLayout();
            // 
            // calendar1
            // 
            this.calendar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calendar1.Location = new System.Drawing.Point(0, 0);
            this.calendar1.MacrocycleMinDuration = System.TimeSpan.Parse("28.00:00:00");
            this.calendar1.MesocycleMinDuration = System.TimeSpan.Parse("7.00:00:00");
            this.calendar1.MicrocycleMinDuration = System.TimeSpan.Parse("2.00:00:00");
            this.calendar1.Name = "calendar1";
            this.calendar1.SeasonMinDuration = System.TimeSpan.Parse("84.00:00:00");
            this.calendar1.Size = new System.Drawing.Size(1193, 425);
            this.calendar1.TabIndex = 0;
            this.calendar1.Text = "calendar1";
            this.calendar1.ChartsClick += new System.EventHandler(this.calendar1_ChartsClick);
            this.calendar1.NewTrainingSessionButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_NewTrainingSessionButtonClick);
            this.calendar1.NewMacrocycleButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_NewMacrocycleButtonClick);
            this.calendar1.NewMesocycleButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_NewMesocycleButtonClick);
            this.calendar1.NewMicrocycleButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_NewMicrocycleButtonClick);
            this.calendar1.NewSeasonButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_NewSeasonButtonClick);
            this.calendar1.EditButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_EditButtonClick);
            this.calendar1.DeleteButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_DeleteButtonClick);
            this.calendar1.ReportButtonClick += new TrainingSchedule.Calendar.CalendarEventHandler(this.calendar1_ReportButtonClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 425);
            this.Controls.Add(this.calendar1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private TrainingSchedule.Calendar calendar1;



    }
}