using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client.Technical
{
    /// <summary>
    /// класс круглой кнопки
    /// </summary>
    public class RoundButton : Button
    {
        private int _borderRadius = 15;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private System.Windows.Forms.Timer _animationTimer;
        private float _pressOffset = 0f;
        private Color _currentColor;

        /// <summary>
        /// Радиус закругления углов кнопки.
        /// </summary>
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        /// <summary>
        /// Цвет кнопки при наведении курсора.
        /// </summary>
        public Color HoverColor { get; set; }

        /// <summary>
        /// Цвет кнопки при нажатии.
        /// </summary>
        public Color PressColor { get; set; }

        /// <summary>
        /// Цвет границы кнопки. По умолчанию прозрачный.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Transparent;

        // <summary>
        /// Глубина "вдавливания" кнопки при нажатии (значение от 0 до 0.5).
        /// </summary>
        public float PressDepth { get; set; } = 0.15f;

        public RoundButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(209, 131, 170);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            MinimumSize = new Size(100, 46);

            HoverColor = ControlPaint.Light(BackColor, 0.15f);
            PressColor = ControlPaint.Dark(BackColor, 0.2f);

            _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _animationTimer.Tick += (s, e) => UpdateAnimation();

            MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _isHovered = false; _isPressed = false; Invalidate(); };
            MouseDown += (s, e) => { _isPressed = true; _animationTimer.Start(); };
            MouseUp += (s, e) => { _isPressed = false; _animationTimer.Start(); };

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.Opaque, true);
        }

        private void UpdateAnimation()
        {
            if (_isPressed)
            {
                _pressOffset = Math.Min(_pressOffset + 0.05f, PressDepth);
            }
            else
            {
                _pressOffset = Math.Max(_pressOffset - 0.05f, 0f);
            }

            Invalidate();

            if (_pressOffset <= 0 && !_isPressed)
            {
                _animationTimer.Stop();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            _currentColor = _isPressed ? PressColor :
                          _isHovered ? HoverColor : BackColor;

            var rect = new Rectangle(
                (int)(_pressOffset * 4),
                (int)(_pressOffset * 4),
                Width - (int)(_pressOffset * 8) - 1,
                Height - (int)(_pressOffset * 8) - 1);

            using (GraphicsPath path = GetRoundRectangle(rect, BorderRadius))
            using (SolidBrush brush = new SolidBrush(_currentColor))
            {
                e.Graphics.FillPath(brush, path);

                if (!_isHovered && !_isPressed)
                {
                    using (Pen pen = new Pen(BorderColor, 1))
                        e.Graphics.DrawPath(pen, path);
                }
            }

            var textRect = new Rectangle(
                0,
                (int)(_pressOffset * 2),
                Width,
                Height);

            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                textRect,
                ForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath GetRoundRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseAllFigures();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}