using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class UnitController : Controller
    {
        private readonly UnitService _service;

        public UnitController(UnitService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        public IActionResult Create()
        {
            return PartialView("_Form", new Unit());
        }

        [HttpPost]
        public async Task<IActionResult> Save(Unit model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.UnitId == 0)
                await _service.Create(model);
            else
                await _service.Update(model);

            return Ok();
        }

        public async Task<IActionResult> Edit(int id)
        {
            return PartialView("_Form", await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok();
        }

    }
}
