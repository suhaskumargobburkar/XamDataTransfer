using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamDataTransfer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SenderViewPage : ContentPage
    {
        private IAdapter _adapter;
        private IDevice _device;
        private IService _service;
        private ICharacteristic _characteristic;

        public SenderViewPage()
        {
            InitializeComponent();
            _adapter = CrossBluetoothLE.Current.Adapter;
        }

        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            _device = await _adapter.ConnectToKnownDeviceAsync(new Guid("RECEIVER_DEVICE_UUID")); // Replace with the receiver device's UUID
            _service = await _device.GetServiceAsync(new Guid("SERVICE_UUID")); // Replace with the shared service UUID
            _characteristic = await _service.GetCharacteristicAsync(new Guid("CHARACTERISTIC_UUID")); // Replace with the shared characteristic UUID
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            if (_characteristic != null)
            {
                var dataToSend = Encoding.UTF8.GetBytes("Hello from sender!");
                await _characteristic.WriteAsync(dataToSend);
            }
        }
    }
}