using GameClient.Models;
using GameClient.Network;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class EntryForm : Form
    {
        private GameClient.Network.GameClient? _client;
        private bool _isConnecting = false;
        private bool _connected = false;

        public EntryForm(GameClient.Network.GameClient? _client)
        {
            this._client = _client;
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            string serverUrl = "ws://localhost:5000/";
            _client = new GameClient.Network.GameClient(serverUrl);

            // Подписываемся на события клиента
            _client.MessageReceived += OnServerMessage;
            _client.ConnectionClosed += OnConnectionClosed;
            _client.ErrorOccurred += OnErrorOccurred;
        }

        private async Task<bool> ConnectToServerAsync()
        {
            if (_client == null)
            {
                InitializeClient();
            }

            try
            {
                await _client.ConnectAsync();
                _connected = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _connected = false;
                return false;
            }
        }

        private void HandleSuccessfulAuth(ServerMessage message, string successMessage)
        {
            try
            {
                var player = new Player()
                {
                    Id = message.Id,
                    Username = message.Username,
                    TotalGames = message.TotalGames,
                    Score = message.TotalScore,
                    Wins = message.Wins
                };

                UnsubscribeFromEvents();

                // Передаем клиент в MenuForm для дальнейшего использования
                var menuForm = new MenuForm(player, _client);

                this.Hide();
                menuForm.Show();
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переходе в меню: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_client != null)
            {
                _client.MessageReceived -= OnServerMessage;
                _client.ConnectionClosed -= OnConnectionClosed;
                _client.ErrorOccurred -= OnErrorOccurred;
            }
        }

        private void OnServerMessage(object sender, ServerMessage serverMessage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ServerMessage>(OnServerMessage), sender, serverMessage);
                return;
            }

            try
            {
                switch (serverMessage.Type)
                {
                    case "LoginSuccess":
                        HandleSuccessfulAuth(serverMessage, "Вход выполнен успешно!");
                        break;

                    case "RegisterSuccess":
                        HandleSuccessfulAuth(serverMessage, "Регистрация выполнена успешно!");
                        break;
                        
                    case "Error":
                        MessageBox.Show($"Ошибка сервера: {serverMessage.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        EnableConnectButton();
                        break;

                    default:
                        Console.WriteLine($"Получено неизвестное сообщение: {serverMessage.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки сообщения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnConnectionClosed(object sender, string reason)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(OnConnectionClosed), sender, reason);
                return;
            }

            _connected = false;
            _isConnecting = false;

            MessageBox.Show($"Соединение закрыто: {reason}", "Соединение потеряно",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            EnableConnectButton();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Exception>(OnErrorOccurred), sender, ex);
                return;
            }

            _connected = false;
            _isConnecting = false;

            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            EnableConnectButton();
        }

        private void EnableConnectButton()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(EnableConnectButton));
                return;
            }

            btnConnect.Enabled = true;
            btnConnect.Text = "Подключиться";
            _isConnecting = false;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnecting)
                return;

            // Валидация входных данных
            if (string.IsNullOrWhiteSpace(AuthorizationTxt.Text))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PassswordTxt.Text))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isConnecting = true;
            btnConnect.Enabled = false;
            btnConnect.Text = "Подключение...";

            try
            {
                if (!_connected)
                {
                    bool connectionResult = await ConnectToServerAsync();
                    if (!connectionResult)
                    {
                        EnableConnectButton();
                        return;
                    }
                }

                // Отправляем данные для авторизации
                await _client.LoginAsync(AuthorizationTxt.Text.Trim(), PassswordTxt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableConnectButton();
            }
        }

        private async void btnAddRoom_Click(object sender, EventArgs e)
        {
            if (_isConnecting)
                return;

            _isConnecting = true;
            btnAddRoom.Enabled = false;
            btnAddRoom.Text = "Подключение...";

            try
            {
                if (!_connected)
                {
                    bool connectionResult = await ConnectToServerAsync();
                    if (!connectionResult)
                    {
                        btnAddRoom.Enabled = true;
                        btnAddRoom.Text = "Создать комнату";
                        _isConnecting = false;
                        return;
                    }
                }

                await _client.SelectQuestionAsync(1);
                MessageBox.Show("Команда отправлена на сервер", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке команды: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAddRoom.Enabled = true;
                btnAddRoom.Text = "Создать комнату";
                _isConnecting = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnsubscribeFromEvents();

            if (_client != null && _connected)
            {
                try
                {
                    _client.DisconnectAsync().Wait(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отключении: {ex.Message}");
                }
            }

            base.OnFormClosing(e);
        }
    }
}
