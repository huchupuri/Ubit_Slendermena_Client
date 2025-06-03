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


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DescrLbl = new Label();
            UsernameTxt = new TextBox();
            PasswordTxt = new TextBox();
            ChangePasswordBtn = new Button();
            UploadImgBtn = new Button();
            ExitBtn = new Button();
            BackBtn = new Button();
            DeleteAccountBtn = new Button();
            AvatarPic = new PictureBox();
            LoginLbl = new Label();
            PasswordLbl = new Label();

            SuspendLayout();

            // DescrLbl
            DescrLbl.AutoSize = true;
            DescrLbl.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            DescrLbl.ForeColor = Color.Orange;
            DescrLbl.Location = new Point(20, 20);
            DescrLbl.Text = "Профиль";

            // Login label
            LoginLbl.AutoSize = true;
            LoginLbl.ForeColor = Color.Orange;
            LoginLbl.Location = new Point(25, 80);
            LoginLbl.Text = "логин";

            // UsernameTxt
            UsernameTxt.Location = new Point(25, 100);
            UsernameTxt.Size = new Size(160, 27);

            // Password label
            PasswordLbl.AutoSize = true;
            PasswordLbl.ForeColor = Color.Orange;
            PasswordLbl.Location = new Point(25, 140);
            PasswordLbl.Text = "пароль";

            // PasswordTxt
            PasswordTxt.Location = new Point(25, 160);
            PasswordTxt.Size = new Size(160, 27);
            PasswordTxt.PasswordChar = '*';

            // AvatarPic
            AvatarPic.Location = new Point(250, 80);
            AvatarPic.Size = new Size(128, 128);
            AvatarPic.SizeMode = PictureBoxSizeMode.StretchImage;

            // ChangePasswordBtn
            ChangePasswordBtn.Text = "СМЕНИТЬ ПАРОЛЬ";
            ChangePasswordBtn.BackColor = Color.Yellow;
            ChangePasswordBtn.FlatStyle = FlatStyle.Flat;
            ChangePasswordBtn.Location = new Point(25, 210);
            ChangePasswordBtn.Size = new Size(160, 40);

            // UploadImgBtn
            UploadImgBtn.Text = "ЗАГРУЗИТЬ АВАТАР";
            UploadImgBtn.BackColor = Color.Yellow;
            UploadImgBtn.FlatStyle = FlatStyle.Flat;
            UploadImgBtn.Location = new Point(250, 210);
            UploadImgBtn.Size = new Size(160, 40);

            // ExitBtn
            ExitBtn.Text = "ВЫЙТИ ИЗ АККАУНТА";
            ExitBtn.BackColor = Color.Yellow;
            ExitBtn.FlatStyle = FlatStyle.Flat;
            ExitBtn.Location = new Point(25, 270);
            ExitBtn.Size = new Size(160, 40);

            // DeleteAccountBtn
            DeleteAccountBtn.Text = "УДАЛИТЬ АККАУНТ";
            DeleteAccountBtn.BackColor = Color.Yellow;
            DeleteAccountBtn.FlatStyle = FlatStyle.Flat;
            DeleteAccountBtn.Location = new Point(250, 270);
            DeleteAccountBtn.Size = new Size(160, 40);

            // BackBtn
            BackBtn.Text = "НАЗАД";
            BackBtn.BackColor = Color.Yellow;
            BackBtn.FlatStyle = FlatStyle.Flat;
            BackBtn.Location = new Point(160, 330);
            BackBtn.Size = new Size(120, 40);

            // ProfileForm
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(138, 57, 57); // бордовый
            ClientSize = new Size(450, 400);
            Controls.Add(DescrLbl);
            Controls.Add(LoginLbl);
            Controls.Add(UsernameTxt);
            Controls.Add(PasswordLbl);
            Controls.Add(PasswordTxt);
            Controls.Add(AvatarPic);
            Controls.Add(ChangePasswordBtn);
            Controls.Add(UploadImgBtn);
            Controls.Add(ExitBtn);
            Controls.Add(DeleteAccountBtn);
            Controls.Add(BackBtn);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Профиль";
            ResumeLayout(false);
            PerformLayout();
        }
        private Label DescrLbl;
        private Label LoginLbl;
        private Label PasswordLbl;
        private TextBox UsernameTxt;
        private TextBox PasswordTxt;
        private Button ChangePasswordBtn;
        private Button UploadImgBtn;
        private Button ExitBtn;
        private Button DeleteAccountBtn;
        private Button BackBtn;
        private PictureBox AvatarPic;


    }
}