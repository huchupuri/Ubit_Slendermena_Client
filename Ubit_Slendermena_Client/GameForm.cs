using GameClient.Models;
using GameClient.Network;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class JeopardyGameForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GameClient.Network.GameClient _networkClient;
        private readonly Player _currentPlayer;

        private List<Category> _categories = new();
        private List<Player> _players = new();
        private Question _currentQuestion;
        private bool[,] _answeredQuestions = new bool[6, 5];
        private bool _canAnswer = true;
        private int _timeLeft = 60;
        private bool _isMyTurn = true;

        public JeopardyGameForm(Player currentPlayer, GameClient.Network.GameClient client, List<Player> players)
        {
            Logger.Info($"Инициализация JeopardyGameForm для игрока: {currentPlayer?.Username}");
            _players = players ?? new List<Player>(); 

            _currentPlayer = currentPlayer;
            _networkClient = client;
            InitializeComponent();
            InitializePlayersList();
            SubscribeToEvents();

            if (!_networkClient.IsConnected)
            {
                Logger.Error("Попытка создания JeopardyGameForm без соединения с сервером");
                this.Close();
                return;
            }

            Logger.Debug($"JeopardyGameForm успешно создана для игрока ID={currentPlayer?.Id}");
        }

        private void InitializePlayersList()
        {
            Logger.Debug("Инициализация списка игроков");
            _playersListBox.Items.Clear();

            if (_players?.Any() == true)
            {
                Logger.Info($"Добавление {_players.Count} игроков в список");
                foreach (var player in _players)
                {
                    string playerInfo = $"{player.Username}";
                    if (player.Id == _currentPlayer?.Id)
                    {
                        playerInfo += " (вы)";
                    }
                    playerInfo += $" - {player.Score} очков";

                    _playersListBox.Items.Add(playerInfo);
                    Logger.Debug($"Добавлен игрок: {playerInfo}");
                }
            }
            else
            {
                Logger.Warn("Список игроков пуст или null");
                _playersListBox.Items.Add("Загрузка игроков...");
            }
        }


        private void SubscribeToEvents()
        {
            Logger.Debug("Подписка на события сетевого клиента в JeopardyGameForm");
            if (_networkClient != null)
            {
                _networkClient.MessageReceived += OnServerMessage;
                _networkClient.ConnectionClosed += OnConnectionClosed;
                _networkClient.ErrorOccurred += OnErrorOccurred;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_networkClient != null)
            {
                _networkClient.MessageReceived -= OnServerMessage;
                _networkClient.ConnectionClosed -= OnConnectionClosed;
                _networkClient.ErrorOccurred -= OnErrorOccurred;
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
                Logger.Debug($"JeopardyGameForm получил сообщение: {serverMessage.Type}");

                switch (serverMessage.Type)
                {
                    case "GameData":
                        Logger.Info("Получены данные игры");
                        LoadGameData(serverMessage);
                        break;

                    case "Question":
                        Logger.Info($"Получен вопрос ID={serverMessage.QuestionId}");
                        HandleQuestionReceived(serverMessage);
                        break;

                    case "AnswerResult":
                        Logger.Info($"Получен результат ответа: {(serverMessage.IsCorrect ? "правильно" : "неправильно")}");
                        HandleAnswerResult(serverMessage);
                        break;

                    case "QuestionCompleted":
                        Logger.Info("Вопрос завершен");
                        HandleQuestionCompleted(serverMessage);
                        break;

                    case "QuestionTimeout":
                        Logger.Warn("Время на вопрос истекло");
                        HandleQuestionTimeout(serverMessage);
                        break;

                    case "GameOver":
                        Logger.Info("Игра завершена");
                        HandleGameOver(serverMessage);
                        break;

                    case "Error":
                        Logger.Error($"Получена ошибка от сервера: {serverMessage.Message}");
                        UpdateGameStatus($"❌ Ошибка: {serverMessage.Message}", Color.Red);
                        break;

                    default:
                        Logger.Warn($"Получено неизвестное сообщение: {serverMessage.Type}");
                        Console.WriteLine($"Неизвестное сообщение: {serverMessage.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка jgf");
                MessageBox.Show($"Ошибка обработки сообщения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleQuestionReceived(ServerMessage serverMessage)
        {
            try
            {
                if (serverMessage.Question != null)
                {
                    var question = new Question
                    {
                        Id = serverMessage.Question.Id,
                        Text = serverMessage.Question.Text,
                        Price = serverMessage.Question.Price,
                        CategoryId = serverMessage.Question.CategoryId,
                        CategoryName = serverMessage.Question.CategoryName
                    };
                    var btn = _gameButtons[question.CategoryId-1, question.Id - 6* question.CategoryId + 6]; 
                    if (btn != null)
                    {
                        btn.Enabled = false;
                        btn.BackColor = Color.DarkGray;
                        btn.Text = "закрыт";
                    }
                    Logger.Info($"Показ вопроса: ID={question.Id}, Цена={question.Price}, Категория={question.CategoryName}");
                    ShowQuestionInForm(question);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при обработке полученного вопроса");
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (!string.IsNullOrEmpty(serverMessage.Message))
                {
                    var question = new Question
                    {
                        Id = serverMessage.QuestionId,
                        Text = serverMessage.Message,
                        Price = 100,
                        CategoryName = "Категория"
                    };
                    ShowQuestionInForm(question);
                }
            }
        }

        private void ShowQuestionInForm(Question question)
        {
            _currentQuestion = question;
            var questionForm = new QuestionForm(question, _networkClient, _currentPlayer);
            questionForm.Show();
        }

        private void LoadGameData(ServerMessage message)
        {
            Logger.Debug("Загрузка данных игры");

            if (message.Categories?.Any() == true)
            {
                _categories = message.Categories;
                Logger.Info($"Загружено {_categories.Count} категорий");
                for (int i = 0; i < Math.Min(_categories.Count, 6); i++)
                {
                    if (_gameButtons[i, 0] != null)
                    {
                        _gameButtons[i, 0].Text = _categories[i].Name;
                        Logger.Debug($"Установлена категория {i}: {_categories[i].Name}");
                    }
                }
            }
            if (message.Players?.Any() == true)
            {
                Logger.Info($"Получен список игроков от сервера: {message.Players.Count}");
                _players.Clear();
                foreach (var serverPlayer in message.Players)
                {
                    var player = new Player
                    {
                        Id = serverPlayer.Id,
                        Username = serverPlayer.Username,
                        Score = serverPlayer.Score
                    };
                    _players.Add(player);
                    Logger.Debug($"Добавлен игрок: {player.Username} (Score: {player.Score})");
                }

                UpdatePlayersList();
            }
            else
            {
                Logger.Warn("Сервер не прислал список игроков или список пуст");
            }
        }

        private void HandleAnswerResult(ServerMessage message)
        {
            string playerName = message.PlayerName ?? "Неизвестный игрок";
            Logger.Info($"Результат ответа игрока {playerName}: {(message.IsCorrect ? "правильно" : "неправильно")}, новый счет: {message.NewScore}");

            if (message.IsCorrect)
            {
                UpdateGameStatus($"✅ {playerName} ответил правильно", Color.Green);
            }
            else
            {
                UpdateGameStatus($"❌ {playerName} ответил неправильно (-{_currentQuestion?.Price} очков)", Color.Red);
            }
            var player = _players.FirstOrDefault(p => p.Id == message.Id);
            if (player != null)
            {
                player.Score = message.NewScore;
                Logger.Debug($"Обновлен счет игрока {player.Username}: {player.Score}");
                UpdatePlayersList(); 
            }
            else
            {
                var playerByName = _players.FirstOrDefault(p => p.Username == playerName);
                if (playerByName != null)
                {
                    playerByName.Score = message.NewScore;
                    Logger.Debug($"Обновлен счет игрока по имени {playerByName.Username}: {playerByName.Score}");
                    UpdatePlayersList();
                }
            }
        }

        private void HandleQuestionCompleted(ServerMessage message)
        {
            Logger.Debug("Обработка завершения вопроса");

            if (_currentQuestion != null)
            {
                //поиск индексов
                int categoryIndex = _categories.FindIndex(c => c.Name == _currentQuestion.CategoryName);
                int questionIndex = (_currentQuestion.Price / 100) - 1;

                if (categoryIndex >= 0 && questionIndex >= 0 && categoryIndex < 6 && questionIndex < 5)
                {
                    _answeredQuestions[categoryIndex, questionIndex] = true;
                    var button = _gameButtons[categoryIndex, questionIndex + 1];
                    button.BackColor = Color.DarkGray;
                    button.ForeColor = Color.Gray;
                    button.Enabled = false;
                    button.Text = "ок";

                    Logger.Debug($"Вопрос отмечен как завершенный: категория {categoryIndex}, вопрос {questionIndex}");
                }
            }

            UpdateGameStatus("Вопрос завершен", Color.Green);
        }

        private void HandleQuestionTimeout(ServerMessage message)
        {
            Logger.Warn($"Время на вопрос истекло. Правильный ответ: {message.CorrectAnswer}");
            UpdateGameStatus($"Время вышло! Правильный ответ: {message.CorrectAnswer}", Color.Red);
        }

        private void HandleGameOver(ServerMessage message)
        {
            string winnerName = message.Winner?.Username ?? "Неизвестный";
            Logger.Info($"Игра завершена. Победитель: {winnerName}");

            string winnerText = message.Winner != null
                ? $"Победитель: {message.Winner.Username}!"
                : "🎮 Игра окончена!";

            UpdateGameStatus($"🎊 Игра завершена! {winnerText}", Color.Purple);
            if (message.Players?.Any() == true)
            {
                _players = message.Players;
                UpdatePlayersList();
                string results = "📊 ИТОГОВЫЕ РЕЗУЛЬТАТЫ:\n\n";
                var sortedPlayers = message.Players.OrderByDescending(p => p.Score).ToList();

                for (int i = 0; i < sortedPlayers.Count; i++)
                {
                    var player = sortedPlayers[i];
                    string medal = i == 0 ? "1 место " : i == 1 ? "2 место " : i == 2 ? "3 место " : "участник ";
                    results += $"{medal} {i + 1}. {player.Username}: {player.Score} очков\n";
                }

                Logger.Info($"Показ итоговых результатов игры с {sortedPlayers.Count} игроками");
                MessageBox.Show(results, "Игра завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Возвращаемся в меню через 5 секунд
            Logger.Debug("Запуск таймера для автоматического закрытия формы");
            var exitTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            exitTimer.Tick += (s, e) =>
            {
                exitTimer.Stop();
                UnsubscribeFromEvents();
                this.Close();
            };
            exitTimer.Start();
        }

        private async void QuestionButton_Click(object sender, EventArgs e)
        {
            if (!_networkClient.IsConnected)
            {
                Logger.Error("Попытка выбора вопроса без соединения с сервером");
                MessageBox.Show("Нет соединения с сервером", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var button = sender as Button;
            var tag = button?.Tag as dynamic;

            if (tag == null)
            {
                Logger.Warn("Нажата кнопка вопроса без корректного тега");
                return;
            }

            int categoryIndex = tag.CategoryIndex;
            int questionIndex = tag.QuestionIndex;

            Logger.Info($"Выбран вопрос: категория {categoryIndex}, вопрос {questionIndex}");
            int categoryId = categoryIndex + 1;

            Logger.Debug($"Отправка запроса на выбор вопроса");
            await _networkClient.SendMessageAsync(new
            {
                Type = "SelectQuestion",
                QuestionId = questionIndex + 1,
                CategoryId = categoryIndex + 1,
                PlayerId = _currentPlayer.Id
            });

            UpdateGameStatus("Вопрос выбран, ожидание от сервера...", Color.Orange);
        }

        private void UpdatePlayersList()
        {
            Logger.Debug("Обновление списка игроков");

            if (InvokeRequired)
            {
                Invoke(new Action(UpdatePlayersList));
                return;
            }

            _playersListBox.Items.Clear();

            if (_players?.Any() != true)
            {
                Logger.Warn("Список игроков пуст при обновлении");
                _playersListBox.Items.Add("Нет игроков");
                return;
            }

            var sortedPlayers = _players.OrderByDescending(p => p.Score).ToList();
            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                string position = i == 0 ? "1 " : i == 1 ? "2 " : i == 2 ? "3 " : $"{i + 1}.";
                string playerInfo = $"{position} {player.Username}";

                playerInfo += $" - {player.Score} очков";

                _playersListBox.Items.Add(playerInfo);
                Logger.Debug($"Обновлен игрок: {playerInfo}");
            }
        }



        private void UpdateGameStatus(string message, Color color)
        {
            if (_gameStatusLabel != null)
            {
                _gameStatusLabel.Text = message;
                _gameStatusLabel.ForeColor = color;
            }
        }

        private void OnConnectionClosed(object sender, string reason)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(OnConnectionClosed), sender, reason);
                return;
            }

            Logger.Error($"Соединение потеряно в JeopardyGameForm: {reason}");
            MessageBox.Show($"Соединение потеряно: {reason}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            UnsubscribeFromEvents();
            this.Close();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Exception>(OnErrorOccurred), sender, ex);
                return;
            }

            Logger.Error(ex, "Ошибка соединения в JeopardyGameForm");
            UpdateGameStatus($"Ошибка соединения: {ex.Message}", Color.Red);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info($"Закрытие JeopardyGameForm для игрока: {_currentPlayer?.Username}");
            UnsubscribeFromEvents();
            base.OnFormClosing(e);
        }
    }
}