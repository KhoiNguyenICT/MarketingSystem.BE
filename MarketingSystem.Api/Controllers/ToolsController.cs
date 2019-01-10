using System.Linq;
using System.Threading.Tasks;
using MarketingSystem.Common;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketingSystem.Api.Controllers
{
    public class ToolsController : BaseController
    {
        private readonly IUrlTrackingService _urlTrackingService;

        public ToolsController(IUrlTrackingService urlTrackingService)
        {
            _urlTrackingService = urlTrackingService;
        }

        [HttpPost("generateTracking")]
        public async Task<IActionResult> GenerateTracking(UrlTracking linkTracking)
        {
            await _urlTrackingService.Add(linkTracking);
            return Ok();
        }

        [HttpGet("listUrlTracking")]
        public async Task<IActionResult> ListUrlTracking(int skip = 0, int take = 10, string query = null)
        {
            var queryable = _urlTrackingService.Queryable();

            var list = new QueryResult<UrlTracking>
            {
                Items = await queryable
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(take)
                    .Select(x => new UrlTracking()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Domain = x.Domain,
                        CreatedDate = x.CreatedDate,
                        UpdatedDate = x.UpdatedDate
                    }).ToListAsync(),
                Count = await queryable.CountAsync()
            };
            return Ok(list);
        }
    }
}