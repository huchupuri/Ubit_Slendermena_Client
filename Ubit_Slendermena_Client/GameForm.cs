using GameClient.Models;
using GameClient.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameClient;
using GameClient.Forms;
using Ubit_Slendermena_Client;

namespace Ubit_Slendermena_Client
{
    public partial class JeopardyGameForm : Form
    {
        private readonly GameNetworkClient _networkClient = Program.form._client;
        private readonly Player _currentPlayer;

        // Игровое поле
       
       
        private List<Category> _categories = new();
        private List<Player> _players = new();
        private Question _currentQuestion;
        private bool[,] _answeredQuestions = new bool[6, 5]; 
        private bool _canAnswer = true;
        private int _timeLeft = 30;
        private bool _isMyTurn = false;

        public JeopardyGameForm(Player currentPlayer)
        {
            //_networkClient = networkClient;
            _currentPlayer = currentPlayer;
            InitializeComponent();
            SetupEventHandlers();
            ShowGameBoard();
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
                    break;

                case "RegisterSuccess":
                    break;

                case "Question":
                    MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _networkClient.MessageReceived -= OnServerMessage;
                    var questionForm= new QuestionForm(message.Message);
                    questionForm.ShowDialog();
                    break;
                default : MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
            }
        }

        private void SetupEventHandlers()
        {
            // Добавляем отладочную информацию
            Console.WriteLine($"Настройка обработчиков событий. Клиент подключен: {_networkClient.IsConnected}");

            _networkClient.Disconnected += () =>
            {
                Console.WriteLine("Клиент отключен");
                UpdateGameStatus("❌ Соединение потеряно", Color.Red);
            };
            _networkClient.MessageReceived += OnServerMessage;
            this.FormClosing += JeopardyGameForm_FormClosing;
        }

        private async void QuestionButton_Click(object sender, EventArgs e)
        {

            var button = sender as Button;
            var tag = button?.Tag as dynamic;

            if (tag == null) return;

            int categoryIndex = tag.CategoryIndex;
            int questionIndex = tag.QuestionIndex;

            // Проверяем, не отвечен ли уже этот вопрос
            if (_answeredQuestions[categoryIndex, questionIndex])
            {
                UpdateGameStatus("❌ Этот вопрос уже был отвечен!", Color.Red);
                return;
            }
            button.Enabled = false;
            button.BackColor = Color.Gray;
            button.Text = "...";

            // Отправляем запрос на сервер
            int categoryId = categoryIndex + 1; // ID категории (1-based)
            int price = (questionIndex + 1) * 100; // Цена вопроса

            //// Пересоздаем клиент
            //_client?.Disconnect();
            //_client = new GameNetworkClient();
            await _networkClient.SendMessageAsync(new
            {
                Type = "SelectQuestion",
                CategoryId = categoryId,
                PlayerId = _currentPlayer.Id
            });

            UpdateGameStatus("📤 Вопрос выбран, ожидание от сервера...", Color.Orange);
        }

        private async void SubmitAnswerButton_Click(object sender, EventArgs e)
        {
            await SubmitAnswer();
        }

