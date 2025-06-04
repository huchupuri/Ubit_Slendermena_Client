using Ubit_Slendermena_Client.Technical;
namespace Ubit_Slendermena_Client
{
    partial class JeopardyGameForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // JeopardyGameForm
            // 
            BackColor = Color.Black;
            BackgroundImage = Properties.Resource.bigbrain;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1382, 853);
            MinimumSize = new Size(1200, 800);
            Name = "JeopardyGameForm";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
            CreateGamePanel();
            CreateQuestionPanel();
            CreatePlayersPanel();
            CreateTimer();
            this.Controls.Add(_gamePanel);
            this.Controls.Add(_questionPanel);
            this.Controls.Add(_playersPanel);
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

           

            _questionPanel.Controls.AddRange(new Control[] {
                _questionCategoryLabel, _questionPriceLabel, _questionTextLabel,
                answerLabel, _answerTextBox, _submitAnswerButton, _timerLabel
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
                        button.MouseEnter += (s, e) =>
                        {
                            if (button.Enabled && _isMyTurn)
                            {
                                button.BackColor = Color.LightBlue;
                                button.ForeColor = Color.DarkBlue;
                            }
                        };
                        button.MouseLeave += (s, e) =>
                        {
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
        #endregion
        private ListBox _playersListBox;
        private Label _gameStatusLabel;
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
    }
}