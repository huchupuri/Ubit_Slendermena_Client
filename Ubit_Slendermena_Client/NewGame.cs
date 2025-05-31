using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class NewGame : Form
    {
        public NewGame()
        {
            InitializeComponent();
        }

        private void btnUploadQuestions_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Question Packages (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Здесь можно обработать выбранный файл
                MessageBox.Show("Файл загружен: " + dialog.FileName);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPlayerCount.Text, out int playerCount) && playerCount > 0)
            {
                MessageBox.Show($"Создание игры на {playerCount} игроков.");
                // Логика создания игры
            }
            else
            {
                MessageBox.Show("Введите корректное количество игроков.");
            }
        }
    }
}
