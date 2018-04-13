using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel m_Model = null;
        private ILoggingService m_Logger = null;

        public NewFileCommand(IImageServiceModel i_Model, ILoggingService i_Logger)
        {
            m_Logger = i_Logger;
            m_Model = i_Model;
        }

        public string Execute(string[] args, out bool result)
        {
            string filePath = args[0];
            string fileName = args[1];
            bool readConfigResult = false;
            string outputPath = ImageService.ReadSetting("OutputDir", out readConfigResult);
            m_Logger.Log($"Executing new file command for {filePath}", MessageTypeEnum.INFO);
            // Get image information.
            DateTime date = this.GetImageTakenDate(filePath);
            m_Logger.Log($"File @ {filePath} created @ {date.Day}/{date.Month}/{date.Year}", MessageTypeEnum.INFO);

            // Create sub-folders.
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string createDirectorySuccess = m_Model.CreateDirectory($@"{outputPath}\{year}\{month}", out result);
            if (false == result)
            {
                return createDirectorySuccess;
            }

            createDirectorySuccess = m_Model.CreateDirectory($@"{outputPath}\Thumbnails\{year}\{month}", out result);
            if (false == result)
            {
                return createDirectorySuccess;
            }

            // Create thumbnail.
            string thumnailCreateSuccess = m_Model.CreateThumbnail(filePath, $@"{outputPath}\Thumbnails\{year}\{month}", fileName, out result);
            if (false == result)
            {
                return thumnailCreateSuccess;
            }

            // Moving the file.
            return m_Model.MoveFile(filePath, $@"{outputPath}\{year}\{month}", fileName, out result);
        }

        private DateTime GetImageTakenDate(string i_Path)
        {
            int numberOfTries = 5;
            while (true)
            {
                try
                {
                    Regex r = new Regex(":");
                    using (FileStream fs = new FileStream(i_Path, FileMode.Open, FileAccess.Read))
                    using (Image myImage = Image.FromStream(fs, false, false))
                    {
                        PropertyItem propItem = myImage.GetPropertyItem(36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        return DateTime.Parse(dateTaken);
                    }
                }

                catch (Exception e)
                {
                    if (0 == numberOfTries)
                    {
                        m_Logger.Log($"Error: {e.ToString()}\nSetting default date", MessageTypeEnum.FAIL);
                        return new DateTime(2018, 1, 1);
                    }

                    numberOfTries--;
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
