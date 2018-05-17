using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Model
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        public MessageRecievedEventArgs(MessageTypeEnum i_Status, string i_Message)
        {
            this.Status = i_Status;
            this.Message = i_Message;
        }

        public string hashMessageType(MessageTypeEnum i_Type)
        {
            if (MessageTypeEnum.INFO == i_Type)
            {
                return "INFO";
            }

            else if (MessageTypeEnum.WARNING == i_Type)
            {
                return "WARNING";
            }

            else
            {
                return "FAIL";
            }
        }
    }
}
