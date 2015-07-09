using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrainingSchedule;

namespace Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            GetTrainingItems();
        }

        public Form2(ObservableCollection<TrainingItem> items)
        {
            InitializeComponent();
            gantt1.Items = items; 
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            gantt1.AthleteOrTeamName = "fictitius athlete";
        }

        private void GetTrainingItems()
        {
            TrainingItem season = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(6).AddHours(23).AddMinutes(59),
                TrainingSchedule.TrainingEvent.Season);
            season.Objective = "Here goes the athlete's objective for this season";

            TrainingItem macrocycle = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(6).AddHours(23).AddMinutes(59),
                 "Macrocycle", TrainingSchedule.TrainingEvent.Macrocycle);

            TrainingItem mesocycle = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddHours(23).AddMinutes(59),
                 "Mesocycle", TrainingSchedule.TrainingEvent.Mesocycle);
            mesocycle.Objective = "Since a objective has been defined, the mesocycle bar is coloured";
            mesocycle.Orientation = "Realization";

            TrainingItem microcycle = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddDays(-23).AddHours(21),
                 "Microcycle", TrainingSchedule.TrainingEvent.Microcycle);
            microcycle.Objective = "Since a objective has been defined, the microcycle bar is coloured";
            microcycle.Orientation = "Competition";
            
            TrainingItem trainingSession1 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(9), DateTime.Today.AddMonths(-1).AddHours(14),
                "Training session #1", TrainingEvent.TrainingSession);
            trainingSession1.Objective = "here goes the objective for this training session";

            TrainingItem trainingSession2 = new TrainingItem(DateTime.Today.AddMonths(-1).AddHours(18), DateTime.Today.AddMonths(-1).AddHours(23),
                "Training session #2", TrainingEvent.TrainingSession);
            trainingSession2.Objective = "here goes the objective for this training session";

            TrainingItem trainingSession3 = new TrainingItem(DateTime.Today.AddDays(-23).AddHours(10), DateTime.Today.AddDays(-23).AddHours(20),
                "Training session #3", TrainingEvent.TrainingSession);
            trainingSession3.Objective = "here goes the objective for this training session";

            gantt1.Items.Add(season);
            
            gantt1.Items.Add(macrocycle);
            gantt1.Items.Add(mesocycle);
            gantt1.Items.Add(microcycle);

            gantt1.Items.Add(trainingSession1);
            gantt1.Items.Add(trainingSession2);
            gantt1.Items.Add(trainingSession3);
             
        }
    }
}
