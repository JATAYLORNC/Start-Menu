﻿using System;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string execute)
        {

            if (ModelState.IsValid)
            {
                String queryTitle = execute;

                Program executeProgram =
                    (from Program in db.Programs
                    where Program.Title == queryTitle
                    select Program).SingleOrDefault();

                String pathName = executeProgram.Path;

                Process executeFile = new Process();

                executeFile.StartInfo.FileName = pathName;
                executeFile.Start();

                executeProgram.Count++;

                try
                {
                    db.Entry(executeProgram).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }
            programList = (from Program in db.Programs select Program).ToList();
            return View(programList);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string searchString)
        {

            var programs = (from Program in db.Programs
                            select Program);

            if (!String.IsNullOrEmpty(searchString))
            {
               programs = programs.Where(s => s.Title.Contains(searchString));
            }

            return View("Index", programs);
        }

        // GET: Programs
        public ActionResult Index()
        {
            try
            {
                Program programQuery = (from Program in db.Programs select Program).FirstOrDefault();

                if (programQuery == null)
                {
                    String searchDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

                    List<String> progFiles = new List<String>(Directory.EnumerateFiles(searchDir, "*.*", SearchOption.AllDirectories));

                    foreach (String filePath in progFiles)
                    {

                        if (ModelState.IsValid)
                        {
                            String fileName = Path.GetFileNameWithoutExtension(filePath);

                            if(fileName == "desktop" || fileName == "Desktop" || filePath.Contains("Accessories"))
                            {
                                continue;
                            }

                            Program program = new Program();

                            program = new Program()
                            {
                                Title = fileName,
                                Path = filePath,
                                IconPath = "/Images/noun_Lnk_903502_000000.png", 
                                Count = 0
                            };

                            programList.Add(program);
                            db.Programs.Add(program);
                            db.SaveChanges();                       
                        }
                    }
                    return View(programList);
                }
                else
                {
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

        public ActionResult Settings()
        {
            List<Program> iconData =
                   (from Program in db.Programs
                    select Program).ToList();

            return View(iconData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings (HttpPostedFileBase[] uploadFile, string[] progName)
        {

            if (ModelState.IsValid)
            {
                int count = 0;

                foreach (HttpPostedFileBase file in uploadFile)
                {

                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            string path = Path.Combine(Server.MapPath("/Images/"),
                                                       Path.GetFileName(file.FileName));
                            file.SaveAs(path);

                            String name = progName[count];

                            Program updateProgram =
                                (from Program in db.Programs
                                 where Program.Title == name
                                 select Program).SingleOrDefault();

                            updateProgram.IconPath = path;

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

            List<Program> iconData =
                   (from Program in db.Programs
                    select Program).ToList();

            return View(iconData);

        }

        // GET: Programs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }

        // GET: Programs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Path")] Program program)
        {
            if (ModelState.IsValid)
            {
                db.Programs.Add(program);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(program);
        }

        // GET: Programs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }

        // POST: Programs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Path")] Program program)
        {
            if (ModelState.IsValid)
            {
                db.Entry(program).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(program);
        }

        // GET: Programs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }

        // POST: Programs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Program program = db.Programs.Find(id);
            db.Programs.Remove(program);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
