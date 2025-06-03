using GameClient.Models;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class NewGame : Form
    {
        private readonly Player Player;
        private readonly GameClient.Network.GameClient _networkClient;
        public NewGame(Player player, GameClient.Network.GameClient client)
        {
            InitializeComponent();
            _networkClient = client;
            _networkClient.MessageReceived += OnServerMessage;
            Player = player;
        }
        private void OpenGameForm()
        {
            
            var gameForm = new JeopardyGameForm(Player, _networkClient);
            gameForm.ShowDialog();
            this.Close();

        }
        private void btnUploadQuestions_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Question Packages (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Здесь можно обработать выбранный файл
                MessageBox.Show("Файл загружен: " + dialog.FileName);
            }
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            await _networkClient.SendMessageAsync(new
            {
                Type = "StartGame",
                playerCount = int.Parse(txtPlayerCount.Text)
            });
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
                MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                switch (message.Type)
                {
                    case "GameStarted":
                        MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        OpenGameForm();
                        break;
                    default: MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки сообщения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
