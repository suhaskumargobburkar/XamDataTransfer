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
        //IBluetoothLE ble = CrossBluetoothLE.Current;
        //IAdapter adapter = CrossBluetoothLE.Current.Adapter;
        //ObservableCollection<IDevice> deviceList = null;
        //MainViewModel viewModel= new MainViewModel();

        string UIID = "831C16CC-34C2-11B2-A85C-FA7B604B699B";
        bool keepScanning;
        private BluetoothService _bluetoothService;
        public MainPage()
        {
            InitializeComponent();
            _bluetoothService = new BluetoothService();
            //ble.StateChanged += (s, e) =>
            //{
            //    DisplayAlert("Information", string.Format("Bluetooth Connection status changed to {0}", e.NewState), "OK");
            //    Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
            //};

            //BindingContext = viewModel;
        }

        async Task StartScanService()
        {
            try
            {
                var ListDevice = await DependencyService.Get<IBluetoothDeviceHelper>().DiscoverPairedDevicesAsync();
                //var ListDevice = await DependencyService.Get<IBluetoothDeviceHelper>().DiscoverNonLEDevices();
                lstDevice.ItemsSource = new ObservableCollection<BluetoothDeviceInfo>(ListDevice);
                //lstDevice.ItemsSource= viewModel.BluetoothDeviceInfoList;
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

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var device = (BluetoothDeviceInfo)lstDevice.SelectedItem;
            if (device != null)
            {
                await DisplayAlert("Alert", "Device Selected " + device.Name, "ok");
                await DependencyService.Get<IBluetoothDeviceHelper>().ConnectAndCommunicate(device.Id);
                await DisplayAlert("Alert", "Device Selected " + device.Id, "ok");
            }
            else
            {
                await DisplayAlert("Alert", "No Device Selected", "ok");
            }

        }

        //async Task<bool> ConnectToDevice(BluetoothDeviceInfo device)
        //{
        //   // IDevice selectedDevice= new IDevice
        //}

        private async void SearchAndConnectButton_Clicked(object sender, EventArgs e)
        {
            var devices = await _bluetoothService.DiscoverDevicesAsync();
            // Assuming you select the first device from the list; you can customize this as needed.
            var selectedDevice = devices.FirstOrDefault();

            if (selectedDevice != null)
            {
                var connected = await _bluetoothService.ConnectToDeviceAsync(selectedDevice);

                if (connected)
                {
                    await DisplayAlert("Alert", "Device connected, you can now send and receive data", "OK");
                }
                else
                {
                    await DisplayAlert("Alert", "Failed to connect to the device", "OK");
                }
            }
            else
            {
                await DisplayAlert("Alert", "No devices found or no paired devices","OK");
            }
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            var dataToSend = "Hello, Bluetooth!";
            var dataSent = await _bluetoothService.SendDataAsync(dataToSend);

            if (dataSent)
            {
                // Data sent successfully
            }
            else
            {
                // Failed to send data
            }
        }

        private async void ReceiveButton_Clicked(object sender, EventArgs e)
        {
            var receivedData = await _bluetoothService.ReceiveDataAsync();

            if (!string.IsNullOrEmpty(receivedData))
            {
                // Process received data
            }
            else
            {
                // Failed to receive data
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            keepScanning = false;
        }
    }

    //public class MainViewModel : INotifyPropertyChanged
    //{
    //    private ObservableCollection<BluetoothDeviceInfo> bluetoothDeviceInfoList;
    //    public ObservableCollection<BluetoothDeviceInfo> BluetoothDeviceInfoList
    //    {
    //        set
    //        {
    //            if (bluetoothDeviceInfoList != value)
    //            {
    //                bluetoothDeviceInfoList = value;
    //                OnPropertyChanged("BluetoothDeviceInfoList");
    //            }
    //        }
    //        get { return bluetoothDeviceInfoList; }
    //    }

    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //}
}
