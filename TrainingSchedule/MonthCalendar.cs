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
using System.Text;
using System.Drawing;

namespace TrainingSchedule
{

    internal class MonthCalendar
    {

        #region fields

        private DateTime _date;
        private Rectangle _monthBar;
        private Area _previousButton; 
        private Area _nextButton;
        private Area[] _days;
        private Point _location;

        private static Size _daySize;
        private static Size _size;
        private static DayOfWeek _firstDay;
        
        #endregion

        #region constructor

        internal MonthCalendar(DateTime date)
        {            
            bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES";

            if (date.Day != 1)
            {
                date = new DateTime(date.Year, date.Month, 1);
            }

            _date = date;
            _days = new Area[6 * 7];
            _firstDay = spanish ? DayOfWeek.Monday : DayOfWeek.Sunday;

            int[] dias = { 0, 1, 2, 3, 4, 5, 6 };
            int ds = (int)date.DayOfWeek;
            DateTime curDate = date.Subtract(new TimeSpan(dias[ds] - (int)_firstDay, 0, 0, 0));
            DateTime curDateTemp = curDate;
            for (int i = 0; i < 7; i++)
            {
                _days[i] = new Area(curDate);
                _days[i].Text = curDateTemp.ToString("ddd").Substring(0, 1).ToUpper();
                if (spanish && curDateTemp.DayOfWeek == DayOfWeek.Wednesday) _days[i].Text =  "X";
                _days[i].Font = Calendar.DefaultFont;               
                _days[i].Selectable = false;
                _days[i].BackgroundColor = Color.Empty;
                curDateTemp = curDateTemp.AddDays(1);
            }
            for (int i = 7; i < _days.Length; i++)
            {
                _days[i] = new Area(curDate); 
                _days[i].Text = curDate.Day.ToString();
                _days[i].Font = Calendar.DefaultFont; 
                _days[i].Selectable = true;
                _days[i].BackgroundColor = SystemColors.ControlLight;
                if (_date.Month != _days[i].Date.Month) { _days[i].TextColor = SystemColors.GrayText; } 
                curDate = curDate.AddDays(1);
                
            }
        }

        #endregion

        #region properties

        internal Rectangle Bounds
        {
            get { return new Rectangle(Location, Size); }
        }

        internal Area PreviousButton
        {
            get { return _previousButton; }
        }

        internal Area NextButton
        {
            get { return _nextButton; }
        }
        
        internal static Size DaySize
        {
            get { return _daySize; }
            set { _daySize = value; }
        }

        internal Point Location
        {
            get { return _location; }
        }

        internal static Size Size
        {
            get { return _size; }
            set { _size = value; }
        }
        
        internal static DayOfWeek FirstDayOfWeek
        {
            get { return _firstDay; }
            set { _firstDay = value; }
        }

        internal Area[] Days
        {
            get { return _days; }
            set { _days = value; }
        }

        internal Rectangle MonthBar
        {
            get { return _monthBar; }
            set { _monthBar = value; }
        }

        internal DateTime Date
        {
            get { return _date; }
        }

        #endregion

        internal void SetCalendar(Point location)
        {
            int x = location.X;
            int y = location.Y;

            _location = location;

            _monthBar = new Rectangle(location, new Size(_size.Width, 20));

            y = location.Y + _daySize.Height+5;
            
            for (int i = 0; i < _days.Length; i++)
            {
                _days[i].Bounds =new Rectangle(new Point(x, y), _daySize);

                x += _daySize.Width;

                if ((i + 1) % 7 == 0)
                {
                    x = location.X;
                    y += _daySize.Height;
                }
            }

            _previousButton = new Area(new Rectangle(Bounds.Left + 2, Bounds.Top + 2, _daySize.Height - 2, _daySize.Height - 2));
            _nextButton = new Area(new Rectangle(Bounds.Right - 2 - _previousButton.Bounds.Width, Bounds.Top + 2, _previousButton.Bounds.Width, _previousButton.Bounds.Height));
        }

    }
}
