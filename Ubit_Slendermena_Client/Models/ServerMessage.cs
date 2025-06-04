using GameClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameClient.Models
{
    public class ServerMessage
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; } = Guid.Empty;
        public string Username { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new();
        public Player Player { get; set; }
        public List<Player> Players { get; set; } = new();
        public Question? Question { get; set; }
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public int NewScore { get; set; }
        public Player? Winner { get; set; }
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int TotalScore { get; set; }
        public string PlayerName { get; set; }    
        public static ServerMessage FromJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ServerMessage>(json, options) ?? new ServerMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка десериализации сообщения: {ex.Message}");
                return new ServerMessage { Type = "Error", Message = "Ошибка обработки сообщения" };
            }
        }
    }
}
