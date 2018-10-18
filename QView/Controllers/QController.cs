using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QView.Models;
using QView.ViewModels;
using QView.App_Data;
using System.IO;
using System.Configuration;

namespace QView.Controllers
{
    public class QController : Controller
    {
        // GET: Q
        public ActionResult Index()
        {
            //return View();
            //return Content("Using the Q Controller");
            return View("QFormView");
        }

        [HttpPost]
        public ActionResult Find(QRecord qFind)
        {
            string artServer = ConfigurationManager.AppSettings["artServer"];

            string filePath = @"\\" + artServer + @"\prodart\";
            string path = "";

            var viewModel = new QListViewModel();
            var dc = new ScheduleDBDataContext();

            string sql = "SELECT * FROM que WHERE ";

            if (qFind.Id != 0)
            {
                sql += "ID = " + qFind.Id.ToString();
            }

            if (qFind.Company != null)
            {
                if (!sql.EndsWith("WHERE "))
                {
                    sql += " AND ";
                }
                sql += "COMPANY LIKE '%" + qFind.Company + "%'";
            }

            if (qFind.Description != null)
            {
                if (!sql.EndsWith("WHERE "))
                {
                    sql += " AND ";
                }
                sql += "DESCRIPTION LIKE '%" + qFind.Description + "%'";
            }

            if (qFind.Type != null)
            {
                if (!sql.EndsWith("WHERE "))
                {
                    sql += " AND ";
                }
                sql += "TYPE LIKE '%" + qFind.Type + "%'";
            }

            sql += " AND (LastScheduled >= CAST(DATEPART(yy, GETDATE()) - 2 AS varchar) + '/07/01')";


            List<que> QList = dc.ExecuteQuery<que>(sql).ToList();

            if (QList.Count() == 0)
            {
                //return HttpNotFound();
                return Content("No records found");
            }

            foreach (que q in QList)
            {
                QRecord QR = new QRecord
                {
                    Id = q.ID,
                    Type = q.Type,
                    Company = q.Company,
                    Description = q.Description,
                    Created = q.created.ToShortDateString(),
                    LastScheduled = "",
                    PreviousScheduled = ""
                };
                if (q.LastScheduled.HasValue)
                {
                    QR.LastScheduled = DateTime.Parse(q.LastScheduled.ToString()).ToShortDateString();
                }
                if (q.PreviousScheduled.HasValue)
                {
                    QR.PreviousScheduled = DateTime.Parse(q.PreviousScheduled.ToString()).ToShortDateString();
                }

                string fileName1 = "Q" + q.ID.ToString() + ".pdf";
                string fileName2 = "Q" + q.ID.ToString() + ".gif";

                if (DirSearch(filePath, fileName1, out path))
                {
                    QR.FilePath = path + @"\" + fileName1;
                }
                else if (DirSearch(filePath, fileName2, out path))
                {
                    QR.FilePath = path + @"\" + fileName2;
                }

                viewModel.QRecords.Add(QR);
            }

            return View("QListView", viewModel);
        }

        private bool DirSearch(string sDir, string fileName, out string path)
        {
            path = sDir;
            try
            {
                foreach (string f in Directory.GetFiles(sDir, fileName))
                {
                    path = sDir.Substring(0, sDir.Length - 1); // take off the last slash in this case
                    return true;
                }
                return DirSearchSubs(sDir, fileName, out path);
            }
            catch (System.Exception excpt)
            {
                //Console.WriteLine(excpt.Message);
            }
            return false;
        }
        private bool DirSearchSubs(string sDir, string fileName, out string path)
        {
            path = sDir;
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, fileName))
                    {
                        path = d;
                        return true;
                    }
                    DirSearchSubs(d, fileName, out path);
                }
            }
            catch (System.Exception excpt)
            {
                //Console.WriteLine(excpt.Message);
            }
            return false;
        }

    }
}