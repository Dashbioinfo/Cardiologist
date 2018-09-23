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
    public class DoctorController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Doctor/

        public ActionResult Index()
        {
            return View(db.Doctors.ToList());
        }

        //
        // GET: /Doctor/Details/5

        public ActionResult Details(int id = 0)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //
        // GET: /Doctor/Create

        public ActionResult Create()
        {
            return View(new RegisterDoctor());
        }

        //
        // POST: /Doctor/Create

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterDoctor doctor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(doctor.RM.UserName, doctor.RM.Password);

                    WebSecurity.Login(doctor.RM.UserName, doctor.RM.Password);

                    bool doc_role = false;
                    string[] roles = Roles.GetAllRoles();
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (roles[i] == "Doctor")
                        { doc_role = true; }
                    }

                    if (doc_role == false)
                    { Roles.CreateRole("Doctor"); }


                    Roles.AddUsersToRole(new[] { doctor.RM.UserName }, "Doctor");
                    CardiologistV2.DAL.DatabaseContext db = new DAL.DatabaseContext();
                    db.Users.Add(new Models.User()
                    {
                        UserID = WebSecurity.GetUserId(doctor.RM.UserName)
                    });

                    doctor.TheDoctor.DoctorID = WebSecurity.GetUserId(doctor.RM.UserName);
                    db.Doctors.Add(doctor.TheDoctor);
                    //bool conf = WebSecurity.IsConfirmed(Doctor.RM.UserName);
                    //conf = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (MembershipCreateUserException e)
                {
                    // ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(doctor);
        }

        //
        // GET: /Doctor/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //
        // POST: /Doctor/Edit/5

        [HttpPost]
        public ActionResult Edit(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        //
        // GET: /Doctor/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //
        // POST: /Doctor/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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