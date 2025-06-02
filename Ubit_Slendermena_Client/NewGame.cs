using GameClient.Models;
using GameClient.Network;
using GameClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ubit_Slendermena_Client
{
    public partial class NewGame : Form
    {
        private readonly Player Player;
        private readonly GameNetworkClient _networkClient = Program.form._client;
        public NewGame(Player player)
        {
            InitializeComponent();
            _networkClient.MessageReceived += OnServerMessage;
            Player = player;
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

        private  async void btnCreate_Click(object sender, EventArgs e)
        {
            await _networkClient.SendMessageAsync(new
            {
                Type = "StartGame",
                playerCount = int.Parse(txtPlayerCount.Text)
            });

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
                case "GameStarted":
                    _networkClient.MessageReceived -= OnServerMessage;
                    var gameForm = new JeopardyGameForm(Player);
                    gameForm.ShowDialog();
                    this.Close();
                    break;
                default: MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
            }
        }
    }
}
