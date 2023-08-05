using Plugin.BLE.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Xamarin.Forms;
using XamDataTransfer.UWP;

[assembly: Dependency(typeof(BluetoothDeviceHelper))]
namespace XamDataTransfer.UWP
{
    public class BluetoothDeviceHelper : IBluetoothDeviceHelper
    {
        StreamSocket socket = null;
        public async Task<IEnumerable<BluetoothDeviceInfo>> DiscoverPairedDevicesAsync()
        {
            var deviceList = new List<BluetoothDeviceInfo>();

            var bluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelector());
            foreach (var device in bluetoothDevices)
            {
                try
                {
                    var bluetoothDevice = await BluetoothDevice.FromIdAsync(device.Id);
                    if (bluetoothDevice != null)
                    {
                        var item = new BluetoothDeviceInfo();
                        item.Id = device.Id;
                        item.Name = device.Name;
                        item.IsConnected = bluetoothDevice.ConnectionStatus == BluetoothConnectionStatus.Connected ? true : false;
                        //var otherdetails = await ConnectToDevice(device);
                        //if (otherdetails?.DeviceUuid != null)
                        //{
                        //    item.ServiceUuid = otherdetails.ServiceUuid;
                        //    item.DeviceUuid = otherdetails.DeviceUuid;
                        //    item.CharacteristicUuid = otherdetails.CharacteristicUuid;
                        //}
                        deviceList.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            return deviceList;
        }

        public async Task<IEnumerable<BluetoothDeviceInfo>> DiscoverNonLEDevices()
        {
            string rfcommSelector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.GenericFileTransfer);
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(rfcommSelector);
            var deviceList = new List<BluetoothDeviceInfo>();
            foreach (DeviceInformation device in devices)
            {
                // Display or use deviceInfo.Id to identify the device
                // This is the unique identifier of the Bluetooth device
                //string deviceId = deviceInfo.Id;

                // You can populate a list or UI with the found devices and their IDs

                var item = new BluetoothDeviceInfo();
                item.Id = device.Id;
                item.Name = device.Name;
                deviceList.Add(item);
            }
           return deviceList;
        }

        public async Task ConnectAndCommunicate(string deviceId)
        {
            try
            {
                DeviceInformation deviceInfo = await DeviceInformation.CreateFromIdAsync(deviceId);
                RfcommDeviceService rfcommService = await RfcommDeviceService.FromIdAsync(deviceInfo.Id);

                if (rfcommService != null)
                {
                    using (StreamSocket socket = new StreamSocket())
                    {
                        await socket.ConnectAsync(rfcommService.ConnectionHostName, rfcommService.ConnectionServiceName);

                        // Send data
                        using (var writer = new DataWriter(socket.OutputStream))
                        {
                            string messageToSend = "Hello from sender!";
                            writer.WriteString(messageToSend);
                            await writer.StoreAsync();
                        }

                        // Receive data
                        using (var reader = new DataReader(socket.InputStream))
                        {
                            uint bytesRead = await reader.LoadAsync(uint.MaxValue);
                            string receivedData = reader.ReadString(bytesRead);

                            // Handle received data
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
    }
  
}
