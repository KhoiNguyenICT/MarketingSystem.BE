using System.Threading.Tasks;
using MarketingSystem.Common;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MarketingSystem.Api.Controllers
{
    public class DomainController : BaseController
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var queryable = await _domainService.Queryable().OrderBy(x => x.Name).ToListAsync();
            return Ok(queryable);
        }

        [HttpGet]
        public async Task<IActionResult> Query(int skip = 0, int take = 10, string query = null)
        {
            var queryable = _domainService.Queryable();

            var list = new QueryResult<Domain>
            {
                Items = await queryable
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(take)
                    .Select(x => new Domain()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.Address,
                        CreatedDate = x.CreatedDate,
                        UpdatedDate = x.UpdatedDate
                    }).ToListAsync(),
                Count = await queryable.CountAsync()
            };
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Domain domain)
        {
            await _domainService.Add(domain);
            return Ok();
        }
    }
}