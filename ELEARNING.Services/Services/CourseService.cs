using System;
using System.Linq;
using System.Threading.Tasks;
using ELEARNING.Services.Helpers;
using ELEARNING.Services.Interfaces;
using ELEARNING.Services.Models.Request;
using VimeoDotNet;
using VimeoDotNet.Models;
using VimeoDotNet.Net;
using VimeoDotNet.Parameters;

namespace ELEARNING.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly VimeoClient _vimeoClient;
        public CourseService()
        {
            _vimeoClient = new VimeoClient("5ca9fa0c26420da73ed5e2b20add71b7");
        }

        public async Task<CreateCourseResponse> CreateCourse(CreateCourseRequest request)
        {
            CreateCourseResponse response = new CreateCourseResponse();

            try
            {
                var folder = await _vimeoClient.GetUserFolders(UserId.Me.Id, null, null);
                long folderID = 0;
                if (!folder.Data.Exists(c => c.Name == "FOLDER TEST"))
                {
                    var createFolderRes = await _vimeoClient.CreateFolder(UserId.Me.Id, "FOLDER TEST");
                    folderID = createFolderRes.Id.Value;
                }
                else
                {
                    folderID = folder.Data.FirstOrDefault(c => c.Name == "FOLDER TEST").Id.Value;
                }

                (BinaryContent binaryContent, int chunkSize) = Utility.ConvertIFromFileToBinary(request.coverImageVideo);

                var uploadRequest = await _vimeoClient.UploadEntireFileAsync(binaryContent, chunkSize, null);
            }
            catch (Exception ex)
            {

            }
            throw new System.NotImplementedException();
        }

    }
}