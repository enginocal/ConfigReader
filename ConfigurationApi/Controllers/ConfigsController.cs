using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfigurationApi.Infrastructure;
using ConfigurationApi.Models;
using ConfigurationApi.Repository;
using ConfigurationApi.Infrastructure.Repository;
using RabbitMQ.Client;
using System.Text;

namespace ConfigurationApi.Controllers
{
    public class ConfigsController : Controller
    {
        ConnectionFactory factory = new ConnectionFactory() { DispatchConsumersAsync = true };
        IConnection connection;
        IModel channel;
        const string queueName = "myqueue";
        private readonly ConfigDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IRepository<Config> _configRepository;
        //public private 

        public ConfigsController(ConfigDbContext context)
        {
            _context = context;
            _unitOfWork = new EfUnitOfWork(_context);
            _configRepository = new EFRepository<Config>(_context);
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        // GET: Configs
        public async Task<IActionResult> Index()
        {
            return View(_configRepository.GetAll());
        }

        // GET: Configs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var config = await _configRepository.Find(id.Value);
            if (config == null)
            {
                return NotFound();
            }

            return View(config);
        }

        // GET: Configs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Configs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationName,Name,Value,Type,IsActive")] Config config)
        {
            if (ModelState.IsValid)
            {
                channel.QueueDeclare(queueName + config.ApplicationName, true, false, false, null);
                var props = channel.CreateBasicProperties();
                var messageBody = Encoding.UTF8.GetBytes("New configValues added with applicationName.");

                config.Id = Guid.NewGuid();
                //_context.Add(config);
                await _configRepository.Insert(config);
                channel.BasicPublish("", queueName, props, messageBody);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(config);
        }

        // GET: Configs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var config = await _configRepository.Find(id.Value);
            if (config == null)
            {
                return NotFound();
            }
            return View(config);
        }

        // POST: Configs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ApplicationName,Name,Value,Type,IsActive")] Config config)
        {
            if (id != config.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(config);
                    //await _context.SaveChangesAsync();
                    await _configRepository.Update(config);
                    _unitOfWork.SaveChanges();
                    var props = channel.CreateBasicProperties();
                    var messageBody = Encoding.UTF8.GetBytes("configValues edited.");
                    channel.BasicPublish("", queueName + config.ApplicationName, props, messageBody);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConfigExists(config.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(config);
        }

        // GET: Configs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var config = await _context.Configurations.SingleOrDefaultAsync(m => m.Id == id);
            if (config == null)
            {
                return NotFound();
            }

            return View(config);
        }

        // POST: Configs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var config = await _context.Configurations.SingleOrDefaultAsync(m => m.Id == id);
            _context.Configurations.Remove(config);
            await _context.SaveChangesAsync();
            var props = channel.CreateBasicProperties();
            var messageBody = Encoding.UTF8.GetBytes("configValues deleted.");
            channel.BasicPublish("", queueName + config.ApplicationName, props, messageBody);
            return RedirectToAction(nameof(Index));
        }

        private bool ConfigExists(Guid id)
        {
            return _context.Configurations.Any(e => e.Id == id);
        }
    }
}
