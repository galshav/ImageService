using ImageService.Model.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed
        void StartHandleDirectory();         
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);     // The Event that will be activated upon new Command
    }
}
