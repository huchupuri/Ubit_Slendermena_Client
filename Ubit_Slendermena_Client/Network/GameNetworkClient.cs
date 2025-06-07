using GameClient.Models;
using NLog;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameClient.Network
{
    public class GameClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ClientWebSocket _webSocket;
        private readonly string _serverUrl;
        private bool _isConnected;
        private CancellationTokenSource _cts;

        public event EventHandler<ServerMessage> MessageReceived;
        public event EventHandler<string> ConnectionClosed;
        public event EventHandler<Exception> ErrorOccurred;

        public bool IsConnected => _isConnected && _webSocket?.State == WebSocketState.Open;

        public GameClient(string serverUrl)
        {
            _serverUrl = serverUrl;
            _cts = new CancellationTokenSource();
            Logger.Info("GameClient создан");
        }

        public async Task ConnectAsync()
        {
            try
            {
                Logger.Info("Попытка подключения к серверу");
                if (_webSocket?.State != WebSocketState.Open)
                {
                    _webSocket?.Dispose();
                    _webSocket = new ClientWebSocket();
                    _cts?.Cancel();
                    _cts = new CancellationTokenSource();
                }

                await _webSocket.ConnectAsync(new Uri(_serverUrl), _cts.Token);
                _isConnected = true;

                Logger.Info("Успешное подключение к серверу");
                var receiveTask = Task.Run(ReceiveMessagesAsync);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка подключения к серверу");
                _isConnected = false;
                ErrorOccurred?.Invoke(this, ex);
                throw;
            }
        }

        public async Task SendMessageAsync(object message)
        {
            if (!IsConnected)
            {
                Logger.Error($"Попытка отправки сообщения без подключения: {_isConnected}, {_webSocket?.State}");
                throw new InvalidOperationException($"Клиент {_isConnected}, {_webSocket?.State} не подключен к серверу");
            }

            try
            {
                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

                await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes),
                    WebSocketMessageType.Text, true, _cts.Token);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка отправки сообщения");
                ErrorOccurred?.Invoke(this, ex);
                throw;
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[16384]; 

            try
            {
                Logger.Debug("Начало прослушивания сообщений от сервера");

                while (_isConnected && _webSocket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
                {
                    var messageBuilder = new StringBuilder();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                "Закрытие по запросу сервера", CancellationToken.None);
                            _isConnected = false;
                            Logger.Warn("Соединение закрыто сервером");
                            ConnectionClosed?.Invoke(this, "Соединение закрыто сервером");
                            return;
                        }

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            messageBuilder.Append(chunk);
                        }
                    }
                    while (!result.EndOfMessage);

                    if (messageBuilder.Length > 0)
                    {
                        string messageText = messageBuilder.ToString();
                        Logger.Debug($"Получено сообщение: {messageText}");
                        ServerMessage serverMessage = ServerMessage.FromJson(messageText);
                        MessageReceived?.Invoke(this, serverMessage);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("Прослушивание сообщений отменено");
                _isConnected = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                _isConnected = false;
                ErrorOccurred?.Invoke(this, ex);
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            Logger.Info($"Попытка входа пользователя: {username}");
            await SendMessageAsync(new
            {
                Type = "Login",
                Username = username,
                Password = password
            });
        }

        public async Task RegisterAsync(string username, string password)
        {
            Logger.Info($"Попытка регистрации пользователя: {username}");
            await SendMessageAsync(new
            {
                Type = "Register",
                Username = username,
                Password = password
            });
        }
        public async Task CreateGameAsync(int playerCount, string hostName, QuestionFile customQuestions = null)
        {
            var message = new
            {
                Type = "CreateGame",
                playerCount = playerCount,
                hostName = hostName,
                customQuestions = customQuestions
            };

            await SendMessageAsync(message);
        }
        public async Task JoinGameAsync(string playerName)
        {
            Logger.Info($"Присоединение к игре игрока: {playerName}");
            await SendMessageAsync(new
            {
                Type = "JoinGame",
                playerName = playerName
            });
        }

        
        public async Task DisconnectAsync()
        {
            Logger.Info("Отключение от сервера");

            if (_isConnected && _webSocket?.State == WebSocketState.Open)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        "Клиент отключается", CancellationToken.None);
                    Logger.Info("Успешное отключение от сервера");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Ошибка при отключении от сервера");
                }
            }

            _isConnected = false;
            _cts?.Cancel();
        }

        public void Dispose()
        {
            _cts?.Cancel(); 
            _webSocket?.Dispose();
            _cts?.Dispose();
        }
    }
}
