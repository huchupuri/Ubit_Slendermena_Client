using FluentAssertions;
using GameClient.Models;

namespace GameClient.Tests.Models
{
    [TestFixture]
    public class ServerMessageTests
    {
        [Test]
        public void ServerMessage_DefaultProperties_ShouldBeEmpty()
        {
            var message = new ServerMessage();

            message.Type.Should().BeEmpty();
            message.Message.Should().BeEmpty();
            message.Categories.Should().BeEmpty();
            message.Players.Should().BeEmpty();
        }

        [Test]
        public void ServerMessage_WithCategoryList_ShouldContainItems()
        {
            var message = new ServerMessage
            {
                Categories = new List<Category>
                {
                    new Category { Name = "История" },
                    new Category { Name = "География" }
                }
            };

            message.Categories.Should().HaveCount(2);
            message.Categories.Select(c => c.Name).Should().Contain("История").And.Contain("География");
        }

        [Test]
        public void ServerMessage_WithTypeAndMessage_ShouldBeCorrect()
        {
            var message = new ServerMessage
            {
                Type = "LoginResult",
                Message = "Успешный вход"
            };

            message.Type.Should().Be("LoginResult");
            message.Message.Should().Be("Успешный вход");
        }

        [Test]
        public void ServerMessage_WithPlayer_ShouldContainValidData()
        {
            var player = new Player { Username = "admin", Score = 1000 };
            var message = new ServerMessage { Player = player };

            message.Player.Should().NotBeNull();
            message.Player.Username.Should().Be("admin");
            message.Player.Score.Should().Be(1000);
        }
    }
}
