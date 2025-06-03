using GameClient.Forms;
using Ubit_Slendermena_Client;

namespace GameClient;

internal static class Program
{
    
    static public EntryForm form;
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        form = new EntryForm();
        Application.Run(form);
    }
}