using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL;

namespace WorkForceManagement.WEB.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMailService _mailService;
        public EmailController(IMailService mailService)
        {
            _mailService = mailService;
        }
        [HttpPost]
        public async Task<ActionResult> Send([FromForm] MailRequest request)
        {
            try
            {
                await _mailService.SendEmail(request);
                return Ok();
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
