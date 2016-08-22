using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class FolderController : ApiController
    {
        [HttpGet]
        public IEnumerable<DriveInfo> GetDrives()
        {
            List<DriveInfo> HDDList = new List<DriveInfo>();
            CheckHDListForReadiness(ref HDDList);           
          
            return HDDList;
        }

        [HttpPost]
        [Route("api/Folder/DownloadFile")]
        public HttpResponseMessage DownloadFile([FromBody] string filePath)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                if (!string.IsNullOrEmpty(filePath) &(System.IO.File.Exists(filePath)))
                {
                  using (MemoryStream stream = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];

                            file.Read(bytes, 0, (int)file.Length);
                            stream.Write(bytes, 0, (int)file.Length);

                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", filePath);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = filePath;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                        }
                    }
                }
                else
                {
                    httpResponseMessage.StatusCode = HttpStatusCode.NotFound;
                    return httpResponseMessage;
                }
            }
            catch (Exception ex)

            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }


        [HttpPost]
        public FolderData Info([FromBody] string FolderPath)
        {
            FolderData data= new FolderData(FolderPath);
            return data;
        }


        private static void CheckHDListForReadiness(ref List<DriveInfo> HardDrivesList)
        {
            if (HardDrivesList != null)
            {
                HardDrivesList = DriveInfo.GetDrives().ToList();
                try
                {
                    foreach (DriveInfo di in HardDrivesList)
                    {
                        if ((di.IsReady == false) | (di.DriveType != DriveType.Fixed))
                        {
                            HardDrivesList.Remove(di);
                        }
                    }
                }

                catch (Exception ex)
                {
                }
            }
            else
            {
                Console.WriteLine("CheckHDListForReadiness: Null reference for HardDrivesList parameter was received!");
            }
        }

    }
}
