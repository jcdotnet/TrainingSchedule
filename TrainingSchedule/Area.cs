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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TrainingSchedule
{
    /// <summary>
    /// Represents an area (with an optional text) 
    /// </summary>
    internal class Area
    {
        #region fields

        private Graphics _graphics;
        private Color _textColor;
        private Color _backgroundColor;
        private bool _gradient;
        
        private Color _backgroundColorGradient;

        public Color BackgroundColorGradient
        {
            get { return _backgroundColorGradient; }
            set { _backgroundColorGradient = value; }
        }
        private string _text;
        private Color _borderColor;
        private float _borderWidth;
        
        private Rectangle _bounds;
        private Font _font;
        private TextFormatFlags _textFlags;
        private DateTime _date;
        private bool _selectable;
       
        private object _tag;

        
        internal Area(Graphics g, Rectangle bounds, string text, Color textColor, Color backColor, Color borderColor)
        {
            _graphics = g;
            _bounds = bounds;
            _text = text;
            _textColor = textColor;
            _backgroundColor = backColor;
            _borderColor = borderColor;
            _borderWidth = 1;

 
            _textFlags |= TextFormatFlags.HorizontalCenter;
            _textFlags |= TextFormatFlags.VerticalCenter;
        }

        

        internal Area(Rectangle bounds)
            : this(null, bounds, null, Color.Black, Color.Empty, Color.Empty)
        {
        }
        
        internal Area(Graphics g, Rectangle bounds)
            : this(g, bounds, null, Color.Black, Color.Empty, Color.Empty)
        {
         
        }
        internal Area(Graphics g, Rectangle bounds, Color background)
            : this(g, bounds, null, Color.Black, background)
        { }
        internal Area(Graphics g, Rectangle bounds, Color background, Color border)
            : this(g, bounds, null, Color.Black, background, border)
        { }

        internal Area(Graphics g, Rectangle bounds, string text, Color textColor)
            : this(g, bounds, text, textColor, Color.Empty, Color.Empty)
        {}

        internal Area(Graphics g, Rectangle bounds, string text, Color textColor, Color backColor)
            : this(g, bounds, text, textColor, backColor, Color.Empty)
        {}

        internal Area(Graphics g, Rectangle bounds, string text, StringAlignment textAlign, Color textColor, Color backColor)
            : this(g, bounds, text, textColor, backColor, Color.Empty)
        { }

        internal Area(Graphics g, Rectangle bounds, string text, StringAlignment textAlign, Color textColor)
            : this(g, bounds, text, textColor, Color.Empty, Color.Empty)
        { }
        
        internal Area(DateTime date)
            : this(null, Rectangle.Empty, null, Color.Black, Color.Empty, Color.Empty)
        {
            _date = date;
        }        

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the bounds of the area
        /// </summary>
        internal Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        /// <summary>
        /// Applies gradient to the background color of the area
        /// </summary>
        internal bool Gradient
        {
            get { return _gradient; }
            set { _gradient = value; }
        }

        /// <summary>
        /// Gets or sets the font of the text. If null, default will be used.
        /// </summary>
        internal Font Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Gets or sets the Graphics object where to draw
        /// </summary>
        internal Graphics Graphics
        {
            set { _graphics = value; }
            get { return _graphics; }
        }

        /// <summary>
        /// Gets or sets the border color of the area
        /// </summary>
        internal Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// Gets or sets the border width of the area
        /// </summary>
        internal float BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; }
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the area
        /// </summary>
        internal string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Represents a date in case the area is a day 
        /// </summary>
        internal DateTime Date
        {
            get { return _date; }
            set { _date=value; }
        }

        internal bool Selectable
        {
            get { return _selectable; }
            set { _selectable = value; }
        }

        /// <summary>
        /// Gets or sets the background color of the area
        /// </summary>
        internal Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets the text color of the area
        /// </summary>
        internal Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// Gets or sets the flags of the text
        /// </summary>
        internal TextFormatFlags TextFlags
        {
            get { return _textFlags; }
            set { _textFlags = value; }
        }

        internal object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        #endregion

        #region draw

        internal void Draw() 
        {
            if (_gradient && !_backgroundColorGradient.IsEmpty)
            {
                using (Brush b = new LinearGradientBrush(_bounds, _backgroundColor, _backgroundColorGradient, LinearGradientMode.Vertical))
                {
                    _graphics.FillRectangle(b, _bounds);
                }
            }
            else if (!_backgroundColor.IsEmpty)
            {
                using (SolidBrush b = new SolidBrush(_backgroundColor))
                {
                    _graphics.FillRectangle(b, _bounds);
                }
            }

            if (!_textColor.IsEmpty && !string.IsNullOrEmpty(_text))
            {
                TextRenderer.DrawText(_graphics, _text, _font, _bounds, _textColor, _textFlags);
            }

            if (!_borderColor.IsEmpty)
            {
                using (Pen p = new Pen(_borderColor, _borderWidth))
                {
                    Rectangle r = _bounds;
                    r.Width--; r.Height--;
                    _graphics.DrawRectangle(p, r);
                }
            }
        }
        #endregion

    }
}
