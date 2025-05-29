using GameClient.Network;
using GameClient.Models;

namespace GameClient.Forms;

public partial class ConnectionForm : Form
{
    private TabControl tabControl;
    private TabPage tabLogin;
    private TabPage tabRegister;

    // Общие поля
    private TextBox txtServerAddress;
    private TextBox txtPort;
    private Button btnConnect;
    private Label lblStatus;

    // Поля для входа
    private TextBox txtLoginUsername;
    private TextBox txtLoginPassword;
    private Label lblLoginUsername;
    private Label lblLoginPassword;

    // Поля для регистрации
    private TextBox txtRegUsername;
    private TextBox txtRegPassword;
    private TextBox txtRegConfirmPassword;
    private Label lblRegUsername;
    private Label lblRegPassword;
    private Label lblRegConfirmPassword;

    private GameNetworkClient? _client;
    private bool _isConnecting = false;

    public ConnectionForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        grpServer = new GroupBox();
        lblServerAddress = new Label();
        txtServerAddress = new TextBox();
        lblPort = new Label();
        txtPort = new TextBox();
        tabControl = new TabControl();
        btnConnect = new Button();
        lblStatus = new Label();
        grpServer.SuspendLayout();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Location = new Point(45, 73);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(100, 23);
        lblTitle.TabIndex = 0;
        // 
        // grpServer
        // 
        grpServer.Controls.Add(lblTitle);
        grpServer.Controls.Add(lblServerAddress);
        grpServer.Controls.Add(txtServerAddress);
        grpServer.Controls.Add(txtPort);
        grpServer.Location = new Point(0, 0);
        grpServer.Name = "grpServer";
        grpServer.Size = new Size(470, 301);
        grpServer.TabIndex = 1;
        grpServer.TabStop = false;
        // 
        // lblServerAddress
        // 
        lblServerAddress.Location = new Point(206, 62);
        lblServerAddress.Name = "lblServerAddress";
        lblServerAddress.Size = new Size(100, 23);
        lblServerAddress.TabIndex = 0;
        // 
        // txtServerAddress
        // 
        txtServerAddress.Location = new Point(12, 124);
        txtServerAddress.Name = "txtServerAddress";
        txtServerAddress.Size = new Size(100, 27);
        txtServerAddress.TabIndex = 1;
        // 
        // lblPort
        // 
        lblPort.Location = new Point(123, 346);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(100, 23);
        lblPort.TabIndex = 2;
        // 
        // txtPort
        // 
        txtPort.Location = new Point(276, 153);
        txtPort.Name = "txtPort";
        txtPort.Size = new Size(100, 27);
        txtPort.TabIndex = 3;
        // 
        // tabControl
        // 
        tabControl.Location = new Point(0, 0);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(200, 100);
        tabControl.TabIndex = 2;
        // 
        // btnConnect
        // 
        btnConnect.FlatAppearance.BorderColor = Color.Green;
        btnConnect.Location = new Point(0, 0);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(75, 23);
        btnConnect.TabIndex = 3;
        btnConnect.Click += BtnConnect_Click;
        // 
        // lblStatus
        // 
        lblStatus.Location = new Point(0, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(100, 23);
        lblStatus.TabIndex = 4;
        // 
        // ConnectionForm
        // 
        BackColor = Color.WhiteSmoke;
        ClientSize = new Size(482, 403);
        Controls.Add(grpServer);
        Controls.Add(tabControl);
        Controls.Add(btnConnect);
        Controls.Add(lblPort);
        Controls.Add(lblStatus);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "ConnectionForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Подключение к игре 'Своя игра'";
        grpServer.ResumeLayout(false);
        grpServer.PerformLayout();
        ResumeLayout(false);
    }

