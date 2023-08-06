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
        WiFiDirectConnectionListener _connlistener;

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
            //publisher.Advertisement.LegacySettings.Passphrase = new Windows.Security.Credentials.PasswordCredential { Password = "1234" };
            //publisher.Advertisement.IsAutonomousGroupOwnerEnabled = true;
            publisher.Start();

            listener = new StreamSocketListener();
            listener.ConnectionReceived += Listener_ConnectionReceived;
            _connlistener = new WiFiDirectConnectionListener();
            _connlistener.ConnectionRequested += OnConnectionRequested;

            await listener.BindServiceNameAsync("1337");
        }

        private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs connectionEventArgs)
        {
            WiFiDirectConnectionRequest connectionRequest = connectionEventArgs.GetConnectionRequest();
            await HandleConnectionRequestAsync(connectionRequest);

            connectionRequest.Dispose();
        }

        private async Task<bool> IsAepPairedAsync(string deviceId)
        {
            List<string> additionalProperties = new List<string>();
            additionalProperties.Add("System.Devices.Aep.DeviceAddress");
            String deviceSelector = $"System.Devices.Aep.AepId:=\"{deviceId}\"";
            DeviceInformation devInfo = null;

            try
            {
                devInfo = await DeviceInformation.CreateFromIdAsync(deviceId, additionalProperties);
            }
            catch (Exception ex)
            {
               // rootPage.NotifyUser("DeviceInformation.CreateFromIdAsync threw an exception: " + ex.Message, NotifyType.ErrorMessage);
            }

            //if (devInfo == null)
            //{
            //    rootPage.NotifyUser("Device Information is null", NotifyType.ErrorMessage);
            //    return false;
            //}

            deviceSelector = $"System.Devices.Aep.DeviceAddress:=\"{devInfo.Properties["System.Devices.Aep.DeviceAddress"]}\"";
            DeviceInformationCollection pairedDeviceCollection = await DeviceInformation.FindAllAsync(deviceSelector, null, DeviceInformationKind.Device);
            return pairedDeviceCollection.Count > 0;
        }
        private async Task<bool> HandleConnectionRequestAsync(WiFiDirectConnectionRequest connectionRequest)
        {
            string deviceName = connectionRequest.DeviceInformation.Name;

            bool isPaired = (connectionRequest.DeviceInformation.Pairing?.IsPaired == true) ||
                            (await IsAepPairedAsync(connectionRequest.DeviceInformation.Id));

            // Show the prompt only in case of WiFiDirect reconnection or Legacy client connection.
            if (isPaired || publisher.Advertisement.LegacySettings.IsEnabled)
            {
                //var messageDialog = new MessageDialog($"Connection request received from {deviceName}", "Connection Request");

                //// Add two commands, distinguished by their tag.
                //// The default command is "Decline", and if the user cancels, we treat it as "Decline".
                //messageDialog.Commands.Add(new UICommand("Accept", null, true));
                //messageDialog.Commands.Add(new UICommand("Decline", null, null));
                //messageDialog.DefaultCommandIndex = 1;
                //messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                //var commandChosen = await messageDialog.ShowAsync();

                //if (commandChosen.Id == null)
                //{
                //    return false;
                //}
            }


            //rootPage.NotifyUser($"Connecting to {deviceName}...", NotifyType.StatusMessage);

            //// Pair device if not already paired and not using legacy settings
            //if (!isPaired && !_publisher.Advertisement.LegacySettings.IsEnabled)
            //{
            //    if (!await connectionSettingsPanel.RequestPairDeviceAsync(connectionRequest.DeviceInformation.Pairing))
            //    {
            //        return false;
            //    }
            //}

            //WiFiDirectDevice wfdDevice = null;
            //try
            //{
            //    // IMPORTANT: FromIdAsync needs to be called from the UI thread
            //    wfdDevice = await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id);
            //}
            //catch (Exception ex)
            //{
            //    rootPage.NotifyUser($"Exception in FromIdAsync: {ex}", NotifyType.ErrorMessage);
            //    return false;
            //}

            // Register for the ConnectionStatusChanged event handler
            //wfdDevice.ConnectionStatusChanged += OnConnectionStatusChanged;

            //var listenerSocket = new StreamSocketListener();

            //// Save this (listenerSocket, wfdDevice) pair so we can hook it up when the socket connection is made.
            //_pendingConnections[listenerSocket] = wfdDevice;

            //var EndpointPairs = wfdDevice.GetConnectionEndpointPairs();

            //listenerSocket.ConnectionReceived += this.OnSocketConnectionReceived;
            //try
            //{
            //    await listenerSocket.BindServiceNameAsync(Globals.strServerPort);
            //}
            //catch (Exception ex)
            //{
            //    rootPage.NotifyUser($"Connect operation threw an exception: {ex.Message}", NotifyType.ErrorMessage);
            //    return false;
            //}

            //rootPage.NotifyUser($"Devices connected on L2, listening on IP Address: {EndpointPairs[0].LocalHostName}" +
            //                    $" Port: {Globals.strServerPort}", NotifyType.StatusMessage);


            return true;
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

        //private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs args)
        //{ 
        //    var connReq = args.GetConnectionRequest();

        //    WiFiDirectDevice wfd = await WiFiDirectDevice.FromIdAsync(connReq.DeviceInformation.Id);

        //    var endPairs = wfd.GetConnectionEndpointPairs();
        //}

    }

   
}
