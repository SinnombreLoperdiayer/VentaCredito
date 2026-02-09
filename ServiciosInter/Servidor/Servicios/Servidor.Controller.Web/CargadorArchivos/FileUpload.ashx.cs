using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Framework.Servidor.Archivo;

namespace FilesLoader.Web
{
  /// <summary>
  /// Summary description for FileUpload
  /// </summary>
  public class FileUpload : IHttpHandler
  {
    private HttpContext ctx;

    public void ProcessRequest(HttpContext context)
    {
      ctx = context;
      string uploadPath = context.Server.MapPath("~/Upload");
      string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];
      FileUploadProcess fileUpload = FileUploadProcess.Instancia;
      fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
      fileUpload.ProcessRequest(context, uploadPath, filePath);
    }

    private void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
    {
      string id = ctx.Request.QueryString["id"];
    }

    public bool IsReusable
    {
      get
      {
        return false;
      }
    }
  }
}