using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Xamarin.Forms;
using XamDataTransfer.UWP;

[assembly: Dependency(typeof(BluetoothDeviceHelper))]
namespace XamDataTransfer.UWP
{
    public class BluetoothDeviceHelper : IBluetoothDeviceHelper
    {

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
                        deviceList.Add(new BluetoothDeviceInfo
                        {
                            Id = bluetoothDevice.DeviceId,
                            Name = bluetoothDevice.Name,
                            IsConnected = bluetoothDevice.ConnectionStatus == BluetoothConnectionStatus.Connected ? true : false
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            return deviceList;
        }


    }
}
