using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;
using Windows.Devices.WiFiDirect.Services;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Xamarin.Forms;
using XamWifiDirectConnect.UWP;

[assembly: Dependency(typeof(ServerService))]
namespace XamWifiDirectConnect.UWP
{
    public class ServerService : IServerServices
    {
        private WiFiDirectAdvertisementPublisher publisher;
        private StreamSocketListener listener;

        public async Task StartServer()
        {
            string serviceSelector = WiFiDirectService.GetSelector("MyServiceName");

            //DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(serviceSelector);

            //if (devices.Count == 0)
            //{
            //    Console.WriteLine("No devices with the specified service found.");
            //    return;
            //}
            //WiFiDirectDevice wifiDirectDevice = await WiFiDirectDevice.FromIdAsync(devices.FirstOrDefault().Id);

            //if (wifiDirectDevice == null)
            //{
            //    Console.WriteLine("No Wi-Fi Direct device found.");
            //    return;
            //}

            publisher = new WiFiDirectAdvertisementPublisher();
            publisher.Advertisement.ListenStateDiscoverability = WiFiDirectAdvertisementListenStateDiscoverability.Normal;
            publisher.Advertisement.IsAutonomousGroupOwnerEnabled = true;
            publisher.Start();

            listener = new StreamSocketListener();
            listener.ConnectionReceived += Listener_ConnectionReceived;

            await listener.BindServiceNameAsync("1337");
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            // Handle connection received
            StreamSocket socket = args.Socket;
            DataReader reader = new DataReader(socket.InputStream);
            await reader.LoadAsync(sizeof(uint));
            uint messageLength = reader.ReadUInt32();
            await reader.LoadAsync(messageLength);
            string receivedMessage = reader.ReadString(messageLength);

            // Process received message
            Console.WriteLine($"Received: {receivedMessage}");

            // Send response back
            string response = "Message received and processed!";
            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.WriteUInt32(writer.MeasureString(response));
            writer.WriteString(response);
            await writer.StoreAsync();
            writer.DetachStream();
        }

    }

   
}
