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

        // Игровые данные
        private List<Category> _categories = new();
        private List<Player> _players = new();
        private Question _currentQuestion;
        private bool[,] _answeredQuestions = new bool[6, 5];
        private bool _canAnswer = true;
        private int _timeLeft = 60;
        private bool _isMyTurn = true;

        public JeopardyGameForm(Player currentPlayer, GameClient.Network.GameClient client)
        {
            Logger.Info($"Инициализация JeopardyGameForm для игрока: {currentPlayer?.Username}");

            _currentPlayer = currentPlayer;
            _networkClient = client;
            InitializeComponent();
            SubscribeToEvents();

            if (!_networkClient.IsConnected)
            {
                Logger.Error("Попытка создания JeopardyGameForm без соединения с сервером");
                this.Close();
                return;
            }

            Logger.Debug($"JeopardyGameForm успешно создана для игрока ID={currentPlayer?.Id}");
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
            Logger.Debug("Отписка от событий сетевого клиента в JeopardyGameForm");
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
                Logger.Error(ex, "Ошибка обработки сообщения от сервера в JeopardyGameForm");
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

                    Logger.Info($"Показ вопроса: ID={question.Id}, Цена={question.Price}, Категория={question.CategoryName}");
                    // Показываем вопрос в отдельной форме
                    ShowQuestionInForm(question);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при обработке полученного вопроса");
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Fallback для старого формата
                if (!string.IsNullOrEmpty(serverMessage.Message))
                {
                    Logger.Debug("Использование fallback формата для вопроса");
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

            // Создаем и показываем форму вопроса
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

                // Обновляем заголовки категорий
                for (int i = 0; i < Math.Min(_categories.Count, 6); i++)
                {
                    _gameButtons[i, 0].Text = _categories[i].Name;
                }
            }

            if (message.Players?.Any() == true)
            {
                _players = message.Players;
                Logger.Info($"Загружено {_players.Count} игроков");
                UpdatePlayersList();
            }

            UpdateGameStatus("✅ Игра началась! Выберите вопрос.", Color.Green);
        }

        private void HandleAnswerResult(ServerMessage message)
        {
            string playerName = message.PlayerName ?? "Неизвестный игрок";
            Logger.Info($"Результат ответа игрока {playerName}: {(message.IsCorrect ? "правильно" : "неправильно")}, новый счет: {message.NewScore}");

            if (message.IsCorrect)
            {
                UpdateGameStatus($"✅ {playerName} ответил правильно! (+{_currentQuestion?.Price} очков)", Color.Green);
            }
            else
            {
                UpdateGameStatus($"❌ {playerName} ответил неправильно (-{_currentQuestion?.Price} очков)", Color.Red);
            }

            // Обновляем счет игрока
            var player = _players.FirstOrDefault(p => p.Id == message.Id);
            if (player != null)
            {
                player.Score = message.NewScore;
                UpdatePlayersList();
                Logger.Debug($"Обновлен счет игрока {player.Username}: {player.Score}");
            }
        }

        private void HandleQuestionCompleted(ServerMessage message)
        {
            Logger.Debug("Обработка завершения вопроса");

            if (_currentQuestion != null)
            {
                // Отмечаем вопрос как отвеченный
                int categoryIndex = _categories.FindIndex(c => c.Name == _currentQuestion.CategoryName);
                int questionIndex = (_currentQuestion.Price / 100) - 1;

                if (categoryIndex >= 0 && questionIndex >= 0 && categoryIndex < 6 && questionIndex < 5)
                {
                    _answeredQuestions[categoryIndex, questionIndex] = true;

                    // Делаем кнопку неактивной
                    var button = _gameButtons[categoryIndex, questionIndex + 1];
                    button.BackColor = Color.DarkGray;
                    button.ForeColor = Color.Gray;
                    button.Enabled = false;
                    button.Text = "✓";

                    Logger.Debug($"Вопрос отмечен как завершенный: категория {categoryIndex}, вопрос {questionIndex}");
                }
            }

            UpdateGameStatus("✅ Вопрос завершен. Выберите следующий вопрос.", Color.Green);
        }

        private void HandleQuestionTimeout(ServerMessage message)
        {
            Logger.Warn($"Время на вопрос истекло. Правильный ответ: {message.CorrectAnswer}");
            UpdateGameStatus($"⏰ Время вышло! Правильный ответ: {message.CorrectAnswer}", Color.Red);
        }

        private void HandleGameOver(ServerMessage message)
        {
            string winnerName = message.Winner?.Username ?? "Неизвестный";
            Logger.Info($"Игра завершена. Победитель: {winnerName}");

            string winnerText = message.Winner != null
                ? $"🏆 Победитель: {message.Winner.Username}!"
                : "🎮 Игра окончена!";

            UpdateGameStatus($"🎊 Игра завершена! {winnerText}", Color.Purple);

            // Показываем результаты
            if (message.Players?.Any() == true)
            {
                string results = "📊 ИТОГОВЫЕ РЕЗУЛЬТАТЫ:\n\n";
                var sortedPlayers = message.Players.OrderByDescending(p => p.Score).ToList();

                for (int i = 0; i < sortedPlayers.Count; i++)
                {
                    var player = sortedPlayers[i];
                    string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : "🏅";
                    results += $"{medal} {i + 1}. {player.Username}: {player.Score} очков\n";
                }

                Logger.Info($"Показ итоговых результатов игры с {sortedPlayers.Count} игроками");
                MessageBox.Show(results, "🎉 Игра завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Возвращаемся в меню через 5 секунд
            Logger.Debug("Запуск таймера для автоматического закрытия формы");
            var exitTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            exitTimer.Tick += (s, e) =>
            {
                exitTimer.Stop();
                Logger.Info("Автоматическое закрытие JeopardyGameForm");
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
                MessageBox.Show("Нет соединения с сервером!", "Ошибка",
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

            if (_answeredQuestions[categoryIndex, questionIndex])
            {
                Logger.Warn($"Попытка выбора уже отвеченного вопроса: категория {categoryIndex}, вопрос {questionIndex}");
                UpdateGameStatus("❌ Этот вопрос уже был отвечен!", Color.Red);
                return;
            }

            button.Enabled = false;
            button.BackColor = Color.Gray;
            button.Text = "...";

            try
            {
                int categoryId = categoryIndex + 1;

                Logger.Debug($"Отправка запроса на выбор вопроса: CategoryId={categoryId}, PlayerId={_currentPlayer.Id}");
                await _networkClient.SendMessageAsync(new
                {
                    Type = "SelectQuestion",
                    CategoryId = categoryId,
                    PlayerId = _currentPlayer.Id
                });

                UpdateGameStatus("📤 Вопрос выбран, ожидание от сервера...", Color.Orange);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при отправке запроса на выбор вопроса: категория {categoryIndex}, вопрос {questionIndex}");
                UpdateGameStatus($"❌ Ошибка отправки: {ex.Message}", Color.Red);

                button.Enabled = true;
                button.BackColor = Color.Blue;
                button.Text = $"{(questionIndex + 1) * 100}";
            }
        }

        private void UpdatePlayersList()
        {
            Logger.Debug("Обновление списка игроков");
            _playersListBox.Items.Clear();

            var sortedPlayers = _players.OrderByDescending(p => p.Score).ToList();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                string position = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
                string playerInfo = $"{position} {player.Username}";

                if (player.Id == _currentPlayer.Id)
                {
                    playerInfo += " (ВЫ)";
                }

                playerInfo += $"\n    💰 {player.Score} очков";
                playerInfo += $"\n    🏆 Побед: {player.Wins}/{player.TotalGames}";

                _playersListBox.Items.Add(playerInfo);
            }
        }

        private void UpdateGameStatus(string message, Color color)
        {
            Logger.Debug($"Обновление статуса игры: {message}");
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
            UpdateGameStatus($"❌ Ошибка соединения: {ex.Message}", Color.Red);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info($"Закрытие JeopardyGameForm для игрока: {_currentPlayer?.Username}");
            UnsubscribeFromEvents();
            base.OnFormClosing(e);
        }
    }
}