using NLog;
using Ubit_Slendermena_Client;

namespace GameClient;

internal static class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    static public EntryForm form;
    static private GameClient.Network.GameClient? _client;

    [STAThread]
    static void Main()
    {
        try
        {
            Logger.Info("Запуск приложения Game Client");
            ApplicationConfiguration.Initialize();
            form = new EntryForm(_client);
            Logger.Info("EntryForm создана, запуск основного цикла приложения");
            Application.Run(form);
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Критическая ошибка при запуске приложения");
        }
        finally
        {
            Logger.Info("Завершение работы приложения");
            LogManager.Shutdown();
        }
    }
}