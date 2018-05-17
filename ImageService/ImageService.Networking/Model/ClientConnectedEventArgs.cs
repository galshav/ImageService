using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Networking.Model
{
    public class ClientConnectedEventArgs
    {
        public TcpClient Client { get; set; }
    }
}
