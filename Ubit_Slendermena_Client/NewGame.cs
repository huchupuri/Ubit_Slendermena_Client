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

        public NewGame(Player player, GameClient.Network.GameClient client)
        {
            Logger.Info($"Инициализация NewGame для игрока: {player?.Username}");

            InitializeComponent();
            _player = player;
            _networkClient = client;

            // Подписываемся на события ПОСЛЕ инициализации компонентов
            SubscribeToEvents();

            Logger.Debug($"NewGame создана для игрока ID={player?.Id}");
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
            // Отписываемся от событий перед передачей клиента
            UnsubscribeFromEvents();
            var gameForm = new JeopardyGameForm(_player, _networkClient);
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

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку создания игры");

            try
            {
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

                await _networkClient.SendMessageAsync(new
                {
                    Type = "StartGame",
                    playerCount = playerCount
                });

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
                    case "GameStarted":
                        Logger.Info("Получено уведомление о начале игры");
                        OpenGameForm();
                        break;

                    case "Error":
                        Logger.Error($"Получена ошибка от сервера: {message.Message}");
                        MessageBox.Show($"Ошибка: {message.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        btnCreate.Enabled = true;
                        btnCreate.Text = "Создать игру";
                        break;

                    case "PlayerJoined":
                        Logger.Info($"Игрок присоединился к игре: {message.Message}");
                        MessageBox.Show($"Игрок присоединился: {message.Message}", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Можно добавить логику обновления списка игроков
                        OpenGameForm();
                        break;

                    default:
                        Logger.Warn($"Получено неизвестное сообщение в NewGame: {message.Type}");
                        Console.WriteLine($"Неизвестное сообщение: {message.Type}");
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

            // Возвращаемся к предыдущей форме или закрываем
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
            // Отписываемся от событий при закрытии формы
            UnsubscribeFromEvents();
            base.OnFormClosing(e);
        }

        async private void HostBtn_Click(object sender, EventArgs e)
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

                btnCreate.Enabled = false;
                btnCreate.Text = "Подключение...";

                Logger.Debug("Отправка запроса на присоединение к игре");
                await _networkClient.SendMessageAsync(new
                {
                    Type = "JoinGame"
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при присоединении к игре для игрока {_player?.Username}");
                MessageBox.Show($"Ошибка при присоединении к игре: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnCreate.Enabled = true;
                btnCreate.Text = "Присоединиться к игре";
            }
        }
    }
}