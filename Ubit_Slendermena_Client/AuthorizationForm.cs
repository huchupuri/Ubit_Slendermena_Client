using GameClient.Forms;
using GameClient.Models;
using GameClient.Network;
using JeopardyGame;
using System.Windows.Forms;
using Ubit_Slendermena_Client.Technical;


namespace Ubit_Slendermena_Client
{
    public partial class AuthorizationForm : Form
    {
        private GameNetworkClient? _client;
        private bool _isConnecting = false;
        public AuthorizationForm()
        {
            InitializeComponent();

        }

        private void AuthorizationForm_Load(object sender, EventArgs e)
        {

        }
        private void HandleSuccessfulAuth(ServerMessage message, string successMessage)
        {

            // Показываем информацию о игроке
            string playerInfo = $"Добро пожаловать, {message.Username}!\n" +
                               $"Игр сыграно: {message.TotalGames}\n" +
                               $"Побед: {message.Wins}\n" +
                               $"Общий счет: {message.TotalScore}";

            MessageBox.Show(playerInfo, "Успешная аутентификация", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Открываем игровую форму
            var player = new Player()
            {
                Id = message.Id,
                Username = message.Username,
                TotalGames = message.TotalGames,
                Score = message.TotalScore,
                Wins = message.Wins
            };
            
            //var gameForm = new JeopardyGameForm(_client!, player);
            //this.Hide();

            //gameForm.ShowDialog();
            //this.Close();
        }
        private void OnServerMessage(ServerMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ServerMessage>(OnServerMessage), message);
                return;
            }
            MessageBox.Show(message?.Type ?? "Неизвестное сообщение", "Получено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            switch (message.Type)
            {

                case "LoginSuccess":

                    HandleSuccessfulAuth(message, "Вход выполнен успешно!");
                    break;

                case "RegisterSuccess":
                    HandleSuccessfulAuth(message!, "Регистрация выполнена успешно!");
                    break;

                case "LoginFailed":
                    break;
            }
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (_isConnecting) return;

            _isConnecting = true;
            btnConnect.Enabled = false;
            btnConnect.Text = "Подключение...";

            try
            {
                // Подключение к серверу
                
                // Отправка данных аутентификации
                await _client.SendMessageAsync(new
                {
                    Type = "Login",
                    Username = AuthorizationTxt.Text.Trim(),
                    Password = PassswordTxt.Text
                });
            }
            catch (Exception ex)
            {
                
            }
        }
        private void btnAddRoom_Click(object sender, EventArgs e)
        {

        }
    }
}
