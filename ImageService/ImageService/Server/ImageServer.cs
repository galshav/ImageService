using ImageService.Controller;
using ImageService.Logging;
using ImageService.Model.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Logging.Model;
using ImageService.Controller.Handlers;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller = null;
        private ILoggingService m_Logger = null;
        private List<DirectoyHandler> m_DirectoriesHandlers = null;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        #endregion

        public ImageServer(IImageController i_Controller ,ILoggingService i_Logger)
        {
            m_Logger = i_Logger;
            m_controller = i_Controller;
            m_DirectoriesHandlers = new List<DirectoyHandler>();

            bool result = false;
            string watchedDirectories = ImageService.ReadSetting("Handler", out result);
            string[] directories = watchedDirectories.Split(';');
            foreach (string path in directories)
            {
                DirectoyHandler directoryHandler = new DirectoyHandler(path, m_controller, this ,m_Logger);
                m_DirectoriesHandlers.Add(directoryHandler);
                directoryHandler.StartHandleDirectory();
            }

            m_Logger.Log("Image Server Created.", MessageTypeEnum.INFO);
        }

        public void Stop()
        {
            m_Logger.Log("Shutting down server..", MessageTypeEnum.INFO);
            int commandID = 1;
            string[] args = { };
            CommandRecieved.Invoke(this, new CommandRecievedEventArgs(commandID, args, ""));
        }
    }
}
