using BnipiTest.CryptContext;
using BnipiTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BnipiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthorizationController : ControllerBase
    {
        private static string str = "Hello, World!"; 
        [HttpGet]
        public IActionResult GetAuthData()
        {
            var auth = new AuthData()
            {
                Data = str,
                UUID = new Guid().ToString(),//Заглушка
            };
            return Ok(auth); //Вот эту строку на стороне клиента надо подписать
        }
        [HttpPost]
        public IActionResult SaveCert(Cert cert)
        {
            var crypt = new Crypt();
            crypt.SaveCert(cert.base64Cert);
            return Ok();
        }
    }

}
