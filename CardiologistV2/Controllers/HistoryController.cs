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
    public class HistoryController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /History/

        public ActionResult Index()
        {
            return View(db.Histories.ToList());
        }

        //
        // GET: /History/Details/5

        public ActionResult Details(int id = 0)
        {
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

        //
        // GET: /History/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /History/Create

        [HttpPost]
        public ActionResult Create(History history)
        {
            if (ModelState.IsValid)
            {
                db.Histories.Add(history);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(history);
        }

        //
        // GET: /History/Edit/5

        public ActionResult Edit(int id = 0)
        {
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

        //
        // POST: /History/Edit/5

        [HttpPost]
        public ActionResult Edit(History history)
        {
            if (ModelState.IsValid)
            {
                db.Entry(history).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(history);
        }

        //
        // GET: /History/Delete/5

        public ActionResult Delete(int id = 0)
        {
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

        //
        // POST: /History/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            History history = db.Histories.Find(id);
            db.Histories.Remove(history);
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