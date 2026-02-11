using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class BranchController : Controller
    {
        private readonly BranchService _service;
        private readonly CompanyService _companyService;

        public BranchController(
            BranchService service,
            CompanyService companyService)
        {
            _service = service;
            _companyService = companyService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        public IActionResult Create()
        {
            LoadCompany();
            return PartialView("_Form", new Branch());
        }

        public async Task<IActionResult> Edit(int id)
        {
            LoadCompany();
            return PartialView("_Form", await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Save(Branch model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.BranchId == 0)
                await _service.Create(model);
            else
                await _service.Update(model);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok();
        }

        private void LoadCompany()
        {
            ViewBag.CompanyList =
                new SelectList(_companyService.GetAll().Result,
                    "CompanyId",
                    "CompanyName");
        }
    }
}
