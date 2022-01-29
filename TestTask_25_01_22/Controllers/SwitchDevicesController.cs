using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestTask_25_01_22.Models;

namespace TestTask_25_01_22.Controllers
{
    public class SwitchDevicesController : Controller
    {
        private readonly TestTask_25_01_22Context _context;

        Dictionary<int, string> pageSizes = new Dictionary<int, string>
        {
            { 3, "3" },
            { 5, "5" },
            { 10, "10" },
            { 25, "25" }
         };

        public SwitchDevicesController(TestTask_25_01_22Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchInventory, string searchModel, string searchIP, string searchMAC,
            string searchVLAN, string searchSerial, DateTime searchPurchase, DateTime searchInstallation, int? pageSize, int? pageNumber, Dictionary<string, string> parms)
        {
            ViewBag.PageSizes = new SelectList(pageSizes, "Key", "Value");
            if (searchInventory != null || searchModel != null || searchIP != null || searchMAC != null || searchVLAN != null || searchSerial != null ||
                searchPurchase != DateTime.Parse("01.01.0001") || searchInstallation != DateTime.Parse("01.01.0001"))
            {
                pageNumber = 1;
            }
            else
            {
                parms.TryGetValue("InventoryNumber", out searchInventory);
                parms.TryGetValue("ModelName", out searchModel);
                parms.TryGetValue("Ipv4", out searchIP);
                parms.TryGetValue("Mac", out searchMAC);
                parms.TryGetValue("MainVlan", out searchVLAN);
                parms.TryGetValue("SerialNumber", out searchSerial);
                parms.TryGetValue("PurchaseDate", out string searchPurchaseString);
                DateTime.TryParse(searchPurchaseString, out searchPurchase);
                parms.TryGetValue("InstallationDate", out string searchInventoryString);
                DateTime.TryParse(searchInventoryString, out searchInstallation);
            }
            SwitchDevice searchDevices = new SwitchDevice(searchModel, searchIP, searchMAC,
                searchVLAN, searchSerial, searchInventory, searchPurchase, searchInstallation);
            ViewData["CurrentFilter"] = searchDevices;

            var switchDevices = from s in _context.SwitchDevices
                                select s;
            if (!string.IsNullOrEmpty(searchInventory) || !string.IsNullOrEmpty(searchModel) || !string.IsNullOrEmpty(searchIP)
                || !string.IsNullOrEmpty(searchMAC) || !string.IsNullOrEmpty(searchVLAN) || !string.IsNullOrEmpty(searchSerial)
                || searchPurchase != DateTime.Parse("01.01.0001") || searchInstallation != DateTime.Parse("01.01.0001"))
            {
                switchDevices = switchDevices.Where(s => (!string.IsNullOrEmpty(searchInventory) ? s.InventoryNumber.Contains(searchInventory) : true) &&
                                                         (!string.IsNullOrEmpty(searchModel) ? s.ModelName.Contains(searchModel) : true) &&
                                                         (!string.IsNullOrEmpty(searchIP) ? s.Ipv4.Contains(searchIP) : true) &&
                                                         (!string.IsNullOrEmpty(searchMAC) ? s.Mac.Contains(searchMAC) : true) &&
                                                         (!string.IsNullOrEmpty(searchVLAN) ? s.MainVlan.Contains(searchVLAN) : true) &&
                                                         (!string.IsNullOrEmpty(searchSerial) ? s.SerialNumber.Contains(searchSerial) : true) &&
                                                         (searchPurchase != DateTime.Parse("01.01.0001") ? s.PurchaseDate == searchPurchase : true) &&
                                                         (searchInstallation != DateTime.Parse("01.01.0001") ? s.InstallationDate == searchInstallation : true)
                                                   );
            }

            if (pageSize == null)
            {
                pageSize = 5;
            }
            ViewData["CurrentPageSize"] = pageSize;

            return View(await PaginatedList<SwitchDevice>.CreateAsync(switchDevices.AsNoTracking(), pageNumber ?? 1, (int)pageSize));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchDevice = await _context.SwitchDevices
                .FirstOrDefaultAsync(m => m.SerialNumber == id);
            if (switchDevice == null)
            {
                return NotFound();
            }

            return View(switchDevice);
        }

