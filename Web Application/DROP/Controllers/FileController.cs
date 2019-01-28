using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DROP.Models;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DROP.Controllers
{
    public class FileController : Controller
    {
        //Upload Page View
        public ActionResult Upload()
        {
            string filepath = Server.MapPath("~/Uploads");

            DirectoryInfo di = new DirectoryInfo(filepath);
            foreach (FileInfo file in di.GetFiles())
            {
                //Delete Previous Files in Upload Folder to ensure empty folder
                //Avoid file duplication if unexpected redirection occurs
                file.Delete();
            }

            ViewBag.Title = "Upload Files";
            return View();
        }

        //Upload File Post-Process
        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> upload)
        {
            string filepath1 = Server.MapPath("~/Uploads");

            DirectoryInfo di = new DirectoryInfo(filepath1);
            foreach (FileInfo file in di.GetFiles())
            {
                //Delete all files in Upload Folder
                file.Delete();
            }

            ViewBag.Title = "Upload File";

            foreach (var file in upload)
            {
                if (ModelState.IsValid)
                {
                    //Check if selected file is not empty 
                    if (file != null && file.ContentLength > 0)
                    {
                        //Check if selected file is excel
                        if (file.FileName.EndsWith(".xls") || file.FileName.EndsWith(".xlsx"))
                        {
                            //Save file in Upload Folder
                            string filename = Path.GetFileName(file.FileName);
                            string filepath = Path.Combine(Server.MapPath("~/Uploads"), filename);
                            file.SaveAs(filepath);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "File/s not supported");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload A File");
                        return View();
                    }
                }
            }

            return RedirectToAction("APlan", "File");
        }

        //Assessment Plan Page View
        public ActionResult APlan()
        {
            ViewBag.Title = "Generate Plan";
            return View();
        }

        //Assessment Plan Post-Process
        //Actual generation of excel file 
        [HttpPost]
        public ActionResult APost(FileViewModels model)
        {
            projectEntities db = new projectEntities();
            process pro = new process();

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook1;
            Excel.Worksheet xlWorkSheet1;
            Excel.Range range;

            int str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();

            if (xlApp == null)
            {
                TempData["flag"] = 1;
                TempData["MessageTitle"] = "Generate Plan";
                TempData["MessagePrompt"] = "Error occured, please check Excel Software";
                return RedirectToAction("MainPage", "Home");
            }

            int qtr_ref = model.quarter;
            int sy_ref = model.year;

            string[] so = ListSO();
            string[] course = ListCourse();
            string[] pi = ListPI();
            string[] asstool = ListAT();
            float[] studtarget = ListTarget();

            int studpass = 0;
            int studtot = 0;
            int studpassall = 0;
            int studtotall = 0;
            double studpassave;
            double studpassallave = 0;
            int ctr;
            int qtr = 1;
            int sy = 1;
            int qtr1 = 1;
            int qtr2 = 1;
            int qtr3 = 1;
            int qtr4 = 1;
            int sy1 = 1516;
            int sy2 = 1516;
            int sy3 = 1516;
            int sy4 = 1516;

            List<int> studpass1 = new List<int>();
            List<int> studtot1 = new List<int>();
            List<int> studpassall1 = new List<int>();
            List<int> studtotall1 = new List<int>();
            List<double> studpassave1 = new List<double>();
            List<double> studpassallave1 = new List<double>();
            List<double> result = new List<double>();

            pro.quarter = model.quarter;
            pro.year = model.year;
            pro.copiatt_id = 1;
            db.processes.Add(pro);
            db.SaveChanges();

            //////////////////////////////////////////////////////////////////////////////////////////
            //Generate File Location

            if (Session["accID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int accid = (int)Session["accID"];
            string filepath = Server.MapPath("~/Uploads");
            string filepath1 = Server.MapPath("~/UserFiles/" + accid + "/AssessmentPlan_" + accid + "_" + pro.pid + ".xlsx");
            
            for (int i = 0; i < 24; i++)
            {
                ctr = 0;
                do
                {
                    studpass = 0;
                    studtot = 0;
                    if (ctr == 0)
                    {
                        qtr = qtr_ref;
                        sy = sy_ref;
                        qtr1 = qtr;
                        sy1 = sy;
                    }
                    else if (ctr == 1)
                    {
                        qtr2 = qtr1;
                        sy2 = sy1;
                        if (qtr1 == 4)
                        {
                            qtr2 = 1;
                            qtr = qtr2;
                            sy2 += 101;
                            sy = sy2;
                        }
                        else
                        {
                            qtr2++;
                            qtr = qtr2;
                        }
                    }
                    else if (ctr == 2)
                    {
                        qtr3 = qtr2;
                        sy3 = sy2;
                        if (qtr3 == 4)
                        {
                            qtr3 = 1;
                            qtr = qtr3;
                            sy3 += 101;
                            sy = sy3;
                        }
                        else
                        {
                            qtr3++;
                            qtr = qtr3;
                        }
                    }
                    else if (ctr == 3)
                    {
                        qtr4 = qtr3;
                        sy4 = sy3;
                        if (qtr4 == 4)
                        {
                            qtr4 = 1;
                            qtr = qtr4;
                            sy4 += 101;
                            sy = sy4;
                        }
                        else
                        {
                            qtr4++;
                            qtr = qtr4;
                        }
                    }

                    List<int> grades = new List<int>();
                    string[] files = Directory.GetFiles(filepath, qtr + "QSY" + sy + "_" + course[i] + "_*.xlsx");
                    foreach (string file in files)
                    {
                        xlWorkBook1 = xlApp.Workbooks.Open(file, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        xlWorkSheet1 = (Excel.Worksheet)xlWorkBook1.Worksheets.get_Item(1);
                        range = xlWorkSheet1.UsedRange;
                        rw = range.Rows.Count;
                        cl = range.Columns.Count;

                        for (cCnt = 1; cCnt <= cl; cCnt++)
                        {
                            if (xlWorkSheet1.Cells[1, cCnt].Value2 != null)
                            {
                                string columnName = xlWorkSheet1.Cells[1, cCnt].Value2;
                                if (Regex.IsMatch(columnName, asstool[i], RegexOptions.IgnoreCase))
                                {
                                    str = 0;
                                    for (rCnt = 2; rCnt <= rw; rCnt++)
                                    {
                                        str = (int)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                                        grades.Add(str);
                                    }
                                    break;
                                }
                            }
                        }

                        xlWorkBook1.Close(true, null, null);
                        Marshal.ReleaseComObject(xlWorkSheet1);
                        Marshal.ReleaseComObject(xlWorkBook1);
                    }

                    int a = 0;
                    foreach (float val in grades)
                    {
                        if (val >= 70)
                            studpass++;
                        studtot++;
                        a++;
                    }
                    studpass1.Add(studpass);
                    studtot1.Add(studtot);
                    studpassall += studpass;
                    studtotall += studtot;
                    studpassave = ((double)studpass / (double)studtot) * 100;
                    studpassave1.Add(studpassave);
                    if (ctr == 3)
                    {
                        studpassall1.Add(studpassall);
                        studtotall1.Add(studtotall);
                        studpassallave = ((double)studpassall / (double)studtotall) * 100;
                        studpassallave1.Add(studpassallave);
                        result.Add(studpassallave);
                        studpassall = 0;
                        studtotall = 0;
                    }
                    ctr++;
                } while (ctr < 4);
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            //Generate Assessment Plan
            Excel.Workbook xlWorkBook2;
            Excel.Worksheet xlWorkSheet2;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook2 = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook2.Worksheets.get_Item(1);
            Excel.Range last = xlWorkSheet2.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
            range = xlWorkSheet2.get_Range("A1", last);
            range.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Style.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.Style.WrapText = true;

            xlWorkSheet2.Columns[1].ColumnWidth = 18;
            range = xlWorkSheet2.get_Range("A1", "BU1");
            range.Merge(misValue);
            range.Value2 = "STUDENT OUTCOMES AND EVALUATION PLAN FOR COMPUTER ENGINEERING";

            //Student Outcomes
            xlWorkSheet2.Rows[2].RowHeight = 70;
            xlWorkSheet2.Cells[2, 1] = "STUDENT OUTCOMES";
            int index = 0;
            for (int i = 2; i <= 68; i += 6)
            {
                int j = i;
                Excel.Range r1 = xlWorkSheet2.Cells[2, j];
                j += 5;
                Excel.Range r2 = xlWorkSheet2.Cells[2, j];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);
                range.Value2 = so[index++];     
            }
            range = xlWorkSheet2.get_Range("A2", "BU2");
            range.Interior.Color = Color.PeachPuff;

            //Performance Indicators
            xlWorkSheet2.Rows[3].RowHeight = 70;
            xlWorkSheet2.Cells[3, 1] = "Performance Indicators";
            index = 0;
            for (int i = 2; i <= 71; i += 3)
            {
                int j = i;
                Excel.Range r1 = xlWorkSheet2.Cells[3, j++];
                Excel.Range r2 = xlWorkSheet2.Cells[3, ++j];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);
                range.Value2 = pi[index++];
            }
            range = xlWorkSheet2.get_Range("A3", "BU3");
            range.Interior.Color = Color.Yellow;

            //Course
            xlWorkSheet2.Cells[4, 1] = "Course";
            index = 0;
            for (int i = 2; i <= 71; i += 3)
            {
                int j = i;
                Excel.Range r1 = xlWorkSheet2.Cells[4, j++];
                Excel.Range r2 = xlWorkSheet2.Cells[4, ++j];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);
                range.Value2 = course[index++];
            }
            range = xlWorkSheet2.get_Range("A4", "BU4");
            range.Interior.Color = Color.LimeGreen;

            //Assessment Tool
            xlWorkSheet2.Rows[5].RowHeight = 30;
            xlWorkSheet2.Cells[5, 1] = "Assessment Tool";
            index = 0;
            for (int i = 2; i <= 71; i += 3)
            {
                int j = i;
                Excel.Range r1 = xlWorkSheet2.Cells[5, j++];
                Excel.Range r2 = xlWorkSheet2.Cells[5, ++j];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);
                range.Value2 = asstool[index++];
            }
            range = xlWorkSheet2.get_Range("A5", "BU5");
            range.Interior.Color = Color.Orange;

            //Assessment Targets and Results
            xlWorkSheet2.Cells[6, 1] = "Assessment Targets and Results";
            for (int i = 2; i <= 71; i += 3)
            {
                int j = i;
                xlWorkSheet2.Cells[6, i] = "Target";
                Excel.Range r1 = xlWorkSheet2.Cells[6, ++j];
                Excel.Range r2 = xlWorkSheet2.Cells[6, ++j];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);
                range.Value2 = "Results";
            }
            range = xlWorkSheet2.get_Range("A6", "BU6");
            range.Interior.Color = Color.Aqua;

            //Periods
            xlWorkSheet2.Cells[7, 1] = "Period: Q" + qtr1 + " SY" + sy1;
            xlWorkSheet2.Cells[8, 1] = "Period: Q" + qtr2 + " SY" + sy2;
            xlWorkSheet2.Cells[9, 1] = "Period: Q" + qtr3 + " SY" + sy3;
            xlWorkSheet2.Cells[10, 1] = "Period: Q" + qtr4 + " SY" + sy4;
            xlWorkSheet2.Cells[11, 1] = "Overall Results (Q" + qtr1 + " SY" + sy1 + " to Q" + qtr4 + " SY" + sy4;
            range = xlWorkSheet2.get_Range("A11", "BU11");
            range.Interior.Color = Color.HotPink;

            int b = 0;
            for (int i = 2; i <= 71; i += 3)
            {
                Excel.Range r1 = xlWorkSheet2.Cells[7, i];
                Excel.Range r2 = xlWorkSheet2.Cells[11, i];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);
                range.Value2 = studtarget[b] + "% of students should obtain a rating of at least 3.5";
                b++;
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            //computation
            ////////////////////////////////////
            int index1 = 0;
            int index2 = 0;
            for (int i = 3; i <= 72; i += 3)
            {
                for (int j = 7; j <= 11; j++)
                {
                    if (j == 11)
                    {
                        int l = i;
                        xlWorkSheet2.Cells[j, l] = studpassall1[index2] + " out of " + studtotall1[index2];
                        xlWorkSheet2.Cells[j, ++l] = studpassallave1[index2] + "%";
                        index2++;
                    }
                    else
                    {
                        int l = i;
                        xlWorkSheet2.Cells[j, l] = studpass1[index1] + " of " + studtot1[index1] + " students enrolled";
                        xlWorkSheet2.Cells[j, ++l] = studpassave1[index1] + "%";
                        index1++;
                    }

                }
            }

            //Evaluation, Recommendation and Effectivity
            xlWorkSheet2.Cells[12, 1] = "Evaluation";
            xlWorkSheet2.Cells[12, 1].Interior.Color = Color.PaleGreen;
            xlWorkSheet2.Rows[13].RowHeight = 90;
            xlWorkSheet2.Cells[13, 1] = "Recommendation";
            xlWorkSheet2.Cells[14, 1] = "Effectivity";
            range = xlWorkSheet2.get_Range("A14", "BU14");
            range.Interior.Color = Color.Beige;

            index = 0;
            int col = 2;
            if (qtr4 == 4)
            {
                qtr4 = 1;
                sy4 += 101;
            }
            else
                qtr4++;

            int c = 0;
            foreach (float res in result)
            {
                int col1 = col;
                Excel.Range r1 = xlWorkSheet2.Cells[12, col1++];
                Excel.Range r2 = xlWorkSheet2.Cells[12, ++col1];
                range = xlWorkSheet2.get_Range(r1, r2);
                range.Merge(misValue);

                col1 = col;
                Excel.Range r11 = xlWorkSheet2.Cells[13, col1++];
                Excel.Range r21 = xlWorkSheet2.Cells[13, ++col1];
                if (res >= studtarget[c])
                {
                    range.Value2 = "Target Achieved";
                    range.Interior.Color = Color.PaleGreen;
                    range = xlWorkSheet2.get_Range(r11, r21);
                    range.Merge(misValue);
                    range.Value2 = "Retain Performance Indicator, Assessment Tool and Targets for the course " + course[index++];
                }
                else if (res < studtarget[c])
                {
                    range.Value2 = "Target Not Achieved";
                    range.Interior.Color = Color.Red;
                    range = xlWorkSheet2.get_Range(r11, r21);
                    range.Merge(misValue);
                    range.Value2 = "Modify Performance Indicator, Assessment Tool and Targets for the course " + course[index++];
                }
                else
                {
                    range.Value2 = "Evaluation N/A";
                    range.Interior.Color = Color.LightGray;
                    range = xlWorkSheet2.get_Range(r11, r21);
                    range.Merge(misValue);
                    range.Value2 = "Recommendation for the course " + course[index++] + " N/A";
                }
                col1 = col;
                Excel.Range r12 = xlWorkSheet2.Cells[14, col1++];
                Excel.Range r22 = xlWorkSheet2.Cells[14, ++col1];
                range = xlWorkSheet2.get_Range(r12, r22);
                range.Merge(misValue);
                range.Value2 = qtr4 + "Q AY " + sy4;
                col += 3;
                c++;
            }

            range = xlWorkSheet2.get_Range("A1", "BU14");
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders.Weight = Excel.XlBorderWeight.xlThick;

            xlWorkBook2.SaveAs(filepath1, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook2.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkBook2);
            Marshal.ReleaseComObject(xlApp);

            //Update outcome table
            DateTime dt = DateTime.Now;

            outcome oc = new outcome();
            oc.pid = pro.pid;
            oc.acc_id = accid;
            oc.filename = "AssessmentPlan_" + accid + "_" + pro.pid + ".xlsx";
            oc.cdate = dt;
            db.outcomes.Add(oc);
            db.SaveChanges();

            //Delete Files in Upload Folder
            System.IO.DirectoryInfo di = new DirectoryInfo(filepath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            TempData["flag"] = 1;
            TempData["MessageTitle"] = "Generate Plan";
            TempData["MessagePrompt"] = "Assessment Plan has been created, check Downloads";

            if ((int)Session["type"] != 0)
            {
                return RedirectToAction("MainPage", "Home");
            }
            return RedirectToAction("AdminPage", "Home");
        }

        //List all available SO
        public string[] ListSO()
        {
            using (projectEntities db = new projectEntities())
            {
                return db.soes.Where(x => x.so_id != 0).Select(x => x.so_desc).ToArray();
            }
        }

        //List courses per SO and PI
        public string[] ListCourse()
        {
            using (projectEntities db = new projectEntities())
            {
                return db.courses.Join(db.copiatts, x => x.course_id, y => y.course_id, (x, y) => new { x, y }).OrderBy(xy => xy.y.copiatt_id).Where(xy => xy.x.course_id != 0 && xy.y.course_id != 0).Select(xy => xy.x.coursename).ToArray();
            }
        }

        //List AT per SO and PI
        public string[] ListAT()
        {
            using (projectEntities db = new projectEntities())
            {
                return db.ats.Join(db.copiatts, x => x.at_id, y => y.at_id, (x, y) => new { x, y }).OrderBy(xy => xy.y.copiatt_id).Where(xy => xy.x.at_id != 0 && xy.y.at_id != 0).Select(xy => xy.x.at_desc).ToArray();
            }
        }

        //List PI per SO
        public string[] ListPI()
        {
            using (projectEntities db = new projectEntities())
            {
                return db.pis.Where(x => x.pi_id != 0).Select(x => x.pi_desc).ToArray();
            }
        }

        //List target per SO and PI
        public float[] ListTarget()
        {
            using (projectEntities db = new projectEntities())
            {
                return db.copiatts.Where(x => x.target != 0).Select(x => x.target).ToArray();
            }
        }

        //get all logs in db
        public List<FileViewModels> ListLogs()
        {
            using (projectEntities db = new projectEntities())
            {
                return db.outcomes.Join(db.users, x => x.acc_id, y => y.acc_id, (x, y) => new { x, y }).OrderByDescending(xy => xy.x.pid).Where(xy => xy.x.acc_id != 0 && xy.y.acc_id != 0).Select(xy => new FileViewModels{ pid = xy.x.pid, acc_id = xy.x.acc_id, fname = xy.y.FName, filename = xy.x.filename, lname = xy.y.LName, cdate = xy.x.cdate }).ToList();
            }
        }

        //Returns generated files with respect to account id (ex. user 1 can't see the files generated by user 2)
        //Searching capabilities for Download page.
        public ActionResult Downloads(string searching)
        {
            projectEntities db = new projectEntities();

            if (Session["accID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int accid = (int)Session["accID"];
            List<FileViewModels> listFiles = db.outcomes.Where(x => x.filename.Contains(searching) && x.acc_id == accid || searching == null && x.acc_id == accid).OrderByDescending(x => x.pid).Select(x => new FileViewModels { acc_id = x.acc_id, filename = x.filename, cdate = x.cdate }).ToList();
            ViewBag.filelist = listFiles;
            ViewBag.Title = "Downloads";

            return View();
        }

        //View Logs Page
        public ActionResult Logs()
        {
            projectEntities db = new projectEntities();
            List<FileViewModels> listFiles = ListLogs();
            ViewBag.listlogs = listFiles;
            ViewBag.Title = "View Logs";
            return View();
        }

        //Get File and Download in Browser
        public ActionResult getFile(string filename)
        {
            if (Session["accID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int accid = (int)Session["accID"];
            string filepath = Server.MapPath("~/UserFiles/" + accid);
            string fn = Path.GetFileName(filename);
            string fPath = Path.Combine(filepath, fn);
            return File(fPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

    }

}