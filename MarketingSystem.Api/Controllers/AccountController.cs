using MarketingSystem.Common;
using MarketingSystem.Core.Errors;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Dtos.Account;
using MarketingSystem.Service.Dtos.LoginHistory;
using MarketingSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MarketingSystem.Api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ILoginHistoryService _historyService;
        private readonly UserManager<Account> _accountManager;
        private readonly ILogger _logger;
        private readonly SignInManager<Account> _signInManager;

        public AccountController(
            ILoginHistoryService historyService,
            UserManager<Account> accountManager,
            ILogger<AccountController> logger,
            SignInManager<Account> signInManager)
        {
            _historyService = historyService;
            _accountManager = accountManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("checkToken")]
        public bool CheckToken(CheckTokenLoginNeareastDto checkTokenLoginNeareastDto)
        {
            var result = _historyService.CheckTokenLoginNeareast(checkTokenLoginNeareastDto);
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Email))
            {
                throw new CoreException("Email is required");
            }
            else if (string.IsNullOrEmpty(registerDto.Password))
            {
                throw new CoreException("Password is required");
            }
            else if (string.IsNullOrEmpty(registerDto.ConfirmPassword))
            {
                throw new CoreException("Confirm password is required");
            }
            else if (string.IsNullOrEmpty(registerDto.FullName))
            {
                throw new CoreException("Fullname is required");
            }
            else
            {
                var account = new Account
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    PhoneNumber = registerDto.PhoneNumber,
                    IsActive = false
                };
                var result = await _accountManager.CreateAsync(account, registerDto.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(account, isPersistent: false);
                    _logger.LogInformation("Account with email " + registerDto.Email + " created a new account with password.");
                }
            }
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Query(int skip = 0, int take = 10)
        {
            var tmp = "AA AA".Split(' ')[0];
            var queryable = _accountManager.Users
                .Select(x => new AccountsDto
                {
                    CreatedDate = x.CreatedDate,
                    Email = x.Email,
                    FullName = x.FullName,
                    FullNameAcronym = GetFullNameAcronym(x.FullName),
                    Id = x.Id,
                    PhoneNumber = x.PhoneNumber,
                    UpdatedDate = x.UpdatedDate
                });
            var accounts = new QueryResult<AccountsDto>
            {
                Items = await queryable.ToListAsync(),
                Count = await queryable.CountAsync()
            };
            return Ok(accounts);
        }

        private string GetFullNameAcronym(string fullName)
        {
            var lastNameAcronym = fullName.Split(' ')[0].Substring(0, 1);
            var firstNameAcronym = fullName.Split(' ')[fullName.Split(' ').Length - 1].Substring(0, 1);
            var result = lastNameAcronym + firstNameAcronym;
            return result;
        }
    }
}