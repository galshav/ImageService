using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Threading;
using System.Drawing;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {
        private int m_ThumbnailSize = 0;
        private ILoggingService m_Logger = null;

        public ImageServiceModel(ILoggingService i_Logger)
        {
            m_Logger = i_Logger;
            bool result = false;
            int.TryParse(ImageService.ReadSetting("ThumbnailSize", out result), out m_ThumbnailSize);
            if (false == result)
            {
                // Set default value in case key is not listed in appConfig file.
                m_ThumbnailSize = 100;
            }
           
            m_Logger.Log($"Thumbnail size is: {m_ThumbnailSize}", MessageTypeEnum.INFO);
            m_Logger.Log("Image Service Model Created.", MessageTypeEnum.INFO);
        }

        public string AddFile(string path, out bool i_Result)
        {
            throw new NotImplementedException();
        }

        public string CreateDirectory(string i_Path, out bool i_Result)
        {
            try
            {
                m_Logger.Log($"Creating folder: {i_Path}", MessageTypeEnum.INFO);
                if (true == Directory.Exists(i_Path))
                {
                    m_Logger.Log($"{i_Path} already exists.", MessageTypeEnum.WARNING);
                    i_Result = true;
                    return "Directory already exist.";
                }

                DirectoryInfo directoryInfo = Directory.CreateDirectory(i_Path);
                i_Result = true;
                m_Logger.Log($"{directoryInfo.FullName} - created successfuly", MessageTypeEnum.INFO);
                return $"{directoryInfo.FullName} - created successfuly";
            }

            catch (Exception i_Error)
            {
                m_Logger.Log($"Failed to create folder: {i_Path}, Error: {i_Error.ToString()}.", MessageTypeEnum.FAIL);
                i_Result = false;
                return i_Error.ToString();
            }
        }

        public string MoveFile(string i_SourceFileName, string i_DestinationFolder, string fileName, out bool i_Result)
        {
            string destination = $@"{i_DestinationFolder}\{fileName}";
            int numberOfTries = 3;
            while (true)
            {
                try
                {
                    m_Logger.Log($"Moving file: {i_SourceFileName} @ to: {destination}", MessageTypeEnum.INFO);
                    if (false == File.Exists(i_SourceFileName))
                    {
                        m_Logger.Log($"Source file {i_SourceFileName} is not exists.", MessageTypeEnum.WARNING);
                        i_Result = false;
                        return "Source file is not exist.";
                    }

                    int i = 2;
                    while (true == File.Exists(destination))
                    {
                        destination = $@"{i_DestinationFolder}\{i.ToString()}-{fileName}";
                        m_Logger.Log($"Renaming file to {destination}", MessageTypeEnum.INFO);
                        i++;
                    }

                    File.Move(i_SourceFileName, destination);
                    if (true == File.Exists(i_SourceFileName))
                    {
                        m_Logger.Log($"Source file {i_SourceFileName} still exists.", MessageTypeEnum.FAIL);
                        i_Result = false;
                        return "Source file still exists.";
                    }

                    i_Result = true;
                    return $"File move completed from {i_SourceFileName} to {destination}";
                }

                catch (Exception i_Error)
                {
                    if (0 == numberOfTries)
                    {
                        m_Logger.Log($"Cannot move file {i_SourceFileName} to {destination}. Error: {i_Error.ToString()}", MessageTypeEnum.FAIL);
                        i_Result = false;
                        return i_Error.ToString();
                    }

                    numberOfTries--;
                    Thread.Sleep(5000);
                }
            }
        }

        public string CreateThumbnail(string i_ImagePath, string i_Destination, string fileName, out bool result)
        {
            string destination = $@"{i_Destination}\{fileName}";
            int numberOfTries = 3;
            while (true)
            {
                try
                {
                    using (Image image = Image.FromFile(i_ImagePath))
                    {
                        Image thumbnail = image.GetThumbnailImage(m_ThumbnailSize, m_ThumbnailSize, () => false, IntPtr.Zero);
                        int i = 2;
                        while (true == File.Exists(destination))
                        {
                            destination = $@"{i_Destination}\{i.ToString()}-{fileName}";
                            i++;
                        }

                        thumbnail.Save(destination);
                        result = true;
                        m_Logger.Log($"Thumbnail created @ {destination}", MessageTypeEnum.INFO);
                        return "Thumbnail successfully created";
                    }
                }

                catch (Exception i_Error)
                {
                    if (0 == numberOfTries)
                    {
                        result = false;
                        m_Logger.Log($"Cannot create thumbnail for {i_ImagePath}. error: {i_Error.ToString()}", MessageTypeEnum.FAIL);
                        return $"Cannot create thumnbail";
                    }

                    numberOfTries--;
                    Thread.Sleep(5000);
                }
            }
        }
    }
}