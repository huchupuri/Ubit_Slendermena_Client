namespace Ubit_Slendermena_Client.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class Question
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Price { get; set; }
}

public class Player
{
    private string _passwordHash;
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public int TotalGames { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int CurrentScore { get; set; } = 0;
    public int Wins { get; set; } = 0;
}

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
}