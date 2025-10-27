using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeSpan = System.TimeSpan;

namespace MissionPlanner.Controls
{
    public partial class Status : UserControl
    {
        private System.Threading.Timer _hidetimer;
        private double _percent = 50;
        private Color _progressColor = Color.FromArgb(88, 101, 242);
        private Color _progressBackgroundColor = Color.FromArgb(35, 40, 52);
        private int _cornerRadius = 6;
        public double Percent
        {
            get { return _percent; }
            set
            {
                if (value < 0 || value > 100)
                    return;

                _percent = value;
                this.BeginInvoke((Action) delegate { this.Visible = true; });
                _hidetimer.Change(TimeSpan.FromSeconds(10), TimeSpan.Zero);
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "88, 101, 242")]
        public Color ProgressColor
        {
            get { return _progressColor; }
            set
            {
                if (_progressColor == value)
                    return;

                _progressColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "35, 40, 52")]
        public Color ProgressBackgroundColor
        {
            get { return _progressBackgroundColor; }
            set
            {
                if (_progressBackgroundColor == value)
                    return;

                _progressBackgroundColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(6)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                value = Math.Max(0, value);
                if (_cornerRadius == value)
                    return;

                _cornerRadius = value;
                Invalidate();
            }
        }
        public Status()
        {
            InitializeComponent();

            CreateHandle();

            _hidetimer = new System.Threading.Timer(state =>
            {
                this.BeginInvoke((Action) delegate { this.Visible = false; });
            }, null, 1, -1);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_percent > 100)
                _percent = 50;

            RectangleF bounds = new RectangleF(0, 0, Width - 1, Height - 1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Transparent);

            float radius = Math.Min(_cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2f);

            using (GraphicsPath backgroundPath = CreateRoundRectangle(bounds, radius))
            {
                using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(140, _progressBackgroundColor)))
                {
                    e.Graphics.FillPath(backgroundBrush, backgroundPath);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(80, _progressColor), 1f))
                {
                    e.Graphics.DrawPath(borderPen, backgroundPath);
                }
            }

            if (_percent <= 0)
            {
                return;
            }

            float progressWidth = (float) (bounds.Width * (_percent / 100.0));
            progressWidth = Math.Min(progressWidth, bounds.Width);

            if (progressWidth <= 0)
            {
                return;
            }

            RectangleF progressBounds = new RectangleF(bounds.X, bounds.Y, progressWidth, bounds.Height);
            float progressRadius = Math.Min(radius, Math.Min(progressBounds.Width, progressBounds.Height) / 2f);

            using (GraphicsPath progressPath = CreateRoundRectangle(progressBounds, progressRadius))
            {
                using (LinearGradientBrush progressBrush = new LinearGradientBrush(progressBounds, _progressColor, ControlPaint.Light(_progressColor, 0.2f), LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillPath(progressBrush, progressPath);
                }
            }
        }

        private static GraphicsPath CreateRoundRectangle(RectangleF bounds, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            float diameter = radius * 2f;
            if (diameter > bounds.Width)
            {
                diameter = bounds.Width;
            }
            if (diameter > bounds.Height)
            {
                diameter = bounds.Height;
            }

            float right = bounds.Right;
            float bottom = bounds.Bottom;

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(right - diameter, bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
        }
    }
}
