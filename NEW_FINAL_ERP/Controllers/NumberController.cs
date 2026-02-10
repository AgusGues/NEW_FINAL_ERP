using Microsoft.AspNetCore.Mvc;
using NEW_FINAL_ERP.Services;

namespace NEW_FINAL_ERP.Controllers
{
    public class NumberController : Controller
    {
        private readonly NumberService _service;

        public NumberController(NumberService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(int companyId, string entityName)
        {
            try
            {
                var documentId = Guid.NewGuid();
                var number = _service.Generate(companyId, entityName, documentId);

                ViewBag.Number = number;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }
    }
}
