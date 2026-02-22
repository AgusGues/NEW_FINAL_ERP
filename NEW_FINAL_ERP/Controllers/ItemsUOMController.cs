using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class ItemsUOMController : Controller
    {
        private readonly ItemsUOMService _service;

        public ItemsUOMController(ItemsUOMService service)
        {
            _service = service;
            
        }
        // =========================================
        // INDEX (List View)
        // =========================================
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAll();
            return View(data);
        }

        //Auto complete Items
        [HttpGet]
        public async Task<IActionResult> SearchItem(string term)
        {
            var result = await _service.SearchItemAsync(term);
            return Json(result);
        }

        //Auto complete Unit
        [HttpGet]
        public async Task<IActionResult> SearchUnit(string term)
        {
            var result = await _service.SearchUnitAsync(term);
            return Json(result);
        }

        // =========================================
        // LOAD MODAL (Create + Edit)
        // =========================================
        public async Task<IActionResult> Modal(int id = 0)
        {
            var dto = await _service.GetModalDtoAsync(id);
            return PartialView("_ItemUOMModal", dto);
        }


        // =========================================
        // SAVE (Create + Update)
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Modal(ItemUOMModalDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToArray();

                return Json(new { success = false, errors });
            }

            try
            {
                var entity = new ItemsUom
                {
                    ItemUOMId = dto.ItemUOMId,
                    ItemId = dto.ItemId,
                    UnitId = dto.UnitId,
                    ConversionToBase = dto.ConversionToBase,
                    IsBase = dto.IsBase,
                    IsDefaultSales = dto.IsDefaultSales,
                    IsDefaultPurchase = dto.IsDefaultPurchase,
                    Barcode = dto.Barcode,
                    IsInternalBarcode = false,
                    IsActive = true
                };

                if (dto.ItemUOMId == 0)
                    await _service.Create(entity);
                else
                    await _service.Update(entity);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================================
        // DELETE (Soft Delete)
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Json(new { success = true });
        }
    }
}
