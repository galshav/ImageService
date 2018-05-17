using ImageService.Controller;
using ImageService.Logging;
using ImageService.Logging.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Networking
{
    public class ClientHandler : IClientHandler
    {
        public void handleClient(TcpClient i_Client, IImageController i_Controller, ILoggingService i_Logger)
        {
            using (NetworkStream stream = i_Client.GetStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (true)
                {
                    try
                    {
                        int commandID = reader.ReadInt32();
                        i_Logger.Log($"Command id received: {commandID}", MessageTypeEnum.INFO);
                        bool result = false;
                        switch (commandID)
                        {
                            case 2:     // Send configurations to client.
                                {
                                    string output = i_Controller.ExecuteCommand(commandID, null, out result);
                                    i_Logger.Log($"Sending {output}", MessageTypeEnum.INFO);
                                    writer.Write(output);
                                    break;
                                }

                            default:
                                {
                                    i_Logger.Log("Unknown command id", MessageTypeEnum.WARNING);
                                    break;
                                }
                        }
                    }

                    catch (Exception i_Error)
                    {
                        // On socket close.
                        i_Logger.Log($"Connection closed by foreign side.", MessageTypeEnum.WARNING);
                        return;
                    }
                }
            }
        }
    }
}
