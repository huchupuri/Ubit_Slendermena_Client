using GameClient.Network;
using GameClient;
using System;
using System.Windows.Forms;
using GameClient.Models;

namespace Ubit_Slendermena_Client
{
    public partial class QuestionForm : Form
    {
        private readonly GameNetworkClient _networkClient = Program.form._client;

        public QuestionForm(string question)
        {
            InitializeComponent();
            labelQuestion.Text = question;
            _networkClient.MessageReceived += OnServerMessage;
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
                case "AnswerCorrect":
                    MessageBox.Show("ответ правильный", "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _networkClient.MessageReceived -= OnServerMessage;
                    this.Close();
                    break;

                case "AnswerFailed":
                    MessageBox.Show("ответ неправильный", "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _networkClient.MessageReceived -= OnServerMessage;
                    this.Close();
                    break;
                default: MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
            }
        }
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            string userAnswer = textBoxAnswer.Text.Trim().ToLower();
            _networkClient.SendMessageAsync((new
            {
                Type = "Answer",

            }));

            
        }
    }
}