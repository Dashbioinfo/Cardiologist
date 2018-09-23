using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardiologistV2.Models;
using CardiologistV2.DAL;
using WebMatrix.WebData;
using CardiologistV2.Matlab.DomainModels;

namespace CardiologistV2.Controllers
{
    public class RecordController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Record/

        public ActionResult Index()
        {
            var records = db.Records.Include(r => r.patients);
            return View(records.ToList());
        }

        //
        // GET: /Record/Details/5

        public ActionResult Details(int id = 0)
        {
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        //
        // GET: /Record/Create

        public ActionResult Create()
        {
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name");
            return View();
        }

        //
        // POST: /Record/Create

        [HttpPost]
        public ActionResult Create(Record record, HttpPostedFileBase txtfile)
        {
            if (ModelState.IsValid)
            {
                string filename = ""; string path = ""; string Data="";
                if (txtfile != null && txtfile.ContentLength > 0)
                {
                    filename = System.Guid.NewGuid() + "_" + txtfile.FileName;
                    path = Server.MapPath("~/Signals/" + filename);
                    txtfile.SaveAs(path);
                    //   record.Data = filename;
                    Data = System.IO.File.ReadAllText(path);
                    record.Data = Data;
                    
                }
                #region Filter File
                string path1 = Server.MapPath("~/Matlab");
                System.IO.File.WriteAllText(path1 + "\\" + filename, Data);
                MLApp.MLApp matlab = new MLApp.MLApp();
                matlab.Execute("cd('" + path1 + "')");
                matlab.Execute("a=load('" + filename + "')");
                matlab.Execute("c=baselinefilter(a," + 370 + ")");
                var o = (double[,])matlab.GetVariable("c", "base");
                string filedata = "";

                for (int i = 0; i < o.GetLength(0); i++)
                {
                    for (int j = 0; j < o.GetLength(1); j++)
                    {
                        filedata += o[i, j] + " ";
                    }
                    filedata.Trim(' ');

                    filedata += "\n";
                }
                #endregion //DONE!
                
                #region Pass File to Feature Extractor
                ECG ecgFeatureExtractor = new ECG();
              string InputVector = ecgFeatureExtractor.ExtractFeatures(filedata, filename, 370); // fs = 370
                string filename2="feat.txt";
              System.IO.File.WriteAllText(path1 + "\\" + filename2, InputVector);
                #endregion

             
               #region simulate network
                matlab.Execute("b = load ('" + filename2 +"')");
                matlab.Execute("n = load('matlab.mat')");
                matlab.Execute("s = sim(n.net ,b)");
               var obj = (double[,])matlab.GetVariable("s", "base");
                #endregion

              /* #region Make Logical Decision
               int MaxIndex = -1;
               int numpulses = obj.GetLength(1);
               int numpvc = 0; int numlbbb = 0; int numrbbb = 0; int numnormal = 0;
               var vector = new List<double>();
               for (int j = 0; j < numpulses; j++)
               {
                  // for (int i = 0; i < obj.GetLength(0); i++)
                   for (int i = 0; i < 4; i++)
                   {
                       vector.Add(obj[i, j]);
                   }
                   MaxIndex = vector.IndexOf(vector.Max());
                   switch (MaxIndex)
                   {
                       case 0: //pvc
                           numpvc++;
                           break;
                       case 1: //lbbb
                           numlbbb++;
                           break;
                       case 2: //rbbb
                           numrbbb++;
                           break;
                       case 3: //normal
                           numnormal++;
                           break;
                       default:
                           break;
                   }
               }
               string result = "Pvc = " + ((double)(numpvc * 100) / numpulses )+ "%" + "Lbbb = "
     + ((double)(numlbbb * 100) / numpulses )+ "% " + "Rbbb = "
      + ((double)(numrbbb * 100) / numpulses )+ "% " + "Normal = "
      + ((double)(numnormal * 100) / numpulses )+ "%";

               //string result = "Pvc = " + numpvc + "lbbb = " + numlbbb + "rbbb =" + numrbbb + "normal = " + numnormal;
               #endregion
                */
               #region other result
               int non = 0;
               int nol = 0;
               int nor = 0;
               int nop = 0;
               for (int i = 0; i < obj.GetLength(1); i++)
               {
                   double maxx = -1;
                   for (int j = 0; j < 4; j++)
                   {
                       if (obj[0, i] >= maxx)
                       {
                           maxx = obj[0, i];
                       }
                       if (obj[1, i] >= maxx)
                       {
                           maxx = obj[1, i];
                       }
                       if (obj[2, i] >= maxx)
                       {
                           maxx = obj[2, i];
                       }
                       if (obj[3, i] >= maxx)
                       {
                           maxx = obj[3, i];
                       }
                       if (obj[0, i] == maxx)
                           nop++;
                       if (obj[1, i] == maxx)
                           nol++;
                       if (obj[2, i] == maxx)
                           nor++;
                       if (obj[3, i] == maxx)
                           non++;
                   }
               }
               string resultt = "Pvc = " + nop + "lbbb = " + nol + "rbbb =" + nor + "normal = " + non;
            /*   string resultt = "Pvc = " + (double)((nop * 100) / obj.GetLength(0)) + "%" + "Lbbb = "
    + (double)((nol * 100) / obj.GetLength(0)) + "% " + "Rbbb = "
     + (double)((nor * 100) / obj.GetLength(0))+ "% " + "Normal = "
     + (double)((non * 100) / obj.GetLength(0)) + "%";*/
               #endregion
                   record.Result = resultt;
                //int PatientID = WebSecurity.GetUserId(User.Identity.Name); 
                //Patient patient = db.Patients.Find(record.PatientID);
                int pid = 0;
                foreach (Patient pa in db.Patients)
                {
                    if (pa.Name == User.Identity.Name)
                    { pid = pa.PatientID; }
                }
                
                record.PatientID = pid;
                db.Records.Add(record);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name", record.PatientID);
            return View(record);
        }

        //
        // GET: /Record/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name", record.PatientID);
            return View(record);
        }

        //
        // POST: /Record/Edit/5

        [HttpPost]
        public ActionResult Edit(Record record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name", record.PatientID);
            return View(record);
        }

        //
        // GET: /Record/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        //
        // POST: /Record/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Record record = db.Records.Find(id);
            db.Records.Remove(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}