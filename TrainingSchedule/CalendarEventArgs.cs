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
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrainingSchedule
{
    public class CalendarEventArgs:EventArgs
    {

        #region fields

        private Calendar _calendar; 
        private Graphics _graphics;
        private TrainingItem _item;
        private DateTime _newItemStartDate;
        
        private object _tag; 
        
        #endregion

        #region constructors

        internal CalendarEventArgs(Calendar calendar, Graphics g)
        {
            _calendar = calendar;
            _graphics = g;
        }

        internal CalendarEventArgs(TrainingItem item)
        {
            _item = item;
        }

        internal CalendarEventArgs(DateTime newItemStartDate)
        {
            _newItemStartDate = newItemStartDate;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the calendar associated
        /// </summary>
        public Calendar Calendar
        {
            get { return _calendar; }
        }

        /// <summary>
        /// Gets the device where to paint
        /// </summary>
        public Graphics Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        /// Gets the item related to the event
        /// </summary>
        public TrainingItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Gets the minimum start date of an item when creating
        /// </summary>
        public DateTime NewItemStartDate
        {
            get { return _newItemStartDate; }
            set { _newItemStartDate = value; }
        }

        /// <summary>
        /// Gets or sets a tag for the event
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }


        #endregion
    }
}
