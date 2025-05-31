using GameClient.Forms;
using Ubit_Slendermena_Client;

namespace GameClient;

internal static class Program
{
    
    static public AuthorizationForm form;
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        form = new AuthorizationForm();
        Application.Run(form);
    }
}