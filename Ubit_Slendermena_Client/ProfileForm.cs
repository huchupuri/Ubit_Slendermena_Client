using GameClient.Models;
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
    public partial class ProfileForm : Form
    {
        private readonly Player _player;
        public ProfileForm(Player player)
        {
            _player = player;
            InitializeComponent();
        }
    }
}
