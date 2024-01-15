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

        public async Task<SaveCourseResponse> SaveCourse(SaveCourseRequest request)
        {
            SaveCourseResponse response = new SaveCourseResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.courseID))
                {
                    response = await CreateCourse(request);
                }
                else
                {
                    response = await UpdateCourse(request);
                }
            }
            catch (Exception ex)
            {
                response.responseCode = "501";
                response.responseMessage = ex.Message;
            }
            return response;
        }

        private async Task<SaveCourseResponse> CreateCourse(SaveCourseRequest request)
        {
            SaveCourseResponse response = new SaveCourseResponse();

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
                        uploadCoverUri = uploadCoverResponse.Uri;
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
                    Level_ID = 0,
                    Link_Cover_Course_Video_ID = uploadCoverUri,
                    Video_ID = introductionCourseVideoID,
                    Price = request.price,
                    Remark = request.remark,
                    Create_By = "ADMIN",
                    Create_Date = DateTime.Now
                };

                var insertCourseResponse = await _courseRepository.InsertTHCourse(insertCourse);

                int sectionNumber = 1;
                foreach (var itemSection in request.sectionVideo)
                {
                    TDCourseSection courseSection = new TDCourseSection
                    {
                        ID = Guid.NewGuid(),
                        Course_ID = insertCourse.ID,
                        Section_Name = itemSection.sectionName,
                        Section_Number = sectionNumber,
                        Create_By = "ADMIN",
                        Create_Date = DateTime.Now
                    };

                    var insertCourseSectionResponse = await _courseRepository.InsertTDCourseSection(courseSection);

                    sectionNumber++;

                    if (itemSection.videoList != null)
                    {
                        int videoNumber = 1;
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
                                Video_Number = videoNumber,
                                Create_By = "ADMIN",
                                Create_Date = DateTime.Now
                            };

                            var insertVideoCourse = await _courseRepository.InsertTDCourseVideo(courseVideo);
                            videoNumber++;
                        }
                    }

                }

                response.data = new SaveCourseResult
                {
                    courseID = insertCourse.ID.ToString()
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

        private async Task<SaveCourseResponse> UpdateCourse(SaveCourseRequest request)
        {
            SaveCourseResponse response = new SaveCourseResponse();

            try
            {
                var courseDetail = await _courseRepository.GetCourseByID(new Guid(request.courseID));

                if (courseDetail == null)
                {
                    response.responseCode = "404";
                    response.responseMessage = "ไม่พบข้อมูล";
                    return response;
                }

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
                long.TryParse(courseDetail.Video_ID, out long introductionCourseClipID);

                #region Update Introduction Course
                if (request.introductionCourseVideo != null)
                {
                    if (introductionCourseClipID != 0)
                    {
                        await _vimeoClient.DeleteVideoAsync(introductionCourseClipID);
                    }

                    (BinaryContent binaryContentIntroductionCourseVideo, int chunkSize) = Utility.ConvertIFromFileToBinary(request.introductionCourseVideo);
                    var uploadIntroductionCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentIntroductionCourseVideo, chunkSize, null);

                    introductionCourseClipID = uploadIntroductionCourseVideoResponse.ClipId.Value;
                    introductionCourseVideoID = introductionCourseClipID.ToString();

                    VideoUpdateMetadata updateVideo = new VideoUpdateMetadata
                    {
                        Name = request.courseName
                    };
                    await _vimeoClient.UpdateVideoMetadataAsync(introductionCourseClipID, updateVideo);
                    await _vimeoClient.MoveVideoToFolder(folderID, introductionCourseClipID);
                }
                else
                {
                    introductionCourseVideoID = courseDetail.Video_ID;

                    if (!string.IsNullOrWhiteSpace(introductionCourseVideoID))
                    {
                        VideoUpdateMetadata updateVideo = new VideoUpdateMetadata
                        {
                            Name = request.courseName
                        };
                        await _vimeoClient.UpdateVideoMetadataAsync(introductionCourseClipID, updateVideo);
                    }

                }
                #endregion

                #region Update Cover Image
                if (request.coverImageVideo != null && introductionCourseClipID != 0)
                {
                    BinaryContent binaryContentCover = new BinaryContent(request.coverImageVideo.OpenReadStream(), request.coverImageVideo.ContentType);
                    var uploadCoverResponse = await _vimeoClient.UploadThumbnailAsync(introductionCourseClipID, binaryContentCover);
                    uploadCoverUri = uploadCoverResponse.Uri;
                }
                else
                {
                    uploadCoverUri = courseDetail.Link_Cover_Course_Video_ID;
                }
                #endregion

                THCourse updateCourse = new THCourse
                {
                    ID = new Guid(request.courseID),
                    Course_Name = request.courseName,
                    Second_Course_Name = request.secondCourseName,
                    Course_Desc = request.description,
                    Level_ID = 0,
                    Link_Cover_Course_Video_ID = uploadCoverUri,
                    Video_ID = introductionCourseVideoID,
                    Price = request.price,
                    Remark = request.remark,
                    Update_By = "ADMIN",
                    Update_Date = DateTime.Now
                };

                var updateCourseResponse = await _courseRepository.UpdateTHCourse(updateCourse);

                var currentCourseSection = await _courseRepository.GetCourseSection(new Guid(request.courseID));
                var videoCourse = await _courseRepository.GetCourseVideo(new Guid(request.courseID));

                var sectionIDUpdate = request.sectionVideo.Where(d => !string.IsNullOrWhiteSpace(d.courseSectionID)).Select(c => c.courseSectionID.ToLower()).ToList();
                var sectionDelete = currentCourseSection.Where(d => !sectionIDUpdate.Contains(d.ID.ToString()));
                //var sectionDelete = request.sectionVideo.Where(c => c.action == "delete");
                var sectionAdd = request.sectionVideo.Where(c => c.action == "add");
                var sectionEdit = request.sectionVideo.Where(c => c.action == "edit" || string.IsNullOrWhiteSpace(c.action));


                #region Insert New Section
                var lastedSection = currentCourseSection.OrderByDescending(d => d.Section_Number).FirstOrDefault();
                int sectionNumber = lastedSection.Section_Number + 1;
                foreach (var itemSection in sectionAdd)
                {
                    TDCourseSection insertCourseSection = new TDCourseSection
                    {
                        ID = Guid.NewGuid(),
                        Course_ID = new Guid(request.courseID),
                        Section_Name = itemSection.sectionName,
                        Section_Number = sectionNumber,
                        Create_By = "ADMIN",
                        Create_Date = DateTime.Now
                    };

                    var insertCourseSectionResponse = await _courseRepository.InsertTDCourseSection(insertCourseSection);
                    sectionNumber++;

                    if (itemSection.videoList != null)
                    {
                        int videoNumber = 1;
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
                                Course_ID = new Guid(request.courseID),
                                Course_Section_ID = insertCourseSection.ID,
                                Video_ID = courseVideoID,
                                Video_Name = itemVideo.videoName,
                                Video_Number = videoNumber,
                                Create_By = "ADMIN",
                                Create_Date = DateTime.Now
                            };

                            var insertVideoCourse = await _courseRepository.InsertTDCourseVideo(courseVideo);
                            videoNumber++;
                        }
                    }

                }
                #endregion

                #region Delete Section
                foreach (var itemSection in sectionDelete)
                {
                    var videoInSection = videoCourse.Where(s => s.Course_Section_ID == itemSection.ID);
                    foreach (var itemVideoSection in videoInSection)
                    {
                        if (!string.IsNullOrWhiteSpace(itemVideoSection.Video_ID))
                        {
                            await _vimeoClient.DeleteVideoAsync(long.Parse(itemVideoSection.Video_ID));
                        }

                        var deleteVideoResponse = await _courseRepository.DeleteCourseVideo(itemVideoSection.ID);
                    }

                    var deleteSection = await _courseRepository.DeleteCourseSection(itemSection.ID);
                }
                #endregion

                #region Update Section
                foreach (var itemSection in sectionEdit)
                {
                    var videoCourseInSection = videoCourse.Where(d => d.Course_Section_ID == new Guid(itemSection.courseSectionID));
                    var lastedVideoInSection = videoCourseInSection.OrderByDescending(d => d.Video_Number).FirstOrDefault();
                    int videoNumber = 1;
                    if (lastedVideoInSection != null)
                    {
                        videoNumber = lastedVideoInSection.Video_Number + 1;
                    }

                    if (itemSection.videoList == null)
                    {
                        itemSection.videoList = new System.Collections.Generic.List<CourseVideoRequest>();
                    }

                    var videoIDUpdate = itemSection.videoList.Where(d => !string.IsNullOrWhiteSpace(d.courseVideoID)).Select(c => c.courseVideoID.ToLower()).ToList();
                    var videoDelete = videoCourseInSection.Where(d => !videoIDUpdate.Contains(d.ID.ToString()));

                    // var videoDelete = itemSection.videoList.Where(c => c.action == "delete");
                    var videoAdd = itemSection.videoList.Where(c => c.action == "add");
                    var videoEdit = itemSection.videoList.Where(c => c.action == "edit" || string.IsNullOrWhiteSpace(c.action));


                    foreach (var itemVideoAdd in videoAdd)
                    {
                        string courseVideoID = string.Empty;
                        if (itemVideoAdd.video != null)
                        {
                            (BinaryContent binaryContentVideo, int chunkSizeVideo) = Utility.ConvertIFromFileToBinary(itemVideoAdd.video);
                            var uploadCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentVideo, chunkSizeVideo, null);

                            var clipID = uploadCourseVideoResponse.ClipId.Value;
                            VideoUpdateMetadata updateCourseVideo = new VideoUpdateMetadata
                            {
                                Name = itemVideoAdd.videoName
                            };
                            await _vimeoClient.UpdateVideoMetadataAsync(clipID, updateCourseVideo);

                            await _vimeoClient.MoveVideoToFolder(folderID, clipID);

                            courseVideoID = clipID.ToString();
                        }

                        TDCourseVideo courseVideo = new TDCourseVideo
                        {
                            ID = Guid.NewGuid(),
                            Course_ID = new Guid(request.courseID),
                            Course_Section_ID = new Guid(itemSection.courseSectionID),
                            Video_ID = courseVideoID,
                            Video_Name = itemVideoAdd.videoName,
                            Video_Number = videoNumber,
                            Create_By = "ADMIN",
                            Create_Date = DateTime.Now
                        };

                        var insertVideoCourse = await _courseRepository.InsertTDCourseVideo(courseVideo);
                        videoNumber++;
                    }

                    foreach (var itemVideoDelete in videoDelete)
                    {
                        var videoData = videoCourseInSection.FirstOrDefault(s => s.ID == itemVideoDelete.ID);

                        if (videoData != null)
                        {
                            if (!string.IsNullOrWhiteSpace(videoData.Video_ID))
                            {
                                await _vimeoClient.DeleteVideoAsync(long.Parse(videoData.Video_ID));
                            }
                            var deleteVideoResponse = await _courseRepository.DeleteCourseVideo(videoData.ID);
                        }
                    }

                    foreach (var itemVideoEdit in videoEdit)
                    {
                        string courseVideoID = string.Empty;
                        var videoData = videoCourseInSection.FirstOrDefault(s => s.ID.ToString() == itemVideoEdit.courseVideoID.ToLower());

                        // Update Video
                        if (itemVideoEdit.video != null)
                        {
                            // Delete old video
                            if (string.IsNullOrWhiteSpace(videoData.Video_ID))
                            {
                                await _vimeoClient.DeleteVideoAsync(long.Parse(videoData.Video_ID));
                            }

                            // Upload new video
                            (BinaryContent binaryContentVideo, int chunkSizeVideo) = Utility.ConvertIFromFileToBinary(itemVideoEdit.video);
                            var uploadCourseVideoResponse = await _vimeoClient.UploadEntireFileAsync(binaryContentVideo, chunkSizeVideo, null);

                            var clipID = uploadCourseVideoResponse.ClipId.Value;
                            VideoUpdateMetadata updateCourseVideo = new VideoUpdateMetadata
                            {
                                Name = itemVideoEdit.videoName
                            };
                            await _vimeoClient.UpdateVideoMetadataAsync(clipID, updateCourseVideo);

                            await _vimeoClient.MoveVideoToFolder(folderID, clipID);

                            courseVideoID = clipID.ToString();
                        }
                        else
                        {
                            // Update เฉพาะชื่อ Video
                            long.TryParse(videoData.Video_ID, out long courseClipID);
                            if (courseClipID != 0)
                            {
                                VideoUpdateMetadata updateVideo = new VideoUpdateMetadata
                                {
                                    Name = itemVideoEdit.videoName
                                };
                                await _vimeoClient.UpdateVideoMetadataAsync(courseClipID, updateVideo);
                            }
                            courseVideoID = videoData.Video_ID;
                        }

                        TDCourseVideo courseVideo = new TDCourseVideo
                        {
                            ID = videoData.ID,
                            Course_ID = videoData.Course_ID,
                            Course_Section_ID = videoData.Course_Section_ID,
                            Video_ID = courseVideoID,
                            Video_Name = itemVideoEdit.videoName,
                            Video_Number = videoData.Video_Number,
                            Update_By = "ADMIN",
                            Update_Date = DateTime.Now
                        };

                        var updateVideoCourse = await _courseRepository.UpdateTDCourseVideo(courseVideo);
                    }

                    var sectionData = currentCourseSection.FirstOrDefault(c => c.ID.ToString() == itemSection.courseSectionID.ToLower());
                    TDCourseSection updateCourseSection = new TDCourseSection
                    {
                        ID = sectionData.ID,
                        Course_ID = sectionData.Course_ID,
                        Section_Name = itemSection.sectionName,
                        Section_Number = sectionData.Section_Number,
                        Update_By = "ADMIN",
                        Update_Date = DateTime.Now
                    };

                    var updateCourseSectionResponse = await _courseRepository.UpdateTDCourseSection(updateCourseSection);

                }
                #endregion

                response.data = new SaveCourseResult
                {
                    courseID = request.courseID
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
                                courseID = c.ID.ToString(),
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
                        courseID = getCourseByIDResponse.ID.ToString(),
                        courseName = getCourseByIDResponse.Course_Name,
                        courseDescription = getCourseByIDResponse.Course_Desc,
                        createBy = getCourseByIDResponse.Create_By,
                        createDate = getCourseByIDResponse.Create_Date,
                        linkCourseIntroductionVideo = videoIntroduction?.Player_Embed_Url,
                        price = getCourseByIDResponse.Price,
                        secondCourseName = getCourseByIDResponse.Second_Course_Name,
                        remark = getCourseByIDResponse.Remark,
                        courseSection = getCourseSectionResponse.OrderBy(d => d.Section_Number).Select(c => new CourseSection
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

                var getCourseByIDResponse = await _courseRepository.GetCourseByID(new Guid(request.courseID));
                if (getCourseByIDResponse != null)
                {
                    var getCourseSectionResponse = await _courseRepository.GetCourseSection(new Guid(request.courseID));
                    var getCourseVideoResponse = await _courseRepository.GetCourseVideo(new Guid(request.courseID));

                    var allVideoFromFolder = await _vimeoClient.GetAllVideosFromFolderAsync(folderID, UserId.Me.Id);

                    var videoIntroduction = allVideoFromFolder?.Data.FirstOrDefault(c => c.Id.ToString() == getCourseByIDResponse.Video_ID);
                    response.data = new InstructorCourseDetailData
                    {
                        courseID = getCourseByIDResponse.ID.ToString(),
                        courseName = getCourseByIDResponse.Course_Name,
                        courseDescription = getCourseByIDResponse.Course_Desc,
                        createBy = getCourseByIDResponse.Create_By,
                        linkCourseIntroductionVideo = videoIntroduction?.Player_Embed_Url,
                        linkCoverCourseVideo = videoIntroduction?.Pictures?.Link,
                        secondCourseName = getCourseByIDResponse.Second_Course_Name,
                        price = getCourseByIDResponse.Price,
                        remark = getCourseByIDResponse.Remark,
                        courseSection = getCourseSectionResponse.OrderBy(d => d.Section_Number).Select(c => new InstructorCourseSection
                        {
                            courseSectionID = c.ID.ToString(),
                            sectionNumber = c.Section_Number,
                            sectionName = c.Section_Name,
                            courseVideo = getCourseVideoResponse.Where(v => v.Course_Section_ID == c.ID).OrderBy(x => x.Video_Number).Select(d => new InstructorCourseVideo
                            {
                                courseVideoID = d.ID.ToString(),
                                videoName = d.Video_Name,
                                videoNumber = d.Video_Number,
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

                var getCourseByIDResponse = await _courseRepository.GetCourseByID(new Guid(request.courseID));
                if (getCourseByIDResponse != null)
                {
                    var getCourseSectionResponse = await _courseRepository.GetCourseSection(new Guid(request.courseID));
                    var getCourseVideoResponse = await _courseRepository.GetCourseVideo(new Guid(request.courseID));

                    var allVideoFromFolder = await _vimeoClient.GetAllVideosFromFolderAsync(folderID, UserId.Me.Id);

                    response.data = new MyCourseDetailData
                    {
                        courseID = getCourseByIDResponse.ID.ToString(),
                        courseName = getCourseByIDResponse.Course_Name,
                        courseDescription = getCourseByIDResponse.Course_Desc,
                        createBy = getCourseByIDResponse.Create_By,
                        createDate = getCourseByIDResponse.Create_Date,
                        linkCourseIntroductionVideo = allVideoFromFolder?.Data.FirstOrDefault(c => c.Id.ToString() == getCourseByIDResponse.Video_ID)?.Player_Embed_Url,
                        secondCourseName = getCourseByIDResponse.Second_Course_Name,
                        remark = getCourseByIDResponse.Remark,
                        courseSection = getCourseSectionResponse.OrderBy(d => d.Section_Number).Select(c => new MyCourseSection
                        {
                            courseSectionID = c.ID.ToString(),
                            sectionNumber = c.Section_Number,
                            sectionName = c.Section_Name,
                            courseVideo = getCourseVideoResponse.Where(v => v.Course_Section_ID == c.ID).OrderBy(x => x.Video_Number).Select(d => new MyCourseVideo
                            {
                                videoName = d.Video_Name,
                                videoNumber = d.Video_Number,
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

        public async Task<GetAllCourseInstructorResponse> GetAllCourseInstructor(GetAllCourseInstructorRequest request)
        {
            GetAllCourseInstructorResponse response = new GetAllCourseInstructorResponse();

            try
            {
                request.userID = "ADMIN";
                var allCourseInstructor = await _courseRepository.GetAllCourseInstructor(request.userID);

                if (allCourseInstructor.Any())
                {
                    if (request.period != null && request.period.ToLower() != "all")
                    {
                        int periodValue = int.Parse(request.period);
                        DateTime dateNow = DateTime.Now.Date;
                        var dateFrom = dateNow.AddDays(-periodValue);
                        var dateTo = dateNow.AddDays(1).AddMilliseconds(-100);

                        allCourseInstructor = allCourseInstructor.Where(d => d.Create_Date >= dateFrom && d.Create_Date <= dateTo).ToList();
                    }

                    response.data = allCourseInstructor.Select(c => new CourseInstructorData
                    {
                        courseID = c.ID.ToString(),
                        courseName = c.Course_Name,
                        createBy = c.Create_By,
                        price = c.Price,
                        status = "Active",
                        createDate = c.Create_Date.ToString("dd/MM/yyyy")
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
            catch (Exception ex)
            {
                response.responseCode = "501";
                response.responseMessage = ex.Message;
            }

            return response;
        }

        public async Task<DeleteCourseResponse> DeleteCourse(DeleteCourseRequest request)
        {
            DeleteCourseResponse response = new DeleteCourseResponse();

            try
            {
                var courseDetail = await _courseRepository.GetCourseByID(new Guid(request.courseID));

                if (courseDetail == null)
                {
                    response.responseCode = "404";
                    response.responseMessage = "ไม่พบข้อมูล";
                    return response;
                }

                var sectionDelete = await _courseRepository.GetCourseSection(new Guid(request.courseID));
                var videoCourse = await _courseRepository.GetCourseVideo(new Guid(request.courseID));


                #region Delete Section
                foreach (var itemSection in sectionDelete)
                {
                    var videoInSection = videoCourse.Where(s => s.Course_Section_ID == itemSection.ID);
                    foreach (var itemVideoSection in videoInSection)
                    {
                        if (!string.IsNullOrWhiteSpace(itemVideoSection.Video_ID))
                        {
                            await _vimeoClient.DeleteVideoAsync(long.Parse(itemVideoSection.Video_ID));
                        }

                        var deleteVideoResponse = await _courseRepository.DeleteCourseVideo(itemVideoSection.ID);
                    }

                    var deleteSection = await _courseRepository.DeleteCourseSection(itemSection.ID);
                }
                #endregion

                if (!string.IsNullOrWhiteSpace(courseDetail.Video_ID))
                {
                    await _vimeoClient.DeleteVideoAsync(long.Parse(courseDetail.Video_ID));
                }
                
                var deleteCourseResponse = await _courseRepository.DeleteCourse(courseDetail.ID);

                response.data = true;
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

        public async Task<GetAllCourseResponse> GetAllMyCourse(GetAllMyCourseRequest request)
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
                                courseID = c.ID.ToString(),
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