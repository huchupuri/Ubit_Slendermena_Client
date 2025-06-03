using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int TotalGames { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int CurrentScore { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public double WinRate => TotalGames > 0 ? (double)Wins / TotalGames * 100 : 0;
    }
}
