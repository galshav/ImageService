using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using ImageService.Server;
using ImageService.Controller;
using System.IO;
using System.Configuration;
using System.Speech.Synthesis;

namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        #region members
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private ImageServer m_ImageServer = null;
        private IImageController m_Controller = null;
        private ILoggingService m_Logger = null;
        private IImageServiceModel m_Model = null;
        private string m_SourceName = "";
        private string m_LogName = "";
        #endregion

        public ImageService()
        {
            InitializeComponent();
            // Create the windows event log component.
            bool result = false;
            m_SourceName = ImageService.ReadSetting("SourceName", out result);
            if (false == result)
            {
                m_SourceName = "ImageServiceSource";
            }

            m_LogName = ImageService.ReadSetting("LogName", out result);
            if (false == result)
            {
                m_LogName = "ImageServiceLog";
            }

            m_eventLog = new System.Diagnostics.EventLog();
            if (false == System.Diagnostics.EventLog.SourceExists(m_SourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(m_SourceName, m_LogName);
            }

            m_eventLog.Source = m_SourceName;
            m_eventLog.Log = m_LogName;

            // Create logger wrapper.
            m_Logger = new LoggingService();
            m_Logger.MessageRecieved += WriteToEventLog;
        }

        public static string ReadSetting(string i_Key, out bool result)
        {
            try
            {
                result = true;
                var appSettings = ConfigurationManager.AppSettings;
                string value = appSettings[i_Key] ?? "Not Found";
                if (value.Equals("Not Found"))
                {
                    result = false;
                }

                return value;
            }

            catch (ConfigurationErrorsException i_error)
            {
                result = false;
                return i_error.ToString();
            }
        }

        public void WriteToEventLog(object sender, MessageRecievedEventArgs args)
        {
            m_eventLog.WriteEntry($"{args.Status}: {args.Message}");
        }

        protected override void OnStart(string[] args)
        {
            m_Logger.Log("Starting service.", MessageTypeEnum.INFO);
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Creating service components.
            m_Logger.Log("Creating service components.", MessageTypeEnum.INFO);
            m_Model = new ImageServiceModel(m_Logger);
            m_Controller = new ImageController(m_Model, m_Logger);
            m_ImageServer = new ImageServer(m_Controller ,m_Logger);

            // Create output and thumbnails directories.
            bool result = false;
            string outputPath = ImageService.ReadSetting("OutputDir", out result);
            if (false == result)
            {
                m_Logger.Log($"OutputDir attribute cannot be found in appConfig.\nSetting default value.", MessageTypeEnum.INFO);
                outputPath = @"C:\Output";
            }

            this.CreateOutputFolder(outputPath);
            string thumbnailsPath = $@"{outputPath}\Thumbnails";
            this.CreateOutputFolder(thumbnailsPath);
        }

        private void CreateOutputFolder(string i_Path)
        {
            bool result = false;
            string resultInfo = m_Model.CreateDirectory(i_Path, out result);
            if (false == result)
            {
                m_Logger.Log($"Error creating outputfolder @ {i_Path}, Erro Info: {resultInfo}", MessageTypeEnum.FAIL);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(i_Path);
            directoryInfo.Attributes |= FileAttributes.Hidden;
        }

        protected override void OnStop()
        {
            m_ImageServer.Stop();
            m_Logger.Log("Service stopped.", MessageTypeEnum.INFO);
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            m_Logger.Log("Service continue.", MessageTypeEnum.INFO);
        }

        protected override void OnPause()
        {
            base.OnPause();
            m_Logger.Log("Service paused.", MessageTypeEnum.INFO);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            m_Logger.Log("Service shutting down.", MessageTypeEnum.INFO);
        }
    }
}
