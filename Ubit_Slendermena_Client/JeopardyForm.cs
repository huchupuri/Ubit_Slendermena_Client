using GameClient.Models;
using GameClient.Network;

namespace GameClient.Forms;

public partial class JeopardyForm : Form
{
    private readonly GameNetworkClient _client;
    private readonly Player _currentPlayer;

    // Панели интерфейса
    private Panel pnlHeader;
    private Panel pnlGameBoard;
    private Panel pnlQuestionView;
    private Panel pnlPlayers;

    // Элементы заголовка
    private Label lblGameTitle;
    private Label lblCurrentPlayer;
    private Button btnLeaveGame;

    // Элементы игрового поля
    private Button[,] _categoryButtons = new Button[6, 6]; // 6 категорий x 6 строк (заголовок + 5 цен)
    private List<Category> _categories = new();
    private List<Question> _questions = new();
    private bool[,] _answeredQuestions = new bool[6, 5]; // 6 категорий x 5 вопросов

    // Элементы экрана вопроса
    private Label lblQuestionCategory;
    private Label lblQuestionText;
    private Label lblQuestionPrice;
    private TextBox txtAnswer;
    private Button btnSubmitAnswer;
    private Button btnBackToBoard;
    private Label lblQuestionTimer;
    private System.Windows.Forms.Timer _questionTimer;
    private int _timeLeft = 30;

    // Элементы списка игроков
    private Label lblPlayersTitle;
    private ListBox lstPlayers;
    private Label lblGameStatus;

    // Состояние игры
    private Question? _currentQuestion;
    private List<Player> _players = new();
    private bool _isGameStarted = false;
    private bool _isMyTurn = false;

    public JeopardyForm(GameNetworkClient client, Player currentPlayer)
    {
        _client = client;
        _currentPlayer = currentPlayer;
        InitializeComponent();
        SetupEventHandlers();
        ShowGameBoard();
    }

    private void InitializeComponent()
    {
        this.Text = $"Своя игра - {_currentPlayer.Username}";
        this.Size = new Size(1400, 900);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.DarkBlue;
        this.MinimumSize = new Size(1200, 800);

        CreateHeaderPanel();
        CreateGameBoardPanel();
        CreateQuestionViewPanel();
        CreatePlayersPanel();
        CreateTimer();

        this.Controls.AddRange(new Control[] { pnlHeader, pnlGameBoard, pnlQuestionView, pnlPlayers });
    }

    private void CreateHeaderPanel()
    {
        pnlHeader = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(1400, 80),
            BackColor = Color.Navy,
            Dock = DockStyle.Top
        };

