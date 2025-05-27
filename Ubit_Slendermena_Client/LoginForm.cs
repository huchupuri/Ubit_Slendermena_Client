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
        this.Text = "Подключение к игре 'Своя игра'";
        this.Size = new Size(500, 450);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.WhiteSmoke;

        // Заголовок
        var lblTitle = new Label
        {
            Text = "🎮 Своя игра",
            Location = new Point(20, 15),
            Size = new Size(460, 35),
            Font = new Font("Arial", 18, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.DarkBlue
        };

        // Настройки сервера
        var grpServer = new GroupBox
        {
            Text = "Настройки сервера",
            Location = new Point(20, 60),
            Size = new Size(460, 80),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        var lblServerAddress = new Label
        {
            Text = "Адрес сервера:",
            Location = new Point(15, 25),
            Size = new Size(100, 20)
        };

        txtServerAddress = new TextBox
        {
            Text = "localhost",
            Location = new Point(120, 23),
            Size = new Size(150, 23)
        };

        var lblPort = new Label
        {
            Text = "Порт:",
            Location = new Point(290, 25),
            Size = new Size(40, 20)
        };

        txtPort = new TextBox
        {
            Text = "5000",
            Location = new Point(335, 23),
            Size = new Size(80, 23)
        };

        grpServer.Controls.AddRange(new Control[] { lblServerAddress, txtServerAddress, lblPort, txtPort });

        // Вкладки для входа/регистрации
        tabControl = new TabControl
        {
            Location = new Point(20, 150),
            Size = new Size(460, 220),
            Font = new Font("Arial", 9)
        };

        CreateLoginTab();
        CreateRegisterTab();

        tabControl.TabPages.Add(tabLogin);
        tabControl.TabPages.Add(tabRegister);

        // Кнопка подключения
        btnConnect = new Button
        {
            Text = "Подключиться",
            Location = new Point(200, 385),
            Size = new Size(120, 35),
            BackColor = Color.LightGreen,
            Font = new Font("Arial", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat
        };
        btnConnect.FlatAppearance.BorderColor = Color.Green;
        btnConnect.Click += BtnConnect_Click;

        // Статус
        lblStatus = new Label
        {
            Text = "Готов к подключению",
            Location = new Point(20, 430),
            Size = new Size(460, 20),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Gray,
            Font = new Font("Arial", 9)
        };

        // Добавление всех элементов на форму
        this.Controls.AddRange(new Control[] { lblTitle, grpServer, tabControl, btnConnect, lblStatus });
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
        ShowError("Пароли не совпадают");
        switch (message.Type)
        {
            case "LoginSuccess":
                ShowError("Пароли не совпадают");
                HandleSuccessfulAuth(message.Player!, "Вход выполнен успешно!");
                break;

            case "RegisterSuccess":
                HandleSuccessfulAuth(message.Player!, "Регистрация выполнена успешно!");
                break;

            case "LoginFailed":
                ShowError($"Ошибка входа: {message.Message}");
                break;

            case "RegisterFailed":
                ShowError($"Ошибка регистрации: {message.Message}");
                break;
        }
    }

    private void HandleSuccessfulAuth(Player player, string successMessage)
    {
        lblStatus.Text = successMessage;
        lblStatus.ForeColor = Color.Green;

        // Показываем информацию о игроке
        string playerInfo = $"Добро пожаловать, {player.Username}!\n" +
                           $"Игр сыграно: {player.TotalGames}\n" +
                           $"Побед: {player.Wins}\n" +
                           $"Процент побед: {player.WinRate:F1}%\n" +
                           $"Общий счет: {player.Score}";

        MessageBox.Show(playerInfo, "Успешная аутентификация", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Открываем игровую форму
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
}
