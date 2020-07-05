using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLGeneral.WebApi.Models;
using JLGeneral.WebApi.Services;
using JLGeneral.WebApi.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JLGeneral.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailerController : ControllerBase
    {
        private readonly IMailerService mailerService;
        private readonly ILogger<MailerController> _logger;
        private readonly MailSettings _mailSettings;

        public MailerController(IMailerService mailerService, ILogger<MailerController> logger, IOptions<MailSettings> mailSettings)
        {
            this.mailerService = mailerService;
            _logger = logger;
            _mailSettings = mailSettings.Value;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request, string Token)
        {
            if (Token != _mailSettings.Token)
            {
                _logger.LogError("Unauthorised access attempt: " + request.ToEmail);
                return Unauthorized();
            }
            try
            {
                _logger.LogInformation("Authorised Access");
                await mailerService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Area connecting.");
                throw;
            }

        }
        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeMail([FromForm] WelcomeRequest request, string Token)
        {
            if (Token != _mailSettings.Token)
            {
                _logger.LogError("Unauthorised access attempt: " + request.ToEmail);
                return Unauthorized("You need to be authorised to use this service!");
            }
            try
            {
                _logger.LogInformation("Authorised Access");
                _logger.LogInformation("Email sent to: " + request.ToEmail);
                await mailerService.SendWelcomeEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Area connecting.");
                throw;
            }

        }
    }
}