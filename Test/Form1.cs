using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrainingSchedule;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Let's insert some training items into the calendar

            TrainingItem season = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(6).AddHours(23).AddMinutes(59), 
                TrainingSchedule.TrainingEvent.Season);
            season.Objective = "Here goes the athlete's objective for this season";

            TrainingItem newFollowingSeason = new TrainingItem(DateTime.Today.AddMonths(7).AddHours(9), DateTime.Today.AddMonths(8).AddHours(23).AddMinutes(59),
                 TrainingSchedule.TrainingEvent.Season);

            TrainingItem newFollowingSeason2 = new TrainingItem(DateTime.Today.AddMonths(8).AddHours(9), DateTime.Today.AddMonths(16).AddHours(23).AddMinutes(59),
                 TrainingSchedule.TrainingEvent.Season);

            TrainingItem macrocycle1 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(6).AddHours(23).AddMinutes(59),
                 "Macrocycle", TrainingSchedule.TrainingEvent.Macrocycle);
            // macrocycle1.Objective = 
            // macrocycle1.TrainingContent =

            TrainingItem macrocycle2 = new TrainingItem(DateTime.Today.AddMonths(-6).AddHours(9), DateTime.Today.AddMonths(-2).AddHours(23).AddMinutes(59),
                 "Macrocycle", TrainingSchedule.TrainingEvent.Macrocycle);

            TrainingItem macrocycle3 = new TrainingItem(DateTime.Today, DateTime.Today.AddMonths(6).AddHours(23).AddMinutes(59),
                 "Macrocycle", TrainingSchedule.TrainingEvent.Macrocycle);

            TrainingItem mesocycle1 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(-1).AddDays(6).AddHours(9),
                 "Mesocycle", TrainingSchedule.TrainingEvent.Mesocycle);

            TrainingItem mesocycle2 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddHours(23).AddMinutes(59),
                 "Mesocycle", TrainingSchedule.TrainingEvent.Mesocycle);
            mesocycle2.Objective = "Since a objective has been defined, the mesocycle bar is coloured";
            mesocycle2.Orientation = "Realization";

            TrainingItem microcycle1 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddDays(-23).AddHours(21),
                 "Microcycle", TrainingSchedule.TrainingEvent.Microcycle);
            microcycle1.Objective = "Since a objective has been defined, the microcycle bar is coloured";
            microcycle1.Orientation = "Competition";
            // microcycle1.TrainingContent =

            TrainingItem trainingSession1 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(-1).AddHours(14),
                "Training session #1", TrainingEvent.TrainingSession);
            trainingSession1.Objective = "here goes the objective for this training session";

            TrainingItem trainingSession2 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(18), DateTime.Today.AddMonths(-1).AddHours(23),
                "Training session #2", TrainingEvent.TrainingSession);
            trainingSession2.Objective = "here goes the objective for this training session";
            
            TrainingItem trainingSession3 = new TrainingItem(DateTime.Today.AddDays(-23).AddHours(10), DateTime.Today.AddDays(-23).AddHours(20),
                "Training session #3", TrainingEvent.TrainingSession);
            trainingSession3.Objective = "here goes the objective for this training session";

            TrainingItem trainingSession4 = new TrainingItem(DateTime.Today.AddDays(-23).AddHours(22), DateTime.Today.AddDays(-23).AddHours(23),
                "Training session #3", TrainingEvent.TrainingSession);
            trainingSession3.Objective = "here goes the objective for this training session";

            calendar1.Items.Add(season); 
            calendar1.Items.Add(newFollowingSeason); // won't be inserted unless calendar.SeasonMinDuration is set to a value smaller than one month
            calendar1.Items.Add(newFollowingSeason2); 
            // no season will be inserted between season and newFollowingseason2 unless calendar.SeasonMinDuration is set to a value smaller than two months
            
            calendar1.Items.Add(macrocycle1);
            calendar1.Items.Add(macrocycle2); // won't be inserted because no season was created first
            calendar1.Items.Add(macrocycle3); // won't be inserted because intersects with macrocycle1

            calendar1.Items.Add(mesocycle1); // won't be inserted unless calendar.MesocycleMinDuration is set to a value smaller than 6 days
            calendar1.Items.Add(mesocycle2);

            /* No mycrocycle will be created today because mesocycle2 finished also today and calendar.MicrocycleMinDuration is 2 days
            ***** That's why the "new microcycle" button doesn't appear *****/ 

            calendar1.Items.Add(microcycle1);

            calendar1.Items.Add(trainingSession1);
            calendar1.Items.Add(trainingSession2);
            calendar1.Items.Add(trainingSession3);
            calendar1.Items.Add(trainingSession4); // won't be inserted because microcycle1 has already ended 
        }

        #region events

        private void calendar1_NewSeasonButtonClick(object sender, CalendarEventArgs e)
        {
            /* The argument e.NewItemStartDate passes the minimum start date for the season to create if there is any.
             * Once the season is created (by creating a TrainingItem instance and setting the property Type to TrainingEvent.Season) 
             * has to be added to calendar1.Items to be displayed in the calendar in case the cycle dates are valid */
            MessageBox.Show("Please read the information inside the event handler to learn how to add a new season");
        }

        private void calendar1_NewMacrocycleButtonClick(object sender, CalendarEventArgs e)
        {
            /* The argument e.NewItemStartDate passes the minimum start date for the macrocycle to create if there is any.
             * Once the macro is created (by creating a TrainingItem instance and setting the property Type to TrainingEvent.Macrocycle) 
             * has to be added to calendar1.Items to be displayed in the calendar in case the cycle dates are valid */
            MessageBox.Show("Please read the information inside the event handler to learn how to add a new macrocycle");
        }

        private void calendar1_NewMesocycleButtonClick(object sender, CalendarEventArgs e)
        {
            // same as new season and new macrocycle, see the handlers above
            MessageBox.Show("Please read the information inside the event handler to learn how to add a new mesocycle");
        }

        private void calendar1_NewMicrocycleButtonClick(object sender, CalendarEventArgs e)
        {
            // same as new season and new macrocycle, see the handlers above
            MessageBox.Show("Please read the information inside the event handler to learn how to add a new microcycle");
        }

        private void calendar1_NewTrainingSessionButtonClick(object sender, TrainingSchedule.CalendarEventArgs e)
        {
            // same as new season and new macrocycle, see the handlers above
            MessageBox.Show("Please read the information inside the event handler to learn how to add a new training session");
        }

        private void calendar1_EditButtonClick(object sender, CalendarEventArgs e)
        {
            /* The argument e.Item passes the training item to edit.
             * If the item dates are changed and the new dates are valid, the item will be redrawn
             * otherwise, the item duration will be unchanged. Any other changes will be reflected in the calendar */
            MessageBox.Show("Please read the information inside the event handler to learn how to edit a training item");
        }

        private void calendar1_ReportButtonClick(object sender, CalendarEventArgs e)
        {
            /* The argument e.Item passes the training item
             * The PDF report can be generated as you consider more appropiate */
            MessageBox.Show("Please read the information inside the event handler to learn how to create a PDF report");
        }

        private void calendar1_DeleteButtonClick(object sender, TrainingSchedule.CalendarEventArgs e)
        {
            /* The argument e.Item passes the training item to delete.
             * calendar1.Items.Remove(e.item) removes the item from the calendar */
            MessageBox.Show("Please read the information inside the event handler to learn how to delete a training item");
        }

        private void calendar1_ChartsClick(object sender, EventArgs e)
        {
            Form2 chartsForm = new Form2(calendar1.Items);
            chartsForm.ShowDialog();
        }

        #endregion

    }
}
