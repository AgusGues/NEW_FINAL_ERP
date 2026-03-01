using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class ItemPriceController : Controller
    {
        private readonly ItemPriceService _service;

        public ItemPriceController(ItemPriceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 5;
            var result = await _service.GetAll(search, page, pageSize);
            ViewBag.Search = search;
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetById(id);
            if (data == null)
                return Json(new { success = false, message = "Data tidak ditemukan." });

            return Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Save(ItemPrice model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Data tidak valid." });

            try
            {
                if (model.ItemPriceId == 0)
                    await _service.Create(model);
                else
                    await _service.Update(model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ✅ FIXED (WAJIB AWAIT)
        [HttpGet]
        public async Task<IActionResult> SearchItem(string term)
        {
            var result = await _service.SearchItemAsync(term ?? "");
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchUnit(string term)
        {
            var result = await _service.SearchUnitAsync(term ?? "");
            return Json(result);
        }
    }
}