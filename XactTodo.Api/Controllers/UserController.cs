using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XactTodo.Api.Queries;
using XactTodo.Api.Utils;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Security.Session;

namespace XactTodo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IUserRepository userRepository;
        private readonly IClaimsSession session;
        private readonly SmtpConfig smtpConfig;

        public UserController(
            ILogger<UserController> logger,
            IUserRepository userRepository,
            IClaimsSession session,
            IOptions<SmtpConfig> smtpConfig)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.session = session;
            this.smtpConfig = smtpConfig.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCode(SendVerificationCodeInput input)
        {
            var user = userRepository.GetAll().FirstOrDefault(p => p.UserName == input.UserName && (!p.EmailConfirmed || p.Email == input.Email));
            if (user == null)
            {
                return BadRequest("未找到与指定用户名及电邮地址相符的账号");
            }
            user.Email = input.Email;
            userRepository.Update(user);
            try
            {
                await SendVerificationCode(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "邮件发送失败！" + ex.Message);
            }
            await userRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        private async Task SendVerificationCode(User user)
        {
            await EmailHelper.SendMailAsync(
                smtpConfig.Host,
                smtpConfig.EnableSsl,
                smtpConfig.UserName,
                smtpConfig.Password,
                smtpConfig.EmailAddress,
                user.DisplayName,
                user.Email,
                "验证码",
                "您的验证码为："
                );
        }

        [Route("{id:int}")] //加上类型声明的好处是，如果传入的参数不是整数则直接返回404，不加则返回400并报告错误"The value 'xxx' is not valid."
        [HttpGet]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = userRepository.FindById(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("[action]")]
        [Authorize]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> CurrentUser()
        {
            return await GetById(session.UserId.Value);
        }

    }

    public class SendVerificationCodeInput
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}