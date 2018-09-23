using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardiologistV2.Models;
using CardiologistV2.DAL;
using CardiologistV2.Filters;
using CardiologistV2.View_Models;
using WebMatrix.WebData;
using System.Web.Security;

namespace CardiologistV2.Controllers
{
    [InitializeSimpleMembership]
    public class PatientController : Controller
    { 
        
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Patient/

        public ActionResult Index()
        {
            return View(db.Patients.ToList());
        }

        //
        // GET: /Patient/Details/5

        public ActionResult Details(int id = 0)
        {
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        //
        // GET: /Patient/Create

        public ActionResult Create()
        {
            return View(new RegisterPatient());
        }

        //
        // POST: /Patient/Create

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterPatient patient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(patient.RM.UserName, patient.RM.Password);

                    WebSecurity.Login(patient.RM.UserName, patient.RM.Password);
                    
                    bool pat_role=false;
                    string [] roles = Roles.GetAllRoles();
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (roles[i] == "Patient")
                        { pat_role = true; }
                    }


                    if (pat_role == false)
                    { Roles.CreateRole("Patient"); }

                    Roles.AddUsersToRole(new[] { patient.RM.UserName }, "Patient");
                    CardiologistV2.DAL.DatabaseContext db = new DAL.DatabaseContext();
                    db.Users.Add(new Models.User()
                    {
                        UserID = WebSecurity.GetUserId(patient.RM.UserName)
                    });

                    patient.ThePatient.PatientID = WebSecurity.GetUserId(patient.RM.UserName);
                    db.Patients.Add(patient.ThePatient);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (MembershipCreateUserException e)
                {
                    // ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(patient);
        }

        //
        // GET: /Patient/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        //
        // POST: /Patient/Edit/5

        [HttpPost]
        public ActionResult Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        //
        // GET: /Patient/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        //
        // POST: /Patient/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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