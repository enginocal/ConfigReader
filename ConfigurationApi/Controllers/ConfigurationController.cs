using ConfigurationApi.Models;
using ConfigurationApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ConfigurationApi.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ConfigRepository configRepository;

        public ConfigurationController()
        {
            configRepository = new ConfigRepository();
        }

        // GET: Configuration
        public ActionResult Index()
        {
            return View();
        }

        // GET: Configuration/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Configuration/Create
        public async Task<ActionResult> CreateAsync(Config config)
        {
            if (ModelState.IsValid)
            {
                await configRepository.Insert(config);
            }
            return View();
        }

        // POST: Configuration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Configuration/Edit/5
        public async Task<ActionResult> EditAsync(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Config config = await configRepository.Find(id);
            if (config == null)
            {
                return NotFound();
            }
            return View(config);
        }

        // POST: Configuration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,Name,LastUpdatedTime")] Config config)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    await configRepository.Update(config);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(config);
            }
        }

        // GET: Configuration/Delete/5
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var config = await configRepository.Find(id);
            if (config == null)
            {
                return NotFound();
            }


            return View();
        }

        // POST: Configuration/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}