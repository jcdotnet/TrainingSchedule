/*
 * TRAINING SCHEDULE CLASS LIBRARY Copyright (C) 2014 Jose Carlos Román Rubio (jcdotnet@hotmail.com)
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser Gereral Public Licence as published by the Free Software Foundation; 
 * either version 3 of the Licence, or (at your opinion) any later version.
 * 
 * This library is distributed in the hope that it will be usefull, but WITHOUT ANY WARRANTY; without even the implied warranty of merchantability or fitness for a particular purpose. 
 * See the GNU Lesser General Public Licence for more details.
 *
 * You should have received a copy of the GNU General Public License along with this library.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TrainingSchedule
{

    #region enum

    public enum TrainingEvent
    {
        Other,
        Season,
        Macrocycle,
        Mesocycle,
        Microcycle,
        TrainingSession,
        TrainingNote
    }
    #endregion

    /// <summary>
    /// Represents a training item 
    /// </summary>
    [Serializable]
    public class TrainingItem
    {
        #region static fields

        internal static TimeSpan seasonMinDuration = new TimeSpan(28 * 3, 0, 0, 0);
        internal static TimeSpan macrocycleMinDuration = new TimeSpan(28, 0, 0, 0);
        internal static TimeSpan mesocycleMinDuration = new TimeSpan(7, 0, 0, 0);
        internal static TimeSpan microcycleMinDuration = new TimeSpan(2, 0, 0, 0);
        internal static TimeSpan trainingSessionMinDuration = new TimeSpan(0, 1, 0);

        #endregion

        #region events

        public delegate void ItemEventHandler(object sender, ItemEventArgs e);
        public event ItemEventHandler DatesChanged;

        #endregion

        #region fields

        private Color _background;
        private DateTime _startDate;
        private DateTime _endDate;
        
        private Rectangle _bounds;
        private TrainingEvent _type;        
        private string _barText;
        private string _objective;
        private string _orientation; // in case the item is a mesocycle or microcycle
        private string _trainingContent; // saves training content in a cycle or observations in a training session

        private bool _isValid;
        private object _tag;

        #endregion

        #region properties

        public DateTime StartDate
        {
            get { return _startDate; }
            set {
                ItemEventArgs evt=null;
                if (_startDate != null)
                {
                    evt = new ItemEventArgs();
                    evt.OldItemStartDate = _startDate;// passes the old start date 
                }
                _startDate = value; // set the new dates
                if (_type == TrainingEvent.TrainingSession) _endDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, 23, 59, 0);
                if (_startDate != null && DatesChanged != null) DatesChanged(this, evt);
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                ItemEventArgs evt = null;
                if (_endDate != null)
                {
                    evt = new ItemEventArgs();
                    evt.OldItemEndDate = _endDate;// passes the old end date
                }
                if (this.Type != TrainingEvent.TrainingNote) _endDate = value;
                if (this.Type != TrainingEvent.TrainingNote && _endDate != null && DatesChanged != null) DatesChanged(this, evt);
            }
        }

        public TrainingEvent Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Color Background
        {
            get { return _background; }
            set { _background = value; }
        }

        public string Text        
        {
            get { return _barText; }
            set { _barText = value; }
        }

        public string Objective
        {
            get { return _objective; }
            set { _objective = value; /*GetBackground();*/ }
        }

        public string Orientation { get { return _orientation; } set { _orientation = value; } }
        public string TrainingContent { get { return _trainingContent; } set { _trainingContent = value; } }
        public object Tag { get { return _tag; } set { _tag = value; } }

        internal Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        internal bool isValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }

        #endregion

        #region constructors

        public TrainingItem(DateTime startDate, DateTime endDate, TrainingEvent type)
        {
            _startDate = startDate;
            _endDate = endDate;
            _type = type;
        }

        public TrainingItem(DateTime startDate, DateTime endDate, string text, TrainingEvent type)
            :this(startDate, endDate, type)
        {
            _barText = text;         
        }

        #endregion

        #region static methods

        public static string ToString(TrainingEvent type, bool spanish)
        {

            switch (type)
            {
                case TrainingEvent.Season: return spanish? "temporada": "season";
                case TrainingEvent.Macrocycle: return spanish ? "macrociclo" : "macrocycle";
                case TrainingEvent.Mesocycle: return spanish ? "mesociclo" : "mesocycle";
                case TrainingEvent.Microcycle: return spanish ? "microciclo" : "microcycle";
                case TrainingEvent.TrainingSession: return spanish ? "sesión" : "session";
                case TrainingEvent.TrainingNote: return spanish ? "nota" : "note";
            }
            return null;
        }
        public static bool CheckDuration(TrainingEvent type, TimeSpan newDuration, ObservableCollection<TrainingItem> items)
        {
            foreach (TrainingItem item in items)
            {
                if (item.Type == type)
                {
                    if (item.EndDate.Subtract(item.StartDate) < newDuration) return false;
                }
            }
            return true;
        }

        #endregion

    }

    #region ItemEventArgs

    public class ItemEventArgs : EventArgs
    {
        private DateTime _oldItemStartDate = DateTime.MinValue;
        private DateTime _oldItemEndDate = DateTime.MinValue;

        public DateTime OldItemStartDate
        {
            get { return _oldItemStartDate; }
            set { _oldItemStartDate = value; }
        }

        public DateTime OldItemEndDate
        {
            get { return _oldItemEndDate; }
            set { _oldItemEndDate = value; }
        }
    }

   #endregion

}
