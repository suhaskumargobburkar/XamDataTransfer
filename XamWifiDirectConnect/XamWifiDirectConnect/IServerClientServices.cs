using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamWifiDirectConnect
{
    public interface IServerServices
    {
        Task StartServer();
    }

    public interface IClientServices
    {
        Task ConnectToServer();
    }
}
