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
            HostBtn = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Arial", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Orange;
            lblTitle.Location = new Point(70, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(228, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Create Game";
            // 
            // btnUploadQuestions
            // 
            btnUploadQuestions.BackColor = Color.Yellow;
            btnUploadQuestions.FlatStyle = FlatStyle.Flat;
            btnUploadQuestions.Location = new Point(70, 100);
            btnUploadQuestions.Name = "btnUploadQuestions";
            btnUploadQuestions.Size = new Size(250, 40);
            btnUploadQuestions.TabIndex = 1;
            btnUploadQuestions.Text = Properties.NewGame.Resources.btnUploadQuestions_Text;
            btnUploadQuestions.UseVisualStyleBackColor = false;
            btnUploadQuestions.Click += btnUploadQuestions_Click;
            // 
            // txtPlayerCount
            // 
            txtPlayerCount.Location = new Point(70, 215);
            txtPlayerCount.Name = "txtPlayerCount";
            txtPlayerCount.Size = new Size(250, 27);
            txtPlayerCount.TabIndex = 6;
            // 
            // lblPlayerCount
            // 
            lblPlayerCount.AutoSize = true;
            lblPlayerCount.ForeColor = Color.White;
            lblPlayerCount.Location = new Point(70, 170);
            lblPlayerCount.Name = "lblPlayerCount";
            lblPlayerCount.Size = new Size(132, 20);
            lblPlayerCount.TabIndex = 2;
            lblPlayerCount.Text = "Number of players";
            // 
            // btnCreate
            // 
            btnCreate.BackColor = Color.Yellow;
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Location = new Point(70, 270);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(250, 40);
            btnCreate.TabIndex = 4;
            btnCreate.Text = Properties.NewGame.Resources.btnCreate;
            btnCreate.UseVisualStyleBackColor = false;
            btnCreate.Click += btnCreate_Click;
            // 
            // HostBtn
            // 
            HostBtn.BackColor = Color.Yellow;
            HostBtn.FlatStyle = FlatStyle.Flat;
            HostBtn.Location = new Point(70, 349);
            HostBtn.Name = "HostBtn";
            HostBtn.Size = new Size(250, 40);
            HostBtn.TabIndex = 5;
            HostBtn.Text = Properties.NewGame.Resources.HostBtn;
            HostBtn.UseVisualStyleBackColor = false;
            HostBtn.Click += HostBtn_Click;
            // 
            // NewGame
            // 
            BackColor = Color.Black;
            BackgroundImage = Properties.Resource.bigbrain;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(400, 600);
            Controls.Add(HostBtn);
            Controls.Add(lblTitle);
            Controls.Add(btnUploadQuestions);
            Controls.Add(lblPlayerCount);
            Controls.Add(txtPlayerCount);
            Controls.Add(btnCreate);
            Name = "NewGame";
            Text = "Create Game";
            ResumeLayout(false);
            PerformLayout();
        }

        private Button HostBtn;
    }
}