        private void AnswerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                _ = SubmitAnswer();
            }
        }

        private async Task SubmitAnswer()
        {
            if (_currentQuestion == null || string.IsNullOrWhiteSpace(_answerTextBox.Text) || !_canAnswer)
                return;

            string answer = _answerTextBox.Text.Trim();

            // Отключаем возможность отвечать
            _answerTextBox.Enabled = false;
            _submitAnswerButton.Enabled = false;
            _canAnswer = false;
            _questionTimer.Stop();

            // Отправляем ответ на сервер
            await _networkClient.SendMessageAsync(new
            {
                Type = "SubmitAnswer",
                QuestionId = _currentQuestion.Id,
                Answer = answer,
                PlayerId = _currentPlayer.Id
            });

            UpdateGameStatus("📤 Ответ отправлен. Ожидание результата...", Color.Orange);
        }

        private void LoadGameData(ServerMessage message)
        {
            _categories = message.Categories;
            _players = message.Players;

            // Обновляем заголовки категорий
            for (int i = 0; i < Math.Min(_categories.Count, 6); i++)
            {
                _gameButtons[i, 0].Text = _categories[i].Name;
            }

            UpdatePlayersList();
            UpdateGameStatus("✅ Данные игры загружены", Color.Green);
        }

        private void HandlePlayerTurn(ServerMessage message)
        {
            _isMyTurn = message.Id == _currentPlayer.Id;

            if (_isMyTurn)
            {
                UpdateGameStatus("🎯 Ваш ход! Выберите вопрос.", Color.Green);
                EnableQuestionButtons(true);
            }
            else
            {
                var currentPlayer = _players.FirstOrDefault(p => p.Id == message.Id);
                string playerName = currentPlayer?.Username ?? "Неизвестный игрок";
                UpdateGameStatus($"⏳ Ход игрока: {playerName}", Color.Orange);
                EnableQuestionButtons(false);
            }
        }

        private void ShowQuestion(Question question)
        {
            if (question == null) return;

            _currentQuestion = question;
            _canAnswer = true;

            // Заполняем данные вопроса
            _questionCategoryLabel.Text = question.CategoryName.ToUpper();
            _questionPriceLabel.Text = $"💰 {question.Price} ОЧКОВ";
            _questionTextLabel.Text = question.Text;

            // Сбрасываем поле ответа
            _answerTextBox.Text = "";
            _answerTextBox.Enabled = true;
            _submitAnswerButton.Enabled = true;
            _answerTextBox.Focus();

            // Запускаем таймер
            _timeLeft = 30;
            _timerLabel.Text = $"⏰ {_timeLeft}";
            _timerLabel.ForeColor = Color.Yellow;
            _questionTimer.Start();

            // Переключаемся на экран вопроса
            ShowQuestionView();

            UpdateGameStatus("⏰ Время отвечать! У вас 30 секунд.", Color.Blue);
        }

        private void HandleCorrectAnswer(ServerMessage message)
        {
            _questionTimer.Stop();
            _canAnswer = false;

            if (message.Id == _currentPlayer.Id)
            {
                // Мой правильный ответ
                _currentPlayer.CurrentScore = message.NewScore;
                UpdateGameStatus($"✅ Правильно! +{_currentQuestion?.Price} очков", Color.Green);

                // Показываем сообщение
                MessageBox.Show($"🎉 Правильный ответ!\n\nВаш счет: {_currentPlayer.CurrentScore} очков",
                    "Отлично!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var player = _players.FirstOrDefault(p => p.Id == message.Id);
                if (player != null)
                {
                    player.CurrentScore = message.NewScore;
                    UpdateGameStatus($"✅ {player.Username} ответил правильно! (+{_currentQuestion?.Price} очков)", Color.Green);
                }
            }

            UpdatePlayersList();
        }

        private void HandleIncorrectAnswer(ServerMessage message)
        {
            if (message.Id == _currentPlayer.Id)
            {
                // Мой неправильный ответ
                _currentPlayer.CurrentScore = message.NewScore;
                _canAnswer = false; // Запрещаем отвечать дальше
                _answerTextBox.Enabled = false;
                _submitAnswerButton.Enabled = false;
                _questionTimer.Stop();

                UpdateGameStatus($"❌ Неправильно! -{_currentQuestion?.Price} очков. Правильный ответ: {message.CorrectAnswer}", Color.Red);

                // Показываем сообщение
                MessageBox.Show($"❌ Неправильный ответ!\n\nПравильный ответ: {message.CorrectAnswer}\nВаш счет: {_currentPlayer.CurrentScore} очков",
                    "Неправильно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var player = _players.FirstOrDefault(p => p.Id == message.Id);
                if (player != null)
                {
                    player.CurrentScore = message.NewScore;
                    UpdateGameStatus($"❌ {player.Username} ответил неправильно (-{_currentQuestion?.Price} очков)", Color.Red);
                }
            }

            UpdatePlayersList();
        }

        private void HandleQuestionCompleted(ServerMessage message)
        {
            _questionTimer.Stop();

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
                }
            }

            UpdateGameStatus("✅ Вопрос завершен.", Color.Green);

            // Возвращаемся к игровому полю через 3 секунды
            var returnTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            returnTimer.Tick += (s, e) => {
                returnTimer.Stop();
                ShowGameBoard();
            };
            returnTimer.Start();
        }

        private void HandlePlayerJoined(ServerMessage message)
        {
            if (message.Player != null)
            {
                _players.Add(message.Player);
                UpdatePlayersList();
                UpdateGameStatus($"🎉 {message.Player.Username} присоединился к игре", Color.Blue);
            }
        }

        private void HandlePlayerLeft(ServerMessage message)
        {
            var player = _players.FirstOrDefault(p => p.Username == message.Username);
            if (player != null)
            {
                _players.Remove(player);
                UpdatePlayersList();
                UpdateGameStatus($"👋 {message.Username} покинул игру", Color.Orange);
            }
        }

        private void HandleGameOver(ServerMessage message)
        {
            _questionTimer.Stop();
            ShowGameBoard();

            string winnerText = message.Winner != null
                ? $"🏆 Победитель: {message.Winner.Username}!"
                : "🎮 Игра окончена!";

            UpdateGameStatus($"🎊 Игра завершена! {winnerText}", Color.Purple);

            // Показываем результаты
            if (message.Players.Any())
            {
                string results = "📊 ИТОГОВЫЕ РЕЗУЛЬТАТЫ:\n\n";
                var sortedPlayers = message.Players.OrderByDescending(p => p.CurrentScore).ToList();

                for (int i = 0; i < sortedPlayers.Count; i++)
                {
                    var player = sortedPlayers[i];
                    string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : "🏅";
                    results += $"{medal} {i + 1}. {player.Username}: {player.CurrentScore} очков\n";
                }

                MessageBox.Show(results, "🎉 Игра завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _players = message.Players;
            UpdatePlayersList();
        }

        private void UpdatePlayersList()
        {
            _playersListBox.Items.Clear();

            var sortedPlayers = _players.OrderByDescending(p => p.CurrentScore).ToList();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                string position = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
                string playerInfo = $"{position} {player.Username}";

                if (player.Id == _currentPlayer.Id)
                {
                    playerInfo += " (ВЫ)";
                }

                playerInfo += $"\n    💰 {player.CurrentScore} очков";
                playerInfo += $"\n    🏆 Побед: {player.Wins}/{player.TotalGames}";

                _playersListBox.Items.Add(playerInfo);
            }
        }

        private void EnableQuestionButtons(bool enabled)
        {
            for (int col = 0; col < 6; col++)
            {
                for (int row = 1; row < 6; row++) // Пропускаем заголовки (row 0)
                {
                    var button = _gameButtons[col, row];
                    if (!_answeredQuestions[col, row - 1]) // row - 1 для индекса массива
                    {
                        button.Enabled = enabled;
                        if (enabled)
                        {
                            button.BackColor = Color.Blue;
                            button.ForeColor = Color.White;
                        }
                        else
                        {
                            button.BackColor = Color.DarkBlue;
                            button.ForeColor = Color.LightGray;
                        }
                    }
                }
            }
        }

        private void ShowGameBoard()
        {
            //_gamePanel.Visible = true;
            //_questionPanel.Visible = false;
        }

        private void ShowQuestionView()
        {
            _gamePanel.Visible = false;
            _questionPanel.Visible = true;
        }

        private void UpdateGameStatus(string message, Color color)
        {
            
        }

        

        private void JeopardyGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _questionTimer?.Stop();
            _networkClient?.Disconnect();
        }
    }
}