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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            PlayBtn = new Button();
            ProfileBtn = new Button();
            LocalizationBtn = new Button();
            ExitBtn = new Button();
            titleLbl = new Label();
            SuspendLayout();
            // 
            // PlayBtn
            // 
            PlayBtn.BackColor = Color.Yellow;
            resources.ApplyResources(PlayBtn, "PlayBtn");
            PlayBtn.Name = "PlayBtn";
            PlayBtn.UseVisualStyleBackColor = false;
            PlayBtn.Click += PlayBtn_Click;
            // 
            // ProfileBtn
            // 
            ProfileBtn.BackColor = Color.Yellow;
            resources.ApplyResources(ProfileBtn, "ProfileBtn");
            ProfileBtn.Name = "ProfileBtn";
            ProfileBtn.UseVisualStyleBackColor = false;
            ProfileBtn.Click += ProfileBtn_Click;
            // 
            // LocalizationBtn
            // 
            LocalizationBtn.BackColor = Color.Yellow;
            resources.ApplyResources(LocalizationBtn, "LocalizationBtn");
            LocalizationBtn.Name = "LocalizationBtn";
            LocalizationBtn.UseVisualStyleBackColor = false;
            LocalizationBtn.Click += LocalizationBtn_Click;
            // 
            // ExitBtn
            // 
            ExitBtn.BackColor = Color.Yellow;
            resources.ApplyResources(ExitBtn, "ExitBtn");
            ExitBtn.Name = "ExitBtn";
            ExitBtn.UseVisualStyleBackColor = false;
            // 
            // titleLbl
            // 
            resources.ApplyResources(titleLbl, "titleLbl");
            titleLbl.BackColor = Color.Transparent;
            titleLbl.ForeColor = Color.Red;
            titleLbl.Name = "titleLbl";
            // 
            // MenuForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resource.menufback;
            Controls.Add(titleLbl);
            Controls.Add(ExitBtn);
            Controls.Add(LocalizationBtn);
            Controls.Add(ProfileBtn);
            Controls.Add(PlayBtn);
            Name = "MenuForm";
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