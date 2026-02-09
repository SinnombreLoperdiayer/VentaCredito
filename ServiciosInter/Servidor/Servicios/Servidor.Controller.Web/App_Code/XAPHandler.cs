using System;
using System.Globalization;
using System.IO;
using System.Web;

namespace CO.Servidor.Controller.Web
{
  public class XAPHandler : IHttpHandler
  {
    public XAPHandler()
    {
    }

    public void ProcessRequest(HttpContext context)
    {
      HttpRequest Request = context.Request;
      string filePath = Request.FilePath;
      filePath = filePath.Remove(filePath.Length - 1, 1);

      DateTime fechaxap;
      string fechaParam = context.Request.Params["FechaXap"];

      string physicalPath = context.Request.PhysicalPath;
      physicalPath = physicalPath.Remove(physicalPath.Length - 1, 1);

      if (File.Exists(physicalPath))
      {
        DateTime lastWriteTime = File.GetLastWriteTime(physicalPath);
        DateTime.TryParse(fechaParam, new CultureInfo("en-US"), DateTimeStyles.None, out fechaxap);

        if (lastWriteTime > fechaxap)
        {
          // Prevent caching of the response
          context.Response.Expires = 1;
          context.Response.ContentType = "application/octet-stream";
          context.Response.WriteFile(filePath);
        }
      }
    }

    public bool IsReusable
    {
      // To enable pooling, return true here.
      // This keeps the handler in memory.
      get { return false; }
    }
  }
}