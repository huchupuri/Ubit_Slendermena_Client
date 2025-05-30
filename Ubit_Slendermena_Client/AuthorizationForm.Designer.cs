using Ubit_Slendermena_Client.Technical;
namespace Ubit_Slendermena_Client
{
    partial class AuthorizationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorizationForm));
            AuthorizationTxt = new TextBox();
            PassswordTxt = new TextBox();
            AuthorizationLabel = new Label();
            btnConnect = new Button();
            btnAddRoom = new RoundButton();
            SuspendLayout();
            // 
            // AuthorizationTxt
            // 
            resources.ApplyResources(AuthorizationTxt, "AuthorizationTxt");
            AuthorizationTxt.Name = "AuthorizationTxt";
            // 
            // PassswordTxt
            // 
            resources.ApplyResources(PassswordTxt, "PassswordTxt");
            PassswordTxt.Name = "PassswordTxt";
            // 
            // AuthorizationLabel
            // 
            resources.ApplyResources(AuthorizationLabel, "AuthorizationLabel");
            AuthorizationLabel.BackColor = Color.Transparent;
            AuthorizationLabel.Name = "AuthorizationLabel";
            // 
            // btnConnect
            // 
            resources.ApplyResources(btnConnect, "btnConnect");
            btnConnect.Name = "btnConnect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += BtnConnect_Click;
            // 
            // btnAddRoom
            // 
            resources.ApplyResources(btnAddRoom, "btnAddRoom");
            btnAddRoom.BackColor = Color.FromArgb(60, 60, 100);
            btnAddRoom.BorderColor = Color.Transparent;
            btnAddRoom.BorderRadius = 15;
            btnAddRoom.ForeColor = Color.FromArgb(243, 200, 220);
            btnAddRoom.HoverColor = Color.FromArgb(213, 140, 176);
            btnAddRoom.Name = "btnAddRoom";
            btnAddRoom.PressColor = Color.FromArgb(132, 49, 90);
            btnAddRoom.PressDepth = 0.15F;
            btnAddRoom.UseVisualStyleBackColor = false;
            btnAddRoom.Click += btnAddRoom_Click;
            // 
            // AuthorizationForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnConnect);
            Controls.Add(btnAddRoom);
            Controls.Add(AuthorizationLabel);
            Controls.Add(PassswordTxt);
            Controls.Add(AuthorizationTxt);
            ForeColor = SystemColors.ButtonHighlight;
            Name = "AuthorizationForm";
            Load += AuthorizationForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox AuthorizationTxt;
        private TextBox PassswordTxt;
        private Label AuthorizationLabel;
        private Button btnConnect;
        private RoundButton btnAddRoom;
    }
}