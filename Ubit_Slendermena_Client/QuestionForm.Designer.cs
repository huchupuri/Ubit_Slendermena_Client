namespace Ubit_Slendermena_Client
{
    partial class QuestionForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label labelQuestion;
        private TextBox textBoxAnswer;
        private Button buttonSubmit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
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
            textBoxAnswer.Focus();
        }
        private void InitializeComponent()
        {
            labelQuestion = new Label();
            textBoxAnswer = new TextBox();
            buttonSubmit = new Button();
            SuspendLayout();
            // 
            // labelQuestion
            // 
            labelQuestion.AutoSize = true;
            labelQuestion.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelQuestion.ForeColor = Color.Orange;
            labelQuestion.Location = new Point(20, 20);
            labelQuestion.MaximumSize = new Size(360, 0);
            labelQuestion.Name = "labelQuestion";
            labelQuestion.Size = new Size(203, 28);
            labelQuestion.TabIndex = 0;
            labelQuestion.Text = "Здесь будет вопрос";
            // 
            // textBoxAnswer
            // 
            textBoxAnswer.Font = new Font("Segoe UI", 10F);
            textBoxAnswer.Location = new Point(104, 136);
            textBoxAnswer.Name = "textBoxAnswer";
            textBoxAnswer.Size = new Size(360, 30);
            textBoxAnswer.TabIndex = 1;
            // 
            // buttonSubmit
            // 
            buttonSubmit.BackColor = Color.Yellow;
            buttonSubmit.FlatStyle = FlatStyle.Flat;
            buttonSubmit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            buttonSubmit.Location = new Point(220, 229);
            buttonSubmit.Name = "buttonSubmit";
            buttonSubmit.Size = new Size(130, 35);
            buttonSubmit.TabIndex = 2;
            buttonSubmit.Text = "ОТПРАВИТЬ";
            buttonSubmit.UseVisualStyleBackColor = false;
            buttonSubmit.Click += buttonSubmit_Click;
            // 
            // QuestionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(138, 57, 57);
            ClientSize = new Size(540, 276);
            Controls.Add(labelQuestion);
            Controls.Add(textBoxAnswer);
            Controls.Add(buttonSubmit);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuestionForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Вопрос";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}