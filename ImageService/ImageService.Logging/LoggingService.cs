using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Logging.Model;

namespace ImageService.Logging
{
    class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs messageReceivedEventArgs = new MessageRecievedEventArgs(type, message);
            MessageRecieved?.Invoke(this, messageReceivedEventArgs);
        }
    }
}
