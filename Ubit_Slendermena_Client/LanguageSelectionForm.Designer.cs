namespace Ubit_Slendermena_Client
{
    partial class LanguageSelectionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            englishBtn = new Button();
            russianBtn = new Button();

            SuspendLayout();

            // 
            // englishBtn
            // 
            englishBtn.Location = new Point(50, 30);
            englishBtn.Name = "englishBtn";
            englishBtn.Size = new Size(120, 40);
            englishBtn.TabIndex = 0;
            englishBtn.Text = "English";
            englishBtn.UseVisualStyleBackColor = true;
            englishBtn.Click += englishBtn_Click;

            // 
            // russianBtn
            // 
            russianBtn.Location = new Point(50, 80);
            russianBtn.Name = "russianBtn";
            russianBtn.Size = new Size(120, 40);
            russianBtn.TabIndex = 1;
            russianBtn.Text = "Русский";
            russianBtn.UseVisualStyleBackColor = true;
            russianBtn.Click += russianBtn_Click;

            // 
            // LanguageSelectionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(220, 160);
            Controls.Add(russianBtn);
            Controls.Add(englishBtn);
            Name = "LanguageSelectionForm";
            Text = "Select Language";
            ResumeLayout(false);
        }

        private Button englishBtn;
        private Button russianBtn;
    }
}
