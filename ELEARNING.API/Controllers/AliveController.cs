using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ELEARNING.API.Models;
using VimeoDotNet.Models;
using System.Net;
using VimeoDotNet;
using VimeoDotNet.Net;
using System.Text.Json;
using VimeoDotNet.Parameters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ELEARNING.API.Controllers
{
    [ApiController]
    public class AliveController : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Common" }, Description = "Check alive api for monitoring")]
        [Route("alive")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AliveResponse))]
        public ActionResult Get()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            var _result = new AliveResponse()
            {
                alive = true,
                version = fileVersionInfo.ProductVersion,
                env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            };

            return Ok(_result);
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Common" }, Description = "Check alive api for monitoring")]
        [Route("uploadvideo")]
        public async Task<IActionResult> NewRequestFromForm([FromForm] IFormFile videoFile)
        {
            string tagName = "tagName";

            //var files = Request.Form.Files;
            //IFormFile file = files[0];
            string uploadStatus = "";
            var getVideo = new Video();
            try
            {
                if (videoFile != null)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    VimeoClient vimeoClient = new VimeoClient("5ca9fa0c26420da73ed5e2b20add71b7");
                    var authcheck = await vimeoClient.GetAccountInformationAsync();

                    long AlbumID = 0;
                    var allAlbum = await vimeoClient.GetAlbumsAsync(UserId.Me.Id);

                    if (allAlbum.Data.Exists(c => c.Name == "TEST ALBUM"))
                    {
                        //
                        AlbumID = allAlbum.Data.FirstOrDefault(c => c.Name == "TEST ALBUM").GetAlbumId().Value;
                        var albumVideo = await vimeoClient.GetAlbumVideosAsync(UserId.Me.Id, AlbumID);
                        if (albumVideo.Data.Any())
                        {
                            await vimeoClient.DeleteVideoAsync(long.Parse(albumVideo.Data.First().Id.Value.ToString()));
                           // await vimeoClient.RemoveFromAlbumAsync(UserId.Me.Id, AlbumID, long.Parse(albumVideo.Data.First().Id.Value.ToString()));
                        }
                    }
                    // EditAlbumParameters parameterAl = new EditAlbumParameters
                    // {
                    //     Name = "TEST ALBUM"
                    // };
                    // var createAlbum = await vimeoClient.CreateAlbumAsync(UserId.Me.Id, parameterAl);
                    // Create Folder
                    // var createFolderRes = await vimeoClient.CreateFolder(UserId.Me.Id,"TESTUSER");
                    // var jsonString = JsonSerializer.Serialize(createFolderRes);

                    // Get all video
                    var all = await vimeoClient.GetVideosAsync(UserId.Me.Id, 1, 10);
                    if (authcheck.Name != null)
                    {
                        IUploadRequest uploadRequest = new UploadRequest();
                        //Stream stream = file.OpenReadStream();
                        //using(var memoryStream = new MemoryStream())
                        //{
                        //    stream.CopyTo(memoryStream);
                        //    memoryStream.ToArray();
                        //}
                        BinaryContent binaryContent = new BinaryContent(videoFile.OpenReadStream(), videoFile.ContentType);
                        int chunkSize = 0;
                        int contentLength = Convert.ToInt32(videoFile.Length);
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
                        var checkChunk = chunkSize;
                        var status = "uploading";
                        // uploadRequest = await vimeoClient.UploadEntireFileAsync(binaryContent, chunkSize, null);

                        // var _tag = tagName;
                        // var tagVideo = await vimeoClient.AddVideoTagAsync(uploadRequest.ClipId.GetValueOrDefault(), _tag);

                        // await vimeoClient.AddToAlbumAsync(UserId.Me.Id,createAlbum.GetAlbumId().GetValueOrDefault(), long.Parse(uploadRequest.ClipId.Value.ToString()));
                        // while (status != "available")
                        // {
                        //     getVideo = await vimeoClient.GetVideoAsync(long.Parse(uploadRequest.ClipId.Value.ToString()));
                        //     status = getVideo.Status;
                        // }
                        // uploadStatus = String.Concat("file Uploaded ", getVideo.Files[0].LinkSecure);
                    }
                }
                return Ok(new { status = uploadStatus, video = getVideo });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}