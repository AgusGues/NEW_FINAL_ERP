using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Services;


namespace NEW_FINAL_ERP.Controllers
{
    public class BranchController : Controller
    {
        private readonly BranchService _service;
        private readonly CompanyService _company;

        public BranchController(BranchService s, CompanyService c)
        {
            _service = s;
            _company = c;
        }

        public async Task<IActionResult> Index()
            => View(await _service.GetAll());

        public async Task<IActionResult> Create()
        {
            ViewBag.CompanyList = await _company.GetAll();
            return View(new Branch());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Branch model)
        {
            ViewBag.CompanyList = await _company.GetAll();

            if (!ModelState.IsValid)
                return View(model);

            await _service.Create(model);
            return RedirectToAction("Index");
        }
    }

}
