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