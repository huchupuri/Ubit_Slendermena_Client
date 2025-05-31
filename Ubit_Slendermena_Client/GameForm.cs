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

namespace JeopardyGame
{
    public  class JeopardyGameForm : Form
    {
        private readonly GameNetworkClient _networkClient = Program.form._client;
        private readonly Player _currentPlayer;

        // Игровое поле
        private Button[,] _gameButtons = new Button[6, 6]; 
        private Panel _gamePanel;
        private Panel _questionPanel;
        private Panel _playersPanel;

        // Элементы вопроса
        private Label _questionCategoryLabel;
        private Label _questionTextLabel;
        private Label _questionPriceLabel;
        private TextBox _answerTextBox;
        private Button _submitAnswerButton;
        private Label _timerLabel;
        private System.Windows.Forms.Timer _questionTimer;
        private ListBox _playersListBox;
        private Label _gameStatusLabel;
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
                    _networkClient.MessageReceived -= OnServerMessage;
                    var questionForm= new QuestionForm(message.Message);
                    questionForm.ShowDialog();
                    break;
                default : MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"Своя игра - {_currentPlayer.Username}";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.DarkBlue;
            this.MinimumSize = new Size(1200, 800);

            CreateGamePanel();
            CreateQuestionPanel();
            CreatePlayersPanel();
            CreateTimer();

            this.Controls.AddRange(new Control[] { _gamePanel, _questionPanel, _playersPanel });
        }

