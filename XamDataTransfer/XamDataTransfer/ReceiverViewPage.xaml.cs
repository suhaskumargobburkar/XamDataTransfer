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
    public partial class ReceiverViewPage : ContentPage
    {
        private IAdapter _adapter;
        private IDevice _device;
        private IService _service;
        private ICharacteristic _characteristic;
        string UIID = "831C16CC-34C2-11B2-A85C-FA7B604B699B";
        string C_UIID = "831C16CC-34C2-11B2-A85C-FA7B604B699B";

        public ReceiverViewPage()
        {
            InitializeComponent();
            _adapter = CrossBluetoothLE.Current.Adapter;
            _adapter.DeviceConnected += DeviceConnected;
        }
        private async void DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            _device = e.Device;
            _service = await _device.GetServiceAsync(new Guid("YOUR_SERVICE_UUID")); // Replace with the same common service UUID used in the sender
            _characteristic = await _service.GetCharacteristicAsync(new Guid("YOUR_CHARACTERISTIC_UUID")); // Replace with the same common characteristic UUID used in the sender

            _characteristic.ValueUpdated += Characteristic_ValueUpdated;
            await _characteristic.StartUpdatesAsync();
        }

        private void Characteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            var receivedData = Encoding.UTF8.GetString(e.Characteristic.Value);
            // Handle received data (e.g., update UI)
        }
    }
}