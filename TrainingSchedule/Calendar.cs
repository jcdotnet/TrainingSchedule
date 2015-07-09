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
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TrainingSchedule
{

    #region enums

    public enum CalendarSelectionMode
    {

        Manual, OneDay, ThreeDays, Week,
    }
    
    #endregion

    public class Calendar: Control
    {

        #region events

        public delegate void CalendarEventHandler(object sender, CalendarEventArgs e);

        /// <summary>
        /// Occurs when the current selection in the month calendar is changed
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Occururs when the charts button is clicked
        /// </summary>
        public event EventHandler ChartsClick;

        /// <summary>
        /// Occurs when a training note has been created
        /// </summary>
        public event CalendarEventHandler NoteCreated;

        /// <summary>
        /// Occurs when a training note has been changed
        /// </summary>
        public event CalendarEventHandler NoteChanged;

        /// <summary>
        /// Occurs when a training note has been deleted 
        /// </summary>
        public event CalendarEventHandler NoteDeleted;

        /// <summary>
        /// Occurs when the new training session button is clicked
        /// </summary>
        public event CalendarEventHandler NewTrainingSessionButtonClick;

        /// <summary>
        /// Occurs when the new macrocycle button is clicked 
        /// </summary>
        public event CalendarEventHandler NewMacrocycleButtonClick;

        /// <summary>
        /// Occurs when the new mesocycle button is clicked
        /// </summary>
        public event CalendarEventHandler NewMesocycleButtonClick;

        /// <summary>
        /// Occurs when the  new microcycle button is clicked
        /// </summary>
        public event CalendarEventHandler NewMicrocycleButtonClick;

        /// <summary>
        /// Occurs when the new season button is clicked
        /// </summary>
        public event CalendarEventHandler NewSeasonButtonClick;

        /// <summary>
        /// Occurs when a training item is clicked 
        /// </summary>
        public event CalendarEventHandler ItemClick;

        /// <summary>
        /// Occurs when a training item is double-clicked
        /// </summary>
        public event CalendarEventHandler ItemDoubleClick;

        /// <summary>
        /// Occurs when the edit button of a training item is clicked
        /// </summary>
        public event CalendarEventHandler EditButtonClick;

        /// <summary>
        /// Occurs when the delete button of a training item is clicked
        /// </summary>
        public event CalendarEventHandler DeleteButtonClick;

        /// <summary>
        /// Occurs when the report button of a training item is clicked
        /// </summary>
        public event CalendarEventHandler ReportButtonClick;
        
        #endregion

        #region fields

        private bool _mouseDown, _mouseMove, _isAdding, _isAddingNote, _isReAdding, _isReEditing, _isRemovingPermanently;
        private CalendarSelectionMode _selectionMode;
        private DateTime _selectionStart;
        private DateTime _selectionEnd;
        private DateTime _calendarStart;

        private Color _backgrounds;
        private Color _activeBackgrounds;

        private MonthCalendar[] _months;
        
        private Area[] _monthDays;
        private Area[] _selectionButtons;
        private Area _calendarArea;
        private Area[] _calendarDays;
        private Area[] _calendarNotes;
        private TextBox _calendarNote;
        private Area _trainingInfo;
        private Area[] _trainingButtons;
        private int _selectedButton;

        private ObservableCollection<TrainingItem> _items = new ObservableCollection<TrainingItem>();
        private TrainingItem _selectedItem;
        private Area _selectedDay;
        private Area _lastDaySelected;

        private CalendarRenderer _renderer;

        #endregion

        #region constructor

        public Calendar()
        {            
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true); 
            //this.DoubleBuffered = true;

            _items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                {
                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    {
                        if (_isAddingNote) return;
                        _isAdding = true;

                        foreach (TrainingItem item in e.NewItems)
                            item.DatesChanged += new TrainingItem.ItemEventHandler(item_DatesChanged);

                        if (_isReAdding)
                        {
                            _isAdding = false;
                            _isReAdding = false;
                        }
                    }
                    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    {
                        if (_isRemovingPermanently) { _isRemovingPermanently = false; return; }
                        if (!CheckDates())
                        {
                            _isReAdding = true;
                            _items.Add((TrainingItem)e.OldItems[0]);
                        }
                        
                    }
                    InitializeMonths();
                });

            _calendarStart = DateTime.Now;

            _backgrounds = Color.FromArgb(196, 218, 250);
            _activeBackgrounds = Color.FromArgb(160, 192, 245);

            _selectedButton = -1;
            _renderer = new CalendarRenderer();

            _selectionButtons = new Area[4];

            _selectionStart = _selectionEnd = DateTime.Today;

            MonthCalendar.DaySize = new Size(27, 19);
            MonthCalendar.Size = new Size(27 * 7, 19 * 7);

            UpdateSelection();

            InitializeMonths();
        }  

        #endregion

        #region internal properties

        internal CalendarSelectionMode SelectionMode
        {
            get { return _selectionMode; }
        }

        internal DateTime SelectionStart
        {
            get { return _selectionStart; }
        }

        internal DateTime SelectionEnd
        {
            get { return _selectionEnd; }
        }

        internal MonthCalendar[] Months
        {
            get { return _months; }
        }

        internal Area[] MonthDays
        {
            get { return _monthDays; }
        }
        
        internal Area CalendarArea
        {
            get { return _calendarArea; }
        }

        internal Area[] CalendarDays
        {
            get { return _calendarDays; }
        }

        internal Area[] CalendarNotes
        {
            get { return _calendarNotes; }
        }

        internal Color ActiveBackgrounds
        {
            get { return _activeBackgrounds; }
        }

        internal Color Backgrounds
        {
            get { return _backgrounds; }
        }
    
        internal Area TrainingInfo
        {
            get { return _trainingInfo; }
        }
        internal Area[] SelectionButtons
        {
            get { return _selectionButtons; }
        }
        internal TrainingItem SelectedItem
        {
            get { return _selectedItem; }        
        }
        internal int SelectedButton
        {
            get { return _selectedButton; }
            //set { _selectedButton = value; }
        }
        internal Area LastDaySelected
        {
            set { _lastDaySelected = value; }
        }
        internal Area[] TrainingButtons
        {
            get { return _trainingButtons; }
            set { _trainingButtons = value; }
        }

        internal Area SelectedDay
        {
            get { return _selectedDay; }
            set { _selectedDay = value; }
        }
        #endregion

        #region public properties
        
        /// <summary>
        ///  Gets the training items list of the calendar
        /// </summary>
        public ObservableCollection<TrainingItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the minimum duration allowed for a season
        /// </summary>
        public TimeSpan SeasonMinDuration
        {
            get { return TrainingItem.seasonMinDuration; }
            set
            {
                if (!TrainingItem.CheckDuration(TrainingEvent.Season, value, _items)) { throw new InvalidCycleDurationException(); }
                TrainingItem.seasonMinDuration = value;
            }
        }
        /// <summary>
        /// Gets or sets the minimum duration allowed for a macrocycle
        /// </summary>
        public TimeSpan MacrocycleMinDuration
        {
            get { return TrainingItem.macrocycleMinDuration; }
            set
            {
                if (!TrainingItem.CheckDuration(TrainingEvent.Macrocycle, value, _items)) { throw new InvalidCycleDurationException(); }
                TrainingItem.macrocycleMinDuration = value;
            }
        }
        /// <summary>
        /// Gets or sets the minimum duration allowed for a mesocycle
        /// </summary>
        public TimeSpan MesocycleMinDuration
        {
            get { return TrainingItem.mesocycleMinDuration; }
            set
            {
                if (!TrainingItem.CheckDuration(TrainingEvent.Mesocycle, value, _items)) { throw new InvalidCycleDurationException(); }
                TrainingItem.mesocycleMinDuration = value;
            }
        }
        /// <summary>
        /// Gets or sets the minimum duration allowed for a microcycle
        /// </summary>
        public TimeSpan MicrocycleMinDuration
        {
            get { return TrainingItem.microcycleMinDuration; }
            set
            {
                if (!TrainingItem.CheckDuration(TrainingEvent.Microcycle, value, _items)) { throw new InvalidCycleDurationException(); }
                TrainingItem.microcycleMinDuration = value;
            }
        }
        #endregion

        #region internal methods
    
        /// <summary>
        /// Determines if the item is in the current selection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool ItemInSelection(TrainingItem item)
        {
            return _selectionStart.Date <= item.EndDate.Date && item.StartDate.Date <= _selectionEnd.Date;
        }

        /// <summary>
        /// Determines if the note belongs to a given day
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool TrainingNoteInDay(TrainingItem note, DateTime day)
        {
            return note.StartDate.Date == day.Date;
        }
        #endregion

        #region private methods
        
        /// <summary>
        /// gets the day of the calendar the mouse is hover on
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns> 
        private Area PointerOnDay(Point p)
        {
            for (int i = 0; i < Months.Length; i++)
            {
                if (Months[i].Bounds.Contains(p))
                {
                    for (int j = 7; j < Months[i].Days.Length; j++)
                    {
                        if (Months[i].Days[j].Bounds.Contains(p))
                        {
                            return Months[i].Days[j];
                        }
                    }
                }
            }
            if (_calendarDays != null)
            {
                for (int i = 0; i < _calendarDays.Length; i++)
                {
                    if (PointerOnArea(p, _calendarDays[i].Bounds))
                    //if (_days[i].Bounds.Contains(p))
                    {
                        return _calendarDays[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// gets the note that the mouse is hover on
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Area PointerOnCalendarNote(Point p)
        {
            foreach (Area calendarNote in _calendarNotes)
            {
                if (PointerOnArea(p, calendarNote.Bounds))
                    return calendarNote;
            }
            return null;
        }

        /// <summary>
        /// Gets the item the mouse is hover on
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private TrainingItem PointerOnItem(Point p)
        {

            foreach (TrainingItem item in _items)
            {
                if (PointerOnArea(p, item.Bounds))
                    return item;
            }
            return null;
        }
        
        private bool PointerOnArea(Point p, Rectangle area)
        {
            return area.Contains(p);
        }

        /// <summary>
        /// Sets the calendar view one month forward
        /// </summary>
        private void GoForward()
        {
            _calendarStart = _calendarStart.AddMonths(1);
            InitializeMonths(); Invalidate();
        }

        /// <summary>
        /// Sets the calendar view one month backward
        /// </summary>
        private void GoBackward()
        {
            _calendarStart = _calendarStart.AddMonths(-1);
            InitializeMonths(); Invalidate();
        }

        private void InitializeMonths()
        {
            int spacing = 20; // space between calendars
            int calendars = Convert.ToInt32(Math.Max(Math.Floor((double)ClientSize.Height / (double)(MonthCalendar.Size.Height + spacing)), 1.0));
            // if (calendars>4) calendars = 4; // shows up to four calendars 
            int calendarWidth = MonthCalendar.Size.Width;
            int calendarHeight = (calendars * MonthCalendar.Size.Height) + (calendars - 1) * spacing;
            int calendarX = 10;
            int calendarY = 100;

            _months = new MonthCalendar[calendars];

            for (int i = 0; i < Months.Length; i++)
            {
                _months[i] = new MonthCalendar(_calendarStart.AddMonths(i));
                _months[i].SetCalendar(new Point(calendarX, calendarY));

                calendarY += spacing + MonthCalendar.Size.Height;
            }

            if (_items != null)
            {
                foreach (TrainingItem item in _items)
                {
                    HighLightSession(item);
                }
            }
        }       

        private void UpdateSelection()
        {            
            _selectionEnd = new DateTime(_selectionEnd.Year, _selectionEnd.Month, _selectionEnd.Day, 23, 59, 59);
            TimeSpan span = _selectionEnd.Subtract(_selectionStart.Date);
            span = span.Add(new TimeSpan(0, 0, 0, 1, 0));

            _monthDays = new Area[span.Days];
            _calendarDays = new Area[span.Days];
            _calendarNotes = new Area[span.Days];

            bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES";
            for (int i = 0; i < _monthDays.Length; i++)
            {
                _monthDays[i] = new Area(_selectionStart.AddDays(i));
                _calendarDays[i] = new Area(_selectionStart.AddDays(i));
                _calendarNotes[i] = new Area(_selectionStart.AddDays(i)); // los puedo crear en calendarrenderer
                _calendarNotes[i].Text = spanish?"doble clic para insertar una nota\n[ESC para guardarla]":"double click to add a note\n[ESC when finished]";
            }
            if (_selectionMode != CalendarSelectionMode.Manual) _selectedDay = null;
            _selectedItem = null; // think this goes here
        }

        private void SelectWeek(DateTime day)
        {
            int[] dias = { 0, 1, 2, 3, 4, 5, 6 };
            int ds = (int)day.DayOfWeek;
            _selectionStart = day.Subtract(new TimeSpan(dias[ds] - (int)MonthCalendar.FirstDayOfWeek, 0, 0, 0)); // monday 
            _selectionEnd = SelectionStart.AddDays(6);
        }

        private void SelectThreeDays(DateTime day)
        {
            _selectionStart = day.Subtract(new TimeSpan(1, 0, 0, 0));
            _selectionEnd = SelectionStart.AddDays(2);
        }

        private void ChangeSelection(Area day)
        {
            if (_selectionMode == CalendarSelectionMode.Week)
                SelectWeek(day.Date);
            else if (_selectionMode == CalendarSelectionMode.ThreeDays)
                SelectThreeDays(day.Date);
            else if (_selectionMode == CalendarSelectionMode.Manual || _selectionMode == CalendarSelectionMode.OneDay)
                _selectionStart = _selectionEnd = day.Date;
            //SelectionEnd = _selectionStart = day.Date;
            OnSelectionChanged(EventArgs.Empty);
        }

        private void OnSelectionChanged(EventArgs e)
        {
            UpdateSelection();
            Invalidate();
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        private void OnTrainingNoteChanged(CalendarEventArgs e)
        {
            if (NoteChanged != null)
            {
                NoteChanged(this, e);
            }
        }

        private void OnTrainingNoteDeleted(CalendarEventArgs e)
        {
            if (NoteDeleted != null)
            {
                NoteDeleted(this, e);
            }
        }

        private void OnTrainingNoteCreated(CalendarEventArgs e)
        {
            if (NoteCreated != null)
            {
                NoteCreated(this, e);
            }
        }

        private void TrainingItemCreate(object[] itemInfo)
        {
            _isAdding = true;
            CalendarEventArgs e = new CalendarEventArgs((DateTime)itemInfo[1]);
            switch ((TrainingEvent)(itemInfo[0]))
            {
                case TrainingEvent.Season:
                    if (NewSeasonButtonClick != null) NewSeasonButtonClick(this, e); break;
                case TrainingEvent.Macrocycle:
                    if (NewMacrocycleButtonClick != null) NewMacrocycleButtonClick(this, e); break;
                case TrainingEvent.Mesocycle:
                    if (NewMesocycleButtonClick != null) NewMesocycleButtonClick(this, e); break;
                case TrainingEvent.Microcycle:
                    if (NewMicrocycleButtonClick != null) NewMicrocycleButtonClick(this, e); break;
                case TrainingEvent.TrainingSession:
                    if (NewTrainingSessionButtonClick != null) NewTrainingSessionButtonClick(this, e); break;
            }
        }

        private void onTrainingItemEdit(TrainingItem item)
        {
            CalendarEventArgs evt = new CalendarEventArgs(item);
            if (EditButtonClick != null) EditButtonClick(this, evt);
        }

        private void onTrainingItemDelete(TrainingItem item)
        {
            CalendarEventArgs evt = new CalendarEventArgs(item);
            if (DeleteButtonClick != null) DeleteButtonClick(this, evt);
            _selectedItem = null;
        }

        private void ChangeColor()
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                _selectedItem.Background = colorDialog.Color;
            }
        }

        private bool CheckDates()
        {
            foreach (TrainingItem item in _items)
            {
                if (item.Type == TrainingEvent.Season && item.EndDate.Subtract(item.StartDate) < TrainingItem.seasonMinDuration)
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Season && !CheckDates(item, TrainingEvent.Other))
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Macrocycle && item.EndDate.Subtract(item.StartDate) < TrainingItem.macrocycleMinDuration)
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Macrocycle && !CheckDates(item, TrainingEvent.Season))
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Mesocycle && item.EndDate.Subtract(item.StartDate) < TrainingItem.mesocycleMinDuration)
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Mesocycle && !CheckDates(item, TrainingEvent.Macrocycle))
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Microcycle && item.EndDate.Subtract(item.StartDate) < TrainingItem.microcycleMinDuration)
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.Microcycle && !CheckDates(item, TrainingEvent.Mesocycle))
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.TrainingSession && item.EndDate.Subtract(item.StartDate) < TrainingItem.trainingSessionMinDuration)
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }
                else if (item.Type == TrainingEvent.TrainingSession && !CheckDates(item, TrainingEvent.Microcycle))
                { if (_isAdding) { _isRemovingPermanently = true; _items.Remove(item); } return false; }

                item.isValid = true;
            }
            _isAdding = false;     
            return true;
        }

        private bool CheckDates(TrainingItem cycle, TrainingEvent type)
        {
            bool inUpperCycle = false, intersects = false, newMacros = false, newMesos = false, newMicros = false;
            foreach (TrainingItem item in _items)
            {
                if (item == cycle && _items.Count == 1) { inUpperCycle = true; continue; }
                else if (item == cycle) continue;
                if (cycle.Type == TrainingEvent.Season) inUpperCycle = true;
                else if (cycle.Type == TrainingEvent.Macrocycle && item.Type == TrainingEvent.Season) newMacros = true;
                else if (cycle.Type == TrainingEvent.Mesocycle && item.Type == TrainingEvent.Macrocycle) newMesos = true;
                else if (cycle.Type == TrainingEvent.Microcycle && item.Type == TrainingEvent.Mesocycle) newMicros = true;

                if (cycle.Type == TrainingEvent.TrainingSession && item.Type == type && cycle.StartDate >= item.StartDate && cycle.StartDate < item.EndDate)
                    inUpperCycle = true; // distingo entre sesiones y resto de ciclos, comprobando en éstas solo la fecha de inicio
                else if (cycle.Type != TrainingEvent.TrainingSession && item.Type == type && cycle.StartDate >= item.StartDate && cycle.EndDate <= item.EndDate)
                    inUpperCycle = true; // en el resto de ciclos compruebo que se encuentre desde el inicio al fin dentro del ciclo superior

                if (item.Type == cycle.Type && item.Type != TrainingEvent.TrainingSession && cycle.StartDate <= item.EndDate && item.StartDate <= cycle.EndDate)
                { if (item.isValid) intersects = true; } 
            }
            if (cycle.Type == TrainingEvent.Macrocycle && !newMacros) return false;
            if (cycle.Type == TrainingEvent.Mesocycle && !newMesos) return false;
            if (cycle.Type == TrainingEvent.Microcycle && !newMicros) return false;

            return inUpperCycle && !intersects;
        }

        private void HighLightSession(TrainingItem item)
        {
            if (item.Type == TrainingEvent.TrainingSession)
            {
                for (int i = 0; i < _months.Length; i++)
                {
                    for (int day = 0; day < _months[i].Days.Length; day++)
                    {
                        if (_months[i].Days[day].Date.Date == item.StartDate.Date && day >= 7)
                        {
                            _months[i].Days[day].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
                        }
                    }
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Area note = (Area)textBox.Tag;
            TrainingItem removedItem = null; 
            if (e.KeyCode == Keys.Escape)
            {
                note.Text = textBox.Text;
                foreach (TrainingItem item in _items)
                {
                    if (ItemInSelection(item) && item.Type == TrainingEvent.TrainingNote && item.StartDate.Date == note.Date.Date)
                    {
                        _isAddingNote = false;
                        item.Text = note.Text;
                        if (item.Text.Length > 0)
                            OnTrainingNoteChanged(new CalendarEventArgs(item));
                        else
                        {
                            removedItem = item;                            
                        }
                    }
                }
                if (removedItem != null)
                {
                    _items.Remove(removedItem);
                    OnTrainingNoteDeleted(new CalendarEventArgs(removedItem));
                }
                if (_isAddingNote && note.Text.Length>0)
                {
                    TrainingItem newItem = new TrainingItem(note.Date, note.Date, note.Text, TrainingEvent.TrainingNote);
                    _items.Add(newItem);
                    _isAddingNote = false;
                    OnTrainingNoteCreated(new CalendarEventArgs(newItem));                    
                }
                this.Controls.Remove(textBox);
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            this.Controls.Remove(textBox);
            _isAddingNote = false;
        }

        // handler of the DatesChanged event associated to the item collection
        private void item_DatesChanged(object sender, ItemEventArgs e)
        {
            if (_isReEditing) { _isReEditing = false; return; }
            TrainingItem item = (TrainingItem)sender;
            DateTime oldStartDate = (DateTime)e.OldItemStartDate != DateTime.MinValue ? (DateTime)e.OldItemStartDate : item.StartDate;
            DateTime oldEndDate = (DateTime)e.OldItemEndDate != DateTime.MinValue ? (DateTime)e.OldItemEndDate : item.EndDate;

            if (!CheckDates()) // invalid new item dates
            {
                _isReEditing = true;
                item.StartDate = oldStartDate;
                _isReEditing = true;
                item.EndDate = oldEndDate;                
                // throw new InvalidCycleDatesException(); // the item is unchanged, uncomment to let the app know the incident  
            }
            InitializeMonths();
        }
        #endregion

        #region overrides

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (_isAdding)
            {
                while (!CheckDates()) continue;
            }
            _isAdding = false;

            Invalidate(); //comento para depurar, descomentar!!!!!!!!!!!! 
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {

            base.OnMouseClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
               
            Area day = PointerOnDay(e.Location);
            if (e.Location.X < 200) // the pointer is on the month calendar area
            {
                for (int i = 0; i < 4; i++)
                {
                    if (PointerOnArea(e.Location, _selectionButtons[i].Bounds))
                    {
                        _selectionMode = (CalendarSelectionMode)i;
                        if (_lastDaySelected != null)
                            ChangeSelection(_lastDaySelected);
                    }
                }
                if (day != null)
                {
                    _lastDaySelected = day;
                    ChangeSelection(day);
                }

                for (int i = 0; i < Months.Length; i++)
                {
                    if (Months[i].NextButton.Bounds.Contains(e.Location))
                    {
                        GoForward();
                    }
                    if (Months[i].PreviousButton.Bounds.Contains(e.Location))
                    { 
                        GoBackward();
                    }
                                   
                }
                if (_selectionMode == CalendarSelectionMode.Manual || _selectionMode == CalendarSelectionMode.OneDay) _selectedDay = day;
                _mouseDown = true;
            }
            else 
            {
                if (day != null)
                {
                    _selectedItem = null;
                    foreach (TrainingItem item in _items) 
                    {
                        if (ItemInSelection(item) && PointerOnArea(e.Location, item.Bounds))
                        {
                            _selectedItem = item;
                            if (ItemClick != null)
                                ItemClick(this, new CalendarEventArgs(_selectedItem));
                            if (item.Type != TrainingEvent.TrainingNote)
                            {
                                Invalidate();
                                return;
                            }
                        }
                    }
                    _selectedDay = day;
                    Invalidate();
                }

                if (_trainingButtons != null)
                {
                    for (int i = 0; i < _trainingButtons.Length; i++)
                    {

                        if (PointerOnArea(e.Location, _trainingButtons[i].Bounds))
                        {
                            _selectedButton = i;
                            Invalidate(); 
                            if (_selectedItem != null)
                            {
                                switch (i)
                                {
                                    case 0: onTrainingItemEdit(_selectedItem); break;
                                    case 1: onTrainingItemDelete(_selectedItem); break;
                                    case 2: if (ReportButtonClick != null) ReportButtonClick(this, new CalendarEventArgs(_selectedItem)); break;
                                    case 3: ChangeColor(); break; 
                                }
                                if (i == _trainingButtons.Length - 1) if (ChartsClick != null) ChartsClick(this, new EventArgs()); 
                                return;
                            }
                            else if (_selectedDay != null)
                            {
                                object[] additionalInfo = (object[])_trainingButtons[i].Tag;
                                if (additionalInfo != null) TrainingItemCreate(additionalInfo);
                            }
                            if (i == _trainingButtons.Length - 1) if (ChartsClick != null) ChartsClick(this, new EventArgs());
                        }

                    }
                }
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_selectionMode == CalendarSelectionMode.Manual && e.Location.X < 200 && _selectedDay != null)
            {
                if (_mouseDown)
                {
                    Area day = PointerOnDay(e.Location);
                    if (day != null)
                    {
                        TimeSpan span = day.Date.Subtract(_selectedDay.Date);
                        span = span.Add(new TimeSpan(0, 0, 0, 1, 0));
                        if (span.Days > 0)
                        {
                            _selectionEnd = span.Days > 6 ? _selectionStart.AddDays(6) : day.Date;
                            OnSelectionChanged(EventArgs.Empty);
                        }
                    }
                    _mouseMove = true;
                }                
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mouseDown = false;
            if (_mouseMove)
            {
                if (_selectionMode == CalendarSelectionMode.Manual) _selectedDay = null;
                Invalidate();
                _mouseMove = false;
            }

            if (_selectedButton != -1)
            {
                _selectedButton = -1;
                Invalidate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Area note = PointerOnCalendarNote(e.Location);
            if (note != null)
            {
                _calendarNote = new System.Windows.Forms.TextBox();
                _calendarNote.KeyDown += new KeyEventHandler(TextBox_KeyDown);
                _calendarNote.MouseLeave += new EventHandler(TextBox_LostFocus);
                Rectangle r = note.Bounds;
                r.Inflate(-2, -2);
                _calendarNote.Bounds = r;
                _calendarNote.BorderStyle = BorderStyle.None;
                _calendarNote.Text = note.Text;
                _calendarNote.Multiline = true;
                _calendarNote.Tag = note;

                Controls.Add(_calendarNote);
                _calendarNote.Visible = true;
                _calendarNote.Focus();
                _isAddingNote = true;
            }
            else
            {
                _selectedItem = null;
                foreach (TrainingItem item in _items) 
                {
                    if (ItemInSelection(item) && PointerOnArea(e.Location, item.Bounds))
                    {
                        _selectedItem = item;
                        if (ItemDoubleClick != null)
                            ItemDoubleClick(this, new CalendarEventArgs(_selectedItem));
                        onTrainingItemEdit(_selectedItem);
                        return;
                    }
                }
            }


        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.Clear(SystemColors.Window);
            
            CalendarEventArgs evt = new CalendarEventArgs(this, e.Graphics);

            _renderer.DrawSelectionButtons(this, e.Graphics);

            _renderer.DrawMonthCalendars(this, e.Graphics);

            _calendarArea = new Area(e.Graphics, new Rectangle(MonthCalendar.Size.Width + 16, 30, this.Width - 500, this.Height));
            _trainingInfo = new Area(e.Graphics, new Rectangle(this.Width - 290, 5, 280, 200), SystemColors.Info, SystemColors.ActiveBorder);

            _renderer.DrawDays(evt);

            // draw items
            List<TrainingItem> sessions = new List<TrainingItem>();
            foreach (TrainingItem item in _items)
            {
                if (ItemInSelection(item))
                {
                    evt.Item = item;
                    //if (item.Type == TrainingEvent.TrainingSession || item.Type == TrainingEvent.Other /* e.g. training session in another training plan */)
                    //{
                    //}
                    _renderer.DrawItem(evt, sessions);
                }
            }

            _renderer.DrawCuadroInfo(evt);

            try
            {
                _renderer.DrawOpciones(evt);
            }
            catch (Exception Exception) { MessageBox.Show(Exception.Message); }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InitializeMonths();
            Invalidate();
        }

        #endregion

    }

    #region exceptions

    public class InvalidCycleDurationException : Exception
    {
        public InvalidCycleDurationException()
                : base("Duración no permitida")
        {
        }
        public InvalidCycleDurationException(string message) : base(message) { }        
    }

    public class InvalidCycleDatesException : Exception
    {
        public InvalidCycleDatesException()
            : base("Fechas no permitidas")
        {
        }
        public InvalidCycleDatesException(string message) : base(message) { }
    }

    #endregion
     
}

