using System.Drawing;
using System.Windows.Forms;
namespace Ubit_Slendermena_Client.Technical
{
    /// <summary>
    /// класс кнопки переключения пароля
    /// </summary>
    public class PasswordToggleButton : Button
    {
        private readonly TextBox _passwordTextBox;

        public PasswordToggleButton(TextBox passwordTextBox)
        {
            _passwordTextBox = passwordTextBox;
            BackColor = Color.FromArgb(158, 157, 189);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(30, 30);
            Paint += OnPaint;
            Click += OnClick;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var isPasswordVisible = _passwordTextBox.PasswordChar == '\0';
            using (var brush = new SolidBrush(Color.FromArgb(243, 200, 220)))
            {
                var eyeIcon = "👁️‍🗨️";
                var textSize = TextRenderer.MeasureText(eyeIcon, new Font("Segoe UI Emoji", 16F, FontStyle.Regular));
                var x = (Width - textSize.Width) / 2;
                var y = (Height - textSize.Height) / 2;
                e.Graphics.DrawString(
                    eyeIcon,
                    new Font("Segoe UI Emoji", 16F, FontStyle.Regular),
                    brush,
                    new PointF(x, y)
                );
                if (!isPasswordVisible)
                {
                    using (var pen = new Pen(Color.FromArgb(243, 200, 220), 2)) 
                    {
                        e.Graphics.DrawLine(pen, x + 5, y + 5, x + textSize.Width - 5, y + textSize.Height - 5);
                    }
                }
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            _passwordTextBox.PasswordChar = _passwordTextBox.PasswordChar == '\0' ? '•' : '\0';
            Invalidate();
        }
    }
}