using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ItemsServices _service;
        private readonly UnitService _unitService;

        public ItemsController(ItemsServices service, UnitService unitService)
        {
            _service = service;
            _unitService = unitService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        
        public IActionResult Create()
        {
            LoadUnit();
            return PartialView("_Form", new Items());
        }

        [HttpPost]
        public async Task<IActionResult> Save(Items model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.ItemId == 0)
                await _service.Create(model);
            else
                await _service.Update(model);

            return Ok();

        }

        public async Task<IActionResult> Edit(int id)
        {
            LoadUnit();
            return PartialView("_Form", await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok();
        }

        private void LoadUnit()
        {
            ViewBag.UnitList = new SelectList(_unitService.GetAll().Result,"UnitId","UnitName");
        }
    }
}
