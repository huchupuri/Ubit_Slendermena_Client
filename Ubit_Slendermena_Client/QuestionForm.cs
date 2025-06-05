using GameClient.Network;
using GameClient.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class QuestionForm : Form
    {
        private readonly GameClient.Network.GameClient _client;
        private readonly Question _question;
        private readonly Player _currentPlayer;
        private System.Windows.Forms.Timer _questionTimer;
        private int _timeLeft = 60;
        private bool _canAnswer = true;

        public QuestionForm(Question question, GameClient.Network.GameClient client, Player currentPlayer)
        {
            InitializeComponent();
            _question = question;
            _client = client;
            _currentPlayer = currentPlayer;

            InitializeQuestionForm();
            SubscribeToEvents();
            StartTimer();
        }

        private void InitializeQuestionForm()
        {
            this.Text = $"Вопрос - {_question.CategoryName}";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Отображаем информацию о вопросе
            labelQuestion.Text = _question.Text;

            // Добавляем информацию о категории и стоимости
            var categoryLabel = new Label
            {
                Text = $"Категория: {_question.CategoryName}",
                Location = new Point(20, 20),
                Size = new Size(560, 25),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var priceLabel = new Label
            {
                Text = $"Стоимость: {_question.Price} очков",
                Location = new Point(20, 50),
                Size = new Size(560, 25),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Green
            };

            // Таймер
            var timerLabel = new Label
            {
                Name = "timerLabel",
                Text = $"⏰ Осталось: {_timeLeft} сек",
                Location = new Point(20, 80),
                Size = new Size(200, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Red
            };

            this.Controls.Add(categoryLabel);
            this.Controls.Add(priceLabel);
            this.Controls.Add(timerLabel);

            // Фокус на поле ввода
            textBoxAnswer.Focus();
        }

        private void SubscribeToEvents()
        {
            if (_client != null)
            {
                _client.MessageReceived += OnServerMessage;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_client != null)
            {
                _client.MessageReceived -= OnServerMessage;
            }
        }

        private void StartTimer()
        {
            _questionTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // 1 секунда
            };
            _questionTimer.Tick += QuestionTimer_Tick;
            _questionTimer.Start();
        }

        private void QuestionTimer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;

            var timerLabel = this.Controls.Find("timerLabel", false)[0] as Label;
            if (timerLabel != null)
            {
                timerLabel.Text = $"⏰ Осталось: {_timeLeft} сек";

                if (_timeLeft <= 10)
                {
                    timerLabel.ForeColor = Color.Red;
                }
                else if (_timeLeft <= 30)
                {
                    timerLabel.ForeColor = Color.Orange;
                }
            }

            if (_timeLeft <= 0)
            {
                _questionTimer.Stop();
                _canAnswer = false;
                textBoxAnswer.Enabled = false;
                buttonSubmit.Enabled = false;

                MessageBox.Show("Время вышло!", "Время истекло",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                case "AnswerResult":
                    HandleAnswerResult(message);
                    break;

                case "QuestionCompleted":
                    HandleQuestionCompleted();
                    break;

                case "QuestionTimeout":
                    HandleQuestionTimeout(message);
                    break;

                case "Error":
                    MessageBox.Show($"Ошибка: {message.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    Console.WriteLine($"QuestionForm получил сообщение: {message.Type}");
                    break;
            }
        }

        private void HandleAnswerResult(ServerMessage message)
        {
            if (message.Id == _currentPlayer.Id)
            {
                // Мой ответ
                _questionTimer?.Stop();
                _canAnswer = false;
                textBoxAnswer.Enabled = false;
                buttonSubmit.Enabled = false;

                if (message.IsCorrect)
                {
                    MessageBox.Show($"🎉 Правильный ответ!\n\n+{_question.Price} очков\nВаш счет: {message.NewScore}",
                        "Отлично!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"❌ Неправильный ответ!\n\nПравильный ответ: {message.CorrectAnswer}\n-{_question.Price} очков\nВаш счет: {message.NewScore}",
                        "Неправильно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // Ответ другого игрока
                string result = message.IsCorrect ? "правильно" : "неправильно";
                string resultMessage = $"Игрок {message.PlayerName} ответил {result}";

                if (!message.IsCorrect)
                {
                    resultMessage += $"\nПравильный ответ: {message.CorrectAnswer}";
                }

                MessageBox.Show(resultMessage, "Результат другого игрока",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void HandleQuestionCompleted()
        {
            _questionTimer?.Stop();
            MessageBox.Show("Вопрос завершен!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            CloseForm();
        }

        private void HandleQuestionTimeout(ServerMessage message)
        {
            _questionTimer?.Stop();
            _canAnswer = false;
            textBoxAnswer.Enabled = false;
            buttonSubmit.Enabled = false;

            MessageBox.Show($"⏰ Время вышло!\n\nПравильный ответ: {message.CorrectAnswer}",
                "Время истекло", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CloseForm();
        }

        private async void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (!_canAnswer || string.IsNullOrWhiteSpace(textBoxAnswer.Text))
            {
                MessageBox.Show("Введите ответ!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string userAnswer = textBoxAnswer.Text.Trim();
            textBoxAnswer.Enabled = false;
            buttonSubmit.Enabled = false;
            _canAnswer = false;

            try
            {
                await _client.SendMessageAsync(new
                {
                    Type = "Answer",
                    QuestionId = _question.Id,
                    Answer = userAnswer,
                    PlayerId = _currentPlayer.Id
                });

                buttonSubmit.Text = "Отправлено...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки ответа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxAnswer.Enabled = true;
                buttonSubmit.Enabled = true;
                buttonSubmit.Text = "ОТПРАВИТЬ";
                _canAnswer = true;
            }
        }

        private void textBoxAnswer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && _canAnswer)
            {
                e.Handled = true;
                buttonSubmit_Click(sender, e);
            }
        }

        private void CloseForm()
        {
            UnsubscribeFromEvents();
            _questionTimer?.Stop();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnsubscribeFromEvents();
            _questionTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}
