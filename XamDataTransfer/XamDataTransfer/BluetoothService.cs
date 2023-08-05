using System;
using System.Collections.Generic;
using System.Text;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading.Tasks;
using System.Linq;
using Plugin.BLE.Abstractions;
using Xamarin.Forms;

namespace XamDataTransfer
{
    public class BluetoothService
    {
        private IAdapter _adapter;
        private IDevice _device;
        //private IEnumerable<IDevice> _deviceList;
        private IService _service;
        private ICharacteristic _characteristic;

        public BluetoothService()
        {
            _adapter = CrossBluetoothLE.Current.Adapter;
        }

        public async Task<IEnumerable<IDevice>> DiscoverDevicesAsync()
        {
            try
            {
                var _deviceList = new List<IDevice>();
                _adapter.DeviceDiscovered += (s, a) => _deviceList.Add(a.Device);
                await _adapter.StartScanningForDevicesAsync();
                
                return _deviceList;
                //return null;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        public async Task<IDevice> DiscoverMyDevicesAsync(string MyDeviceAddress)
        {
            try
            {
               // var _deviceList = new List<IDevice>();
                //_adapter.DeviceDiscovered += (s, a) => _deviceList.Add(a.Device);
                var scanFilterOptions = new ScanFilterOptions();
                //scanFilterOptions.ServiceUuids = new[] { guid1, guid2, etc }; // cross platform filter
                //scanFilterOptions.ManufacturerDataFilters = new[] { new ManufacturerDataFilter(1), new ManufacturerDataFilter(2) }; // android only filter
               // scanFilterOptions.DeviceAddresses = new[] { MyDeviceAddress }; // android only filter
                return await _adapter.ConnectToKnownDeviceAsync(new Guid(MyDeviceAddress));
                
                //await _adapter.StartScanningForDevicesAsync(scanFilterOptions);
                //return _deviceList.FirstOrDefault();
                //return null;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        public async Task<bool> ConnectToDeviceAsync(IDevice device)
        {
            try
            {
                _device = device;
                await _adapter.ConnectToDeviceAsync(_device);
                return true;
            }
            catch (Exception ex)
            {
                // Handle exception
                return false;
            }
        }

        public async Task<bool> SendDataAsync(string data)
        {
            try
            {
                _service = await _device.GetServiceAsync(Guid.Parse("YOUR_SERVICE_UUID"));
                _characteristic = await _service.GetCharacteristicAsync(Guid.Parse("YOUR_CHARACTERISTIC_UUID"));

                var bytes = Encoding.UTF8.GetBytes(data);
                await _characteristic.WriteAsync(bytes);
                return true;
            }
            catch (Exception ex)
            {
                // Handle exception
                return false;
            }
        }

        public async Task<string> ReceiveDataAsync()
        {
            try
            {
                if (_characteristic != null)
                {
                    var result = await _characteristic.ReadAsync();
                    return Encoding.UTF8.GetString(result.data);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }

            return null;
        }
    }
}
