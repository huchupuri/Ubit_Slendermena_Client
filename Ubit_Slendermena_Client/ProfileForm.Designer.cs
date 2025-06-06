namespace Ubit_Slendermena_Client
{
    partial class ProfileForm
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
            Username = new Label();
            label2 = new Label();
            DescrLbl = new Label();
            ChangePasswordBtn = new Button();
            UploadImgBtn = new Button();
            BackBtn = new Button();
            ExitBtn = new Button();
            DeleteAccountBtn = new Button();
            SuspendLayout();
            // 
            // Username
            // 
            Username.AutoSize = true;
            Username.BackColor = Color.White;
            Username.Font = new Font("Segoe UI", 22.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Username.ForeColor = Color.FromArgb(255, 128, 0);
            Username.Location = new Point(12, 70);
            Username.Name = "Username";
            Username.Size = new Size(120, 50);
            Username.TabIndex = 0;
            Username.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.Font = new Font("Segoe UI", 22.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label2.ForeColor = Color.FromArgb(255, 128, 0);
            label2.Location = new Point(12, 139);
            label2.Name = "label2";
            label2.Size = new Size(120, 50);
            label2.TabIndex = 1;
            label2.Text = "label2";
            // 
            // DescrLbl
            // 
            DescrLbl.AutoSize = true;
            DescrLbl.BackColor = Color.Transparent;
            DescrLbl.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Regular, GraphicsUnit.Point, 204);
            DescrLbl.ForeColor = Color.FromArgb(255, 128, 0);
            DescrLbl.Location = new Point(12, 10);
            DescrLbl.Name = "DescrLbl";
            DescrLbl.Size = new Size(142, 60);
            DescrLbl.TabIndex = 2;
            DescrLbl.Text = "label1";
            // 
            // ChangePasswordBtn
            // 
            ChangePasswordBtn.Location = new Point(12, 264);
            ChangePasswordBtn.Name = "ChangePasswordBtn";
            ChangePasswordBtn.Size = new Size(171, 29);
            ChangePasswordBtn.TabIndex = 3;
            ChangePasswordBtn.Text = "сменить пароль";
            ChangePasswordBtn.UseVisualStyleBackColor = true;
            // 
            // UploadImgBtn
            // 
            UploadImgBtn.Location = new Point(227, 264);
            UploadImgBtn.Name = "UploadImgBtn";
            UploadImgBtn.Size = new Size(166, 29);
            UploadImgBtn.TabIndex = 4;
            UploadImgBtn.Text = "загрузить аватар";
            UploadImgBtn.UseVisualStyleBackColor = true;
            // 
            // BackBtn
            // 
            BackBtn.Location = new Point(12, 417);
            BackBtn.Name = "BackBtn";
            BackBtn.Size = new Size(94, 29);
            BackBtn.TabIndex = 5;
            BackBtn.Text = "назад";
            BackBtn.UseVisualStyleBackColor = true;
            // 
            // ExitBtn
            // 
            ExitBtn.Location = new Point(12, 351);
            ExitBtn.Name = "ExitBtn";
            ExitBtn.Size = new Size(171, 33);
            ExitBtn.TabIndex = 6;
            ExitBtn.Text = "выйти из аккаунта";
            ExitBtn.UseVisualStyleBackColor = true;
            // 
            // DeleteAccountBtn
            // 
            DeleteAccountBtn.Location = new Point(227, 353);
            DeleteAccountBtn.Name = "DeleteAccountBtn";
            DeleteAccountBtn.Size = new Size(166, 31);
            DeleteAccountBtn.TabIndex = 7;
            DeleteAccountBtn.Text = "удалить аккаунт";
            DeleteAccountBtn.UseVisualStyleBackColor = true;
            // 
            // ProfileForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(192, 0, 0);
            ClientSize = new Size(448, 458);
            Controls.Add(DeleteAccountBtn);
            Controls.Add(ExitBtn);
            Controls.Add(BackBtn);
            Controls.Add(UploadImgBtn);
            Controls.Add(ChangePasswordBtn);
            Controls.Add(DescrLbl);
            Controls.Add(label2);
            Controls.Add(Username);
            Name = "ProfileForm";
            Text = "ProfileForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Username;
        private Label label2;
        private Label DescrLbl;
        private Button ChangePasswordBtn;
        private Button UploadImgBtn;
        private Button BackBtn;
        private Button ExitBtn;
        private Button DeleteAccountBtn;
    }
}