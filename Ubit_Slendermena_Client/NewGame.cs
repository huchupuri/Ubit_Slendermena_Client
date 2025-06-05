using GameClient.Models;
using NLog;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class NewGame : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Player _player;
        private readonly GameClient.Network.GameClient _networkClient;
        private bool _gameCreated = false;
        private bool _isHost = false;
        private int _requiredPlayers = 0;
        private int _currentPlayers = 0;

        public NewGame(Player player, GameClient.Network.GameClient client)
        {
            Logger.Info($"Инициализация NewGame для игрока: {player?.Username}");

            InitializeComponent();
            _player = player;
            _networkClient = client;
            SubscribeToEvents();

            // Обновляем интерфейс
            UpdateUI();

            Logger.Debug($"NewGame создана для игрока ID={player?.Id}");
        }

        private void UpdateUI()
        {
            if (_gameCreated)
            {
                btnCreate.Text = _isHost ? "Игра создана" : "Игра найдена";
                btnCreate.BackColor = Color.Green;
                btnCreate.Enabled = false;

                HostBtn.Text = "Ожидание игроков...";
                HostBtn.Enabled = false;
            }
            else
            {
                btnCreate.Text = "Создать игру";
                btnCreate.BackColor = SystemColors.Control;
                btnCreate.Enabled = true;

                HostBtn.Text = "Присоединиться к игре";
                HostBtn.Enabled = true;
            }
        }

        private void SubscribeToEvents()
        {
            Logger.Debug("Подписка на события сетевого клиента в NewGame");
            if (_networkClient != null)
            {
                _networkClient.MessageReceived += OnServerMessage;
                _networkClient.ConnectionClosed += OnConnectionClosed;
                _networkClient.ErrorOccurred += OnErrorOccurred;
            }
        }

        private void UnsubscribeFromEvents()
        {
            Logger.Debug("Отписка от событий сетевого клиента в NewGame");
            if (_networkClient != null)
            {
                _networkClient.MessageReceived -= OnServerMessage;
                _networkClient.ConnectionClosed -= OnConnectionClosed;
                _networkClient.ErrorOccurred -= OnErrorOccurred;
            }
        }

        private void OpenGameForm()
        {
            Logger.Info("Переход к JeopardyGameForm");
            UnsubscribeFromEvents();
            var gameForm = new JeopardyGameForm(_player, _networkClient);
            //this.Hide();
            gameForm.Show();
        }

        private void btnUploadQuestions_Click(object sender, EventArgs e)
        {
            Logger.Info("Нажата кнопка загрузки вопросов");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Question Packages (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Logger.Info($"Выбран файл вопросов: {dialog.FileName}");
                MessageBox.Show("Файл загружен: " + dialog.FileName);
            }
            else
            {
                Logger.Debug("Загрузка файла вопросов отменена");
            }
        }

        // СОЗДАНИЕ ИГРЫ
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку создания игры");

            try
            {
                // Проверяем, не создана ли уже игра
                if (_gameCreated)
                {
                    MessageBox.Show("Игра уже создана! Дождитесь подключения игроков.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Проверяем соединение
                if (!_networkClient.IsConnected)
                {
                    Logger.Warn("Попытка создания игры без соединения с сервером");
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Валидация ввода
                if (!int.TryParse(txtPlayerCount.Text, out int playerCount) || playerCount < 1 || playerCount > 255)
                {
                    Logger.Warn($"Некорректное количество игроков: {txtPlayerCount.Text}");
                    MessageBox.Show("Введите корректное количество игроков (1-255)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Logger.Info($"Создание игры на {playerCount} игроков");
                btnCreate.Enabled = false;
                btnCreate.Text = "Создание...";

                _requiredPlayers = playerCount;
                _isHost = true;

                // Используем новый метод CreateGameAsync
                await _networkClient.CreateGameAsync(playerCount, _player.Username);

                Logger.Debug("Запрос на создание игры отправлен");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при создании игры для игрока {_player?.Username}");
                MessageBox.Show($"Ошибка при создании игры: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnCreate.Enabled = true;
                btnCreate.Text = "Создать игру";
            }
        }

        // ПРИСОЕДИНЕНИЕ К ИГРЕ
        private async void HostBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку присоединения к игре");

            try
            {
                // Проверяем соединение
                if (!_networkClient.IsConnected)
                {
                    Logger.Warn("Попытка присоединения к игре без соединения с сервером");
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HostBtn.Enabled = false;
                HostBtn.Text = "Подключение...";
                Logger.Debug("Отправка запроса на присоединение к игре");
                // Используем новый метод JoinGameAsync
                await _networkClient.JoinGameAsync(_player.Username);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при присоединении к игре для игрока {_player?.Username}");
                MessageBox.Show($"Ошибка при присоединении к игре: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                HostBtn.Enabled = true;
                HostBtn.Text = "Присоединиться к игре";
            }
        }

        private void OnServerMessage(object sender, ServerMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ServerMessage>(OnServerMessage), sender, message);
                return;
            }

            try
            {
                Logger.Debug($"NewGame получил сообщение: {message.Type}");

                switch (message.Type)
                {
                    case "GameCreated":
                        Logger.Info("Игра успешно создана");
                        _gameCreated = true;
                        _currentPlayers = 1; // Хост уже в игре
                        UpdateUI();
                        OpenGameForm();


                        MessageBox.Show($"Игра создана! Ожидание игроков ({_currentPlayers}/{_requiredPlayers})", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case "GameJoined":
                        Logger.Info("Успешное присоединение к игре");
                        _gameCreated = true;
                        _isHost = false;
                        UpdateUI();

                        MessageBox.Show("Вы успешно присоединились к игре!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case "PlayerJoined":
                        Logger.Info($"Игрок присоединился: {message.PlayerName}");
                        _currentPlayers++;

                        if (_isHost)
                        {
                            MessageBox.Show($"Игрок {message.PlayerName} присоединился! ({_currentPlayers}/{_requiredPlayers})",
                                "Новый игрок", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        // Проверяем, достаточно ли игроков для начала 
                        if (_currentPlayers >= _requiredPlayers)
                        {
                            Logger.Info("Достаточно игроков для начала игры");
                            MessageBox.Show("Все игроки подключились! Игра начинается...", "Игра начинается",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case "GameStarted":
                        Logger.Info("Получено уведомление о начале игры");

                        // Проверяем, достаточно ли игроков
                        if (message.Players?.Count >= _requiredPlayers || _requiredPlayers == 0)
                        {
                            OpenGameForm();
                        }
                        else
                        {
                            Logger.Warn($"Недостаточно игроков для начала: {message.Players?.Count}/{_requiredPlayers}");
                            MessageBox.Show($"Недостаточно игроков для начала игры! Подключено: {message.Players?.Count}, требуется: {_requiredPlayers}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "GameFull":
                        Logger.Warn("Игра заполнена");
                        MessageBox.Show("Игра заполнена. Попробуйте позже.", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        HostBtn.Enabled = true;
                        HostBtn.Text = "Присоединиться к игре";
                        break;

                    case "NoGameAvailable":
                        Logger.Warn("Нет доступных игр");
                        MessageBox.Show("Нет доступных игр для подключения. Создайте новую игру.", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        HostBtn.Enabled = true;
                        HostBtn.Text = "Присоединиться к игре";
                        break;

                    case "Error":
                        Logger.Error($"Получена ошибка от сервера: {message.Message}");
                        MessageBox.Show($"Ошибка: {message.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Восстанавливаем состояние кнопок
                        if (!_gameCreated)
                        {
                            btnCreate.Enabled = true;
                            btnCreate.Text = "Создать игру";
                        }

                        HostBtn.Enabled = true;
                        HostBtn.Text = "Присоединиться к игре";
                        break;

                    default:
                        Logger.Warn($"Получено неизвестное сообщение в NewGame: {message.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка обработки сообщения от сервера в NewGame");
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

            Logger.Error($"Соединение потеряно в NewGame: {reason}");
            MessageBox.Show($"Соединение потеряно: {reason}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Сбрасываем состояние
            _gameCreated = false;
            _isHost = false;
            _currentPlayers = 0;
            UpdateUI();

            this.Close();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Exception>(OnErrorOccurred), sender, ex);
                return;
            }

            Logger.Error(ex, "Ошибка соединения в NewGame");
            MessageBox.Show($"Ошибка соединения: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info($"Закрытие NewGame для игрока: {_player?.Username}");
            UnsubscribeFromEvents();
            base.OnFormClosing(e);
        }
    }
}