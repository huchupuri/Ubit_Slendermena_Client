using Ubit_Slendermena_Client.Technical;
using GameClient.Network;
using System.Globalization;
using System.Threading;

namespace Ubit_Slendermena_Client
{
    partial class EntryForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            AuthorizationTxt = new TextBox();
            PassswordTxt = new TextBox();
            AuthorizationLabel = new Label();
            btnConnect = new RoundButton();
            btnAddRoom = new RoundButton();
            GameTitleLabel = new Label();
            LanguageComboBox = new ComboBox();

            SuspendLayout();

            // 
            // AuthorizationTxt
            // 
            AuthorizationTxt.Font = new Font("Segoe UI", 12F);
            AuthorizationTxt.Location = new Point(196, 205);
            AuthorizationTxt.Name = "AuthorizationTxt";
            AuthorizationTxt.PlaceholderText = Properties.Resources.LoginPlaceholder;
            AuthorizationTxt.Size = new Size(444, 34);
            AuthorizationTxt.TabIndex = 5;

            // 
            // PassswordTxt
            // 
            PassswordTxt.Font = new Font("Segoe UI", 12F);
            PassswordTxt.Location = new Point(196, 274);
            PassswordTxt.Name = "PassswordTxt";
            PassswordTxt.PlaceholderText = Properties.Resources.PasswordPlaceholder;
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
            AuthorizationLabel.Text = Properties.Resources.LoginLabel;

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
            btnConnect.Text = Properties.Resources.LoginButton;
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
            btnAddRoom.Text = Properties.Resources.RegisterButton;
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
            GameTitleLabel.Text = Properties.Resources.GameTitle;

            // 
            // LanguageComboBox
            // 
            LanguageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            LanguageComboBox.Font = new Font("Segoe UI", 10F);
            LanguageComboBox.Items.AddRange(new object[] { "English", "Русский" });
            LanguageComboBox.SelectedIndex = Thread.CurrentThread.CurrentUICulture.Name.StartsWith("ru") ? 1 : 0;
            LanguageComboBox.Location = new Point(650, 12);
            LanguageComboBox.Name = "LanguageComboBox";
            LanguageComboBox.Size = new Size(120, 30);
            LanguageComboBox.TabIndex = 6;
            LanguageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            // 
            // EntryForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resource.AuthorizationBackground;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(788, 487);
            Controls.Add(LanguageComboBox);
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
        private ComboBox LanguageComboBox;
    }
}
