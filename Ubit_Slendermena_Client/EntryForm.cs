using GameClient.Models;
using GameClient.Network;
using NLog;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class EntryForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private GameClient.Network.GameClient? _client;
        private bool _isConnecting = false;
        private bool _connected = false;
        /// <summary>
        /// конструктор класса EntryForm
        /// </summary>
        /// <param name="_client"></param>
        public EntryForm(GameClient.Network.GameClient? _client)
        {
            Logger.Info("Инициализация EntryForm");
            this._client = _client;
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            string serverUrl = "ws://localhost:5000/";
            Logger.Info($"Инициализация клиента {serverUrl}");

            _client = new GameClient.Network.GameClient(serverUrl);
            _client.MessageReceived += OnServerMessage;
            _client.ConnectionClosed += OnConnectionClosed;
            _client.ErrorOccurred += OnErrorOccurred;

            Logger.Debug("События клиента подписаны");
        }

        private async Task<bool> ConnectToServerAsync()
        {
            if (_client == null)
            {
                Logger.Warn("Клиент не инициализирован, выполняется повторная инициализация");
                InitializeClient();
            }

            try
            {
                Logger.Info("Попытка подключения к серверу");
                await _client.ConnectAsync();
                _connected = true;
                Logger.Info("Успешное подключение к серверу");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка подключения к серверу");
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
                Logger.Info($"Успешная аутентификация пользователя: {message.Username}");

                var player = new Player()
                {
                    Id = message.Id,
                    Username = message.Username,
                    TotalGames = message.TotalGames,
                    Score = message.TotalScore,
                    Wins = message.Wins
                };
                UnsubscribeFromEvents();
                var menuForm = new MenuForm(player, _client);

                Logger.Info("Переход к MenuForm");
                this.Hide();
                menuForm.Show();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при переходе в меню");
                MessageBox.Show($"{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnsubscribeFromEvents()
        {
            Logger.Debug("Отписка от событий клиента");
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
                Logger.Debug($"Получено сообщение от сервера: {serverMessage.Type}");

                switch (serverMessage.Type)
                {
                    case "LoginSuccess":
                        Logger.Info($"Успешный вход пользователя: {serverMessage.Username}");
                        HandleSuccessfulAuth(serverMessage, "Вход выполнен успешно!");
                        break;

                    case "RegisterSuccess":
                        Logger.Info($"Успешная регистрация пользователя: {serverMessage.Username}");
                        HandleSuccessfulAuth(serverMessage, "Регистрация выполнена успешно!");
                        break;

                    case "Error":
                        Logger.Warn($"ошибка: {serverMessage.Message}");
                        MessageBox.Show($"Ошибка: {serverMessage.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        EnableButtons();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка обработки сообщения от сервера");
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

            Logger.Warn($"Соединение закрыто: {reason}");
            _connected = false;
            _isConnecting = false;

            MessageBox.Show($"Соединение закрыто: {reason}", "Соединение потеряно",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            EnableButtons();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            
        }

        private void EnableButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(EnableButtons));
                return;
            }

            Logger.Debug("Активация кнопок интерфейса");
            btnConnect.Enabled = true;
            btnConnect.Text = "ВОЙТИ";
            btnAddRoom.Enabled = true;
            btnAddRoom.Text = "ЗАРЕГИСТРИРОВАТЬСЯ";
            _isConnecting = false;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnecting)
            {
                Logger.Debug("Попытка повторного нажатия кнопки входа во время подключения");
                return;
            }

            Logger.Info("Начало процесса входа в аккаунт");

            if (string.IsNullOrWhiteSpace(AuthorizationTxt.Text))
            {
                Logger.Warn("Попытка входа с пустым именем пользователя");
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PassswordTxt.Text))
            {
                Logger.Warn("Попытка входа с пустым паролем");
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = AuthorizationTxt.Text.Trim();
            Logger.Info($"Попытка входа пользователя: {username}");

            _isConnecting = true;
            btnConnect.Enabled = false;
            btnAddRoom.Enabled = false;
            btnConnect.Text = "Подключение...";

            try
            {
                if (!_connected)
                {
                    Logger.Debug("Соединение не установлено, выполняется подключение");
                    bool connectionResult = await ConnectToServerAsync();
                    if (!connectionResult)
                    {
                        Logger.Error("Не удалось установить соединение для входа");
                        EnableButtons();
                        return;
                    }
                }
                Logger.Debug($"Отправка данных для входа пользователя: {username}");
                await _client.LoginAsync(username, PassswordTxt.Text);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при входе пользователя: {username}");
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableButtons();
            }
        }
        private async void btnAddRoom_Click(object sender, EventArgs e)
        {
            if (_isConnecting)
            {
                Logger.Debug("Попытка повторного нажатия кнопки регистрации во время подключения");
                return;
            }

            Logger.Info("Начало процесса регистрации");
            if (string.IsNullOrWhiteSpace(AuthorizationTxt.Text))
            {
                Logger.Warn("Попытка регистрации с пустым именем пользователя");
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthorizationTxt.Text.Trim().Length < 3)
            {
                Logger.Warn($"Попытка регистрации с коротким именем пользователя: {AuthorizationTxt.Text.Trim()}");
                MessageBox.Show("Имя пользователя должно содержать минимум 3 символа", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PassswordTxt.Text))
            {
                Logger.Warn("Попытка регистрации с пустым паролем");
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (PassswordTxt.Text.Length < 6)
            {
                Logger.Warn("Попытка регистрации с коротким паролем");
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = AuthorizationTxt.Text.Trim();
            Logger.Info($"Попытка регистрации пользователя: {username}");

            _isConnecting = true;
            btnConnect.Enabled = false;
            btnAddRoom.Enabled = false;
            btnAddRoom.Text = "Регистрация...";

            try
            {
                if (!_connected)
                {
                    Logger.Debug("Соединение не установлено, выполняется подключение");
                    bool connectionResult = await ConnectToServerAsync();
                    if (!connectionResult)
                    {
                        Logger.Error("Не удалось установить соединение для регистрации");
                        EnableButtons();
                        return;
                    }
                }

                // Отправляем данные для регистрации
                Logger.Debug($"Отправка данных для регистрации пользователя: {username}");
                await _client.RegisterAsync(username, PassswordTxt.Text);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при регистрации пользователя: {username}");
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableButtons();
            }
        }
        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLang = LanguageComboBox.SelectedItem.ToString();
            string culture = selectedLang == "Русский" ? "ru" : "en";

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Controls.Clear();
            InitializeComponent();
            LanguageComboBox.SelectedItem = selectedLang; 
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info("Закрытие EntryForm");
            UnsubscribeFromEvents();

            if (_client != null && _connected)
            {
                try
                {
                    Logger.Debug("Отключение от сервера при закрытии формы");
                    _client.DisconnectAsync().Wait(1000);
                    Logger.Info("Успешное отключение от сервера");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Ошибка при отключении от сервера");
                    Console.WriteLine($"Ошибка при отключении: {ex.Message}");
                }
            }

            base.OnFormClosing(e);
        }
    }
}