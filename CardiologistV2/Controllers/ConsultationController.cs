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

namespace CardiologistV2.Controllers
{
    public class ConsultationController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Consultation/

        public ActionResult Index()
        {    
            var consultations = db.Consultations.Include(c => c.patients).Include(c => c.doctors);
            //Consultation [] carray = new Consultation[consultations.Count()];
            //int i=0;
            //foreach (Consultation c in consultations)
            //{
            //    if (User.IsInRole("Patient"))
            //    {
            //        int patientID = WebSecurity.GetUserId(User.Identity.Name);
            //        Patient p = db.Patients.Find(patientID);
            //        string pname = p.Name;
            //        Patient p2 = db.Patients.Find(c.PatientID);
            //        string p2name = p2.Name;
            //        if (pname==p2name)
            //        {
            //            carray[i]=c;
            //            i++;
            //        }
            //    }
            //    if (User.IsInRole("Doctor"))
            //    {
            //        int doctorID = WebSecurity.GetUserId(User.Identity.Name);
            //        Doctor d = db.Doctors.Find(doctorID);
            //        string dname = d.Name;
            //        Doctor d2 = db.Doctors.Find(c.DoctorID);
            //        string d2name = d2.Name;
            //        if (dname == d2name)
            //        {
            //            carray[i] = c;
            //            i++;
            //        }
            //    }
            //}
            ////return View(consultations.ToList());
            //// counting the null places in the array to remove them:
            //int sum = 0;
            //for (int j=0;j<carray.Count();j++)
            //{
            //    if (carray[j] != null)
            //    {
            //        sum++;
            //    }

            //}
            //Consultation[] c2array = new Consultation[sum];
            //for (int k = 0; k < sum; k++)
            //{
            //    c2array[k] = carray[k];
            //}
            //return View(c2array.ToList());
            return View(consultations.ToList());
        }

        //
        // GET: /Consultation/Details/5

        public ActionResult Details(int id = 0)
        {
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }

        //
        // GET: /Consultation/Create

        public ActionResult Create()
        {
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name");
            ViewBag.DoctorID = new SelectList(db.Doctors, "DoctorID", "Name");
            return View();
        }

        //
        // POST: /Consultation/Create

        [HttpPost]
        public ActionResult Create(Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                //int patientID = WebSecurity.GetUserId(User.Identity.Name);
                int pid=0;
                foreach (Patient pa in db.Patients)
                {
                    if (pa.Name == User.Identity.Name)
                    { pid = pa.PatientID; }
                }
                //Patient p = db.Patients.Find(User.Identity.Name);
                consultation.PatientID = pid;
                db.Consultations.Add(consultation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name", consultation.PatientID);
            ViewBag.DoctorID = new SelectList(db.Doctors, "DoctorID", "Name", consultation.DoctorID);
            return View(consultation);
        }

        //
        // GET: /Consultation/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name", consultation.PatientID);
            ViewBag.DoctorID = new SelectList(db.Doctors, "DoctorID", "Name", consultation.DoctorID);
            return View(consultation);
        }

        //
        // POST: /Consultation/Edit/5

        [HttpPost]
        public ActionResult Edit(Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consultation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "Name", consultation.PatientID);
            ViewBag.DoctorID = new SelectList(db.Doctors, "DoctorID", "Name", consultation.DoctorID);
            return View(consultation);
        }

        //
        // GET: /Consultation/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }

        //
        // POST: /Consultation/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Consultation consultation = db.Consultations.Find(id);
            db.Consultations.Remove(consultation);
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