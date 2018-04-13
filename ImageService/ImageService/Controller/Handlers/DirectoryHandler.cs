using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Model.Event;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.IO;
using ImageService.Server;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private ImageServer m_Owner = null;
        private IImageController m_Controller = null;
        private ILoggingService m_Logger = null;
        private System.IO.FileSystemWatcher m_DirWatcher = null;
        private string m_Path = "";
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(string i_Path, IImageController i_Controller, ImageServer i_Owner ,ILoggingService i_Logger)
        {
            m_Logger = i_Logger;
            m_Owner = i_Owner;
            m_Owner.CommandRecieved += this.OnCommandRecieved;
            m_Controller = i_Controller;
            m_Path = i_Path;
            m_DirWatcher = new FileSystemWatcher(m_Path);
            m_DirWatcher.Created += new FileSystemEventHandler(this.FileCreatedEvent);
            m_Logger.Log($"Directory handler created for: {m_Path}", MessageTypeEnum.INFO);
        }

        private void FileCreatedEvent(object sender, System.IO.FileSystemEventArgs e)
        {
            m_Logger.Log($"File created name: {e.Name} @ {m_Path}", MessageTypeEnum.INFO);
            string[] extensions = { ".jpg", ".png", ".gif", ".bmp"};
            var ext = (Path.GetExtension(e.FullPath) ?? string.Empty).ToLower();
            if (extensions.Any(ext.Equals))
            {
                m_Logger.Log($"Image file extention detectet: {ext}, \nProcessing..", MessageTypeEnum.INFO);
                bool commandResult = false;
                string[] args = { e.FullPath, e.Name };
                string resultInfo = m_Controller.ExecuteCommand(1, args, out commandResult);
                if (false == commandResult)
                {
                    m_Logger.Log($"Command #1 failed with error: {resultInfo}", MessageTypeEnum.FAIL);
                }

                else
                {
                    m_Logger.Log($"Command 1 success - info: {resultInfo}", MessageTypeEnum.INFO);
                }
            }

            else
            {
                m_Logger.Log($"Not watching {ext} files.", MessageTypeEnum.INFO);
            }
        }

        public void StartHandleDirectory()
        {
            m_DirWatcher.EnableRaisingEvents = true;
            m_Logger.Log($"Starting listen to directory: {m_Path}", MessageTypeEnum.INFO);
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            // Check if command is the stop handle.
            if (1 == e.CommandID)
            {
                m_Logger.Log($"Disposing watcher @ {m_Path}", MessageTypeEnum.INFO);
                m_DirWatcher?.Dispose();
            }
        }
    }
}
