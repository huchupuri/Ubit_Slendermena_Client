using Ubit_Slendermena_Client.Technical;

namespace Ubit_Slendermena_Client
{
    partial class EntryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntryForm));
            AuthorizationTxt = new TextBox();
            PassswordTxt = new TextBox();
            AuthorizationLabel = new Label();
            btnConnect = new RoundButton();
            btnAddRoom = new RoundButton();
            GameTitleLabel = new Label();
            SuspendLayout();
            // 
            // AuthorizationTxt
            // 
            AuthorizationTxt.Font = new Font("Segoe UI", 12F);
            AuthorizationTxt.Location = new Point(196, 205);
            AuthorizationTxt.Name = "AuthorizationTxt";
            AuthorizationTxt.PlaceholderText = "логин";
            AuthorizationTxt.Size = new Size(444, 34);
            AuthorizationTxt.TabIndex = 5;
            // 
            // PassswordTxt
            // 
            PassswordTxt.Font = new Font("Segoe UI", 12F);
            PassswordTxt.Location = new Point(196, 274);
            PassswordTxt.Name = "PassswordTxt";
            PassswordTxt.PlaceholderText = "пароль";
            PassswordTxt.Size = new Size(444, 34);
            PassswordTxt.TabIndex = 4;
            PassswordTxt.UseSystemPasswordChar = true;
            // 
            // AuthorizationLabel
            // 
            AuthorizationLabel.BackColor = Color.Transparent;
            AuthorizationLabel.Font = new Font("Arial", 18F, FontStyle.Bold);
            AuthorizationLabel.ForeColor = Color.Orange;
            AuthorizationLabel.Location = new Point(307, 122);
            AuthorizationLabel.Name = "AuthorizationLabel";
            AuthorizationLabel.Size = new Size(210, 48);
            AuthorizationLabel.TabIndex = 3;
            AuthorizationLabel.Text = "Авторизация";
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(255, 230, 0);
            btnConnect.BorderColor = Color.Transparent;
            btnConnect.BorderRadius = 15;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.HoverColor = Color.FromArgb(213, 140, 176);
            btnConnect.Location = new Point(440, 338);
            btnConnect.MinimumSize = new Size(100, 46);
            btnConnect.Name = "btnConnect";
            btnConnect.PressColor = Color.FromArgb(132, 49, 90);
            btnConnect.PressDepth = 0.15F;
            btnConnect.Size = new Size(200, 46);
            btnConnect.TabIndex = 1;
            btnConnect.Text = "ВОЙТИ";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnAddRoom
            // 
            btnAddRoom.BackColor = Color.FromArgb(255, 230, 0);
            btnAddRoom.BorderColor = Color.Transparent;
            btnAddRoom.BorderRadius = 15;
            btnAddRoom.FlatStyle = FlatStyle.Flat;
            btnAddRoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddRoom.ForeColor = Color.White;
            btnAddRoom.HoverColor = Color.FromArgb(213, 140, 176);
            btnAddRoom.Location = new Point(196, 338);
            btnAddRoom.MinimumSize = new Size(100, 46);
            btnAddRoom.Name = "btnAddRoom";
            btnAddRoom.PressColor = Color.FromArgb(132, 49, 90);
            btnAddRoom.PressDepth = 0.15F;
            btnAddRoom.Size = new Size(200, 46);
            btnAddRoom.TabIndex = 2;
            btnAddRoom.Text = "ЗАРЕГИСТРИРОВАТЬСЯ";
            btnAddRoom.UseVisualStyleBackColor = false;
            btnAddRoom.Click += btnAddRoom_Click;
            // 
            // GameTitleLabel
            // 
            GameTitleLabel.AutoSize = true;
            GameTitleLabel.BackColor = Color.Transparent;
            GameTitleLabel.Font = new Font("Arial", 28F, FontStyle.Bold);
            GameTitleLabel.ForeColor = Color.Red;
            GameTitleLabel.Location = new Point(196, 37);
            GameTitleLabel.Name = "GameTitleLabel";
            GameTitleLabel.Size = new Size(444, 55);
            GameTitleLabel.TabIndex = 0;
            GameTitleLabel.Text = "СВОЯ СВОЯ ИГРА";
            // 
            // EntryForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(788, 487);
            Controls.Add(GameTitleLabel);
            Controls.Add(btnConnect);
            Controls.Add(btnAddRoom);
            Controls.Add(AuthorizationLabel);
            Controls.Add(PassswordTxt);
            Controls.Add(AuthorizationTxt);
            ForeColor = SystemColors.ButtonHighlight;
            Name = "EntryForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox AuthorizationTxt;
        private TextBox PassswordTxt;
        private Label AuthorizationLabel;
        private RoundButton btnConnect;
        private RoundButton btnAddRoom;
        private Label GameTitleLabel;

    }
}
