using Ubit_Slendermena_Client.Forms;
using Ubit_Slendermena_Client.Network;
using Ubit_Slendermena_Client.Technical;


namespace Ubit_Slendermena_Client
{
    public partial class AuthorizationForm : Form
    {
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void AuthorizationForm_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AuthorizationTxt.Text))
            {
                MessageBox.Show("Введите имя игрока!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var client = new GameNetworkClient();
            bool connected = await client.ConnectAsync("localhost", 5000);

            if (connected)
            {
                // Отправляем сообщение о входе
                await client.SendMessageAsync(new { Type = "Login", PlayerName = AuthorizationTxt , Password = textBox2.Text});

                // Открываем игровую форму
                var gameForm = new GameForm(client, AuthorizationTxt.Text);
                this.Hide();
                gameForm.ShowDialog();
                this.Close();
            }
            else
            {
                button1.Enabled = true;
                button1.Text = "Подключиться";
            }
        }
    }
}
