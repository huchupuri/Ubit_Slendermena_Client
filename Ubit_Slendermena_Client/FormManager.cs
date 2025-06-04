using GameClient.Models;
using GameClient.Network;
using System;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    /// <summary>
    /// Менеджер форм для правильного управления жизненным циклом форм и событиями WebSocket
    /// </summary>
    public static class FormManager
    {
        private static GameClient.Network.GameClient _client;

        /// <summary>
        /// Инициализирует клиент WebSocket
        /// </summary>
        public static void InitializeClient(string serverUrl = "ws://localhost:5000/")
        {
            if (_client != null)
            {
                try
                {
                    _client.DisconnectAsync().Wait(1000);
                }
                catch { }
                _client = null;
            }

            _client = new GameClient.Network.GameClient(serverUrl);
        }

        /// <summary>
        /// Возвращает текущий экземпляр клиента WebSocket
        /// </summary>
        public static GameClient.Network.GameClient GetClient()
        {
            if (_client == null)
            {
                InitializeClient();
            }
            return _client;
        }

        /// <summary>
        /// Переключает на форму входа
        /// </summary>
        public static void SwitchToEntryForm(Form currentForm)
        {
            var entryForm = new EntryForm(_client);
            SwitchForm(currentForm, entryForm);
        }

        /// <summary>
        /// Переключает на форму меню
        /// </summary>
        public static void SwitchToMenuForm(Form currentForm, Player player)
        {
            var menuForm = new MenuForm(player, _client);
            SwitchForm(currentForm, menuForm);
        }

        /// <summary>
        /// Переключает на форму создания игры
        /// </summary>
        public static void SwitchToNewGameForm(Form currentForm, Player player)
        {
            var newGameForm = new NewGame(player, _client);
            SwitchForm(currentForm, newGameForm);
        }

        /// <summary>
        /// Переключает на игровую форму
        /// </summary>
        public static void SwitchToGameForm(Form currentForm, Player player)
        {
            var gameForm = new JeopardyGameForm(player, _client);
            SwitchForm(currentForm, gameForm);
        }

        /// <summary>
        /// Общий метод для переключения между формами
        /// </summary>
        private static void SwitchForm(Form currentForm, Form newForm)
        {
            if (currentForm != null)
            {
                currentForm.Hide();
            }

            newForm.FormClosed += (s, args) =>
            {
                if (currentForm != null && !currentForm.IsDisposed)
                {
                    currentForm.Close();
                }
            };

            newForm.Show();
        }

        /// <summary>
        /// Закрывает соединение при выходе из приложения
        /// </summary>
        public static void CleanupOnExit()
        {
            if (_client != null)
            {
                try
                {
                    _client.DisconnectAsync().Wait(1000);
                }
                catch { }
            }
        }
    }
}
