using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardiologistV2.Models;
using CardiologistV2.DAL;

namespace CardiologistV2.Controllers
{
    public class FeaturesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Features/

        public ActionResult Index()
        {
            return View(db.Features.ToList());
        }

        //
        // GET: /Features/Details/5

        public ActionResult Details(int id = 0)
        {
            Features features = db.Features.Find(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        //
        // GET: /Features/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Features/Create

        [HttpPost]
        public ActionResult Create(Features features)
        {
            if (ModelState.IsValid)
            {
                db.Features.Add(features);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(features);
        }

        //
        // GET: /Features/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Features features = db.Features.Find(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        //
        // POST: /Features/Edit/5

        [HttpPost]
        public ActionResult Edit(Features features)
        {
            if (ModelState.IsValid)
            {
                db.Entry(features).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(features);
        }

        //
        // GET: /Features/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Features features = db.Features.Find(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        //
        // POST: /Features/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Features features = db.Features.Find(id);
            db.Features.Remove(features);
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