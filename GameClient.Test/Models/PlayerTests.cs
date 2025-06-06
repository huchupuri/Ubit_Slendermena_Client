using FluentAssertions;
using GameClient.Models;

namespace GameClient.Tests.Models
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Player_DefaultProperties_ShouldHaveDefaultValues()
        {
            var player = new Player { Username = "tester" };

            player.Id.Should().NotBe(Guid.Empty);
            player.Username.Should().Be("tester");
            player.TotalGames.Should().Be(0);
            player.Score.Should().Be(0);
            player.CurrentScore.Should().Be(0);
            player.Wins.Should().Be(0);
        }

        [Test]
        public void Player_WithSetValues_ShouldStoreCorrectData()
        {
            var player = new Player
            {
                Username = "testuser",
                TotalGames = 5,
                Score = 1000,
                Wins = 3
            };

            player.Username.Should().Be("testuser");
            player.TotalGames.Should().Be(5);
            player.Score.Should().Be(1000);
            player.Wins.Should().Be(3);
        }

        [Test]
        public void Player_RequiredUsername_ThrowsNoException_WhenSet()
        {
            var player = new Player { Username = "admin" };
            player.Username.Should().Be("admin");
        }
    }
}
