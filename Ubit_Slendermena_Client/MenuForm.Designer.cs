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
            label1 = new Label();
            SuspendLayout();
            // 
            // PlayBtn
            // 
            PlayBtn.Location = new Point(308, 156);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(231, 56);
            PlayBtn.TabIndex = 0;
            PlayBtn.Text = "Играть";
            PlayBtn.UseVisualStyleBackColor = true;
            // 
            // ProfileBtn
            // 
            ProfileBtn.Location = new Point(308, 218);
            ProfileBtn.Name = "ProfileBtn";
            ProfileBtn.Size = new Size(231, 50);
            ProfileBtn.TabIndex = 1;
            ProfileBtn.Text = "Профиль";
            ProfileBtn.UseVisualStyleBackColor = true;
            ProfileBtn.Click += ProfileBtn_Click;
            // 
            // LocalizationBtn
            // 
            LocalizationBtn.Location = new Point(308, 274);
            LocalizationBtn.Name = "LocalizationBtn";
            LocalizationBtn.Size = new Size(231, 47);
            LocalizationBtn.TabIndex = 2;
            LocalizationBtn.Text = "Локализация";
            LocalizationBtn.UseVisualStyleBackColor = true;
            // 
            // ExitBtn
            // 
            ExitBtn.Location = new Point(308, 327);
            ExitBtn.Name = "ExitBtn";
            ExitBtn.Size = new Size(231, 53);
            ExitBtn.TabIndex = 3;
            ExitBtn.Text = "Выйти из игры";
            ExitBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(387, 51);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 4;
            label1.Text = "label1";
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
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
        private Label label1;
    }
}