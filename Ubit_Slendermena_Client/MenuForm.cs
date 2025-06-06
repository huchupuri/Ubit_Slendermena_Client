using GameClient;
using GameClient.Models;
using GameClient.Network;
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
    public partial class MenuForm : Form
    {
        private readonly Player Player;
        private readonly GameNetworkClient _networkClient = Program.form._client;
        public MenuForm(Player player)
        {
            InitializeComponent();
            Player = player;
        }
        private void ProfileBtn_Click(object sender, EventArgs e)
        {

        }

        private void LocalizationBtn_Click(object sender, EventArgs e)
        {

        }

        private async void PlayBtn_Click(object sender, EventArgs e)
        {
            var newGame = new NewGame(Player);
            newGame.ShowDialog();
            this.Close();
        }
    }
}
