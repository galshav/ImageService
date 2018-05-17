using ImageService.Commands;
using ImageService.Controller;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_Model = null;
        private ILoggingService m_Logger = null;
        private Dictionary<int, ICommand> m_Commands = null;

        public ImageController(IImageServiceModel i_Model , ILoggingService i_Logger)
        {
            m_Logger = i_Logger;
            m_Model = i_Model;
            m_Commands = new Dictionary<int, ICommand>()
            {
                { 1, new NewFileCommand(m_Model, m_Logger) },
                { 2, new GetSettingsCommand(m_Model, m_Logger) }
            };
        }

        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            ICommand command = m_Commands[commandID];
            return command.Execute(args, out resultSuccesful);   
        }
    }
}
