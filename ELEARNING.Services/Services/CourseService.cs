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

                string introductionCourseVideoID = string.Empty;
                string uploadCoverUri = string.Empty;

                if (request.introductionCourseVideo != null)
                {
                    (BinaryContent binaryContentIntroductionCourseVideo, int chunkSize) = Utility.ConvertIFromFileToBinary(request.introductionCourseVideo);
                    var uploadIntroductionCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentIntroductionCourseVideo, chunkSize, null);

                    var introductionCourseClipID = uploadIntroductionCourseVideoResponse.ClipId.Value;
                    introductionCourseVideoID = introductionCourseClipID.ToString();

                    if (request.coverImageVideo != null)
                    {
                        BinaryContent binaryContentCover = new BinaryContent(request.coverImageVideo.OpenReadStream(), request.coverImageVideo.ContentType);
                        var uploadCoverResponse = await _vimeoClient.UploadThumbnailAsync(introductionCourseClipID, binaryContentCover);
                        uploadCoverUri = uploadCoverResponse.Link;
                    }

                    VideoUpdateMetadata updateVideo = new VideoUpdateMetadata
                    {
                        Name = request.courseName
                    };
                    await _vimeoClient.UpdateVideoMetadataAsync(introductionCourseClipID, updateVideo);
                    await _vimeoClient.MoveVideoToFolder(folderID, introductionCourseClipID);
                }

                THCourse insertCourse = new THCourse
                {
                    ID = Guid.NewGuid(),
                    Course_Name = request.courseName,
                    Second_Course_Name = request.secondCourseName,
                    Course_Desc = request.description,
                    Level_ID = request.levelID,
                    Link_Cover_Course_Video_ID = uploadCoverUri,
                    Video_ID = introductionCourseVideoID,
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
                        string courseVideoID = string.Empty;
                        if (itemVideo.video != null)
                        {
                            (BinaryContent binaryContentVideo, int chunkSizeVideo) = Utility.ConvertIFromFileToBinary(itemVideo.video);
                            var uploadCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentVideo, chunkSizeVideo, null);

                            var clipID = uploadCourseVideoResponse.ClipId.Value;
                            VideoUpdateMetadata updateCourseVideo = new VideoUpdateMetadata
                            {
                                Name = itemVideo.videoName
                            };
                            await _vimeoClient.UpdateVideoMetadataAsync(clipID, updateCourseVideo);

                            await _vimeoClient.MoveVideoToFolder(folderID, clipID);

                            courseVideoID = clipID.ToString();
                        }

                        TDCourseVideo courseVideo = new TDCourseVideo
                        {
                            ID = Guid.NewGuid(),
                            Course_ID = insertCourse.ID,
                            Course_Section_ID = courseSection.ID,
                            Video_ID = courseVideoID,
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

        public async Task<GetCourseDetailResponse> GetCourseDetail(Guid courseID)
        {
            GetCourseDetailResponse response = new GetCourseDetailResponse();

            try
            {
                var getCourseByIDResponse = await _courseRepository.GetCourseByID(courseID);
                if (getCourseByIDResponse != null)
                {
                    var getCourseSectionResponse = await _courseRepository.GetCourseSection(courseID);

                    var videoIntroduction = await _vimeoClient.GetVideoAsync(long.Parse(getCourseByIDResponse.Video_ID));

                    response.data = new CourseDetailData
                    {
                        courseID = getCourseByIDResponse.ID,
                        courseName = getCourseByIDResponse.Course_Name,
                        courseDescription = getCourseByIDResponse.Course_Desc,
                        createBy = getCourseByIDResponse.Create_By,
                        linkCourseIntroductionVideo = videoIntroduction?.Player_Embed_Url,
                        price = getCourseByIDResponse.Price,
                        secondCourseName = getCourseByIDResponse.Second_Course_Name,
                        courseSection = getCourseSectionResponse.Select(c => new CourseSection
                        {
                            sectionNumber = c.Section_Number,
                            sectionName = c.Section_Name
                        }).ToList()
                    };
                    response.responseCode = "200";
                    response.responseMessage = "Success";
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

        public async Task<GetInstructorCourseDetailResponse> GetInstructorCourseDetail(GetMyCourseDetailRequest request)
        {
            GetInstructorCourseDetailResponse response = new GetInstructorCourseDetailResponse();

            try
            {
                var folder = await _vimeoClient.GetUserFolders(UserId.Me.Id, null, null);
                long folderID = 0;
                request.userID = "FOLDER TEST";

                if (!folder.Data.Exists(c => c.Name == request.userID))
                {
                    response.responseCode = "404";
                    response.responseMessage = "ไม่พบข้อมูล";
                    return response;
                }
                else
                {
                    folderID = folder.Data.FirstOrDefault(c => c.Name == request.userID).Id.Value;
                }

                var getCourseByIDResponse = await _courseRepository.GetCourseByID(request.courseID);
                if (getCourseByIDResponse != null)
                {
                    var getCourseSectionResponse = await _courseRepository.GetCourseSection(request.courseID);
                    var getCourseVideoResponse = await _courseRepository.GetCourseVideo(request.courseID);

                    var allVideoFromFolder = await _vimeoClient.GetAllVideosFromFolderAsync(folderID, UserId.Me.Id);

                    var videoIntroduction = allVideoFromFolder?.Data.FirstOrDefault(c => c.Id.ToString() == getCourseByIDResponse.Video_ID);
                    response.data = new InstructorCourseDetailData
                    {
                        courseID = getCourseByIDResponse.ID,
                        courseName = getCourseByIDResponse.Course_Name,
                        courseDescription = getCourseByIDResponse.Course_Desc,
                        createBy = getCourseByIDResponse.Create_By,
                        linkCourseIntroductionVideo = videoIntroduction?.Player_Embed_Url,
                        linkCoverCourseVideo = videoIntroduction?.Pictures?.Link,
                        secondCourseName = getCourseByIDResponse.Second_Course_Name,
                        price = getCourseByIDResponse.Price,
                        courseSection = getCourseSectionResponse.Select(c => new InstructorCourseSection
                        {
                            courseSectionID = c.ID,
                            sectionNumber = c.Section_Number,
                            sectionName = c.Section_Name,
                            courseVideo = getCourseVideoResponse.Where(v => v.Course_Section_ID == c.ID).Select(d => new InstructorCourseVideo
                            {
                                courseVideoID = d.ID,
                                videoName = d.Video_Name,
                                linkCourseVideo = allVideoFromFolder?.Data.FirstOrDefault(c => c.Id.ToString() == d.Video_ID)?.Player_Embed_Url,
                            }).ToList()
                        }).ToList()
                    };
                    response.responseCode = "200";
                    response.responseMessage = "Success";
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

        public async Task<GetMyCourseDetailResponse> GetMyCourseDetail(GetMyCourseDetailRequest request)
        {
            GetMyCourseDetailResponse response = new GetMyCourseDetailResponse();

            try
            {
                var folder = await _vimeoClient.GetUserFolders(UserId.Me.Id, null, null);
                long folderID = 0;
                request.userID = "FOLDER TEST";

                if (!folder.Data.Exists(c => c.Name == request.userID))
                {
                    response.responseCode = "404";
                    response.responseMessage = "ไม่พบข้อมูล";
                    return response;
                }
                else
                {
                    folderID = folder.Data.FirstOrDefault(c => c.Name == request.userID).Id.Value;
                }

                var getCourseByIDResponse = await _courseRepository.GetCourseByID(request.courseID);
                if (getCourseByIDResponse != null)
                {
                    var getCourseSectionResponse = await _courseRepository.GetCourseSection(request.courseID);
                    var getCourseVideoResponse = await _courseRepository.GetCourseVideo(request.courseID);

                    var allVideoFromFolder = await _vimeoClient.GetAllVideosFromFolderAsync(folderID, UserId.Me.Id);

                    response.data = new MyCourseDetailData
                    {
                        courseID = getCourseByIDResponse.ID,
                        courseName = getCourseByIDResponse.Course_Name,
                        courseDescription = getCourseByIDResponse.Course_Desc,
                        createBy = getCourseByIDResponse.Create_By,
                        linkCourseIntroductionVideo = allVideoFromFolder?.Data.FirstOrDefault(c => c.Id.ToString() == getCourseByIDResponse.Video_ID)?.Player_Embed_Url,
                        secondCourseName = getCourseByIDResponse.Second_Course_Name,
                        courseSection = getCourseSectionResponse.Select(c => new MyCourseSection
                        {
                            courseSectionID = c.ID,
                            sectionNumber = c.Section_Number,
                            sectionName = c.Section_Name,
                            courseVideo = getCourseVideoResponse.Where(v => v.Course_Section_ID == c.ID).Select(d => new MyCourseVideo
                            {
                                videoName = d.Video_Name,
                                linkCourseVideo = allVideoFromFolder?.Data.FirstOrDefault(c => c.Id.ToString() == d.Video_ID)?.Player_Embed_Url,
                            }).ToList()
                        }).ToList()
                    };
                    response.responseCode = "200";
                    response.responseMessage = "Success";
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