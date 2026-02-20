using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult>Index()
        {
            return View(await _service.GetAll());
        }

        public IActionResult Create()
        {
            //LoadUnit();
            return PartialView("_Form", new Items());
        }

        [HttpPost]
        public async Task<IActionResult> Save(ItemsUom model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.ItemId == 0)
                await _service.Create(model);
            //else
            //    await _service.Update(model);

            return Ok();

        }
    }
}
