using GameClient.Network;
using GameClient;
using System;
using System.Windows.Forms;
using GameClient.Models;

namespace Ubit_Slendermena_Client
{
    public partial class QuestionForm : Form
    {
        private GameClient.Network.GameClient? _client;

        public QuestionForm(string question, GameClient.Network.GameClient client)
        {
            InitializeComponent();
            labelQuestion.Text = question;
            _client = client;
            _client.MessageReceived += OnServerMessage;
        }
        private void OnServerMessage(object sender, ServerMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ServerMessage>(OnServerMessage), sender, message);
                return;
            }
            switch (message.Type)
            {
                case "AnswerCorrect":
                    MessageBox.Show("ответ правильный", "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _client.MessageReceived -= OnServerMessage;
                    this.Close();
                    break;

                case "AnswerFailed":
                    MessageBox.Show("ответ неправильный", "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _client.MessageReceived -= OnServerMessage;
                    this.Close();
                    break;
                default: MessageBox.Show(message.Type, "Получено11", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
            }
        }
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            string userAnswer = textBoxAnswer.Text.Trim().ToLower();
            _client.SendMessageAsync((new
            {
                Type = "Answer",

            }));


        }
    }
}