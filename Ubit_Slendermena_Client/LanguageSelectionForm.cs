using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class LanguageSelectionForm : Form
    {
        public string SelectedLanguageCode { get; private set; } = "en"; // default

        public LanguageSelectionForm()
        {
            InitializeComponent();
        }

        private void englishBtn_Click(object sender, EventArgs e)
        {
            SelectedLanguageCode = "en";
            DialogResult = DialogResult.OK;
            Close();
        }

        private void russianBtn_Click(object sender, EventArgs e)
        {
            SelectedLanguageCode = "ru";
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
