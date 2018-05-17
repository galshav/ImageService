using ImageService.Commands;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using ImageService.Logging.Model;
using ImageService.Structures;
using Newtonsoft.Json;

namespace ImageService.Commands
{
    class GetSettingsCommand : ICommand
    {
        private IImageServiceModel m_Model = null;
        private ILoggingService m_Logger = null;

        public GetSettingsCommand(IImageServiceModel i_Model, ILoggingService i_Logger)
        {
            m_Model = i_Model;
            m_Logger = i_Logger;
        }

        public string Execute(string[] args, out bool result)
        {
            m_Logger.Log("Getting settings from App.config file", MessageTypeEnum.INFO);
            bool succcess = false;
            result = true;

            string outputDir = ImageService.ReadSetting("OutputDir", out succcess);
            if (false == succcess)
            {
                m_Logger.Log("OutputDir not found", MessageTypeEnum.FAIL);
                result = false;
                return outputDir;
            }

            string sourceName = ImageService.ReadSetting("SourceName", out succcess);
            if (false == succcess)
            {
                m_Logger.Log("SourceName not found", MessageTypeEnum.FAIL);
                result = false;
                return sourceName;
            }

            string logName = ImageService.ReadSetting("LogName", out succcess);
            if (false == succcess)
            {
                m_Logger.Log("LogName not found", MessageTypeEnum.FAIL);
                result = false;
                return logName;
            }

            string thumbnailSize = ImageService.ReadSetting("ThumbnailSize", out succcess);
            if (false == succcess)
            {
                m_Logger.Log("ThumbnailSize not found", MessageTypeEnum.FAIL);
                result = false;
                return thumbnailSize;
            }

            string directories = ImageService.ReadSetting("Handler", out succcess);
            if (false == succcess)
            {
                m_Logger.Log("Handler not found", MessageTypeEnum.FAIL);
                result = false;
                return directories;
            }

            Settings settings = new Settings();
            settings.OutputDir = outputDir;
            settings.SourceName = sourceName;
            settings.LogName = logName;
            settings.ThumbnailSize = thumbnailSize;
            settings.Directories = directories;

            return JsonConvert.SerializeObject(settings);
        }
    }
}
