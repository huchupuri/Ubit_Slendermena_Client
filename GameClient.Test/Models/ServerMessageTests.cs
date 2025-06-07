using FluentAssertions;
using GameClient.Models;
using Moq;
using System.Diagnostics;
using System.Net.WebSockets;

using NUnit.Framework;
using GameClient.Network;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Moq;
using System.Threading;
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

namespace GameClient.Tests
{
    [TestFixture]
    [Category("Performance")]
    public class GameClientPerformanceNUnitTests
    {
        private GameClient.Network.GameClient _gameClient;
        private Mock<ClientWebSocket> _mockWebSocket;

        [SetUp]
        public void Setup()
        {
            _gameClient = new GameClient.Network.GameClient("ws://localhost:5000/");
            _mockWebSocket = new Mock<ClientWebSocket>();
        }

        [TearDown]
        public void TearDown()
        {
            _gameClient?.Dispose();
        }

        [Test]
        [Timeout(1000)] // Тест должен завершиться за 1 секунду
        public async Task LoginAsync_Performance_CompletesWithinTimeLimit()
        {
            // Arrange
            _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
            _gameClient._webSocket = _mockWebSocket.Object;
            _gameClient._isConnected = true;

            _mockWebSocket.Setup(ws => ws.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                It.IsAny<WebSocketMessageType>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var stopwatch = Stopwatch.StartNew();

            // Act
            await _gameClient.LoginAsync("testuser", "testpassword");

            // Assert
            stopwatch.Stop();
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(100),
                "LoginAsync должен выполняться быстро");
        }

        [Test]
        [Repeat(100)]
        public async Task LoginAsync_RepeatedCalls_ConsistentPerformance()
        {
            // Arrange
            _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
            _gameClient._webSocket = _mockWebSocket.Object;
            _gameClient._isConnected = true;

            _mockWebSocket.Setup(ws => ws.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                It.IsAny<WebSocketMessageType>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var stopwatch = Stopwatch.StartNew();

            // Act
            await _gameClient.LoginAsync("testuser", "testpassword");

            // Assert
            stopwatch.Stop();
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(50),
                $"Вызов #{TestContext.CurrentContext.CurrentRepeatCount} занял слишком много времени");
        }

        [Test]
        public async Task LoginAsync_LargeCredentials_HandlesEfficiently()
        {
            // Arrange
            string largeUsername = new string('a', 10);
            string largePassword = new string('b', 10);

            _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
            _gameClient._webSocket = _mockWebSocket.Object;
            _gameClient._isConnected = true;

            _mockWebSocket.Setup(ws => ws.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                It.IsAny<WebSocketMessageType>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var stopwatch = Stopwatch.StartNew();

            // Act
            await _gameClient.LoginAsync(largeUsername, largePassword);

            // Assert
            stopwatch.Stop();
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(500),
                "Обработка больших данных должна быть эффективной");
        }
    }
}