        lblGameTitle = new Label
        {
            Text = "🎮 СВОЯ ИГРА",
            Location = new Point(20, 20),
            Size = new Size(300, 40),
            Font = new Font("Arial", 24, FontStyle.Bold),
            ForeColor = Color.Gold,
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblCurrentPlayer = new Label
        {
            Text = $"Игрок: {_currentPlayer.Username}",
            Location = new Point(400, 25),
            Size = new Size(400, 30),
            Font = new Font("Arial", 14, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft
        };

        btnLeaveGame = new Button
        {
            Text = "❌ Покинуть игру",
            Location = new Point(1200, 20),
            Size = new Size(150, 40),
            BackColor = Color.DarkRed,
            ForeColor = Color.White,
            Font = new Font("Arial", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat
        };
        btnLeaveGame.FlatAppearance.BorderColor = Color.Red;
        btnLeaveGame.Click += BtnLeaveGame_Click;

        pnlHeader.Controls.AddRange(new Control[] { lblGameTitle, lblCurrentPlayer, btnLeaveGame });
    }

    private void CreateGameBoardPanel()
    {
        pnlGameBoard = new Panel
        {
            Location = new Point(10, 90),
            Size = new Size(1000, 700),
            BackColor = Color.DarkBlue,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = true
        };

        // Создаем сетку кнопок для игрового поля
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
                    Tag = new { Category = col, Question = row - 1 } // row 0 = заголовок категории
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
                    button.Click += QuestionButton_Click;
                    button.FlatAppearance.BorderColor = Color.LightBlue;
                    button.FlatAppearance.BorderSize = 2;
                }

                _categoryButtons[col, row] = button;
                pnlGameBoard.Controls.Add(button);
            }
        }
    }

    private void CreateQuestionViewPanel()
    {
        pnlQuestionView = new Panel
        {
            Location = new Point(10, 90),
            Size = new Size(1000, 700),
            BackColor = Color.DarkBlue,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = false
        };

        // Категория вопроса
        lblQuestionCategory = new Label
        {
            Location = new Point(20, 20),
            Size = new Size(960, 40),
            Font = new Font("Arial", 18, FontStyle.Bold),
            ForeColor = Color.Gold,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "КАТЕГОРИЯ"
        };

        // Стоимость вопроса
        lblQuestionPrice = new Label
        {
            Location = new Point(20, 70),
            Size = new Size(960, 50),
            Font = new Font("Arial", 24, FontStyle.Bold),
            ForeColor = Color.Yellow,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "500"
        };

        // Текст вопроса
        lblQuestionText = new Label
        {
            Location = new Point(20, 140),
            Size = new Size(960, 300),
            Font = new Font("Arial", 20, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Текст вопроса будет здесь...",
            BackColor = Color.Navy,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Поле для ответа
        var lblAnswerPrompt = new Label
        {
            Text = "💭 Ваш ответ:",
            Location = new Point(20, 460),
            Size = new Size(200, 30),
            Font = new Font("Arial", 14, FontStyle.Bold),
            ForeColor = Color.White
        };

        txtAnswer = new TextBox
        {
            Location = new Point(20, 500),
            Size = new Size(600, 40),
            Font = new Font("Arial", 16),
            Enabled = false,
            BackColor = Color.White,
            ForeColor = Color.Black
        };
        txtAnswer.KeyPress += TxtAnswer_KeyPress;

        btnSubmitAnswer = new Button
        {
            Text = "✅ Ответить",
            Location = new Point(640, 500),
            Size = new Size(120, 40),
            BackColor = Color.Green,
            ForeColor = Color.White,
            Font = new Font("Arial", 12, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        btnSubmitAnswer.FlatAppearance.BorderColor = Color.LightGreen;
        btnSubmitAnswer.Click += BtnSubmitAnswer_Click;

        // Таймер
        lblQuestionTimer = new Label
        {
            Location = new Point(780, 460),
            Size = new Size(200, 80),
            Font = new Font("Arial", 24, FontStyle.Bold),
            ForeColor = Color.Red,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "⏰ 30",
            BackColor = Color.Black,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Кнопка возврата к игровому полю
        btnBackToBoard = new Button
        {
            Text = "🔙 К игровому полю",
            Location = new Point(20, 560),
            Size = new Size(180, 40),
            BackColor = Color.DarkGray,
            ForeColor = Color.White,
            Font = new Font("Arial", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Visible = false
        };
        btnBackToBoard.Click += BtnBackToBoard_Click;

        pnlQuestionView.Controls.AddRange(new Control[] {
            lblQuestionCategory, lblQuestionPrice, lblQuestionText,
            lblAnswerPrompt, txtAnswer, btnSubmitAnswer,
            lblQuestionTimer, btnBackToBoard
        });
    }

    private void CreatePlayersPanel()
    {
        pnlPlayers = new Panel
        {
            Location = new Point(1020, 90),
            Size = new Size(350, 700),
            BackColor = Color.Navy,
            BorderStyle = BorderStyle.FixedSingle
        };

        lblPlayersTitle = new Label
        {
            Text = "🏆 ИГРОКИ",
            Location = new Point(10, 10),
            Size = new Size(330, 40),
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.Gold,
            TextAlign = ContentAlignment.MiddleCenter
        };

        lstPlayers = new ListBox
        {
            Location = new Point(10, 60),
            Size = new Size(330, 400),
            Font = new Font("Arial", 12),
            BackColor = Color.White,
            ForeColor = Color.Black
        };

        lblGameStatus = new Label
        {
            Location = new Point(10, 480),
            Size = new Size(330, 200),
            Font = new Font("Arial", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Text = "🔗 Подключен к серверу\n⏳ Ожидание начала игры...",
            TextAlign = ContentAlignment.TopLeft
        };

        pnlPlayers.Controls.AddRange(new Control[] { lblPlayersTitle, lstPlayers, lblGameStatus });
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
        _client.MessageReceived += OnMessageReceived;
        _client.Disconnected += OnDisconnected;
        this.FormClosing += GameForm_FormClosing;
    }

    private void QuestionButton_Click(object? sender, EventArgs e)
    {
        if (!_isGameStarted || !_isMyTurn) return;

        var button = sender as Button;
        var tag = button?.Tag as dynamic;

        if (tag == null) return;

        int categoryIndex = tag.Category;
        int questionIndex = tag.Question;

        // Проверяем, не отвечен ли уже этот вопрос
        if (_answeredQuestions[categoryIndex, questionIndex])
        {
            ShowStatus("❌ Этот вопрос уже был отвечен!", Color.Red);
            return;
        }

        // Отправляем выбор вопроса на сервер
        _ = _client.SendMessageAsync(new
        {
            Type = "SelectQuestion",
            CategoryId = categoryIndex + 1, // ID категории (1-based)
            Price = (questionIndex + 1) * 100 // Цена вопроса
        });

        ShowStatus("📤 Вопрос выбран, ожидание от сервера...", Color.Orange);
    }

    private void OnMessageReceived(ServerMessage message)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<ServerMessage>(OnMessageReceived), message);
            return;
        }

        switch (message.Type)
        {
            case "GameData":
                LoadGameData(message);
                break;

            case "GameStarted":
                HandleGameStarted(message);
                break;

            case "PlayerTurn":
                HandlePlayerTurn(message);
                break;

            case "Question":
                ShowQuestion(message.Question);
                break;

            case "AnswerResult":
                HandleAnswerResult(message);
                break;

            case "QuestionCompleted":
                HandleQuestionCompleted(message);
                break;

            case "GameOver":
                HandleGameOver(message);
                break;

            case "PlayerJoined":
                if (message.Player != null)
                {
                    ShowStatus($"🎉 Игрок {message.Player.Username} присоединился к игре", Color.Blue);
                }
                break;

            case "PlayerLeft":
                ShowStatus($"👋 Игрок {message.Username} покинул игру", Color.Orange);
                break;

            case "Error":
                ShowStatus($"❌ Ошибка: {message.Message}", Color.Red);
                break;
        }
    }

    private void LoadGameData(ServerMessage message)
    {
        _categories = message.Categories;
        _isGameStarted = true;

        // Обновляем заголовки категорий
        for (int i = 0; i < Math.Min(_categories.Count, 6); i++)
        {
            _categoryButtons[i, 0].Text = _categories[i].Name;
        }

        ShowStatus("✅ Данные игры загружены", Color.Green);
    }

    private void HandleGameStarted(ServerMessage message)
    {
        _players = message.Players;
        UpdatePlayersList();
        ShowStatus("🎮 Игра началась! Удачи!", Color.Green);
    }

    private void HandlePlayerTurn(ServerMessage message)
    {
        _isMyTurn = message.Id == _currentPlayer.Id;

        if (_isMyTurn)
        {
            ShowStatus("🎯 Ваш ход! Выберите вопрос.", Color.Green);
            lblCurrentPlayer.Text = $"Игрок: {_currentPlayer.Username} (ВАШ ХОД)";
            lblCurrentPlayer.ForeColor = Color.Yellow;
        }
        else
        {
            var currentPlayer = _players.FirstOrDefault(p => p.Id == message.Id);
            string playerName = currentPlayer?.Username ?? "Неизвестный игрок";
            ShowStatus($"⏳ Ход игрока: {playerName}", Color.Orange);
            lblCurrentPlayer.Text = $"Ход: {playerName}";
            lblCurrentPlayer.ForeColor = Color.White;
        }
    }

    private void ShowQuestion(Question? question)
    {
        if (question == null) return;

        _currentQuestion = question;

        // Заполняем данные вопроса
        lblQuestionCategory.Text = question.CategoryName.ToUpper();
        lblQuestionPrice.Text = $"💰 {question.Price} ОЧКОВ";
        lblQuestionText.Text = question.Text;

        // Сбрасываем поле ответа
        txtAnswer.Text = "";
        txtAnswer.Enabled = true;
        btnSubmitAnswer.Enabled = true;
        txtAnswer.Focus();

        // Запускаем таймер
        _timeLeft = 30;
        lblQuestionTimer.Text = $"⏰ {_timeLeft}";
        lblQuestionTimer.ForeColor = Color.Yellow;
        _questionTimer.Start();

        // Переключаемся на экран вопроса
        ShowQuestionView();

        ShowStatus("⏰ Время отвечать! У вас 30 секунд.", Color.Blue);
    }

    private void HandleAnswerResult(ServerMessage message)
    {
        _questionTimer.Stop();
        txtAnswer.Enabled = false;
        btnSubmitAnswer.Enabled = false;

        string resultText = message.IsCorrect
            ? $"✅ {message.Username} ответил правильно! (+{_currentQuestion?.Price} очков)"
            : $"❌ {message.Username} ответил неправильно. Правильный ответ: {message.CorrectAnswer}";

        ShowStatus(resultText, message.IsCorrect ? Color.Green : Color.Red);

        // Обновляем счет игроков
        UpdatePlayersList();
    }

    private void HandleQuestionCompleted(ServerMessage message)
    {
        if (_currentQuestion != null)
        {
            // Отмечаем вопрос как отвеченный
            int categoryIndex = _categories.FindIndex(c => c.Name == _currentQuestion.CategoryName);
            int questionIndex = (_currentQuestion.Price / 100) - 1;

            if (categoryIndex >= 0 && questionIndex >= 0 && categoryIndex < 6 && questionIndex < 5)
            {
                _answeredQuestions[categoryIndex, questionIndex] = true;

                // Делаем кнопку неактивной
                var button = _categoryButtons[categoryIndex, questionIndex + 1];
                button.BackColor = Color.DarkGray;
                button.ForeColor = Color.Gray;
                button.Enabled = false;
                button.Text = "✓";
            }
        }

        // Показываем кнопку возврата к игровому полю
        btnBackToBoard.Visible = true;

        ShowStatus("✅ Вопрос завершен. Можете вернуться к игровому полю.", Color.Green);
    }

    private void HandleGameOver(ServerMessage message)
    {
        _questionTimer.Stop();
        _isGameStarted = false;
        _isMyTurn = false;

        ShowGameBoard();

        string winnerText = message.Winner != null
            ? $"🏆 Победитель: {message.Winner.Username}!"
            : "🎮 Игра окончена!";

        ShowStatus($"🎊 Игра завершена! {winnerText}", Color.Purple);

        // Показываем подробные результаты
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

        // Обновляем список игроков
        _players = message.Players;
        UpdatePlayersList();
    }

    private void UpdatePlayersList()
    {
        lstPlayers.Items.Clear();

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

            lstPlayers.Items.Add(playerInfo);
        }
    }

    private void ShowGameBoard()
    {
        pnlGameBoard.Visible = true;
        pnlQuestionView.Visible = false;
    }

    private void ShowQuestionView()
    {
        pnlGameBoard.Visible = false;
        pnlQuestionView.Visible = true;
    }

    private void ShowStatus(string message, Color color)
    {
        lblGameStatus.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        lblGameStatus.ForeColor = color;
    }

    private async void BtnSubmitAnswer_Click(object? sender, EventArgs e)
    {
        await SubmitAnswer();
    }

    private void TxtAnswer_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            _ = SubmitAnswer();
        }
    }

    private async Task SubmitAnswer()
    {
        if (_currentQuestion == null || string.IsNullOrWhiteSpace(txtAnswer.Text))
            return;

        await _client.SendMessageAsync(new
        {
            Type = "Answer",
            QuestionId = _currentQuestion.Id,
            Answer = txtAnswer.Text.Trim()
        });

        txtAnswer.Enabled = false;
        btnSubmitAnswer.Enabled = false;
        _questionTimer.Stop();

        ShowStatus("📤 Ответ отправлен. Ожидание результата...", Color.Orange);
    }

    private void BtnBackToBoard_Click(object? sender, EventArgs e)
    {
        ShowGameBoard();
        btnBackToBoard.Visible = false;
    }

    private void QuestionTimer_Tick(object? sender, EventArgs e)
    {
        _timeLeft--;
        lblQuestionTimer.Text = $"⏰ {_timeLeft}";

        if (_timeLeft <= 10)
        {
            lblQuestionTimer.ForeColor = Color.Red;
        }
        else if (_timeLeft <= 20)
        {
            lblQuestionTimer.ForeColor = Color.Orange;
        }

        if (_timeLeft <= 0)
        {
            _questionTimer.Stop();
            txtAnswer.Enabled = false;
            btnSubmitAnswer.Enabled = false;
            ShowStatus("⏰ Время вышло!", Color.Red);

            // Отправляем пустой ответ
            _ = _client.SendMessageAsync(new
            {
                Type = "Answer",
                QuestionId = _currentQuestion?.Id ?? 0,
                Answer = ""
            });
        }
    }

    private async void BtnLeaveGame_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Вы уверены, что хотите покинуть игру?",
            "Подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            await _client.SendMessageAsync(new { Type = "LeaveGame" });
            this.Close();
        }
    }

    private void OnDisconnected()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(OnDisconnected));
            return;
        }

        MessageBox.Show("❌ Соединение с сервером потеряно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Close();
    }

    private void GameForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _questionTimer?.Stop();
        _client.Disconnect();
    }
}
