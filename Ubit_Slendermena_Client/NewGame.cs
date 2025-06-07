using GameClient.Models;
using NLog;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private QuestionFile _customQuestions = null;
        private CultureInfo culture;
        public NewGame(Player player, GameClient.Network.GameClient client, CultureInfo culture)
        {
            Logger.Info($"Инициализация NewGame для игрока: {player?.Username}");
            
            InitializeComponent();
            _player = player;
            _networkClient = client;
            SubscribeToEvents();
            UpdateUI();
            this.culture = culture;

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

            if (_customQuestions != null)
            {
                btnUploadQuestions.Text = $"Вопросы загружены ({_customQuestions.Categories.Sum(c => c.Questions.Count)})";
                btnUploadQuestions.BackColor = Color.LightGreen;
            }
            else
            {
                btnUploadQuestions.Text = "Загрузить вопросы";
                btnUploadQuestions.BackColor = SystemColors.Control;
            }
        }

        private void btnUploadQuestions_Click(object sender, EventArgs e)
        {
            Logger.Info("Нажата кнопка загрузки вопросов");

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";
            dialog.Title = "Выберите файл с вопросами";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Logger.Info($"Выбран файл вопросов: {dialog.FileName}");

                    string jsonContent = File.ReadAllText(dialog.FileName);
                    var questionFile = JsonSerializer.Deserialize<QuestionFile>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (ValidateQuestionFile(questionFile))
                    {
                        _customQuestions = questionFile;
                        UpdateUI();

                        int totalQuestions = questionFile.Categories.Sum(c => c.Questions.Count);
                        Logger.Info($"Успешно загружено {totalQuestions} вопросов из {questionFile.Categories.Count} категорий");
                    }
                    else
                    {
                        Logger.Warn("Файл вопросов не прошел валидацию");
                        MessageBox.Show("Файл не соответствует требуемому формату или содержит недостаточно вопросов.",
                                      "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Общая ошибка при загрузке файла");
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Logger.Debug("Загрузка файла вопросов отменена");
            }
        }

        private bool ValidateQuestionFile(QuestionFile questionFile)
        {
            if (questionFile?.Categories == null || questionFile.Categories.Count == 0)
            {
                Logger.Warn("Файл не содержит категорий");
                return false;
            }

            int totalQuestions = 0;
            foreach (var category in questionFile.Categories)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    Logger.Warn("Найдена категория без названия");
                    return false;
                }

                if (category.Questions == null || category.Questions.Count == 0)
                {
                    Logger.Warn($"Категория '{category.Name}' не содержит вопросов");
                    return false;
                }

                foreach (var question in category.Questions)
                {
                    if (string.IsNullOrWhiteSpace(question.Text) ||
                        string.IsNullOrWhiteSpace(question.Answer) ||
                        question.Price <= 0)
                    {
                        Logger.Warn($"Некорректный вопрос в категории '{category.Name}'");
                        return false;
                    }
                }

                totalQuestions += category.Questions.Count;
            }

            if (totalQuestions < 30)
            {
                Logger.Warn($"Недостаточно вопросов: {totalQuestions}, требуется минимум 30");
                return false;
            }

            return true;
        }
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку создания игры");

            try
            {
                if (_gameCreated)
                {
                    MessageBox.Show("Игра уже создана", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!_networkClient.IsConnected)
                {
                    Logger.Warn("Попытка создания игры без соединения с сервером");
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
                if (_customQuestions != null)
                {
                    Logger.Info($"Создание игры с пользовательскими вопросами: {_customQuestions.Categories.Count} категорий, {_customQuestions.Categories.Sum(c => c.Questions.Count)} вопросов");
                }
                else
                {
                    Logger.Info("Создание игры со стандартными вопросами");
                }
                await _networkClient.CreateGameAsync(playerCount, _player.Username, _customQuestions);
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

        private void OpenGameForm(List<Player> players)
        {
            Logger.Info("Переход к JeopardyGameForm");
            UnsubscribeFromEvents();
            var gameForm = new JeopardyGameForm(_player, _networkClient, players);
            gameForm.Show();
        }

        private async void HostBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку присоединения к игре");

            try
            {
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
                        _currentPlayers = 1; 
                        UpdateUI();

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
                        if (_currentPlayers >= _requiredPlayers)
                        {
                            Logger.Info("Достаточно игроков для начала игры");
                            MessageBox.Show("Все игроки подключились! Игра начинается...", "Игра начинается",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case "GameStarted":
                        Logger.Info("Получено уведомление о начале игры");

                        MessageBox.Show($"{message.Players[0].Username}", "Ошибка",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
                        OpenGameForm(message.Players);
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
