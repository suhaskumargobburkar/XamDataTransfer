using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamWifiDirectConnect
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void btnStartService_Clicked(object sender, EventArgs e)
        {
            await DependencyService.Get<IServerServices>().StartServer();
        }

        private async void btnReadService_Clicked(object sender, EventArgs e)
        {
            await DependencyService.Get<IClientServices>().ConnectToServer();
        }
    }
}
