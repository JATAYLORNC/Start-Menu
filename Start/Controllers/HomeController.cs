using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Start.Controllers
{

    public class HomeController : Controller
    {
        private Dictionary<String, String> pathList = new Dictionary<String, String>();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string execute) {
            if(ModelState.IsValid)
            { 
                Process executeFile = new Process();

                executeFile.StartInfo.FileName = pathList[execute].ToString();
                executeFile.Start();
            }

            return View();

        }

        public ActionResult Index()
        {
            try
            {
                String searchDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

                List<String> progFiles = new List<String>(Directory.EnumerateFiles(searchDir, "*.*", SearchOption.AllDirectories));

                List<String> fileNameList = new List<String>();

                /*progHashtable = new Hashtable();*/

                foreach (String filePath in progFiles)
                {
                    fileNameList.Add(Path.GetFileNameWithoutExtension(filePath));
                    if (!pathList.ContainsKey(Path.GetFileNameWithoutExtension(filePath)))
                    {
                        pathList.Add(Path.GetFileNameWithoutExtension(filePath), filePath);
                    }
                    /*progHashtable.Add(Path.GetFileNameWithoutExtension(filePath), filePath);*/
                }

                ViewData["Executables"] = progFiles;
                ViewData["FileNames"] = fileNameList;
                ViewData["PathList"] = pathList;
                /*ViewData["HT"] = progHashtable;*/

                return View();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
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