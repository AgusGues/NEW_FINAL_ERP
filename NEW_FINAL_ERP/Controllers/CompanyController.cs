using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;


namespace NEW_FINAL_ERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly CompanyService _service;

        public CompanyController(CompanyService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.Create(model);

            return Ok();
        }

    }


}
