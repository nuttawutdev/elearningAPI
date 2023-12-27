using System;
using Microsoft.AspNetCore.Http;
using VimeoDotNet.Net;

namespace ELEARNING.Services.Helpers
{
    public static class Utility
    {
        public static (BinaryContent, int) ConvertIFromFileToBinary(IFormFile file)
        {
            if (file == null)
            {
                return (null, 0);
            }
            
            try
            {
                BinaryContent binaryContent = new BinaryContent(file.OpenReadStream(), file.ContentType);
                int chunkSize = 0;
                int contentLength = Convert.ToInt32(file.Length);
                int temp1 = contentLength / 1024;
                binaryContent.OriginalFileName = "Test Name";
                if (temp1 > 1)
                {
                    chunkSize = temp1 / 1024;
                    if (chunkSize == 0)
                    {
                        chunkSize = 1048576;
                    }
                    else
                    {
                        if (chunkSize > 10)
                        {
                            chunkSize = chunkSize / 10;
                        }
                        chunkSize = chunkSize * 1048576;
                    }
                }
                else
                {
                    chunkSize = 1048576;
                }

                return (binaryContent, chunkSize);
            }
            catch
            {
                return (null, 0);
            }
        }
    }
}