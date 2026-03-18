using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;
using System;
using System.Threading.Tasks;

namespace NEW_FINAL_ERP.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly PurchaseService _service;

        public PurchaseController(PurchaseService service)
        {
            _service = service;
        }

        // =========================================================
        // INDEX
        // =========================================================
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(string? search, int page = 1, int pageSize = 5)
        {
            var result = await _service.GetAll(search, page, pageSize);
            return Json(result);
        }

        // =========================================================
        // GET BY ID (Header + Details)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var header = await _service.GetByIdAsync(id); // Pastikan kamu buat method GetByIdAsync di service
                if (header == null)
                    return Json(new { success = false, message = "Data tidak ditemukan." });

                return Json(new { success = true, data = header });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================================================
        // CREATE
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseFormDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Data tidak valid." });

            try
            {
                await _service.CreateAsync(dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================================================
        // UPDATE
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] PurchaseFormDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Data tidak valid." });

            try
            {
                await _service.UpdateAsync(dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================================================
        // DELETE
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================================================
        // SELECT2 SEARCH
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> SearchSupplier(string term)
        {
            var data = await _service.SearchSupplier(term ?? "");
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> SearchItem(string term)
        {
            var data = await _service.SearchItem(term ?? "");
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> SearchUnit(string term)
        {
            var data = await _service.SearchUnit(term ?? "");
            return Json(data);
        }
    }
}