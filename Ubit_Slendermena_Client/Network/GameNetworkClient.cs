using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Ubit_Slendermena_Client.Models;

namespace Ubit_Slendermena_Client.Network;

public class GameNetworkClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private bool _isConnected;
    private Thread? _receiveThread;

    public event Action<ServerMessage>? MessageReceived;
    public event Action? Disconnected;

    public bool IsConnected => _isConnected;

    public async Task<bool> ConnectAsync(string serverAddress, int port)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(serverAddress, port);
            _stream = _client.GetStream();
            _isConnected = true;

            _receiveThread = new Thread(ReceiveMessages) { IsBackground = true };
            _receiveThread.Start();

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public void Disconnect()
    {
        _isConnected = false;
        _stream?.Close();
        _client?.Close();
        Disconnected?.Invoke();
    }

    public async Task SendMessageAsync(object message)
    {
        if (!_isConnected || _stream == null) return;

        try
        {
            string json = JsonSerializer.Serialize(message);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await _stream.WriteAsync(data);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ReceiveMessages()
    {
        byte[] buffer = new byte[4096];

        while (_isConnected && _stream != null)
        {
            try
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var message = JsonSerializer.Deserialize<ServerMessage>(json);

                if (message != null)
                {
                    MessageReceived?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                if (_isConnected)
                {
                    MessageBox.Show($"Ошибка получения сообщения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                break;
            }
        }

        Disconnect();
    }
}