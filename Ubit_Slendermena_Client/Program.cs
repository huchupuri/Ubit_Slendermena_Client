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
            Logger.Info("������ ���������� Game Client");
            ApplicationConfiguration.Initialize();
            form = new EntryForm(_client);
            Logger.Info("EntryForm �������, ������ ��������� ����� ����������");
            Application.Run(form);
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "����������� ������ ��� ������� ����������");
        }
        finally
        {
            Logger.Info("���������� ������ ����������");
            LogManager.Shutdown();
        }
    }
}