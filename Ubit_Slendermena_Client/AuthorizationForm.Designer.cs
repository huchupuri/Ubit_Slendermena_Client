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
            textBox2 = new TextBox();
            AuthorizationLabel = new Label();
            button1 = new Button();
            btnAddRoom = new RoundButton();
            SuspendLayout();
            // 
            // AuthorizationTxt
            // 
            resources.ApplyResources(AuthorizationTxt, "AuthorizationTxt");
            AuthorizationTxt.Name = "AuthorizationTxt";
            // 
            // textBox2
            // 
            resources.ApplyResources(textBox2, "textBox2");
            textBox2.Name = "textBox2";
            // 
            // AuthorizationLabel
            // 
            resources.ApplyResources(AuthorizationLabel, "AuthorizationLabel");
            AuthorizationLabel.BackColor = Color.Transparent;
            AuthorizationLabel.Name = "AuthorizationLabel";
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnAddRoom
            // 
            btnAddRoom.BackColor = Color.FromArgb(60, 60, 100);
            btnAddRoom.BorderColor = Color.Transparent;
            btnAddRoom.BorderRadius = 15;
            btnAddRoom.FlatStyle = FlatStyle.Flat;
            btnAddRoom.Font = new Font("Microsoft Sans Serif", 12F);
            btnAddRoom.ForeColor = Color.FromArgb(243, 200, 220);
            btnAddRoom.HoverColor = Color.FromArgb(213, 140, 176);
            btnAddRoom.Location = new Point(12, 256);
            btnAddRoom.Margin = new Padding(4, 3, 4, 3);
            btnAddRoom.MinimumSize = new Size(100, 46);
            btnAddRoom.Name = "btnAddRoom";
            btnAddRoom.PressColor = Color.FromArgb(132, 49, 90);
            btnAddRoom.PressDepth = 0.15F;
            btnAddRoom.Size = new Size(117, 46);
            btnAddRoom.TabIndex = 7;
            btnAddRoom.Text = "добавить\nкомнату";
            btnAddRoom.UseVisualStyleBackColor = false;
            // 
            // AuthorizationForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button1);
            Controls.Add(btnAddRoom);
            Controls.Add(AuthorizationLabel);
            Controls.Add(textBox2);
            Controls.Add(AuthorizationTxt);
            ForeColor = SystemColors.ButtonHighlight;
            Name = "AuthorizationForm";
            Load += AuthorizationForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox AuthorizationTxt;
        private TextBox textBox2;
        private Label AuthorizationLabel;
        private Button button1;
        private RoundButton btnAddRoom;
    }
}