    private void CreateLoginTab()
    {
        tabLogin = new TabPage("Вход");
        tabLogin.BackColor = Color.White;

        var lblInfo = new Label
        {
            Text = "Войдите с существующим именем пользователя:",
            Location = new Point(20, 20),
            Size = new Size(400, 20),
            Font = new Font("Arial", 9)
        };

        lblLoginUsername = new Label
        {
            Text = "Имя пользователя:",
            Location = new Point(20, 50),
            Size = new Size(120, 20),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        txtLoginUsername = new TextBox
        {
            Location = new Point(150, 48),
            Size = new Size(250, 23),
            Font = new Font("Arial", 10)
        };

        lblLoginPassword = new Label
        {
            Text = "Пароль:",
            Location = new Point(20, 80),
            Size = new Size(120, 20),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        txtLoginPassword = new TextBox
        {
            Location = new Point(150, 78),
            Size = new Size(250, 23),
            Font = new Font("Arial", 10),
            PasswordChar = '*'
        };

        var lblHint = new Label
        {
            Text = "💡 Тестовые пользователи: ivan_petrov, maria_sidorova, alex_kozlov",
            Location = new Point(20, 120),
            Size = new Size(400, 40),
            Font = new Font("Arial", 8),
            ForeColor = Color.Blue
        };

        tabLogin.Controls.AddRange(new Control[] { lblInfo, lblLoginUsername, txtLoginUsername, lblLoginPassword, txtLoginPassword, lblHint });
    }

    private void CreateRegisterTab()
    {
        tabRegister = new TabPage("Регистрация");
        tabRegister.BackColor = Color.White;

        var lblInfo = new Label
        {
            Text = "Создайте новый аккаунт:",
            Location = new Point(20, 20),
            Size = new Size(400, 20),
            Font = new Font("Arial", 9)
        };

        lblRegUsername = new Label
        {
            Text = "Имя пользователя:",
            Location = new Point(20, 50),
            Size = new Size(120, 20),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        txtRegUsername = new TextBox
        {
            Location = new Point(150, 48),
            Size = new Size(250, 23),
            Font = new Font("Arial", 10)
        };

        lblRegPassword = new Label
        {
            Text = "Пароль:",
            Location = new Point(20, 80),
            Size = new Size(120, 20),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        txtRegPassword = new TextBox
        {
            Location = new Point(150, 78),
            Size = new Size(250, 23),
            Font = new Font("Arial", 10),
            PasswordChar = '*'
        };

        lblRegConfirmPassword = new Label
        {
            Text = "Подтвердите пароль:",
            Location = new Point(20, 110),
            Size = new Size(120, 20),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        txtRegConfirmPassword = new TextBox
        {
            Location = new Point(150, 108),
            Size = new Size(250, 23),
            Font = new Font("Arial", 10),
            PasswordChar = '*'
        };

        var lblHint = new Label
        {
            Text = "⚠️ Имя пользователя должно быть уникальным (минимум 3 символа)",
            Location = new Point(20, 140),
            Size = new Size(400, 20),
            Font = new Font("Arial", 8),
            ForeColor = Color.Orange
        };

        tabRegister.Controls.AddRange(new Control[] {
            lblInfo, lblRegUsername, txtRegUsername,
            lblRegPassword, txtRegPassword, lblRegConfirmPassword, txtRegConfirmPassword, lblHint
        });
    }

    private async void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (_isConnecting) return;

        // Валидация полей
        if (!ValidateInput())
            return;

        _isConnecting = true;
        btnConnect.Enabled = false;
        btnConnect.Text = "Подключение...";
        lblStatus.Text = "Подключение к серверу...";
        lblStatus.ForeColor = Color.Orange;

        try
        {
            // Подключение к серверу
            _client = new GameNetworkClient();
            _client.MessageReceived += OnServerMessage;

            string serverAddress = txtServerAddress.Text.Trim();
            int port = int.Parse(txtPort.Text);

            bool connected = await _client.ConnectAsync(serverAddress, port);

            if (!connected)
            {
                ShowError("Не удалось подключиться к серверу");
                return;
            }

            lblStatus.Text = "Аутентификация...";

            // Отправка данных аутентификации
            if (tabControl.SelectedTab == tabLogin)
            {
                await _client.SendMessageAsync(new
                {
                    Type = "Login",
                    Username = txtLoginUsername.Text.Trim(),
                    Password = txtLoginPassword.Text
                });
            }
            else
            {
                await _client.SendMessageAsync(new
                {
                    Type = "Register",
                    Username = txtRegUsername.Text.Trim(),
                    Password = txtRegPassword.Text
                });
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка подключения: {ex.Message}");
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtServerAddress.Text))
        {
            ShowError("Введите адрес сервера");
            return false;
        }

        if (!int.TryParse(txtPort.Text, out int port) || port <= 0 || port > 65535)
        {
            ShowError("Введите корректный порт (1-65535)");
            return false;
        }

        // Проверка полей аутентификации
        if (tabControl.SelectedTab == tabLogin)
        {
            if (string.IsNullOrWhiteSpace(txtLoginUsername.Text))
            {
                ShowError("Введите имя пользователя");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLoginPassword.Text))
            {
                ShowError("Введите пароль");
                return false;
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(txtRegUsername.Text))
            {
                ShowError("Введите имя пользователя");
                return false;
            }

            if (txtRegUsername.Text.Length < 3)
            {
                ShowError("Имя пользователя должно содержать минимум 3 символа");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRegPassword.Text))
            {
                ShowError("Введите пароль");
                return false;
            }

            if (txtRegPassword.Text.Length < 6)
            {
                ShowError("Пароль должен содержать минимум 6 символов");
                return false;
            }

            if (txtRegPassword.Text != txtRegConfirmPassword.Text)
            {
                ShowError("Пароли не совпадают");
                return false;
            }
        }

        return true;
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
                ShowError(message.Username);

                HandleSuccessfulAuth(message, "Вход выполнен успешно!");
                break;

            case "RegisterSuccess":
                HandleSuccessfulAuth(message!, "Регистрация выполнена успешно!");
                break;

            case "LoginFailed":
                ShowError($"Ошибка входа: {message}");
                break;

            case "RegisterFailed":
                ShowError($"Ошибка регистрации: {message}");
                break;
        }
    }

    private void HandleSuccessfulAuth(ServerMessage message, string successMessage)
    {
        lblStatus.Text = successMessage;
        lblStatus.ForeColor = Color.Green;

        // Показываем информацию о игроке
        string playerInfo = $"Добро пожаловать, {message.Username}!\n" +
                           $"Игр сыграно: {message.TotalGames}\n" +
                           $"Побед: {message.Wins}\n" +
                           $"Общий счет: {message.TotalScore}";

        MessageBox.Show(playerInfo, "Успешная аутентификация", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Открываем игровую форму
        var player = new Player()
        {
            Id = message.Id,
            Username = message.Username,
            TotalGames = message.TotalGames,
            Score = message.TotalScore,
            Wins = message.Wins
        };
        var gameForm = new JeopardyForm(_client!, player);
        this.Hide();
        gameForm.ShowDialog();
        this.Close();
    }

    private void ShowError(string message)
    {
        _isConnecting = false;
        btnConnect.Enabled = true;
        btnConnect.Text = "Подключиться";
        lblStatus.Text = $"Ошибка: {message}";
        lblStatus.ForeColor = Color.Red;

        MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _client?.Disconnect();
        base.OnFormClosing(e);
    }

    private Label lblTitle;
    private GroupBox grpServer;
    private Label lblServerAddress;
    private Label lblPort;
}
