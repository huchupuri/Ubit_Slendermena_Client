namespace Ubit_Slendermena_Client
{
    partial class NewGame: Form
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Button btnUploadQuestions;
        private TextBox txtPlayerCount;
        private Label lblPlayerCount;
        private Button btnCreate;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            btnUploadQuestions = new Button();
            txtPlayerCount = new TextBox();
            lblPlayerCount = new Label();
            btnCreate = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Arial", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Orange;
            lblTitle.Location = new Point(70, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(276, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Создание игры";
            // 
            // btnUploadQuestions
            // 
            btnUploadQuestions.BackColor = Color.Yellow;
            btnUploadQuestions.FlatStyle = FlatStyle.Flat;
            btnUploadQuestions.Location = new Point(70, 100);
            btnUploadQuestions.Name = "btnUploadQuestions";
            btnUploadQuestions.Size = new Size(250, 40);
            btnUploadQuestions.TabIndex = 1;
            btnUploadQuestions.Text = "ЗАГРУЗИТЬ ПАКЕТ ВОПРОСОВ";
            btnUploadQuestions.UseVisualStyleBackColor = false;
            btnUploadQuestions.Click += btnUploadQuestions_Click;
            // 
            // txtPlayerCount
            // 
            txtPlayerCount.Location = new Point(70, 200);
            txtPlayerCount.Name = "txtPlayerCount";
            txtPlayerCount.Size = new Size(250, 27);
            txtPlayerCount.TabIndex = 3;
            // 
            // lblPlayerCount
            // 
            lblPlayerCount.AutoSize = true;
            lblPlayerCount.ForeColor = Color.White;
            lblPlayerCount.Location = new Point(70, 170);
            lblPlayerCount.Name = "lblPlayerCount";
            lblPlayerCount.Size = new Size(151, 20);
            lblPlayerCount.TabIndex = 2;
            lblPlayerCount.Text = "Количество игроков";
            // 
            // btnCreate
            // 
            btnCreate.BackColor = Color.Yellow;
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Location = new Point(70, 270);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(250, 40);
            btnCreate.TabIndex = 4;
            btnCreate.Text = "СОЗДАТЬ";
            btnCreate.UseVisualStyleBackColor = false;
            btnCreate.Click += btnCreate_Click;
            // 
            // NewGame
            // 
            BackColor = Color.Black;
            BackgroundImage = Properties.Resource.bigbrain;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(400, 600);
            Controls.Add(lblTitle);
            Controls.Add(btnUploadQuestions);
            Controls.Add(lblPlayerCount);
            Controls.Add(txtPlayerCount);
            Controls.Add(btnCreate);
            Name = "NewGame";
            Text = "Создание игры";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
