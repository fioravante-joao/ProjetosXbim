using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ExemploEventfulBuildingWebUi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public FileResult RetornoArquivoIFC(string file)
        {
            var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"C:\Users\JVFS\source\repos\ProjetosXbimEstudos\ExemploEventfulBuildingWebUi\ifcFiles\", file + ".wexBIM");
            var fileStream = System.IO.File.OpenRead(path);

            return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}