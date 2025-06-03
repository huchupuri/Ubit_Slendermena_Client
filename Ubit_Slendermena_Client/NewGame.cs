using GameClient.Models;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class NewGame : Form
    {
        private readonly Player _player;
        private readonly GameClient.Network.GameClient _networkClient;

        public NewGame(Player player, GameClient.Network.GameClient client)
        {
            InitializeComponent();
            _player = player;
            _networkClient = client;

            // Подписываемся на события ПОСЛЕ инициализации компонентов
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
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

        private void OpenGameForm()
        {
            // Отписываемся от событий перед передачей клиента
            //UnsubscribeFromEvents();
            var gameForm = new JeopardyGameForm(_player, _networkClient);
            this.Hide();
            gameForm.Show();
            //this.Close();
        }

        private void btnUploadQuestions_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Question Packages (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Файл загружен: " + dialog.FileName);
            }
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем соединение
                if (!_networkClient.IsConnected)
                {
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Валидация ввода
                if (!int.TryParse(txtPlayerCount.Text, out int playerCount) || playerCount < 1 || playerCount > 255)
                {
                    MessageBox.Show("Введите корректное количество игроков (1-255)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnCreate.Enabled = false;
                btnCreate.Text = "Создание...";
                await _networkClient.SendMessageAsync(new
                {
                    Type = "StartGame",
                    playerCount = playerCount
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании игры: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnCreate.Enabled = true;
                btnCreate.Text = "Создать игру";
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
                Console.WriteLine($"NewGame получил сообщение: {message.Type}");

                switch (message.Type)
                {
                    case "GameStarted":
                        MessageBox.Show("Игра началась!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        OpenGameForm();
                        break;

                    case "Error":
                        MessageBox.Show($"Ошибка: {message.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        btnCreate.Enabled = true;
                        btnCreate.Text = "Создать игру";
                        break;

                    case "PlayerJoined":
                        // Можно добавить логику обновления списка игроков
                        Console.WriteLine($"Игрок присоединился: {message.Username}");
                        break;

                    default:
                        Console.WriteLine($"Неизвестное сообщение: {message.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
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

            MessageBox.Show($"Соединение потеряно: {reason}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Возвращаемся к предыдущей форме или закрываем
            this.Close();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Exception>(OnErrorOccurred), sender, ex);
                return;
            }

            MessageBox.Show($"Ошибка соединения: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Отписываемся от событий при закрытии формы
            UnsubscribeFromEvents();
            base.OnFormClosing(e);
        }
    }
}
