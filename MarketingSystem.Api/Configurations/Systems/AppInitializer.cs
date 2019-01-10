using System.Collections.Generic;
using System.IO;
using MarketingSystem.Model;
using MarketingSystem.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MarketingSystem.Api.Extensions;
using Newtonsoft.Json;

namespace MarketingSystem.Api.Configurations.Systems
{
    public class AppInitializer : IWebHostInitializer
    {
        private readonly UserManager<Account> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AppInitializer(
            IConfiguration configuration,
            AppDbContext context,
            UserManager<Account> userManager)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        public async Task InitAsync()
        {
            await _context.Database.MigrateAsync();
            await InitAccount();
            //await InitContact();
        }

        private string CreatePath(string jsonFile)
        {
            return "Configurations/Initializes/" + jsonFile;
        }

        private async Task InitAccount()
        {
            var account = _configuration.GetSection("DefaultAdmin").Get<Account>();
            if (account is null)
            {
                return;
            }
            account.UserName = account.Email;
            account.IsActive = account.IsActive;
            if (await _userManager.FindByEmailAsync(account.Email) != null)
            {
                return;
            }
            await _userManager.CreateAsync(account, _configuration["DefaultAdmin:Password"]);
        }

        private async Task InitContact()
        {
            if (!await _context.Contacts.AnyAsync())
            {
                var input = File.ReadAllText(CreatePath("default-contact.json"));
                var contacts = JsonConvert.DeserializeObject<List<Contact>>(input);
                _context.Contacts.AddRange(contacts);
                await _context.SaveChangesAsync();
            }
        }
    }
}