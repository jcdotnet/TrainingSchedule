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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TrainingSchedule
{
    public partial class Gantt : System.Windows.Forms.Control
    {
        #region constants        
        private const int CHART_X = 20; // left margin of the displayed chart
        private const int BARS_X = 65; // left margin of the bars area inside the chart
        #endregion

        #region fields

        private ObservableCollection<TrainingItem> _items = new ObservableCollection<TrainingItem>();
        private List<Area> _bars = new List<Area>();

        private DateTime _chart1Start = DateTime.MinValue, _chart2Start = DateTime.MinValue, _chart3Start = DateTime.MinValue;
        private DateTime _chart1End = DateTime.MinValue, _chart2End = DateTime.MinValue, _chart3End = DateTime.MinValue;
        private Area _chart1, _chart2, _chart3;
        private bool _onGotFocus;

        private string _athleteOrTeamName;       

        private System.Windows.Forms.ToolTip _toolTip = new System.Windows.Forms.ToolTip();

        private bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES";

        private int _prevX, _prevY; // previous mouse pointer location, created so mousemove event don't fire constantly
        #endregion

        #region events 
        public delegate void ChartEventHandler(object sender, ChartEventArgs e);
        public event ChartEventHandler BarClick;
        public event ChartEventHandler BarDoubleClick;
        #endregion

        #region properties

        public ObservableCollection<TrainingItem> Items
        {
          get { return _items; }
          set { _items = value; }
        }
        
        [DefaultValue("default athlete")]
        public string AthleteOrTeamName
        {
            get { return _athleteOrTeamName; }
            set { _athleteOrTeamName = value; }
        }
        #endregion

        public Gantt()
        {
            InitializeComponent();
            this._toolTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(_toolTip_Draw);
            this._toolTip.Popup += new System.Windows.Forms.PopupEventHandler(_toolTip_Popup);

            _toolTip.OwnerDraw = true;

            this.SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);

        }

        #region private

        private void LoadCharts()
        {
            TrainingItem curSeason, curTrainingSession;
            
            curSeason = null;
            curTrainingSession = null;
            foreach (TrainingItem item in _items)
            {
                if (item.Type == TrainingEvent.Season && DateTime.Now>item.StartDate && DateTime.Now<item.EndDate)
                {
                    curSeason = item;                   
                }

                if (item.Type == TrainingEvent.TrainingSession && DateTime.Now > item.StartDate)
                {
                    if (curTrainingSession==null || curTrainingSession!=null && item.StartDate>curTrainingSession.StartDate)
                        curTrainingSession= item;
                    
                } 
            }

            if (curSeason == null) return;

            _chart1Start = new DateTime(curSeason.StartDate.Year, curSeason.StartDate.Month,1);
            _chart1End = new DateTime(curSeason.EndDate.Year, curSeason.EndDate.Month, DateTime.DaysInMonth(curSeason.EndDate.Year, curSeason.EndDate.Month));

            int[] days = { 6, 0, 1, 2, 3, 4, 5 };
            
            if (curTrainingSession != null)
            {
                DateTime previousMonth = curTrainingSession.StartDate.AddMonths(-1);
                _chart2Start = new DateTime(previousMonth.Year, previousMonth.Month,1);
                _chart2End = _chart2Start.AddMonths(2).AddMinutes(-1);
                
                int dayOfWeek = (int)curTrainingSession.StartDate.DayOfWeek;
                DateTime monday = curTrainingSession.StartDate.Subtract(new TimeSpan(days[dayOfWeek], 0, 0, 0));
                _chart3Start = new DateTime(monday.Year, monday.Month, monday.Day);
                _chart3End = _chart3Start.AddDays(7).AddMinutes(-1);
 
            }
            else
            {
                DateTime previousMonth = DateTime.Now.AddMonths(-1);
                _chart2Start = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                _chart2End = _chart2Start.AddMonths(2).AddMinutes(-1);

                int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                DateTime monday = DateTime.Now.Subtract(new TimeSpan(days[dayOfWeek], 0, 0, 0));
                _chart3Start = new DateTime(monday.Year, monday.Month, monday.Day);
                _chart3End = _chart3Start.AddDays(7).AddMinutes(-1);
            }
        }

        private bool ItemInChart(TrainingItem item, DateTime chartStartDate, DateTime chartEndDate)
        {
            return chartStartDate.Date <= item.EndDate.Date && item.StartDate.Date <= chartEndDate.Date;
        }

        private int MonthsDifference(DateTime start, DateTime end)
        {
            int months = 0;
            while (start <= end)
            {
                int days = DateTime.DaysInMonth(start.Year,start.Month);
                start = start.AddDays(days);
                months++;
            }
            return months;
        }

        private int GetWidth(TimeSpan span, Area chart, double totalMinutes)
        {
            return (int)((span.TotalMinutes * (chart.Bounds.Width - BARS_X)) / totalMinutes);
        }

        private int GetBarX(TrainingItem item, Area chart, DateTime chartStart, double totalMinutes)
        {
            return chart.Bounds.X + BARS_X + GetWidth(item.StartDate.Subtract(chartStart), chart, totalMinutes);
        }

        private int GetBarWidth(TrainingItem item, Area chart, double totalMinutes)
        {
            return GetWidth(item.EndDate.Subtract(item.StartDate), chart, totalMinutes);
        }

        private void DrawHeader(Graphics g, int x, int y, int width, DateTime start, string dateTimeFormat)
        {
            Area monthHeader = new Area(g, new Rectangle(x, y, width, 20));
            monthHeader.Text = start.ToString(dateTimeFormat);
            monthHeader.TextColor = SystemColors.AppWorkspace;
            monthHeader.Font = base.Font;
            monthHeader.TextFlags = System.Windows.Forms.TextFormatFlags.Top | System.Windows.Forms.TextFormatFlags.HorizontalCenter
                | System.Windows.Forms.TextFormatFlags.WordEllipsis;
            monthHeader.Draw();
        }
        
        private int GetIntersectionsCount(List<TrainingItem> sessions, TrainingItem session)
        {
            int intersections = 0;
            foreach (TrainingItem sessionItem in sessions)
            {
                //if (session.Bounds.IntersectsWith(sessionItem.Bounds)) intersections++;
                if (session.StartDate >= sessionItem.StartDate && session.StartDate <= sessionItem.EndDate
                    || session.EndDate >= sessionItem.StartDate && session.EndDate <= sessionItem.EndDate
                    || session.StartDate <= sessionItem.StartDate && session.EndDate >= sessionItem.EndDate)
                    intersections++;
            }
            return intersections;
        }

        private void GetItemColor(TrainingItem item, Area itemArea)
        {
            itemArea.Gradient = true;
            itemArea.BorderColor = Color.Black;
            if (item.Objective == null)
            {
                itemArea.BackgroundColor = Color.Gray;
                itemArea.BackgroundColorGradient = Color.LightGray;
            }
            else
            {
                itemArea.BackgroundColor = Color.Green;
                itemArea.BackgroundColorGradient = Color.LawnGreen;
                /*
                switch (item.Type)
                {
                    case TrainingEvent.Season:
                    case TrainingEvent.TrainingSession:
                    case TrainingEvent.Macrocycle: itemArea.BackgroundColor = Color.Green;
                        itemArea.BackgroundColorGradient = Color.LawnGreen; break;
                    case TrainingEvent.Mesocycle: itemArea.BackgroundColor = Color.Azure;
                        itemArea.BackgroundColorGradient = Color.LightBlue; break;
                    case TrainingEvent.Microcycle:itemArea.BackgroundColor = Color.CadetBlue;
                        itemArea.BackgroundColorGradient = Color.LightBlue; break;
                }*/
            }
        }

        #endregion

        #region overrides

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            LoadCharts();
            _onGotFocus = true;
            Invalidate();
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_onGotFocus)
            {
                LoadCharts();
                _onGotFocus = false;
            }
            try
            {
                e.Graphics.Clear(SystemColors.Window);

                _bars.Clear();

                // draws title and options
                string title;
                if (_chart1Start != DateTime.MinValue && _chart1End != DateTime.MinValue)
                    title = spanish ? ("Temporada en curso de " + _athleteOrTeamName) : (_athleteOrTeamName + "'s current season");
                else
                    title = spanish ? ("No hay ninguna temporada en curso para " + _athleteOrTeamName) : ("No " + _athleteOrTeamName + "'s running seasons have been created yet");
                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, title, new Font("Georgia", 10, FontStyle.Bold),
                    new Rectangle(0, 0, this.Width, 70), SystemColors.InfoText);
                string options = spanish ? "F1 - Ayuda, S - Guardar imagen" : "F1 - Help, S - Save as image";
                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, options, new Font("Georgia", 9),
                    new Rectangle(this.Width - 400, 5, 400, 50), SystemColors.GrayText, System.Windows.Forms.TextFormatFlags.Right);

                // creates chart1 area, if season running, draws chart
                _chart1 = new Area(e.Graphics, new Rectangle(CHART_X, 90, this.Width - CHART_X - CHART_X, 210));

                if (_chart1Start != DateTime.MinValue && _chart1End != DateTime.MinValue)
                {
                    int months = MonthsDifference(_chart1Start, _chart1End);
                    double minutesChart1 = _chart1End.Subtract(_chart1Start).TotalMinutes;
                    int x = _chart1.Bounds.X + BARS_X;
                    using (Pen p = new Pen(SystemColors.AppWorkspace))
                    {
                        e.Graphics.DrawLine(p, new Point(x, _chart1.Bounds.Y - 30), new Point(x, _chart1.Bounds.Height));
                    }
                    DateTime start = _chart1Start;
                    for (int i = 0; i < months; i++)
                    {
                        TimeSpan ts = start.AddDays(DateTime.DaysInMonth(start.Year, start.Month)).Subtract(start);
                        int monthWidth = GetWidth(ts, _chart1, minutesChart1);
                        DrawHeader(e.Graphics, x, _chart1.Bounds.Y - 20, monthWidth, start, "MMMM");
                        start = start.AddMonths(1);
                        x += monthWidth;
                        using (Pen p = new Pen(SystemColors.AppWorkspace))
                        {
                            e.Graphics.DrawLine(p, new Point(x, _chart1.Bounds.Y - 20), new Point(x, _chart1.Bounds.Height));
                        }
                    }

                    // draws info row
                    int y = 25;
                    for (int i = 2; i < 6; i++)
                    {
                        Area row = new Area(e.Graphics, new Rectangle(_chart1.Bounds.X, _chart1.Bounds.Y + y, 65, 25));
                        row.Text = TrainingItem.ToString((TrainingEvent)i, spanish) + ": ";
                        row.TextFlags = System.Windows.Forms.TextFormatFlags.VerticalCenter | System.Windows.Forms.TextFormatFlags.Right;
                        row.TextColor = Color.Gray;
                        row.Font = base.Font;
                        row.Draw();
                        y += 25;
                    }
                    // draws bars          
                    List<TrainingItem> sessions = new List<TrainingItem>();
                    foreach (TrainingItem item in _items)
                    {
                        if (ItemInChart(item, _chart1Start, _chart1End) && item.Type != TrainingEvent.TrainingNote)
                        {
                            Area itemArea = new Area(e.Graphics, item.Bounds);

                            int itemX = GetBarX(item, _chart1, _chart1Start, minutesChart1);
                            int itemY = 0;
                            int itemW;
                            int itemH = 20;
                            if (item.Type == TrainingEvent.TrainingSession) // sessions last an entire day in order to be more visible in the chart
                            {
                                DateTime sessionStart = new DateTime(item.StartDate.Year, item.StartDate.Month, item.StartDate.Day, 0, 0, 0);
                                DateTime sessionEnd = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 0);
                                itemW = GetWidth(sessionEnd.Subtract(sessionStart), _chart1, minutesChart1);
                            }
                            else itemW = GetBarWidth(item, _chart1, minutesChart1);

                            switch (item.Type)
                            {
                                case TrainingEvent.Season: itemY = _chart1.Bounds.Y;
                                    itemArea.Text = (spanish ? "temporada " : "season ") + item.StartDate.Year.ToString();
                                    if (item.EndDate.Year != item.StartDate.Year) itemArea.Text += "/"+item.EndDate.Year.ToString(); break;
                                case TrainingEvent.Macrocycle: itemY = _chart1.Bounds.Y + 25;
                                    itemArea.Text = item.Text; break;
                                case TrainingEvent.Mesocycle: itemY = _chart1.Bounds.Y + 50;
                                    itemArea.Text = item.Orientation != null ? item.Orientation : ""; break;
                                case TrainingEvent.Microcycle: itemY = _chart1.Bounds.Y + 75;
                                    itemArea.Text = item.Orientation != null ? item.Orientation : ""; break;
                                case TrainingEvent.TrainingSession:
                                    int intersections = GetIntersectionsCount(sessions, item); if (intersections > 5) continue;
                                    itemY = _chart1.Bounds.Y + 100 + 5 * intersections + intersections * (25 - intersections - 5);
                                    //itemY = _chart1.Bounds.Y + 100 +  intersections * 25;
                                    sessions.Add(item);
                                    itemH -= intersections * 3; itemArea.Text = ""; break;
                            }

                            item.Bounds = new Rectangle(itemX, itemY, itemW, itemH);
                            itemArea.Bounds = item.Bounds;
                            GetItemColor(item, itemArea);
                            itemArea.Font = base.Font;
                            itemArea.TextFlags |= System.Windows.Forms.TextFormatFlags.WordEllipsis;
                            itemArea.Draw();

                            itemArea.Tag = item;
                            _bars.Add(itemArea);
                        }
                    }

                    // creates chart2 and chart3 area
                    _chart2 = new Area(e.Graphics, new Rectangle(CHART_X, _chart1.Bounds.Y + _chart1.Bounds.Height + 70, this.Width - CHART_X * 2, this.Height / 6));
                    _chart3 = new Area(e.Graphics, new Rectangle(CHART_X, _chart2.Bounds.Y + _chart2.Bounds.Height + 60, this.Width - CHART_X * 2, this.Height / 6));

                    // draws chart2 & chart3 title
                    string title2 = spanish ? "Últimas sesiones realizadas dentro de la temporada en curso:" : "Latest training sessions performed (shown in monthly and weekly view charts)";
                    System.Windows.Forms.TextRenderer.DrawText(e.Graphics, title2, new Font("Georgia", 10, FontStyle.Bold),
                        new Rectangle(0, _chart2.Bounds.Y - 90, this.Width, 70), SystemColors.InfoText);

                    // draws chart2                
                    double minutesChart2 = _chart2End.Subtract(_chart2Start).TotalMinutes;
                    int x2 = _chart2.Bounds.X + BARS_X;
                    using (Pen p = new Pen(SystemColors.AppWorkspace))
                    {
                        e.Graphics.DrawLine(p, new Point(x2, _chart2.Bounds.Y - 30), new Point(x2, _chart2.Bounds.Y + _chart2.Bounds.Height));
                    }
                    DateTime start2 = _chart2Start;
                    for (int i = 0; i < 2; i++)
                    {
                        TimeSpan ts = start2.AddDays(DateTime.DaysInMonth(start2.Year, start2.Month)).Subtract(start2);
                        int monthWidth = GetWidth(ts, _chart2, minutesChart2);
                        DrawHeader(e.Graphics, x2, _chart2.Bounds.Y - 20, monthWidth, start2, "MMMM");
                        start2 = start2.AddMonths(1);
                        x2 += monthWidth;
                        using (Pen p = new Pen(SystemColors.AppWorkspace))
                        {
                            e.Graphics.DrawLine(p, new Point(x2, _chart2.Bounds.Y - 20), new Point(x2, _chart2.Bounds.Y + _chart2.Bounds.Height));
                        }
                    }
                    // draws info rows 
                    // draws bars 
                    sessions = new List<TrainingItem>();
                    foreach (TrainingItem item in _items)
                    {
                        if (ItemInChart(item, _chart2Start, _chart2End) && item.Type == TrainingEvent.TrainingSession)
                        {
                            Area itemArea = new Area(e.Graphics, item.Bounds);

                            int itemX = GetBarX(item, _chart2, _chart2Start, minutesChart2);
                            int itemW = GetBarWidth(item, _chart2, minutesChart2);
                            int itemH = 30;

                            int intersections = GetIntersectionsCount(sessions, item); if (intersections > 5) continue;
                            int itemY = _chart2.Bounds.Y + 5 * intersections + intersections * (30 - intersections - 2);

                            sessions.Add(item);
                            itemH -= intersections * 3;
                            //itemArea.Text = item.Objective!=null?item.Objective:(spanish?"Objetivo sin definir": "Objective not defined yet"); 
                            item.Bounds = new Rectangle(itemX, itemY, itemW, itemH);
                            itemArea.Bounds = item.Bounds;
                            GetItemColor(item, itemArea);
                            itemArea.BorderColor = Color.Black;
    
                            itemArea.Draw();

                            itemArea.Tag = item;
                            _bars.Add(itemArea);

                        }
                    }
                    // draws chart3               
                    double minutesChart3 = _chart3End.Subtract(_chart3Start).TotalMinutes;
                    int x3 = _chart3.Bounds.X + BARS_X;
                    using (Pen p = new Pen(SystemColors.AppWorkspace))
                    {
                        e.Graphics.DrawLine(p, new Point(x3, _chart3.Bounds.Y - 30), new Point(x3, _chart3.Bounds.Y + _chart3.Bounds.Height));
                    }
                    DateTime start3 = _chart3Start;
                    for (int i = 0; i < 7; i++)
                    {
                        TimeSpan ts = start3.AddDays(1).Subtract(start3);
                        int dayWidth = GetWidth(ts, _chart3, minutesChart3);
                        DrawHeader(e.Graphics, x3, _chart3.Bounds.Y - 20, dayWidth, start3, "dd MMMM");
                        start3 = start3.AddDays(1);
                        x3 += dayWidth;
                        using (Pen p = new Pen(SystemColors.AppWorkspace))
                        {
                            e.Graphics.DrawLine(p, new Point(x3, _chart3.Bounds.Y - 20), new Point(x3, _chart3.Bounds.Y + _chart3.Bounds.Height));
                        }
                    }
                    // draws bars
                    sessions = new List<TrainingItem>();
                    foreach (TrainingItem item in _items)
                    {
                        if (ItemInChart(item, _chart3Start, _chart3End) && item.Type == TrainingEvent.TrainingSession)
                        {
                            Area itemArea = new Area(e.Graphics, item.Bounds);

                            int itemX = GetBarX(item, _chart3, _chart3Start, minutesChart3);
                            int itemW = GetBarWidth(item, _chart3, minutesChart3);
                            int itemH = 30;

                            int intersections = GetIntersectionsCount(sessions, item); if (intersections > 5) continue;
                            itemH -= intersections * 3;
                            int itemY = _chart3.Bounds.Y + 5 * intersections + intersections * (30 - intersections - 2);
                            sessions.Add(item);
                            itemArea.Text = item.Objective != null ? item.Objective : (spanish ? "Objetivo sin definir" : "Objective not defined yet");
                            item.Bounds = new Rectangle(itemX, itemY, itemW, itemH);
                            itemArea.Bounds = item.Bounds;
                            GetItemColor(item, itemArea);                            
                            itemArea.Font = base.Font;
                            itemArea.TextFlags = System.Windows.Forms.TextFormatFlags.TextBoxControl |
                                System.Windows.Forms.TextFormatFlags.WordBreak | System.Windows.Forms.TextFormatFlags.EndEllipsis;

                            itemArea.Draw();

                            itemArea.Tag = item;
                            _bars.Add(itemArea);

                        }
                    }
                }
            }
            catch { }
                
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.X == _prevX && e.Y == _prevY) return;
            bool onBar = false;
                foreach (Area bar in _bars)
                {
                    if (bar.Bounds.Contains(e.Location))
                    {
                        _prevX = e.X;
                        _prevY = e.Y;
                        onBar = true;

                        TrainingItem item = (TrainingItem)bar.Tag;
                        _toolTip.ToolTipTitle = TrainingItem.ToString(item.Type, spanish); 
                        _toolTip.Tag = item;                        
                        _toolTip.SetToolTip(this, "something");
                        return;
                    }
                }
                if (!onBar)
                {
                    _toolTip.Hide(this);
                }

            
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.S)
            {
                System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
                saveDialog.Filter = "bmp files (*.bmp)|*.bmp";
                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; 
                        //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // HighSpeed;
                        //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        System.Windows.Forms.PaintEventArgs evt = new System.Windows.Forms.PaintEventArgs(g, new Rectangle(0, 0, this.Width, this.Height));
                        this.OnPaint(evt);
                    }
        
                    bitmap.Save(saveDialog.FileName,System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            foreach (Area bar in _bars)
            {
                if (bar.Bounds.Contains(e.Location))
                {
                    if (BarClick != null)
                    {
                        BarClick(this, new ChartEventArgs((TrainingItem)bar.Tag));
                    }
                }
            }
        }
        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            foreach (Area bar in _bars)
            {
                if (bar.Bounds.Contains(e.Location))
                {
                    if (BarDoubleClick != null)
                    {
                        BarDoubleClick(this, new ChartEventArgs((TrainingItem)bar.Tag));
                    }
                }
            }
        }

        #endregion

        #region tooltip

        private void _toolTip_Draw(object sender, System.Windows.Forms.DrawToolTipEventArgs e)
        {
            System.Windows.Forms.ToolTip tt = (System.Windows.Forms.ToolTip)sender;

            if (e.AssociatedControl == this)
            {
                // draws default background
                e.DrawBackground();

                // draws custom 3D-like border
                e.Graphics.DrawLines(SystemPens.ControlLightLight, new Point[] {
                    new Point (0, e.Bounds.Height - 1), 
                    new Point (0, 0), 
                    new Point (e.Bounds.Width - 1, 0)
                });
                e.Graphics.DrawLines(SystemPens.ControlDarkDark, new Point[] {
                    new Point (0, e.Bounds.Height - 1), 
                    new Point (e.Bounds.Width - 1, e.Bounds.Height - 1), 
                    new Point (e.Bounds.Width - 1, 0)
                });

                // sets a font for the tooltip text 
                Font font = new Font("Georgia", 10, FontStyle.Bold);

                // draws title
                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, tt.ToolTipTitle, font, new Rectangle(0, 0, e.Bounds.Width, 20), Color.Black);

                //draws line between title and text
                e.Graphics.DrawLine(Pens.Black, 0, 21, e.Bounds.Width, 21);

                // reduces font size and sets flags 
                font = new Font("Georgia", 9, FontStyle.Regular);
                System.Windows.Forms.TextFormatFlags flags = System.Windows.Forms.TextFormatFlags.WordBreak |
                    System.Windows.Forms.TextFormatFlags.TextBoxControl |
                    System.Windows.Forms.TextFormatFlags.EndEllipsis;

                // draws custom text (e.ToolTipText = "something")
                TrainingItem item = (TrainingItem)tt.Tag;
                StringBuilder sb = new StringBuilder(spanish ? "Fecha de inicio: " : "Start date: ");
                sb.Append(item.StartDate.ToShortDateString()).Append("\n");
                if (item.Type != TrainingEvent.TrainingSession)
                    sb.Append(spanish ? "Finalización: " : "End date: ").Append(item.EndDate.ToShortDateString()).Append("\n");
                if ((item.Type == TrainingEvent.Microcycle || item.Type == TrainingEvent.Mesocycle) && item.Orientation != null)
                    sb.Append(spanish ? "Orientación: " : "Orientation: ").Append(item.Orientation).Append("\n");
                sb.Append(spanish ? "Objetivo: " : "Objective: ").Append(item.Objective != null ? item.Objective : "N/A");

                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, sb.ToString(), font, new Rectangle(0, 25, e.Bounds.Width, e.Bounds.Height - 25), Color.Black, flags);
            }
        }

        private void _toolTip_Popup(object sender, System.Windows.Forms.PopupEventArgs e)
        {
            e.ToolTipSize = new Size(180, 120);
        }

        #endregion
    }

    public class ChartEventArgs : EventArgs
    {
        private Gantt _ganttChart;
        private Graphics _graphics;
        private TrainingItem _item;

        private object _tag;


        internal ChartEventArgs(TrainingItem item)
        {
            _item = item;
        }
        internal ChartEventArgs(Gantt ganttChart, Graphics g)
        {
            _ganttChart = ganttChart;
            _graphics = g;
        }

        public Gantt GanttCharts
        {
            get { return _ganttChart; }
        }

        public Graphics Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        /// Get the item related with the bar
        /// </summary>
        public TrainingItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Gets or sets a tag for the bar
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

    }
}
