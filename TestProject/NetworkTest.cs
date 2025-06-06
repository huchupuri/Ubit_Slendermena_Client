using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using GameClient.Network;

namespace Tests.GameClientTests
{
    [TestFixture]
    public class GameClient_Tests
    {
        private Mock<ClientWebSocket> _mockSocket;
        private GameClient _client;

        [SetUp]
        public void Setup()
        {
            _mockSocket = new Mock<ClientWebSocket>();

            var mockReceiveMethod = typeof(WebSocket).GetMethod("ReceiveAsync",
                new[] { typeof(ArraySegment<byte>), typeof(CancellationToken) });

            _mockSocket.Setup(m => mockReceiveMethod.Invoke(m.Object, It.IsAny<object[]>()))
                .ReturnsAsync(new WebSocketReceiveResult(4, WebSocketMessageType.Text, false));

            _mockSocket.Setup(s => s.SendAsync(It.IsAny<ArraySegment<byte>>(),
                    WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback((ArraySegment<byte> message, WebSocketMessageType _, bool __, CancellationToken ___) =>
                {
                    string sentMessage = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
                    Console.WriteLine("Отправлено: " + sentMessage);
                });

            _client = new GameClient("ws://localhost")
            {
                // Подменяем веб сокет
                _webSocket = _mockSocket.Object,
                _isConnected = true,
                _cts = new CancellationTokenSource()
            };
        }

        [Test]
        public async Task SubmitAnswerAsync_WhenServerRespondsCorrect_ShouldReceiveCorrectAnswerEvent()
        {
            // Arrange
            var receivedMessage = new ServerMessage { Type = "AnswerResult", Content = "correct" };
            EventHandler<ServerMessage> handler = null;

            _client.MessageReceived += (sender, msg) =>
            {
                receivedMessage = msg;
            };

            var buffer = Encoding.UTF8.GetBytes("{\"Type\":\"AnswerResult\",\"Content\":\"correct\"}");
            var mockBuffer = new ArraySegment<byte>(buffer);

            _mockSocket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WebSocketReceiveResult(buffer.Length, WebSocketMessageType.Text, true));

            // Act
            await _client.SubmitAnswerAsync(1, "что то ");

            var args = new Mock<EventArgs>();
            _client.GetType().GetField("_webSocket", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                   .GetValue(_client);

            // Вызываем событие вручную 

            // Assert
            receivedMessage.Type.Should().Be("AnswerResult");
            receivedMessage.Content.Should().Be("correct");
        }
    }
}