        private void CreateGamePanel()
        {
            _gamePanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1000, 700),
                BackColor = Color.DarkBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = true
            };

            // Создаем сетку кнопок 6x6
            int buttonWidth = 160;
            int buttonHeight = 110;
            int spacing = 5;

            for (int col = 0; col < 6; col++)
            {
                for (int row = 0; row < 6; row++)
                {
                    var button = new Button
                    {
                        Location = new Point(col * (buttonWidth + spacing) + 10, row * (buttonHeight + spacing) + 10),
                        Size = new Size(buttonWidth, buttonHeight),
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Tag = new { CategoryIndex = col, QuestionIndex = row - 1 }
                    };

                    if (row == 0)
                    {
                        // Заголовок категории
                        button.BackColor = Color.Gold;
                        button.ForeColor = Color.DarkBlue;
                        button.Text = $"Категория {col + 1}";
                        button.Enabled = false;
                    }
                    else
                    {
                        // Кнопка вопроса
                        int price = row * 100;
                        button.BackColor = Color.Blue;
                        button.ForeColor = Color.White;
                        button.Text = price.ToString();
                        button.FlatAppearance.BorderColor = Color.LightBlue;
                        button.FlatAppearance.BorderSize = 2;
                        button.Click += QuestionButton_Click;

                        // Эффект наведения
                        button.MouseEnter += (s, e) => {
                            if (button.Enabled && _isMyTurn)
                            {
                                button.BackColor = Color.LightBlue;
                                button.ForeColor = Color.DarkBlue;
                            }
                        };
                        button.MouseLeave += (s, e) => {
                            if (button.Enabled)
                            {
                                button.BackColor = Color.Blue;
                                button.ForeColor = Color.White;
                            }
                        };
                    }

                    _gameButtons[col, row] = button;
                    _gamePanel.Controls.Add(button);
                }
            }

            // Заголовок игры
            var titleLabel = new Label
            {
                Text = "🎮 СВОЯ ИГРА",
                Location = new Point(10, 670),
                Size = new Size(980, 30),
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _gamePanel.Controls.Add(titleLabel);
        }

        private void CreateQuestionPanel()
        {
            _questionPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1000, 700),
                BackColor = Color.DarkBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            // Категория вопроса
            _questionCategoryLabel = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(960, 50),
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "КАТЕГОРИЯ"
            };

            // Стоимость вопроса
            _questionPriceLabel = new Label
            {
                Location = new Point(20, 80),
                Size = new Size(960, 60),
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Yellow,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "500"
            };

            // Текст вопроса
            _questionTextLabel = new Label
            {
                Location = new Point(20, 160),
                Size = new Size(960, 250),
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Текст вопроса будет здесь...",
                BackColor = Color.Navy,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Поле для ответа
            var answerLabel = new Label
            {
                Text = "💭 Ваш ответ:",
                Location = new Point(20, 430),
                Size = new Size(200, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White
            };

            _answerTextBox = new TextBox
            {
                Location = new Point(20, 470),
                Size = new Size(600, 40),
                Font = new Font("Arial", 16),
                Enabled = false,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            _answerTextBox.KeyPress += AnswerTextBox_KeyPress;

            _submitAnswerButton = new Button
            {
                Text = "✅ Ответить",
                Location = new Point(640, 470),
                Size = new Size(120, 40),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _submitAnswerButton.FlatAppearance.BorderColor = Color.LightGreen;
            _submitAnswerButton.Click += SubmitAnswerButton_Click;

            // Таймер
            _timerLabel = new Label
            {
                Location = new Point(780, 430),
                Size = new Size(200, 80),
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "⏰ 30",
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка возврата
            var backButton = new Button
            {
                Text = "🔙 К игровому полю",
                Location = new Point(20, 530),
                Size = new Size(180, 40),
                BackColor = Color.DarkGray,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            backButton.Click += (s, e) => ShowGameBoard();

            _questionPanel.Controls.AddRange(new Control[] {
                _questionCategoryLabel, _questionPriceLabel, _questionTextLabel,
                answerLabel, _answerTextBox, _submitAnswerButton, _timerLabel, backButton
            });
        }

        private void CreatePlayersPanel()
        {
            _playersPanel = new Panel
            {
                Location = new Point(1020, 10),
                Size = new Size(350, 700),
                BackColor = Color.Navy,
                BorderStyle = BorderStyle.FixedSingle
            };

            var playersTitle = new Label
            {
                Text = "🏆 ИГРОКИ",
                Location = new Point(10, 10),
                Size = new Size(330, 40),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _playersListBox = new ListBox
            {
                Location = new Point(10, 60),
                Size = new Size(330, 400),
                Font = new Font("Arial", 12),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            _gameStatusLabel = new Label
            {
                Location = new Point(10, 480),
                Size = new Size(330, 200),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Text = "🔗 Подключен к серверу\n⏳ Ожидание начала игры...",
                TextAlign = ContentAlignment.TopLeft
            };

            _playersPanel.Controls.AddRange(new Control[] { playersTitle, _playersListBox, _gameStatusLabel });
        }

        private void CreateTimer()
        {
            _questionTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // 1 секунда
            };
            _questionTimer.Tick += QuestionTimer_Tick;
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
            string serverAddress = "localhost";
            int port = 5000;

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
            _gamePanel.Visible = true;
            _questionPanel.Visible = false;
        }

        private void ShowQuestionView()
        {
            _gamePanel.Visible = false;
            _questionPanel.Visible = true;
        }

        private void UpdateGameStatus(string message, Color color)
        {
            
        }

        private void QuestionTimer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;
            _timerLabel.Text = $"⏰ {_timeLeft}";

            if (_timeLeft <= 10)
            {
                _timerLabel.ForeColor = Color.Red;
            }
            else if (_timeLeft <= 20)
            {
                _timerLabel.ForeColor = Color.Orange;
            }

            if (_timeLeft <= 0)
            {
                _questionTimer.Stop();
                _answerTextBox.Enabled = false;
                _submitAnswerButton.Enabled = false;
                _canAnswer = false;

                UpdateGameStatus("⏰ Время вышло!", Color.Red);

                // Отправляем пустой ответ
                if (_currentQuestion != null)
                {
                    _ = _networkClient.SendMessageAsync(new
                    {
                        Type = "SubmitAnswer",
                        QuestionId = _currentQuestion.Id,
                        Answer = "",
                        PlayerId = _currentPlayer.Id
                    });
                }
            }
        }

        private void JeopardyGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _questionTimer?.Stop();
            _networkClient?.Disconnect();
        }
    }
}