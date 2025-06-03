using GameClient;
using GameClient.Models;
using GameClient.Network;
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
    public partial class MenuForm : Form
    {
        private readonly Player _player;
        private readonly GameClient.Network.GameClient _client;
            
        public MenuForm(Player player, GameClient.Network.GameClient client)
        {
            InitializeComponent();
            _player = player;
            _client = client;

            InitializeComponent();
            SubscribeToClientEvents();
        }

        private void SubscribeToClientEvents()
        {
            // Подписываемся на события клиента для обработки сообщений сервера
            _client.ConnectionClosed += OnConnectionClosed;
            _client.ErrorOccurred += OnErrorOccurred;
        }

        private void UnsubscribeFromClientEvents()
        {
            // Отписываемся от событий при закрытии формы
            if (_client != null)
            {
                _client.ConnectionClosed -= OnConnectionClosed;
                _client.ErrorOccurred -= OnErrorOccurred;
            }
        }

        private void HandleGameStarted()
        {
            var gameForm = new NewGame(_player, _client);
            //UnsubscribeFromClientEvents();
            this.Hide();
            gameForm.ShowDialog();
            this.Close();
        }
        private void OnConnectionClosed(object sender, string reason)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(OnConnectionClosed), sender, reason);
                return;
            }

            MessageBox.Show($"Соединение с сервером потеряно: {reason}", "Соединение потеряно",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ReturnToEntryForm();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Exception>(OnErrorOccurred), sender, ex);
                return;
            }

            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReturnToEntryForm()
        {
            try
            {
                UnsubscribeFromClientEvents();
                this.Hide();

                var entryForm = new EntryForm(_client);
                entryForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при возврате к форме входа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            // Открываем форму профиля игрока
            var profileForm = new ProfileForm(_player);
            profileForm.ShowDialog();
        }

        private void LocalizationBtn_Click(object sender, EventArgs e)
        {
            // Здесь можно добавить логику смены языка
            MessageBox.Show("Функция локализации будет добавлена позже", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void PlayBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Отключаем кнопку во время обработки
                PlayBtn.Enabled = false;
                PlayBtn.Text = "Создание игры...";

                // Проверяем соединение с сервером
                if (!_client.IsConnected)
                {
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Открываем форму создания новой игры
                var newGameForm = new NewGame(_player, _client);
                UnsubscribeFromClientEvents();
                this.Hide();
                newGameForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании игры: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Восстанавливаем состояние кнопки
                PlayBtn.Enabled = true;
                PlayBtn.Text = "Играть";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Отписываемся от событий и отключаемся от сервера при закрытии формы
            UnsubscribeFromClientEvents();

            if (_client != null && _client.IsConnected)
            {
                try
                {
                    _client.DisconnectAsync().Wait(1000); // Ждем максимум 1 секунду
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отключении: {ex.Message}");
                }
            }

            base.OnFormClosing(e);
        }
    }
}
