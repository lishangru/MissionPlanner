using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Drawing.Drawing2D;

namespace MissionPlanner.Controls
{
    public class MyButton : Button
    {
        bool _mouseover = false;
        bool _mousedown = false;

        internal Color _BGGradTop;
        internal Color _BGGradBot;
        internal Color _TextColor;
        internal Color _TextColorNotEnabled;
        internal Color _Outline;
        internal Color _ColorNotEnabled;
        internal Color _ColorMouseOver;
        internal Color _ColorMouseDown;
        int _cornerRadius;

        bool inOnPaint = false;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "0x94, 0xc1, 0x1f")]
        public Color BGGradTop { get { return _BGGradTop; } set { _BGGradTop = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "0xcd, 0xe2, 0x96")]
        public Color BGGradBot { get { return _BGGradBot; } set { _BGGradBot = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "73, 0x2b, 0x3a, 0x03")]
        public Color ColorNotEnabled { get { return _ColorNotEnabled; } set { _ColorNotEnabled = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "73, 0x2b, 0x3a, 0x03")]
        public Color ColorMouseOver { get { return _ColorMouseOver; } set { _ColorMouseOver = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "150, 0x2b, 0x3a, 0x03")]
        public Color ColorMouseDown { get { return _ColorMouseDown; } set { _ColorMouseDown = value; this.Invalidate(); } }

        // i want to ignore forecolor
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "0x40, 0x57, 0x04")]
        public Color TextColor { get { return _TextColor; } set { _TextColor = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        public Color TextColorNotEnabled { get { return (_TextColorNotEnabled.IsEmpty) ? _TextColor : _TextColorNotEnabled; } set { _TextColorNotEnabled = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        [DefaultValue(typeof(Color), "0x79, 0x94, 0x29")]
        public Color Outline { get { return _Outline; } set { _Outline = value; this.Invalidate(); } }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Appearance")]
        [DefaultValue(typeof(int), "0")]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                if (value < 0)
                    value = 0;

                if (_cornerRadius == value)
                    return;

                _cornerRadius = value;
                Invalidate();
            }
        }

        protected override Size DefaultSize => base.DefaultSize;

        public MyButton()
        {
            _BGGradTop = Color.FromArgb(0x94, 0xc1, 0x1f);
            _BGGradBot = Color.FromArgb(0xcd, 0xe2, 0x96);
            _TextColor = Color.FromArgb(0x40, 0x57, 0x04);
            _Outline = Color.FromArgb(0x79, 0x94, 0x29);
            _ColorNotEnabled = Color.FromArgb(73, 0x2b, 0x3a, 0x03);
            _ColorMouseOver = Color.FromArgb(73, 0x2b, 0x3a, 0x03);
            _ColorMouseDown = Color.FromArgb(150, 0x2b, 0x3a, 0x03);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (inOnPaint)
                return;

            inOnPaint = true;

            try
            {
                Graphics gr = pevent.Graphics;

                gr.Clear(BackColor);
                gr.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);

                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    return;
                }

                using (GraphicsPath outline = CreateOutline(bounds, CornerRadius))
                {
                    using (LinearGradientBrush linear = new LinearGradientBrush(bounds, BGGradTop, BGGradBot, LinearGradientMode.Vertical))
                    {
                        gr.FillPath(linear, outline);
                    }

                    if (_mouseover)
                    {
                        using (SolidBrush brush = new SolidBrush(ColorMouseOver))
                        {
                            gr.FillPath(brush, outline);
                        }
                    }

                    if (_mousedown)
                    {
                        using (SolidBrush brush = new SolidBrush(ColorMouseDown))
                        {
                            gr.FillPath(brush, outline);
                        }
                    }

                    if (!Enabled)
                    {
                        using (SolidBrush brush = new SolidBrush(_ColorNotEnabled))
                        {
                            gr.FillPath(brush, outline);
                        }
                    }

                    using (Pen mypen = new Pen(Outline, 1))
                    {
                        gr.DrawPath(mypen, outline);
                    }

                    using (SolidBrush textBrush = new SolidBrush(Enabled ? TextColor : TextColorNotEnabled))
                    {
                        using (StringFormat stringFormat = new StringFormat())
                        {
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            string display = Text;
                            int amppos = display.IndexOf('&');
                            if (amppos != -1)
                                display = display.Remove(amppos, 1);

                            gr.DrawString(display, Font, textBrush, bounds, stringFormat);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                inOnPaint = false;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseover = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _mouseover = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _mousedown = true;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _mousedown = false;
            base.OnMouseUp(mevent);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        GraphicsPath CreateOutline(Rectangle bounds, int radius)
        {
            GraphicsPath outline = new GraphicsPath();

            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                outline.AddRectangle(bounds);
                return outline;
            }

            int maxRadius = Math.Min(bounds.Width, bounds.Height) / 2;
            if (radius > maxRadius)
                radius = maxRadius;

            if (radius <= 0)
            {
                outline.AddRectangle(bounds);
                outline.CloseFigure();
                return outline;
            }

            int diameter = radius * 2;
            outline.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            outline.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            outline.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            outline.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            outline.CloseFigure();

            return outline;
        }
    }
}
