using System;
using System.Linq;
using System.Threading.Tasks;
using ELEARNING.Repositories.Entities;
using ELEARNING.Repositories.Interfaces;
using ELEARNING.Services.Helpers;
using ELEARNING.Services.Interfaces;
using ELEARNING.Services.Models.Request;
using ELEARNING.Services.Models.Response;
using VimeoDotNet;
using VimeoDotNet.Models;
using VimeoDotNet.Net;
using VimeoDotNet.Parameters;

namespace ELEARNING.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly VimeoClient _vimeoClient;
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
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

                (BinaryContent binaryContentIntroductionCourseVideo, int chunkSize) = Utility.ConvertIFromFileToBinary(request.introductionCourseVideo);
                var uploadIntroductionCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentIntroductionCourseVideo, chunkSize, null);

                var introductionCourseVideoID = uploadIntroductionCourseVideoResponse.ClipId.Value;
                BinaryContent binaryContentCover = new BinaryContent(request.coverImageVideo.OpenReadStream(), request.coverImageVideo.ContentType);
                var uploadCoverResponse = await _vimeoClient.UploadThumbnailAsync(introductionCourseVideoID, binaryContentCover);

                VideoUpdateMetadata updateVideo = new VideoUpdateMetadata
                {
                    Name = request.courseName
                };
                await _vimeoClient.UpdateVideoMetadataAsync(introductionCourseVideoID, updateVideo);

                await _vimeoClient.MoveVideoToFolder(folderID, introductionCourseVideoID);

                THCourse insertCourse = new THCourse
                {
                    ID = Guid.NewGuid(),
                    Course_Name = request.courseName,
                    Second_Course_Name = request.secondCourseName,
                    Course_Desc = request.description,
                    Level_ID = request.levelID,
                    Link_Cover_Course_Video_ID = uploadCoverResponse.Uri,
                    Video_ID = introductionCourseVideoID.ToString(),
                    Price = request.price,
                    Remark = request.remark,
                    Create_By = "ADMIN",
                    Create_Date = DateTime.Now
                };

                var insertCourseResponse = await _courseRepository.InsertTHCourse(insertCourse);

                foreach (var itemSection in request.sectionVideo)
                {
                    TDCourseSection courseSection = new TDCourseSection
                    {
                        ID = Guid.NewGuid(),
                        Course_ID = insertCourse.ID,
                        Section_Name = itemSection.sectionName,
                        Section_Number = itemSection.sectionNumber,
                        Create_By = "ADMIN",
                        Create_Date = DateTime.Now
                    };

                    var insertCourseSectionResponse = await _courseRepository.InsertTDCourseSection(courseSection);
                    foreach (var itemVideo in itemSection.videoList)
                    {
                        (BinaryContent binaryContentVideo, int chunkSizeVideo) = Utility.ConvertIFromFileToBinary(itemVideo.video);
                        var uploadCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentVideo, chunkSizeVideo, null);

                        var courseVideoID = uploadCourseVideoResponse.ClipId.Value;
                        VideoUpdateMetadata updateCourseVideo = new VideoUpdateMetadata
                        {
                            Name = itemVideo.videoName
                        };
                        await _vimeoClient.UpdateVideoMetadataAsync(courseVideoID, updateCourseVideo);

                        await _vimeoClient.MoveVideoToFolder(folderID, courseVideoID);

                        TDCourseVideo courseVideo = new TDCourseVideo
                        {
                            ID = Guid.NewGuid(),
                            Course_ID = insertCourse.ID,
                            Course_Section_ID = courseSection.ID,
                            Video_ID = courseVideoID.ToString(),
                            Video_Name = itemVideo.videoName,
                            Create_By = "ADMIN",
                            Create_Date = DateTime.Now
                        };

                        var insertVideoCourse = await _courseRepository.InsertTDCourseVideo(courseVideo);
                    }

                }

                response.data = new CreateCourseResult
                {
                    courseID = insertCourse.ID
                };
                response.responseCode = "200";
                response.responseMessage = "Success";
            }
            catch (Exception ex)
            {
                response.responseCode = "501";
                response.responseMessage = ex.Message;
            }
            return response;
        }

        public async Task<GetAllCourseResponse> GetAllCourse(GetAllCourseRequest request)
        {
            GetAllCourseResponse response = new GetAllCourseResponse();

            try
            {
                var getAllCourseResponse = await _courseRepository.GetCourse();
                if (getAllCourseResponse.Any())
                {
                    getAllCourseResponse = getAllCourseResponse.Skip((request.pageNo - 1) * request.pageSize)
                                            .Take(request.pageSize).ToList();

                    if (getAllCourseResponse.Any())
                    {
                        var allVideo = await _vimeoClient.GetVideosAsync(UserId.Me.Id, null, null);
                        if (allVideo != null && allVideo.Data.Any())
                        {
                            var videoData = allVideo.Data;
                            response.data = getAllCourseResponse.Select(c => new CourseData
                            {
                                courseID = c.ID,
                                courseName = c.Course_Name,
                                createBy = c.Create_By,
                                price = c.Price,
                                linkCoverCourseVideo = videoData.FirstOrDefault(x => x.Id.ToString() == c.Video_ID)?.Pictures?.Link
                            }).ToList();
                            response.responseCode = "200";
                            response.responseMessage = "Success";
                        }
                        else
                        {
                            response.responseCode = "404";
                            response.responseMessage = "ไม่พบข้อมูล";
                        }
                    }
                    else
                    {
                        response.responseCode = "404";
                        response.responseMessage = "ไม่พบข้อมูล";
                    }

                }
                else
                {
                    response.responseCode = "404";
                    response.responseMessage = "ไม่พบข้อมูล";
                }
            }
            catch (Exception ex)
            {
                response.responseCode = "501";
                response.responseMessage = ex.Message;
            }

            return response;
        }
    }
}