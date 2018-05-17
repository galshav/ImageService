using System;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Networking.Model;

namespace ImageService.Networking
{
    public class NetworkServer
    {
        static readonly string LOCALHOST = "127.0.0.1";
        private readonly int m_Port = 1337;
        private TcpListener m_Listener = null;
        private IImageController m_Controller = null;
        private IClientHandler m_ClientHandler = null;
        private ILoggingService m_Logger = null;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<TcpClient> clientDisconnected;

        public NetworkServer(int i_Port, IImageController i_Controller, ILoggingService i_Logger)
        {
            m_Port = i_Port;
            m_Logger = i_Logger;
            m_Controller = i_Controller;
            m_ClientHandler = new ClientHandler();
        }

        public void Start()
        {
            m_Logger.Log("Starting localhost communication server.", MessageTypeEnum.INFO);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(LOCALHOST), m_Port);
            m_Listener = new TcpListener(endPoint);
            m_Listener.Start();
            m_Logger.Log($"Waiting for new connections on port {m_Port}", MessageTypeEnum.INFO);
            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = m_Listener.AcceptTcpClient();
                        ClientConnectedEventArgs args = new ClientConnectedEventArgs();
                        args.Client = client;
                        ClientConnected.Invoke(this, args);
                        m_Logger.Log("New connection accepted", MessageTypeEnum.INFO);
                        m_ClientHandler.handleClient(client, m_Controller, m_Logger);
                        m_Logger.Log("QUITING CLIENT", MessageTypeEnum.WARNING);
                        clientDisconnected.Invoke(this, client);
                    }

                    catch (SocketException m_Error)
                    {
                        m_Logger.Log($"Got exception: {m_Error.ToString()}", MessageTypeEnum.INFO);
                        break;
                    }
                }
            });
            task.Start();
        }

        public void Stop()
        {
            if (null != m_Listener)
            {
                m_Logger.Log("Closing localhost communication server.", MessageTypeEnum.INFO);
                m_Listener.Stop();
            }
        }
    }
}
