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
            this.labelQuestion = new Label();
            this.textBoxAnswer = new TextBox();
            this.buttonSubmit = new Button();

            // 
            // labelQuestion
            // 
            this.labelQuestion.AutoSize = true;
            this.labelQuestion.Location = new System.Drawing.Point(20, 20);
            this.labelQuestion.MaximumSize = new System.Drawing.Size(360, 0);
            this.labelQuestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelQuestion.TabIndex = 0;
            this.labelQuestion.Text = "Здесь будет вопрос";

            // 
            // textBoxAnswer
            // 
            this.textBoxAnswer.Location = new System.Drawing.Point(20, 70);
            this.textBoxAnswer.Name = "textBoxAnswer";
            this.textBoxAnswer.Size = new System.Drawing.Size(360, 20);
            this.textBoxAnswer.TabIndex = 1;

            // 
            // buttonSubmit
            // 
            this.buttonSubmit.Location = new System.Drawing.Point(20, 110);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(100, 30);
            this.buttonSubmit.TabIndex = 2;
            this.buttonSubmit.Text = "Отправить";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            this.buttonSubmit.Click += new System.EventHandler(this.buttonSubmit_Click);

            // 
            // QuestionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 160);
            this.Controls.Add(this.labelQuestion);
            this.Controls.Add(this.textBoxAnswer);
            this.Controls.Add(this.buttonSubmit);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Вопрос";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}