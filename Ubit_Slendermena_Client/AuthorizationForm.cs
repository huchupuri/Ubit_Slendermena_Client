using GameClient.Forms;
using GameClient.Models;
using GameClient.Network;
using JeopardyGame;
using Microsoft.VisualBasic.Logging;
using System.Windows.Forms;
using Ubit_Slendermena_Client.Technical;


namespace Ubit_Slendermena_Client
{
    public partial class AuthorizationForm : Form
    {
        public GameNetworkClient? _client;
        private bool _isConnecting = true;
        private bool connected;
        public AuthorizationForm()
        {
            InitializeComponent();

            _client = new GameNetworkClient();
            _client.MessageReceived += OnServerMessage;

        }
        private void HandleSuccessfulAuth(ServerMessage message, string successMessage)
        {
            // Показываем информацию о игроке
            string playerInfo = $"Добро пожаловать, {message.Username}!\n" +
                               $"Игр сыграно: {message.TotalGames}\n" +
                               $"Побед: {message.Wins}\n" +
                               $"Общий счет: {message.TotalScore}";

            //MessageBox.Show(playerInfo, "Успешная аутентификация", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var player = new Player()
            {
                Id = message.Id,
                Username = message.Username,
                TotalGames = message.TotalGames,
                Score = message.TotalScore,
                Wins = message.Wins
            };

            // НЕ отписываемся от событий - передаем клиент как есть
            // _client.MessageReceived -= OnServerMessage; // Убираем эту строку

            var gameForm = new JeopardyGameForm(player);
            _client.MessageReceived-= OnServerMessage;
            this.Hide();

            gameForm.ShowDialog();
            this.Close();
        }
        

        private void OnServerMessage(ServerMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ServerMessage>(OnServerMessage), message);
                return;
            }
            switch (message.Type)
            {
                case "LoginSuccess":
                    HandleSuccessfulAuth(message, "Вход выполнен успешно!");
                    break;

                case "RegisterSuccess":
                    HandleSuccessfulAuth(message!, "Регистрация выполнена успешно!");
                    break;

                case "SelectQuestion":
                    //var form2 = new ConnectionForm();
                    //form2.ShowDialog();
                    break;
            }
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            //_isConnecting = true;

            btnConnect.Enabled = false;
            btnConnect.Text = "Подключение...";

            string serverAddress = "localhost";
            int port = 5000;

            //// Пересоздаем клиент
            //_client?.Disconnect();
            //_client = new GameNetworkClient();
            
            connected = await _client.ConnectAsync(serverAddress, port);

            if (!connected)
            {
                
                _isConnecting = false;
                btnConnect.Enabled = true;
                btnConnect.Text = "Подключиться";
                return;
            }

            await _client.SendMessageAsync(new
            {
                Type = "SelectQuestion"
            });
            await _client.SendMessageAsync(new
            {
                Type = "Login",
                Username = AuthorizationTxt.Text.Trim(),
                Password = PassswordTxt.Text
            });

            // _isConnecting = false;
            //btnConnect.Enabled = true;
            btnConnect.Text = "Подключиться";
        }

        private async void btnAddRoom_Click(object sender, EventArgs e)
        {
            _isConnecting = true;

            btnConnect.Text = "Подключение...";

            string serverAddress = "localhost";
            int port = 5000;    
            connected = await _client.ConnectAsync(serverAddress, port);

            if (!connected)
            {
                _isConnecting = false;
                btnConnect.Text = "Подключиться";
                return;
            }

            await _client.SendMessageAsync(new
            {
                Type = "SelectQuestion"
            });
        }
    }
}
