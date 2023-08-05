using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamDataTransfer
{
    public partial class MainPage : ContentPage
    {
        IBluetoothLE ble = CrossBluetoothLE.Current;
        IAdapter adapter = CrossBluetoothLE.Current.Adapter;
        ObservableCollection<IDevice> deviceList = null;
       MainViewModel viewModel= new MainViewModel();
        bool keepScanning;
        public MainPage()
        {
            InitializeComponent();
            ble.StateChanged += (s, e) =>
            {
                DisplayAlert("Information", string.Format("Bluetooth Connection status changed to {0}", e.NewState), "OK");
                Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
            };

            BindingContext = viewModel;
        }

        async Task StartScanService()
        {
            try
            {
                var ListDevice = await DependencyService.Get<IBluetoothDeviceHelper>().DiscoverPairedDevicesAsync();
                viewModel.BluetoothDeviceInfoList = new ObservableCollection<BluetoothDeviceInfo>(ListDevice);
                lstDevice.ItemsSource= viewModel.BluetoothDeviceInfoList;
                //if (keepScanning)
                //    return;

                //deviceList= new ObservableCollection<IDevice>();
                //adapter.DeviceDiscovered += (s, a) => deviceList.Add(a.Device);
                //await adapter.StartScanningForDevicesAsync();
                //keepScanning = true;

                //do
                //{
                //    await Task.Delay(TimeSpan.FromSeconds(3));


                //    Device.BeginInvokeOnMainThread(async () =>
                //    {
                //        var ListDevice = await DependencyService.Get<IBluetoothDeviceHelper>().DiscoverPairedDevicesAsync();
                //        viewModel.BluetoothDeviceInfoList = new ObservableCollection<BluetoothDeviceInfo>(ListDevice);

                //       // lstDevice.ItemsSource = (System.Collections.IEnumerable)ListDevice;

                //        //var systemDevices = adapter.GetSystemConnectedOrPairedDevices();
                //        //lstDevice.ItemsSource = deviceList;
                //        //foreach (var device in systemDevices)
                //        //{
                //        //    //await _adapter.ConnectToDeviceAsync(device);
                //        //}
                //    });


                //} while (keepScanning);


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");

            }
        }

        private async void btnScanDevice_Clicked(object sender, EventArgs e)
        {
            await StartScanService();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var device = (BluetoothDeviceInfo)lstDevice.SelectedItem;
            if (device != null)
            {
                DisplayAlert("Alert", "Device Selected " + device.Name, "ok");
            }
            else
            {
                DisplayAlert("Alert", "No Device Selected", "ok");
            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            keepScanning = false;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BluetoothDeviceInfo> bluetoothDeviceInfoList;
        public ObservableCollection<BluetoothDeviceInfo> BluetoothDeviceInfoList
        {
            set
            {
                if (bluetoothDeviceInfoList != value)
                {
                    bluetoothDeviceInfoList = value;
                    OnPropertyChanged("BluetoothDeviceInfoList");
                }
            }
            get { return bluetoothDeviceInfoList; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
