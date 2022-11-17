

using System;
using System.Threading.Tasks;


namespace WindowsLocalNetwork.Helpers
{
    internal class CheckingEthernet
    {

        public static event Error_Text_CallBack TextErrorEvent;

        public async static Task IsConnections()
        {

            dynamic networkListManager = Activator.CreateInstance(
              Type.GetTypeFromCLSID(new Guid("{DCB00C01-570F-4A9B-8D69-199FDBA5723B}")));
          
            do
            {
                TextErrorEvent("Turn on WiFi");
                await Task.Delay(1000);
                
            } while (!networkListManager.IsConnectedToInternet);

            TextErrorEvent(" ");
        }
        
    }
}
