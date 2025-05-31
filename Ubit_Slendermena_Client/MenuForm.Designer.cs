namespace Ubit_Slendermena_Client
{
    partial class MenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PlayBtn = new Button();
            ProfileBtn = new Button();
            LocalizationBtn = new Button();
            ExitBtn = new Button();
            titleLbl = new Label();
            SuspendLayout();
            // 
            // PlayBtn
            // 
            PlayBtn.Location = new Point(197, 231);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(231, 56);
            PlayBtn.TabIndex = 0;
            PlayBtn.Text = "Играть";
            PlayBtn.UseVisualStyleBackColor = true;
            // 
            // ProfileBtn
            // 
            ProfileBtn.Location = new Point(197, 293);
            ProfileBtn.Name = "ProfileBtn";
            ProfileBtn.Size = new Size(231, 50);
            ProfileBtn.TabIndex = 1;
            ProfileBtn.Text = "Профиль";
            ProfileBtn.UseVisualStyleBackColor = true;
            ProfileBtn.Click += ProfileBtn_Click;
            // 
            // LocalizationBtn
            // 
            LocalizationBtn.Location = new Point(197, 349);
            LocalizationBtn.Name = "LocalizationBtn";
            LocalizationBtn.Size = new Size(231, 47);
            LocalizationBtn.TabIndex = 2;
            LocalizationBtn.Text = "Локализация";
            LocalizationBtn.UseVisualStyleBackColor = true;
            LocalizationBtn.Click += LocalizationBtn_Click;
            // 
            // ExitBtn
            // 
            ExitBtn.Location = new Point(197, 402);
            ExitBtn.Name = "ExitBtn";
            ExitBtn.Size = new Size(231, 53);
            ExitBtn.TabIndex = 3;
            ExitBtn.Text = "Выйти из игры";
            ExitBtn.UseVisualStyleBackColor = true;
            // 
            // titleLbl
            // 
            titleLbl.AutoSize = true;
            titleLbl.BackColor = Color.Transparent;
            titleLbl.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
            titleLbl.ForeColor = Color.Transparent;
            titleLbl.ImageAlign = ContentAlignment.TopRight;
            titleLbl.Location = new Point(180, 159);
            titleLbl.Name = "titleLbl";
            titleLbl.Size = new Size(260, 41);
            titleLbl.TabIndex = 4;
            titleLbl.Text = "СВОЯ СВОЯ ИГРА";
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resource.menufback;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(627, 501);
            Controls.Add(titleLbl);
            Controls.Add(ExitBtn);
            Controls.Add(LocalizationBtn);
            Controls.Add(ProfileBtn);
            Controls.Add(PlayBtn);
            Name = "MenuForm";
            Text = "   ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button PlayBtn;
        private Button ProfileBtn;
        private Button LocalizationBtn;
        private Button ExitBtn;
        private Label titleLbl;
    }
}