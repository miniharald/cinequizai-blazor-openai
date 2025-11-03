using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace CineQuizAI.Web.Controllers
{
    [Route("auth")]
    public class AntiforgeryController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        public AntiforgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpGet("antiforgery-token")]
        public IActionResult GetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            // Use the correct form field name from the tokens object
            var html = $"<input name=\"{tokens.FormFieldName}\" type=\"hidden\" value=\"{tokens.RequestToken}\" />";
            return Content(html, "text/html");
        }
    }
}
