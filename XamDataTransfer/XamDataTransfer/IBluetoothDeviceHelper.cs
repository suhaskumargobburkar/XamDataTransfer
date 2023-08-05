using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamDataTransfer
{
    public interface IBluetoothDeviceHelper
    {
        Task<IEnumerable<BluetoothDeviceInfo>> DiscoverPairedDevicesAsync();
        Task<IEnumerable<BluetoothDeviceInfo>> DiscoverNonLEDevices();
        Task ConnectAndCommunicate(string deviceId);
    }

    public class BluetoothDeviceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public Guid DeviceUuid { get; set; }
        public Guid ServiceUuid { get; set; }
        public Guid CharacteristicUuid { get; set; }
    }

   
}
