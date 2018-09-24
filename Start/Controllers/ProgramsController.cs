using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Start.Models;
using Shell32;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Start.Controllers
{
    public class ProgramsController : Controller
    {
        private StartContext db = new StartContext();
        private List<Program> programList = new List<Program>();


        /*Method to execute desktop program when icon is clicked from web app*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string execute)
        {

            if (ModelState.IsValid)
            {
                String queryTitle = execute;

                /*Use program title passed from web app input field to query database for program path*/
                Program executeProgram =
                    (from Program in db.Programs
                    where Program.Title == queryTitle
                    select Program).SingleOrDefault();

                String pathName = executeProgram.Path;

                /*Use System.Diagnostics "Process" class/methods to launch program*/
                Process executeFile = new Process();

                executeFile.StartInfo.FileName = pathName;
                executeFile.Start();

                /*Update program run count*/
                executeProgram.Count++;

                /*Save program run count to database*/
                try
                {
                    db.Entry(executeProgram).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            /*Query database for updated program records and return query data to the Index view*/
            programList = (from Program in db.Programs select Program).ToList();
            return View(programList);

        }

        /*Method to search for program Title based on searchString passed from view*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string searchString)
        {
            /*Query database for programs*/
            var programs = (from Program in db.Programs
                            select Program);

            /*Add Where clause to select only programs containing the searchString*/
            if (!String.IsNullOrEmpty(searchString))
            {
               programs = programs.Where(s => s.Title.Contains(searchString));
            }

            /*Update the Index view to display only Programs returned from query*/
            return View("Index", programs);
        }

        /* Method to display the Index view*/
        // GET: Programs
        public ActionResult Index()
        {
            try
            {
                /*Check to see if the database already contains program records*/
                Program programQuery = (from Program in db.Programs select Program).FirstOrDefault();

                /*Extract programs from local Start Menu if there are no records in the database*/
                if (programQuery == null)
                {
                    /*Use System.Environments methods to get the full path of the specialFolder "Common Start Menu*/
                    String searchDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

                    /*Use System.IO EnumerateFiles method to get full path of programs in Common Start Menu
                     Folder and all sub-directories.  Save into String Type List*/
                    List<String> progFiles = new List<String>(Directory.EnumerateFiles(searchDir, "*.*", SearchOption.AllDirectories));

                    /*Loop through each path String in the list*/
                    foreach (String filePath in progFiles)
                    {

                        if (ModelState.IsValid)
                        {
                            /*Parse the path to get the name of the file without the extension*/
                            String fileName = Path.GetFileNameWithoutExtension(filePath);

                            /*Skip problematic file names and paths*/
                            if(fileName == "desktop" || fileName == "Desktop" || filePath.Contains("Accessories"))
                            {
                                continue;
                            }
                            
                            /*Create a new Program object based on the Program Class Model and construct it 
                             with program title and path data.  Initialize the icon to a default image*/
                            Program program = new Program()
                            {
                                Title = fileName,
                                Path = filePath,
                                IconPath = "/Images/noun_Lnk_903502_000000.png", 
                                Count = 0
                            };

                            /*Add the Program object to a List and also save the Program to the database*/
                            programList.Add(program);
                            db.Programs.Add(program);
                            db.SaveChanges();                       
                        }
                    }

                    /*Return the complete list of Start Menu programs to the Index view*/
                    return View(programList);
                }
                else
                {
                    /*if the database does contain records, query the database for all records and return 
                     query data to the Index view*/
                    programList = (from Program in db.Programs select Program).ToList();
                    return View(programList);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*Method to create the "Settings" view*/
        public ActionResult Settings()
        {
            /*Query the database for all records*/
            List<Program> iconData =
                   (from Program in db.Programs
                    select Program).ToList();

            /*return query data to the Settings view*/
            return View(iconData);
        }

        /*Method for posting form data containing customized icons from the Settings view.
         Logic used here is awkward.  It would have been better to employ a new Model class
         containing only the data needed to update records with the customized Icons*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings (HttpPostedFileBase[] uploadFile, string[] progName)
        {

            if (ModelState.IsValid)
            {
                /*Initialize a counter to 0*/
                int count = 0;

                /*Loop through each image file uploaded with the icon form*/
                foreach (HttpPostedFileBase file in uploadFile)
                {

                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            /*Define the full path name where the file will be stored*/
                            string path = Path.Combine(Server.MapPath("/Images/"),
                                                       Path.GetFileName(file.FileName));

                            /*Save the file to the applications Images folder*/
                            file.SaveAs(path);

                            /*Use the loop counter to access the program Title from the form input field*/
                            String name = progName[count];

                            /*Query database to get records specific to the Title*/
                            Program updateProgram =
                                (from Program in db.Programs
                                 where Program.Title == name
                                 select Program).SingleOrDefault();

                            /*Change Program record for IconPath to the custom icon name*/
                            updateProgram.IconPath = path;

                            /*Save updated record to the database*/
                            try
                            {
                                db.Entry(updateProgram).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                // Provide for exceptions.
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    count++;
                }
            }

            /*Query the database for updated records*/
            List<Program> iconData =
                   (from Program in db.Programs
                    select Program).ToList();

            /*Return the query data to the Settings view*/
            return View(iconData);

        }
    }
}
