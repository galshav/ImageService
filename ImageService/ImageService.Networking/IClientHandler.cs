using ImageService.Controller;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Networking
{
    public interface IClientHandler
    {
        void handleClient(TcpClient i_Client, IImageController i_Controller, ILoggingService i_Logger);
    }
}
