using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class NumberSequenceController : Controller
    {
        private readonly NumberSequenceService _service;

        public NumberSequenceController(NumberSequenceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Create(NumberSequence model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.Create(model);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
