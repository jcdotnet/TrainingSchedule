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
    public class CalendarRenderer
    {
        
        public CalendarRenderer()
        {            
        }

        #region private methods

        private bool Selected(Calendar calendar, DateTime day)
        {
            return (day.Date >= calendar.SelectionStart && day.Date <= calendar.SelectionEnd);
        }     

        private string GetButtonText(CalendarSelectionMode mode)
        {
            bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name =="es-ES";
            switch (mode)
            {
                case CalendarSelectionMode.Manual: return spanish?"Selección manual":"Manual selection mode";
                case CalendarSelectionMode.OneDay: return spanish ? "Un día" : "One-day selection";
                case CalendarSelectionMode.ThreeDays: return spanish ? "Tres días" : "Three-days selection";
                case CalendarSelectionMode.Week: return spanish ? "Selección semanal" : "One-week selection";
                default: return null;
            }
        }

        private string GetButtonText(TrainingItem item, TrainingEvent lastCycle, int button, bool spanish)
        {
            if (item != null && button == 0) return (spanish ? "Editar " : "Modify ") + TrainingItem.ToString(item.Type, spanish);
            if (item != null && button == 1) return (spanish ? "Eliminar " : "Delete ") + TrainingItem.ToString(item.Type, spanish);
            if (item != null && button == 2) return spanish ? "Guardar informe en PDF" : ("Save a " + TrainingItem.ToString(item.Type, spanish) + "report ");
            if (item != null && button == 3) return spanish ? "Cambiar color de la barra" : ("Change the " + TrainingItem.ToString(item.Type, spanish) + "bar colour ");
            if (item != null && button == 4 || button == -1 && lastCycle==TrainingEvent.Other) 
                return spanish ? "Ver en diagramas" : "View training charts";
            if (button == -1 && lastCycle != TrainingEvent.Other) return (spanish ? "Crear " : "New ") + TrainingItem.ToString(lastCycle, spanish);
            return null;
        }

        private string GetItemText(TrainingItem item)
        {
            bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES";
            if (item.Type == TrainingEvent.Season)
            {
                if (item.Text == null)
                {
                    string itemText = (spanish ? "Temporada " : "Season ") + item.StartDate.Year.ToString();
                    if (item.EndDate.Year > item.StartDate.Year) itemText += "/" + item.EndDate.Year.ToString();
                    return itemText;
                }
                else return item.Text;
            }
            else if (item.Text == null) { return TrainingItem.ToString(item.Type, spanish); }
            else return item.Text;
        }

        private void GetItemBackground(TrainingItem item, Area itemArea, Calendar calendar)
        {
            itemArea.BorderColor = Color.Black;
            if (item.Background != Color.Empty && item != calendar.SelectedItem) itemArea.BackgroundColor = item.Background;
            else if (item.Background != Color.Empty && item == calendar.SelectedItem)
            {
                itemArea.BackgroundColor = Color.FromArgb(item.Background.R + 20 < 255 ? item.Background.R + 50 : item.Background.R,
                    item.Background.G + 50 < 255 ? item.Background.G + 50 : item.Background.G,
                    item.Background.B + 50 < 255 ? item.Background.B + 50 : item.Background.B);
            }
            else
            {
                switch (item.Type)
                {
                    case TrainingEvent.Season:
                    case TrainingEvent.Macrocycle:
                    case TrainingEvent.Mesocycle:
                    case TrainingEvent.Microcycle:
                        if (item != calendar.SelectedItem)
                        {
                            itemArea.Gradient = true;
                            itemArea.BackgroundColor = item.Objective != null ? Color.Green : Color.Gray;
                            itemArea.BackgroundColorGradient = item.Objective != null ? Color.LawnGreen : Color.LightGray;
                        }
                        else
                        {
                            itemArea.Gradient = false;
                            itemArea.BackgroundColor = item.Objective != null ? Color.LawnGreen : Color.LightGray;
                        } break;
                    case TrainingEvent.TrainingSession:
                        if (item != calendar.SelectedItem)
                        {
                            itemArea.Gradient = true;
                            itemArea.BackgroundColor = item.Objective != null ? Color.Red : Color.Gray;
                            itemArea.BackgroundColorGradient = item.Objective != null ? Color.LightSalmon : Color.LightGray;
                        }
                        else
                        {
                            itemArea.Gradient = false;
                            itemArea.BackgroundColor = item.Objective != null ? Color.LightSalmon : Color.LightGray;
                        } break;

                }
            }

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

        private void DrawInfoBoxText(Area infoBox, string text)
        {
            infoBox.TextColor = SystemColors.InfoText;
            infoBox.Font = new Font("Georgia", 9);
            infoBox.TextFlags = System.Windows.Forms.TextFormatFlags.Default | System.Windows.Forms.TextFormatFlags.WordBreak
              | System.Windows.Forms.TextFormatFlags.TextBoxControl | System.Windows.Forms.TextFormatFlags.EndEllipsis;
            infoBox.Text = text;
            infoBox.Draw(); 
        }

        #endregion

        #region public drawing methods

        public void DrawLogo(Calendar calendar, Graphics g, string pathLogo, Point p)
        {
            Bitmap myBitmap = new Bitmap(pathLogo);
            g.DrawImage(myBitmap, p.X, p.Y);
        }

        public void DrawSelectionButtons(Calendar calendar, Graphics g)
        {
            int y = 0;
            for (int i = 0; i < calendar.SelectionButtons.Length; i++)
            {
                
                calendar.SelectionButtons[i] = new Area(g, new Rectangle(10, 5 + y, MonthCalendar.Size.Width, 15),
                    GetButtonText((CalendarSelectionMode)i), SystemColors.ControlText, SystemColors.Control, SystemColors.ActiveBorder);
                if ((int)calendar.SelectionMode == i)
                    calendar.SelectionButtons[i].BorderColor = Color.Black; // e.Calendar.ActiveBackgrounds;
                calendar.SelectionButtons[i].Font = new Font("Segoe UI", 9);
                calendar.SelectionButtons[i].Draw();
                y += 20;
            }
        }
      
        public void DrawMonthCalendars(Calendar calendar, Graphics g)
        {
            for (int i = 0; i < calendar.Months.Length; i++)
            {
                if (calendar.Months[i].Bounds.IntersectsWith(calendar.DisplayRectangle))
                {
                    string title = calendar.Months[i].Date.ToString("MMMM yyyy");
                    Area evtTitle = new Area(g, calendar.Months[i].MonthBar, title, SystemColors.Desktop, SystemColors.AppWorkspace);

                    evtTitle.Font =  new Font("Segoe UI", 9);
                    evtTitle.Draw();
                    
                    foreach (Area day in calendar.Months[i].Days)
                    {
                        day.BorderWidth = 0;
                        day.BorderColor = Color.Empty;
                        day.BackgroundColor = Color.Empty;
    
                        if (day.Date.Equals(DateTime.Now.Date) && day.TextColor != SystemColors.GrayText && day.Selectable)
                        {
                            day.BorderColor = SystemColors.AppWorkspace; // e.Calendar.ActiveBackgrounds;
                            day.BorderWidth = 1.5f;
                        }
     
                        else if (day.TextColor == SystemColors.GrayText)
                        {
                            if (day.Date <= calendar.Months[i].Days[13].Date && i - 1 >= 0)
                            {
                                if (calendar.Months[i - 1].Days[41].Date == day.Date.AddDays(-1)) // si mismo mes puede contemplar mejor todos los casos
                                    day.Selectable = true;
                                else
                                    day.Selectable = false;
                            }
                            
                            if (day.Date >= calendar.Months[i].Days[35].Date && i + 1 < calendar.Months.Length)
                            {                                
                                if (calendar.Months[i+1].Days[7].Date==day.Date.AddDays(1)) // si mismo mes puede contemplar mejor todos los casos
                                    day.Selectable=true;
                                else
                                    day.Selectable = false;
                            }
                                
                        }
                                            
                        if (day.Selectable && Selected(calendar, day.Date)) day.BackgroundColor = calendar.ActiveBackgrounds;

                        day.Graphics = g;
                        day.Draw();
                    }

                    // draws line between day headers and calendar days
                    if (calendar.Months[i].Days[0] != null)
                    {
                        using (Pen p = new Pen(SystemColors.AppWorkspace))
                        {
                            int y = calendar.Months[i].Days[0].Bounds.Bottom;
                            g.DrawLine(p, new Point(calendar.Months[i].Bounds.X, y), new Point(calendar.Months[i].Bounds.Right, y));
                        }
                    }

                    // draws previous button
                    calendar.Months[i].PreviousButton.Graphics = g;
                    calendar.Months[i].PreviousButton.Text = "<";
                    calendar.Months[i].PreviousButton.Draw();
                     
                    // draws next button
                    calendar.Months[i].NextButton.Graphics = g;
                    calendar.Months[i].NextButton.Text = ">";
                    calendar.Months[i].NextButton.Draw();
                }
            }
        }

        public void DrawDays(CalendarEventArgs e)
        {
            if (e.Calendar.CalendarDays == null) return;

            try
            {
                int dayWidth = e.Calendar.CalendarArea.Bounds.Width / e.Calendar.CalendarDays.Length;
                int x = e.Calendar.CalendarArea.Bounds.X;
                int y = 0;
                for (int i = 0; i < e.Calendar.CalendarDays.Length; i++)
                {
                    e.Calendar.CalendarDays[i].Graphics = e.Graphics;
                    e.Calendar.CalendarDays[i].Bounds = new Rectangle(e.Calendar.CalendarArea.Bounds.X + y, 25, dayWidth, e.Calendar.CalendarArea.Bounds.Height);

                    e.Calendar.CalendarDays[i].BorderColor = SystemColors.ActiveBorder;
                    e.Calendar.CalendarDays[i].BackgroundColor = SystemColors.Control;
                    e.Calendar.CalendarDays[i].BackgroundColorGradient = SystemColors.Window;
                    e.Calendar.CalendarDays[i].Gradient = true;                    
                    e.Calendar.CalendarDays[i].Draw();
                    x += dayWidth;
                    y += dayWidth;

                    if (e.Calendar.SelectedDay != null && e.Calendar.CalendarDays[i] == e.Calendar.SelectedDay && e.Calendar.CalendarDays.Length > 1)
                    {
                        using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.SystemColors.ActiveCaption))
                        {
                            e.Graphics.FillRectangle(myBrush, e.Calendar.CalendarDays[i].Bounds);
                        }
                    }

                    // notes area
                    Rectangle noteBounds = new Rectangle(e.Calendar.CalendarDays[i].Bounds.X,
                        e.Calendar.CalendarArea.Bounds.Height - e.Calendar.CalendarArea.Bounds.Height / 5, dayWidth, e.Calendar.CalendarArea.Bounds.Height / 5);
                    e.Calendar.CalendarNotes[i].Graphics = e.Graphics;
                    e.Calendar.CalendarNotes[i].Bounds = noteBounds;
                    e.Calendar.CalendarNotes[i].BorderColor = SystemColors.ActiveBorder;
                    e.Calendar.CalendarNotes[i].TextFlags |= System.Windows.Forms.TextFormatFlags.WordBreak | System.Windows.Forms.TextFormatFlags.TextBoxControl
                        | System.Windows.Forms.TextFormatFlags.EndEllipsis;

                    e.Calendar.CalendarNotes[i].Font = new Font("Segoe UI Light", 8);
                    //if (i==0) System.Windows.Forms.MessageBox.Show(e.Calendar.Items.Count.ToString());

                    foreach (TrainingItem item in e.Calendar.Items)
                    {
                        if (item.Type == TrainingEvent.TrainingNote && e.Calendar.TrainingNoteInDay(item, e.Calendar.CalendarDays[i].Date))
                        {
                            e.Calendar.CalendarNotes[i].Font = e.Calendar.Font;
                            //e.Calendar.CalendarNotes[i].BackgroundColor = SystemColors.ControlDark; // cambiar a uno que pegue
                            e.Calendar.CalendarNotes[i].Text = item.Text;
                            item.Bounds = e.Calendar.CalendarNotes[i].Bounds;
                        }
                    }

                    e.Calendar.CalendarNotes[i].Draw(); 
                }

                Area cld;
                // draws the headers of each day
                if (e.Calendar.CalendarDays != null)
                {
                    foreach (Area day in e.Calendar.CalendarDays)
                    {
                        Rectangle dayHeader = new Rectangle(day.Bounds.X, 5, day.Bounds.Width, 20);
                        cld = new Area(e.Graphics, dayHeader, day.Date.ToShortDateString(), Color.Black, SystemColors.Control);
                        cld.BorderColor = SystemColors.WindowFrame;
                        if (e.Calendar.SelectedDay != null && e.Calendar.SelectedDay == day && e.Calendar.CalendarDays.Length > 1)
                            cld.BackgroundColor = SystemColors.ActiveCaption;

                        cld.Font = e.Calendar.Font;
                        cld.Draw();
                    }
                }

                // set the LastdaySelected and SelectedDay properties  
                if (e.Calendar.CalendarDays.Length == 1)
                    e.Calendar.SelectedDay = e.Calendar.LastDaySelected = e.Calendar.CalendarDays[0];
            }
            catch { }
        }

        public void DrawItem(CalendarEventArgs e, List<TrainingItem>sessions)
        {
            if (e.Calendar.CalendarDays == null) return;
            if (!e.Calendar.ItemInSelection(e.Item)) return;

            try
            {
                if (e.Item.Type != TrainingEvent.TrainingNote)
                {
                    int itemX = 0;
                    int itemY = 0;
                    int itemW = 0;
                    double minutes = 1440; // total minutes in a day
                    double minutesWidth = e.Calendar.CalendarDays[0].Bounds.Width / 1440;
                    bool itemAtLastDay = false;
                    bool itemAtFirstDay = false;
                    Area startDay = null;
                    Area endDay = null;

                    int i = 0;
                    while (i < e.Calendar.CalendarDays.Length)
                    {
                        if (e.Item.StartDate < e.Calendar.CalendarDays[i].Date.Date && startDay == null)
                        {
                            itemX = e.Calendar.CalendarDays[i].Bounds.X;
                            itemW = e.Calendar.CalendarDays[i].Bounds.Width;
                            if (i == 0) itemAtFirstDay = true;
                            startDay = e.Calendar.CalendarDays[i];
                        }
                        else if (e.Item.StartDate.Date == e.Calendar.CalendarDays[i].Date.Date && startDay == null)
                        {
                            double t = e.Item.StartDate.Subtract(e.Calendar.CalendarDays[i].Date).TotalMinutes;
                            itemX = e.Calendar.CalendarDays[i].Bounds.X + (int)((e.Calendar.CalendarDays[i].Bounds.Width * t) / (double)minutes);
                            itemW = e.Calendar.CalendarDays[i].Bounds.Width - (int)((e.Calendar.CalendarDays[i].Bounds.Width * t) / (double)minutes);
                            if (i == 0) itemAtFirstDay = true;
                            startDay = e.Calendar.CalendarDays[i];
                        }
                        else if (e.Item.StartDate.Date > e.Calendar.CalendarDays[i].Date.Date && startDay == null)
                        {
                            i++;
                            continue;
                        }

                        if (e.Item.EndDate.Date == e.Calendar.CalendarDays[i].Date.Date && startDay.Date == e.Calendar.CalendarDays[i].Date.Date) //e.Item.EndDate.Date == e.Item.StartDate.Date)
                        {
                            double t = e.Item.EndDate.Subtract(e.Calendar.CalendarDays[i].Date).TotalMinutes;
                            itemW -= e.Calendar.CalendarDays[i].Bounds.Width - (int)((e.Calendar.CalendarDays[i].Bounds.Width * t) / (double)minutes);
                            endDay = e.Calendar.CalendarDays[i];
                        }
                        else if (e.Item.EndDate.Date == e.Calendar.CalendarDays[i].Date.Date && startDay.Date < e.Calendar.CalendarDays[i].Date.Date) //e.Item.EndDate.Date != e.Item.StartDate.Date)
                        {
                            double t = e.Item.EndDate.Subtract(e.Calendar.CalendarDays[i].Date).TotalMinutes;
                            itemW += (int)((e.Calendar.CalendarDays[i].Bounds.Width * t) / (double)minutes);
                            endDay = e.Calendar.CalendarDays[i];
                        }
                        else if (e.Item.EndDate.Date < e.Calendar.CalendarDays[i].Date.Date && endDay == null) // ¿entra aquí?
                        {
                            itemW += e.Calendar.CalendarDays[i].Bounds.Width;
                            if (i == e.Calendar.CalendarDays.Length - 1) itemAtLastDay = true;
                        }
                        else if (e.Item.EndDate.Date > startDay.Date && startDay != e.Calendar.CalendarDays[i] && endDay == null)
                        {
                            itemW += e.Calendar.CalendarDays[i].Bounds.Width;
                            if (i == e.Calendar.CalendarDays.Length - 1) itemAtLastDay = true;
                        }
                        else if (e.Item.EndDate.Date > startDay.Date && startDay == e.Calendar.CalendarDays[i] && endDay == null)
                        {
                            if (i == e.Calendar.CalendarDays.Length - 1) itemAtLastDay = true;
                        }
                        i++;

                    }
                    switch (e.Item.Type)
                    {
                        case TrainingEvent.Season: itemY = e.Calendar.CalendarArea.Bounds.Y; break;
                        case TrainingEvent.Macrocycle: itemY = e.Calendar.CalendarArea.Bounds.Y + 42; break;
                        case TrainingEvent.Mesocycle: itemY = e.Calendar.CalendarArea.Bounds.Y + 84; break;
                        case TrainingEvent.Microcycle: itemY = e.Calendar.CalendarArea.Bounds.Y + 126; break;
                        case TrainingEvent.TrainingSession:
                            int intersections = GetIntersectionsCount(sessions, e.Item);
                            itemY = e.Calendar.CalendarArea.Bounds.Y + 168 + 5 * intersections + intersections * (42 - intersections - 5);
                            //itemY = e.Calendar.CalendarArea.Bounds.Y + 168 + 
                            //GetIntersectionsCount(sessions, e.Item) * 42; 
                            sessions.Add(e.Item); break;
                    }
                    e.Item.Bounds = new Rectangle(itemX, itemY, itemW, 35);

                    Area cldItem = new Area(e.Graphics, e.Item.Bounds);
                    GetItemBackground(e.Item, cldItem, e.Calendar);


                    //cldItemText.TextFlags = System.Windows.Forms.TextFormatFlags.Top | System.Windows.Forms.TextFormatFlags.HorizontalCenter;
                    cldItem.Draw();

                    // draw item text
                    string itemText = GetItemText(e.Item);
                    Rectangle itemBounds = new Rectangle(itemX, itemY, itemW, 40 / 2);
                    Area cldItemText = new Area(e.Graphics, itemBounds, itemText, Color.Black);
                    cldItemText.Font = e.Calendar.Font;
                    cldItemText.Draw();

                    // draw item dates
                    string startText = itemAtFirstDay && e.Item.StartDate < e.Calendar.CalendarDays[0].Date.Date ?
                        e.Item.StartDate.ToShortDateString() : e.Item.StartDate.ToString("HH:mm");
                    string endText = itemAtLastDay && e.Item.EndDate > e.Calendar.CalendarDays[e.Calendar.CalendarDays.Length - 1].Date.Date.AddHours(23).AddMinutes(59) ?
                        e.Item.EndDate.ToShortDateString() : e.Item.EndDate.ToString("HH:mm");

                    Rectangle datesText = new Rectangle(itemX, itemY + 40 / 2, itemW, 40 / 2);

                    Area cldStartDate = new Area(e.Graphics, datesText, startText, Color.Black);
                    cldStartDate.Font = e.Calendar.Font;
                    if (e.Item.Type != TrainingEvent.TrainingSession) cldStartDate.TextFlags = System.Windows.Forms.TextFormatFlags.Left;
                    else cldStartDate.TextFlags = System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.Left
                        | System.Windows.Forms.TextFormatFlags.WordEllipsis;
                    cldStartDate.Draw();

                    if (e.Item.Type != TrainingEvent.TrainingSession)
                    {
                        Area cldEndDate = new Area(e.Graphics, datesText, endText, Color.Black);
                        cldEndDate.Font = e.Calendar.Font;
                        cldEndDate.TextFlags = System.Windows.Forms.TextFormatFlags.Right;
                        cldEndDate.Draw();
                    }
                }

            }
            catch {}
        }

        public void DrawCuadroInfo(CalendarEventArgs e)
        {
            if (e.Calendar.SelectedItem != null)
            {
                bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES";
                
                if (e.Calendar.SelectedItem.Type != TrainingEvent.TrainingNote)
                {
                    StringBuilder sb = new StringBuilder(spanish?"Fecha de inicio: ":"Start date: ");
                    sb.Append(e.Calendar.SelectedItem.StartDate.ToShortDateString()).Append(spanish?" a las ":" at ");
                    sb.Append(e.Calendar.SelectedItem.StartDate.ToShortTimeString()).Append("\n");
                    sb.Append(spanish?"Fecha de fin: ":"End date: ");
                    sb.Append(e.Calendar.SelectedItem.EndDate.ToShortDateString()).Append(spanish ? " a las " : " at ");
                    sb.Append(e.Calendar.SelectedItem.EndDate.ToShortTimeString()).Append("\n\n");
                    if (e.Calendar.SelectedItem.Type == TrainingEvent.Microcycle || e.Calendar.SelectedItem.Type == TrainingEvent.Mesocycle)
                    {
                        sb.Append(spanish?"Orientación: ":"Orientation: ");
                        sb.Append(e.Calendar.SelectedItem.Orientation != null ? e.Calendar.SelectedItem.Orientation : "N/A").Append("\n\n");
                    }
                    sb.Append(spanish?"Objetivo: ":"Objective: ");
                    sb.Append(e.Calendar.SelectedItem.Objective != null ? e.Calendar.SelectedItem.Objective : "N/A").Append("\n\n"); 
                    if (e.Calendar.SelectedItem.Type != TrainingEvent.Season && e.Calendar.SelectedItem.Type != TrainingEvent.TrainingSession)
                    {
                        sb.Append(spanish ? "Contenidos del entrenamiento: " : "Training content: ");
                        sb.Append(e.Calendar.SelectedItem.TrainingContent != null ? e.Calendar.SelectedItem.TrainingContent : "N/A");
                    }
                    if (e.Calendar.SelectedItem.Type == TrainingEvent.TrainingSession)
                    {
                        sb.Append(spanish ? "Observaciones: " : "Observations: ");
                        sb.Append(e.Calendar.SelectedItem.TrainingContent != null ? e.Calendar.SelectedItem.TrainingContent : "N/A");
                    }
                    DrawInfoBoxText(e.Calendar.TrainingInfo, sb.ToString()); 
                }
                else
                {
                    DrawInfoBoxText(e.Calendar.TrainingInfo, e.Item.Text); // valor e.Item = null parece que arreglado..
                    
                }
                e.Calendar.TrainingInfo.Draw();
            }
            else
                e.Calendar.TrainingInfo.Draw();        
        }
         

        public void DrawOpciones(CalendarEventArgs e)
        {
            bool spanish = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES";

            e.Calendar.TrainingButtons = null;
            
            if (e.Calendar.SelectedItem != null && e.Calendar.SelectedItem.Type!=TrainingEvent.TrainingNote && e.Calendar.ItemInSelection(e.Calendar.SelectedItem))
            {
                e.Calendar.TrainingButtons = new Area[5];
                int y = 0;
                for (int i = 0; i < e.Calendar.TrainingButtons.Length; i++)
                {

                    e.Calendar.TrainingButtons[i] = new Area(e.Graphics, new Rectangle(e.Calendar.Width - 290, 220 + y, 280, 30));
                    e.Calendar.TrainingButtons[i].Text = GetButtonText(e.Calendar.SelectedItem,TrainingEvent.Other, i, spanish);
                    if (i==e.Calendar.SelectedButton)
                    {
                        e.Calendar.TrainingButtons[i].BackgroundColor = e.Calendar.ActiveBackgrounds;
                    }
                    else e.Calendar.TrainingButtons[i].BackgroundColor = e.Calendar.Backgrounds;
                    e.Calendar.TrainingButtons[i].BorderColor = e.Calendar.ActiveBackgrounds;
                    e.Calendar.TrainingButtons[i].BorderWidth = 2;
                    e.Calendar.TrainingButtons[i].Font = new Font("Segoe UI Semibold", (float)9.5); 
                    e.Calendar.TrainingButtons[i].Draw();

                    y += 40;
                }
            }

            TrainingItem season = null, seasonEnd = null;
            bool newSeasons = false, seasonsDone = false;
            TrainingItem macro = null, macroEnd = null;
            bool newMacros = false, macrosDone = false;
            TrainingItem meso = null, mesoEnd = null;
            bool newMesos = false, mesosDone = false;
            TrainingItem micro = null, microEnd = null;
            bool newMicros = false, microsDone = false;
            bool newSessions = false;
            TrainingEvent[] cycles = null;
            if (e.Calendar.TrainingButtons == null && e.Calendar.SelectedDay != null) // TrainingButtons is null when no items selected
            {
                foreach (TrainingItem item in e.Calendar.Items)
                {
                    /* item == SEASON */
                    if (item.Type == TrainingEvent.Season && item.EndDate >= e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.StartDate < e.Calendar.SelectedDay.Date.AddDays(1))
                    {
                        season = item; newSeasons = false; seasonsDone = true;
                    }
                    else if (item.Type == TrainingEvent.Season && season == null && item.EndDate < e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.EndDate.Date >= e.Calendar.SelectedDay.Date)
                    {
                        seasonEnd = item; newSeasons = true;
                    }

                    if (item.Type == TrainingEvent.Season && seasonEnd != null && item != seasonEnd && item.StartDate.Subtract(seasonEnd.EndDate) <= TrainingItem.seasonMinDuration)
                    { newSeasons = false; seasonsDone = true; }
                    else if (item.Type == TrainingEvent.Season && season == null && seasonEnd == null && item.StartDate > e.Calendar.SelectedDay.Date && item.StartDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.seasonMinDuration)
                    { newSeasons = false; seasonsDone = true; }

                    /* item == MACROCICLO */
                    if (item.Type == TrainingEvent.Macrocycle && item.EndDate >= e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.StartDate < e.Calendar.SelectedDay.Date.AddDays(1))
                    {
                        macro = item; newMacros = false; macrosDone = true;
                    }
                    else if (item.Type == TrainingEvent.Macrocycle && macro == null && item.EndDate < e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.EndDate.Date >= e.Calendar.SelectedDay.Date)
                    {
                        macroEnd = item; newMacros = true;
                    }

                    if (item.Type == TrainingEvent.Macrocycle && macroEnd != null && item != macroEnd && item.StartDate.Subtract(macroEnd.EndDate) <= TrainingItem.macrocycleMinDuration)
                    { newMacros = false; macrosDone = true; }
                    else if (item.Type == TrainingEvent.Macrocycle && macro == null && macroEnd == null && item.StartDate > e.Calendar.SelectedDay.Date && item.StartDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.macrocycleMinDuration)
                    { newMacros = false; macrosDone = true; }

                    /* item == MESOCICLO */
                    if (item.Type == TrainingEvent.Mesocycle && item.EndDate >= e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.StartDate < e.Calendar.SelectedDay.Date.AddDays(1))
                    {
                        meso = item; newMesos = false; mesosDone = true;
                    }
                    else if (item.Type == TrainingEvent.Mesocycle && meso == null && item.EndDate < e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.EndDate.Date >= e.Calendar.SelectedDay.Date)
                    {
                        mesoEnd = item; newMesos = true;
                    }

                    if (item.Type == TrainingEvent.Mesocycle && mesoEnd != null && item != mesoEnd && item.StartDate.Subtract(mesoEnd.EndDate) <= TrainingItem.mesocycleMinDuration)
                    { newMacros = false; mesosDone = true; }
                    else if (item.Type == TrainingEvent.Mesocycle && meso == null && mesoEnd == null && item.StartDate > e.Calendar.SelectedDay.Date && item.StartDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.mesocycleMinDuration)
                    { newMacros = false; mesosDone = true; }

                    /* item == MICROCICLO */
                    if (item.Type == TrainingEvent.Microcycle && item.EndDate >= e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.StartDate < e.Calendar.SelectedDay.Date.AddDays(1))
                    {
                        micro = item; newMicros = false; newSessions = true; microsDone = true;
                    }
                    else if (item.Type == TrainingEvent.Microcycle && micro == null && item.EndDate < e.Calendar.SelectedDay.Date.AddHours(23).AddMinutes(59) && item.EndDate.Date >= e.Calendar.SelectedDay.Date)
                    {
                        microEnd = item; newMicros = true; newSessions = true;
                    }

                    if (item.Type == TrainingEvent.Microcycle && microEnd != null && item != microEnd && item.StartDate.Subtract(microEnd.EndDate) <= TrainingItem.microcycleMinDuration)
                    { newMicros = false; microsDone = true; }
                    else if (item.Type == TrainingEvent.Microcycle && micro == null && microEnd == null && item.StartDate > e.Calendar.SelectedDay.Date && item.StartDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.microcycleMinDuration)
                    { newMicros = false; microsDone = true; }
                }

                if (e.Calendar.Items.Count == 0) newSeasons = true;

                if (!seasonsDone && season == null) newSeasons = true;

                if (!macrosDone && macro == null && macroEnd == null && season != null && season.EndDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.macrocycleMinDuration)
                    newMacros = false;
                else if (!macrosDone && macro == null && macroEnd == null && season != null && season.EndDate.Subtract(e.Calendar.SelectedDay.Date) > TrainingItem.macrocycleMinDuration)
                    newMacros = true;
                else if (!macrosDone && macroEnd != null && season != null && season.EndDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.macrocycleMinDuration)
                    newMacros = false;
                else if (!macrosDone && macroEnd != null && season != null && season.EndDate.Subtract(e.Calendar.SelectedDay.Date) > TrainingItem.macrocycleMinDuration)
                    newMacros = true;

                if (!mesosDone && meso == null && mesoEnd == null && macro != null && macro.EndDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.mesocycleMinDuration)
                    newMesos = false;
                else if (!mesosDone && meso == null && mesoEnd == null && macro != null && macro.EndDate.Subtract(e.Calendar.SelectedDay.Date) > TrainingItem.mesocycleMinDuration)
                    newMesos = true;
                else if (!mesosDone && mesoEnd != null && macro != null && macro.EndDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.mesocycleMinDuration)
                    newMesos = false;
                else if (!mesosDone && mesoEnd != null && macro != null && macro.EndDate.Subtract(e.Calendar.SelectedDay.Date) > TrainingItem.mesocycleMinDuration)
                    newMesos = true;

                if (!microsDone && micro == null && microEnd == null && meso != null && meso.EndDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.microcycleMinDuration)
                    newMesos = false;
                else if (!microsDone && micro == null && microEnd == null && meso != null && meso.EndDate.Subtract(e.Calendar.SelectedDay.Date) > TrainingItem.microcycleMinDuration)
                    newMicros = true;
                else if (!microsDone && microEnd != null && meso != null && meso.EndDate.Subtract(e.Calendar.SelectedDay.Date) <= TrainingItem.microcycleMinDuration)
                    newMicros = false;
                else if (!microsDone && microEnd != null && meso != null && meso.EndDate.Subtract(e.Calendar.SelectedDay.Date) > TrainingItem.microcycleMinDuration)
                    newMicros = true;

                // when no item is selected, TrainingEvent.Other refers to a chart 

                if (newSeasons && newMacros && newMesos && newMicros && newSessions) cycles = new TrainingEvent[6] { TrainingEvent.Season, TrainingEvent.Macrocycle, TrainingEvent.Mesocycle, TrainingEvent.Microcycle, TrainingEvent.TrainingSession, TrainingEvent.Other };
                else if (newSeasons && newMacros && newMesos && newMicros) cycles = new TrainingEvent[5] { TrainingEvent.Season, TrainingEvent.Macrocycle, TrainingEvent.Mesocycle, TrainingEvent.Microcycle, TrainingEvent.Other };
                else if (newSeasons && newMacros && newMesos) cycles = new TrainingEvent[4] { TrainingEvent.Season, TrainingEvent.Macrocycle, TrainingEvent.Mesocycle, TrainingEvent.Other };
                else if (newSeasons && newMacros) cycles = new TrainingEvent[3] { TrainingEvent.Season, TrainingEvent.Macrocycle, TrainingEvent.Other };
                else if (newSeasons) cycles = new TrainingEvent[2] { TrainingEvent.Season, TrainingEvent.Other };

                else if ((seasonEnd != null && !newSeasons || season != null) && newMacros && newMesos && newMicros && newSessions) cycles = new TrainingEvent[5] { TrainingEvent.Macrocycle, TrainingEvent.Mesocycle, TrainingEvent.Microcycle, TrainingEvent.TrainingSession, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMacros && newMesos && newMicros) cycles = new TrainingEvent[4] { TrainingEvent.Macrocycle, TrainingEvent.Mesocycle, TrainingEvent.Microcycle, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMacros && newMesos) cycles = new TrainingEvent[3] { TrainingEvent.Macrocycle, TrainingEvent.Mesocycle, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMacros) cycles = new TrainingEvent[2] { TrainingEvent.Macrocycle, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMesos && newMicros && newSessions) cycles = new TrainingEvent[4] { TrainingEvent.Mesocycle, TrainingEvent.Microcycle, TrainingEvent.TrainingSession, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMesos && newMicros) cycles = new TrainingEvent[3] { TrainingEvent.Mesocycle, TrainingEvent.Microcycle, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMesos) cycles = new TrainingEvent[2] { TrainingEvent.Mesocycle, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMicros && newSessions) cycles = new TrainingEvent[3] { TrainingEvent.Microcycle, TrainingEvent.TrainingSession, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newMicros) cycles = new TrainingEvent[2] { TrainingEvent.Microcycle, TrainingEvent.Other };
                else if ((seasonEnd != null && !newSeasons || season != null) && newSessions) cycles = new TrainingEvent[2] { TrainingEvent.TrainingSession, TrainingEvent.Other };

                if (cycles == null)
                {
                    cycles = new TrainingEvent[1] { TrainingEvent.Other };
                    e.Calendar.TrainingButtons = new Area[cycles.Length];
                    if (season == null)
                    {
                        string noSeasons = spanish ? "No puede iniciar una temporada en este día ya que se ha creado una posterior cuyo día de comienzo impide que se respete la duración mínima de ésta. \n\nPor favor seleccione otro día en el calendario" :
                        "You cannot start a season because another one is nearly to start and a minimum duration is required.\n\nPlease select a previous day in the calendar or start the season once the following one has finished.";
                        DrawInfoBoxText(e.Calendar.TrainingInfo, noSeasons);
                    }
                }

                e.Calendar.TrainingButtons = new Area[cycles.Length];

                int y = 0;
                e.Item = null;
                for (int i = 0; i < e.Calendar.TrainingButtons.Length; i++)
                {
                    e.Calendar.TrainingButtons[i] = new Area(e.Graphics, new Rectangle(e.Calendar.Width - 290, 220 + y, 280, 30));                   
                    e.Calendar.TrainingButtons[i].Text = GetButtonText(e.Calendar.SelectedItem, cycles[i], -1, spanish);

                    DateTime newStartDate;
                    if (cycles[i] == TrainingEvent.Season)
                    {
                        e.Calendar.TrainingButtons[i].Text += seasonEnd != null ? (spanish ? " a partir de las " : " from ") + seasonEnd.EndDate.AddMinutes(1).ToString("HH:mm") : null;
                        e.Calendar.TrainingButtons[i].Tag = new object[] { (int)cycles[i], seasonEnd != null ? seasonEnd.EndDate.AddMinutes(1) : e.Calendar.SelectedDay.Date };
                    }
                    else if (cycles[i] == TrainingEvent.Macrocycle)
                    {
                        if (macroEnd != null)
                            newStartDate = macroEnd.EndDate.AddMinutes(1);
                        else if (season != null && season.StartDate > e.Calendar.SelectedDay.Date.Date)
                            newStartDate = season.StartDate;
                        else newStartDate = e.Calendar.SelectedDay.Date;
                        e.Calendar.TrainingButtons[i].Tag = new object[] { (int)cycles[i], newStartDate };
                        e.Calendar.TrainingButtons[i].Text += newStartDate != e.Calendar.SelectedDay.Date ? (spanish ? " a partir de las " : " from ") + newStartDate.ToString("HH:mm") : null;
                    }
                    else if (cycles[i] == TrainingEvent.Mesocycle)
                    {
                        if (mesoEnd != null)
                            newStartDate = mesoEnd.EndDate.AddMinutes(1);
                        else if (macro != null && macro.StartDate > e.Calendar.SelectedDay.Date.Date)
                            newStartDate = macro.StartDate;
                        else newStartDate = e.Calendar.SelectedDay.Date;
                        e.Calendar.TrainingButtons[i].Tag = new object[] { (int)cycles[i], newStartDate };
                        e.Calendar.TrainingButtons[i].Text += newStartDate != e.Calendar.SelectedDay.Date ? (spanish ? " a partir de las " : " from ") + newStartDate.ToString("HH:mm") : null;
                    }
                    else if (cycles[i] == TrainingEvent.Microcycle)
                    {
                        if (microEnd != null)
                            newStartDate = microEnd.EndDate.AddMinutes(1);
                        else if (meso != null && meso.StartDate > e.Calendar.SelectedDay.Date.Date)
                            newStartDate = meso.StartDate;
                        else newStartDate = e.Calendar.SelectedDay.Date;
                        e.Calendar.TrainingButtons[i].Tag = new object[] { (int)cycles[i], newStartDate };
                        e.Calendar.TrainingButtons[i].Text += newStartDate != e.Calendar.SelectedDay.Date ? (spanish ? " a partir de las " : " from ") + newStartDate.ToString("HH:mm") : null;
                    }
                    else if (cycles[i] == TrainingEvent.TrainingSession)
                    {
                        if (micro != null && micro.StartDate > e.Calendar.SelectedDay.Date.Date)
                            newStartDate = micro.StartDate;
                        else newStartDate = e.Calendar.SelectedDay.Date;
                        e.Calendar.TrainingButtons[i].Tag = new object[] { (int)cycles[i], newStartDate };
                        e.Calendar.TrainingButtons[i].Text += newStartDate != e.Calendar.SelectedDay.Date ? (spanish ? " a partir de las " : " from ") + newStartDate.ToString("HH:mm") : null;
                    }

                    
                    e.Calendar.TrainingButtons[i].BackgroundColor = (i == e.Calendar.SelectedButton) ? e.Calendar.ActiveBackgrounds : e.Calendar.Backgrounds;
                    e.Calendar.TrainingButtons[i].BorderColor = e.Calendar.ActiveBackgrounds;
                    e.Calendar.TrainingButtons[i].BorderWidth = 2;
                    e.Calendar.TrainingButtons[i].Font = new Font("Segoe UI Semibold", (float)9.5);
                    e.Calendar.TrainingButtons[i].Draw();

                    y += 40;
                }
            }
            else
            {
                // no item selected, no day selected
            }
        }

        #endregion
    }
}
