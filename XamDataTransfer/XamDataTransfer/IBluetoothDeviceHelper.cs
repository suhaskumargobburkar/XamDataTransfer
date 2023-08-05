using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamDataTransfer
{
    public interface IBluetoothDeviceHelper
    {
        Task<IEnumerable<BluetoothDeviceInfo>> DiscoverPairedDevicesAsync();
    }

    public class BluetoothDeviceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
    }

   
}