        public IActionResult Create()
        {
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelName,Ipv4,Mac,MainVlan,SerialNumber,InventoryNumber,PurchaseDate,InstallationDate,Stage,Comment")] SwitchDevice switchDevice)
        {
            if (switchDevice.InstallationDate < switchDevice.PurchaseDate) ModelState.AddModelError("Dates", "Даты некорректны");
            if (_context.SwitchDevices.Any(sd => sd.SerialNumber == switchDevice.SerialNumber)) ModelState.AddModelError("Serial", "Серийные номера должны быть уникальны");
            if (_context.SwitchDevices.Any(sd => sd.Ipv4 == switchDevice.Ipv4)) ModelState.AddModelError("IP", "IP-адрес уже используется");
            if (_context.SwitchDevices.Any(sd => sd.Mac == switchDevice.Mac)) ModelState.AddModelError("MAC", "MAC-адрес занят");
            if (_context.SwitchDevices.Any(sd => sd.MainVlan == switchDevice.MainVlan)) ModelState.AddModelError("VLAN", "VLAN-адрес уже используется");
            if (_context.SwitchDevices.Any(sd => sd.InventoryNumber == switchDevice.InventoryNumber)) ModelState.AddModelError("InventoryNumber", "Инвентарный номер занят");
            if (ModelState.IsValid)
            {
                _context.Add(switchDevice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(switchDevice);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchDevice = await _context.SwitchDevices.FindAsync(id);
            if (switchDevice == null)
            {
                return NotFound();
            }
            switchDevice.Ipv4 = Regex.Replace(switchDevice.Ipv4, @"\s+", "");
            switchDevice.MainVlan = Regex.Replace(switchDevice.MainVlan, @"\s+", "");
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View(switchDevice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ModelName,Ipv4,Mac,MainVlan,SerialNumber,InventoryNumber,PurchaseDate,InstallationDate,Stage,Comment")] SwitchDevice switchDevice)
        {
            if (id != switchDevice.SerialNumber)
            {
                return NotFound();
            }

            if (switchDevice.InstallationDate < switchDevice.PurchaseDate) ModelState.AddModelError("Dates", "Даты некорректны");
            if (_context.SwitchDevices.Any(sd => sd.SerialNumber != id && sd.Ipv4 == switchDevice.Ipv4)) ModelState.AddModelError("IP", "IP-адрес уже используется");
            if (_context.SwitchDevices.Any(sd => sd.SerialNumber != id && sd.Mac == switchDevice.Mac)) ModelState.AddModelError("MAC", "MAC-адрес занят");
            if (_context.SwitchDevices.Any(sd => sd.SerialNumber != id && sd.MainVlan == switchDevice.MainVlan)) ModelState.AddModelError("VLAN", "VLAN-адрес уже используется");
            if (_context.SwitchDevices.Any(sd => sd.SerialNumber != id && sd.InventoryNumber == switchDevice.InventoryNumber)) ModelState.AddModelError("InventoryNumber", "Инвентарный номер занят");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(switchDevice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SwitchDeviceExists(switchDevice.SerialNumber))
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
            return View(switchDevice);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchDevice = await _context.SwitchDevices
                .FirstOrDefaultAsync(m => m.SerialNumber == id);
            if (switchDevice == null)
            {
                return NotFound();
            }

            return View(switchDevice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var switchDevice = await _context.SwitchDevices.FindAsync(id);
            _context.SwitchDevices.Remove(switchDevice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SwitchDeviceExists(string id)
        {
            return _context.SwitchDevices.Any(e => e.SerialNumber == id);
        }

    }
}
