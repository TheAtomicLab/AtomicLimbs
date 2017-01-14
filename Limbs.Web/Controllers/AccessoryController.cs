using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Models;

namespace Limbs.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AccessoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accessory
        public async Task<ActionResult> Index()
        {
            return View(await db.AccessoryModels.ToListAsync());
        }

        // GET: Accessory/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessoryModel accessoryModel = await db.AccessoryModels.FindAsync(id);
            if (accessoryModel == null)
            {
                return HttpNotFound();
            }
            return View(accessoryModel);
        }

        // GET: Accessory/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Colors = await db.Colors.ToListAsync();

            return View();
        }

        // POST: Accessory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccessoryModel accessoryModel, List<int> colorList)
        {
            var selectedColors = db.Colors.Where(x => colorList.Contains(x.Id)).ToList();
            accessoryModel.Color = selectedColors;

            if (ModelState.IsValid)
            {
                db.AccessoryModels.Add(accessoryModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Colors = db.Colors.Where(x => !colorList.Contains(x.Id)).ToList();
            ViewBag.SelectedColors = selectedColors;
            return View(accessoryModel);
        }

        // GET: Accessory/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var accessoryModel = await db.AccessoryModels.FindAsync(id);
            if (accessoryModel == null)
            {
                return HttpNotFound();
            }

            var colors = db.Colors.ToList();
            foreach (var color in accessoryModel.Color)
            {
                colors.Remove(color);
            }

            ViewBag.Colors = colors;
            ViewBag.SelectedColors = accessoryModel.Color.ToList();

            return View(accessoryModel);
        }

        // POST: Accessory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccessoryModel accessoryModel, List<int> colorList)
        {
            var selectedColors = db.Colors.Where(x => colorList.Contains(x.Id)).ToList();
            accessoryModel.Color = selectedColors;

            if (ModelState.IsValid)
            {
                db.Entry(accessoryModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            ViewBag.Colors = db.Colors.Where(x => !colorList.Contains(x.Id)).ToList();
            ViewBag.SelectedColors = selectedColors;

            return View(accessoryModel);
        }

        // GET: Accessory/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessoryModel accessoryModel = await db.AccessoryModels.FindAsync(id);
            if (accessoryModel == null)
            {
                return HttpNotFound();
            }
            return View(accessoryModel);
        }

        // POST: Accessory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AccessoryModel accessoryModel = await db.AccessoryModels.FindAsync(id);
            db.AccessoryModels.Remove(accessoryModel);
            await db.SaveChangesAsync();
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
