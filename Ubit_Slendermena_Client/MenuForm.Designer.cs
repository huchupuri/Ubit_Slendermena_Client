namespace Ubit_Slendermena_Client
{
    partial class MenuForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            PlayBtn = new Button();
            ProfileBtn = new Button();
            LocalizationBtn = new Button();
            ExitBtn = new Button();
            titleLbl = new Label();
            LanguageComboBox = new ComboBox();
            SuspendLayout();
            // 
            // PlayBtn
            // 
            resources.ApplyResources(PlayBtn, "PlayBtn");
            PlayBtn.BackColor = Color.Yellow;
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Text = Properties.MenuForm.Resources.PlayButton;
            PlayBtn.UseVisualStyleBackColor = false;
            PlayBtn.Click += PlayBtn_Click;
            // 
            // ProfileBtn
            // 
            resources.ApplyResources(ProfileBtn, "ProfileBtn");
            ProfileBtn.BackColor = Color.Yellow;
            ProfileBtn.Name = "ProfileBtn";
            ProfileBtn.Text = Properties.MenuForm.Resources.ProfileButton;
            ProfileBtn.UseVisualStyleBackColor = false;
            ProfileBtn.Click += ProfileBtn_Click;
            // 
            // LocalizationBtn
            // 
            resources.ApplyResources(LocalizationBtn, "LocalizationBtn");
            LocalizationBtn.BackColor = Color.Yellow;
            LocalizationBtn.Name = "LocalizationBtn";
            LocalizationBtn.Text = Properties.MenuForm.Resources.LocalizationButton;
            LocalizationBtn.UseVisualStyleBackColor = false;
            LocalizationBtn.Click += LocalizationBtn_Click;
            // 
            // ExitBtn
            // 
            resources.ApplyResources(ExitBtn, "ExitBtn");
            ExitBtn.BackColor = Color.Yellow;
            ExitBtn.Name = "ExitBtn";
            ExitBtn.Text = Properties.MenuForm.Resources.ExitButton;
            ExitBtn.UseVisualStyleBackColor = false;
            // 
            // titleLbl
            // 
            resources.ApplyResources(titleLbl, "titleLbl");
            titleLbl.BackColor = Color.Transparent;
            titleLbl.ForeColor = Color.Red;
            titleLbl.Name = "titleLbl";
            // 
            // LanguageComboBox
            // 
            resources.ApplyResources(LanguageComboBox, "LanguageComboBox");
            LanguageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            LanguageComboBox.FormattingEnabled = true;
            LanguageComboBox.Name = "LanguageComboBox";
            // 
            // MenuForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resource.menufback;
            Controls.Add(LanguageComboBox);
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
        private ComboBox LanguageComboBox;
    }
}
