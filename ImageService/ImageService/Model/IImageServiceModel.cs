using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Model
{
    public interface IImageServiceModel
    {
        /// <summary>
        /// The Function Addes A file to the system
        /// </summary>
        /// <param name="i_Path">The Path of the Image from the file</param>
        /// <returns>Indication if the Addition Was Successful</returns>
        string AddFile(string i_Path, out bool i_Result);

        /// <summary>
        /// The function create a directory in the system.
        /// </summary>
        /// <param name="i_Path">The path of the directory</param>
        /// <returns>Indication if directory creation succeed</returns>
        string CreateDirectory(string  i_Pat, out bool i_resulth);

        /// <summary>
        /// The function move file from source to destination.
        /// </summary>
        /// <param name="i_SourcefileName">The path of the source file</param>
        /// <param name="i_DestinationFileName">The path of the destination file</param>
        /// <returns>Indication move file operation succeed</returns>
        string MoveFile(string i_SourceFileName, string i_DestinationFileName, string fileName, out bool i_result);

        /// <summary>
        /// The function create thumbnail to given image.
        /// </summary>
        /// <param name="i_ImagePath">The path of the source file image</param>
        /// <param name="i_DestinationFolder">The path of the destination folder</param>
        /// <returns>Indication move file operation succeed</returns>
        string CreateThumbnail(string i_ImagePath, string i_DestinationFolder, string fileName, out bool i_Result);
    }
}
