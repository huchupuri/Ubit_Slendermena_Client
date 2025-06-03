using Ubit_Slendermena_Client;

namespace GameClient;

internal static class Program
{
    
    static public EntryForm form;
    static private GameClient.Network.GameClient? _client;
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        form = new EntryForm(_client);
        Application.Run(form);
    }
}