using Ubit_Slendermena_Client.Models;
using Ubit_Slendermena_Client.Network;

namespace Ubit_Slendermena_Client.Forms;

public partial class GameForm : Form
{
    private readonly GameNetworkClient _client;
    private readonly string _playerName;
    private Guid Id = Guid.Empty;

    // UI элементы
    private Panel pnlPlayers;
    private Panel pnlQuestion;
    private Panel pnlAnswer;
    private Label lblCurrentQuestion;
    private Label lblQuestionPrice;
    private TextBox txtAnswer;
    private Button btnSubmitAnswer;
    private Button btnStartGame;
    private ListBox lstPlayers;
    private Label lblGameStatus;

    private Question? _currentQuestion;
    private List<Player> _players = new();

    public GameForm(GameNetworkClient client, string playerName)
    {
        _client = client;
        _playerName = playerName;
        InitializeComponent();
        SetupEventHandlers();
    }

    private void InitializeComponent()
    {
        this.Text = $"Своя игра - {_playerName}";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;

        // Панель игроков
        pnlPlayers = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(250, 300),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblPlayersTitle = new Label
        {
            Text = "Игроки:",
            Location = new Point(5, 5),
            Size = new Size(100, 20),
            Font = new Font("Arial", 10, FontStyle.Bold)
        };

        lstPlayers = new ListBox
        {
            Location = new Point(5, 30),
            Size = new Size(235, 200)
        };

        btnStartGame = new Button
        {
            Text = "Начать игру",
            Location = new Point(5, 240),
            Size = new Size(100, 30),
            BackColor = Color.LightGreen
        };
        btnStartGame.Click += BtnStartGame_Click;

        pnlPlayers.Controls.AddRange(new Control[] { lblPlayersTitle, lstPlayers, btnStartGame });

        // Панель вопроса
        pnlQuestion = new Panel
        {
            Location = new Point(280, 10),
            Size = new Size(500, 200),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.LightYellow
        };

        lblCurrentQuestion = new Label
        {
            Location = new Point(10, 10),
            Size = new Size(480, 120),
            Font = new Font("Arial", 14, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Ожидание начала игры..."
        };

        lblQuestionPrice = new Label
        {
            Location = new Point(10, 140),
            Size = new Size(480, 30),
            Font = new Font("Arial", 12, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Blue
        };

        pnlQuestion.Controls.AddRange(new Control[] { lblCurrentQuestion, lblQuestionPrice });

        // Панель ответа
        pnlAnswer = new Panel
        {
            Location = new Point(280, 230),
            Size = new Size(500, 100),
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblAnswerTitle = new Label
        {
            Text = "Ваш ответ:",
            Location = new Point(10, 10),
            Size = new Size(100, 20),
            Font = new Font("Arial", 10, FontStyle.Bold)
        };

        txtAnswer = new TextBox
        {
            Location = new Point(10, 35),
            Size = new Size(350, 25),
            Font = new Font("Arial", 12),
            Enabled = false
        };
        txtAnswer.KeyPress += TxtAnswer_KeyPress;

        btnSubmitAnswer = new Button
        {
            Text = "Ответить",
            Location = new Point(370, 35),
            Size = new Size(100, 25),
            BackColor = Color.LightBlue,
            Enabled = false
        };
        btnSubmitAnswer.Click += BtnSubmitAnswer_Click;

        pnlAnswer.Controls.AddRange(new Control[] { lblAnswerTitle, txtAnswer, btnSubmitAnswer });

        // Статус игры
        lblGameStatus = new Label
        {
            Location = new Point(10, 350),
            Size = new Size(770, 30),
            Font = new Font("Arial", 12, FontStyle.Bold),
            ForeColor = Color.Green,
            Text = "Подключение к серверу..."
        };

        // Добавление всех панелей на форму
        this.Controls.AddRange(new Control[] { pnlPlayers, pnlQuestion, pnlAnswer, lblGameStatus });
    }

    private void SetupEventHandlers()
    {
        _client.MessageReceived += OnMessageReceived;
        _client.Disconnected += OnDisconnected;
        this.FormClosing += GameForm_FormClosing;
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
            case "LoginSuccess":
                Id = message.Id;
                lblGameStatus.Text = "Подключение успешно! Ожидание других игроков...";
                break;

            case "PlayerJoined":
                lblGameStatus.Text = $"Игрок {message.Username} присоединился к игре";
                break;

            case "PlayerLeft":
                lblGameStatus.Text = $"Игрок {message.Username} покинул игру";
                break;

            case "GameStarted":
                _players = message.Players;
                UpdatePlayersList();
                lblGameStatus.Text = "Игра началась!";
                btnStartGame.Enabled = false;
                break;

            case "Question":
                ShowQuestion(message.Question);
                break;

            case "AnswerResult":
                ShowAnswerResult(message);
                break;

            case "GameOver":
                ShowGameOver(message);
                break;

            case "Error":
                lblGameStatus.Text = $"Ошибка: {message.Message}";
                lblGameStatus.ForeColor = Color.Red;
                break;
        }
    }

    private void ShowQuestion(Question? question)
    {
        if (question == null) return;

        _currentQuestion = question;
        lblCurrentQuestion.Text = $"Категория: {question.CategoryName}\n\n{question.Text}";
        lblQuestionPrice.Text = $"Стоимость: {question.Price} очков";

        txtAnswer.Enabled = true;
        btnSubmitAnswer.Enabled = true;
        txtAnswer.Text = "";
        txtAnswer.Focus();

        lblGameStatus.Text = "Введите ваш ответ!";
        lblGameStatus.ForeColor = Color.Blue;
    }

    private void ShowAnswerResult(ServerMessage message)
    {
        txtAnswer.Enabled = false;
        btnSubmitAnswer.Enabled = false;
        var player = _players.FirstOrDefault(p => p.Id == message.Id);
        if (player != null)
        {
            player.Score = message.NewScore;
            UpdatePlayersList();
        }

        string resultText = message.IsCorrect
            ? $"✅ {message.Username} ответил правильно!"
            : $"❌ {message.Username} ответил неправильно. Правильный ответ: {message.CorrectAnswer}";

        lblGameStatus.Text = resultText;
        lblGameStatus.ForeColor = message.IsCorrect ? Color.Green : Color.Red;
    }

    private void ShowGameOver(ServerMessage message)
    {
        txtAnswer.Enabled = false;
        btnSubmitAnswer.Enabled = false;
        btnStartGame.Enabled = true;

        string winnerText = message.Winner != null
            ? $"🏆 Победитель: {message.Winner.Username} ({message.Winner.CurrentScore} очков)"
            : "Игра окончена!";

        lblCurrentQuestion.Text = "Игра окончена!";
        lblQuestionPrice.Text = winnerText;
        lblGameStatus.Text = "Игра завершена. Можете начать новую игру.";
        lblGameStatus.ForeColor = Color.Purple;

        // Обновляем финальные результаты
        _players = message.Players;
        UpdatePlayersList();
    }

    private void UpdatePlayersList()
    {
        lstPlayers.Items.Clear();
        foreach (var player in _players.OrderByDescending(p => p.Score))
        {
            string playerInfo = $"{player.Username}: {player.Score} очков";
            if (player.Id == Id)
            {
                playerInfo += " (Вы)";
            }
            lstPlayers.Items.Add(playerInfo);
        }
    }

    private async void BtnStartGame_Click(object? sender, EventArgs e)
    {
        await _client.SendMessageAsync(new { Type = "StartGame" });
        btnStartGame.Enabled = false;
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
        lblGameStatus.Text = "Ответ отправлен. Ожидание результата...";
        lblGameStatus.ForeColor = Color.Orange;
    }

    private void OnDisconnected()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(OnDisconnected));
            return;
        }

        MessageBox.Show("Соединение с сервером потеряно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Close();
    }

    private void GameForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _client.Disconnect();
    }
}