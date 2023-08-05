using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Xamarin.Forms;
using XamWifiDirectConnect.UWP;

[assembly: Dependency(typeof(ClientService))]
namespace XamWifiDirectConnect.UWP
{
    public class ClientService : IClientServices
    {
        //private WiFiDirectDevice wifiDirectDevice;

        public async Task ConnectToServer()
        {
            //wifiDirectDevice = await WiFiDirectDevice.FromIdAsync(null);

            //if (wifiDirectDevice == null)
            //{
            //    Console.WriteLine("No Wi-Fi Direct device found.");
            //    return;
            //}

            var peers = await DeviceInformation.FindAllAsync(WiFiDirectDevice.GetDeviceSelector(WiFiDirectDeviceSelectorType.AssociationEndpoint));

            if (peers.Count > 0)
            {
                WiFiDirectDevice wfdDevice = await WiFiDirectDevice.FromIdAsync(peers[0].Id);

                StreamSocket socket = new StreamSocket();
                await socket.ConnectAsync(new Windows.Networking.HostName("192.168.1.1"), "1337");

                string message = "Hello from client!";
                DataWriter writer = new DataWriter(socket.OutputStream);
                writer.WriteUInt32(writer.MeasureString(message));
                writer.WriteString(message);
                await writer.StoreAsync();
                writer.DetachStream();

                // Receive response
                DataReader reader = new DataReader(socket.InputStream);
                await reader.LoadAsync(sizeof(uint));
                uint responseLength = reader.ReadUInt32();
                await reader.LoadAsync(responseLength);
                string response = reader.ReadString(responseLength);

                // Process response
                Console.WriteLine($"Received response: {response}");
            }
        }
    

    }
}
