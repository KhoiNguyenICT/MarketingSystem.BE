using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingSystem.Common;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Dtos.Contact;
using MarketingSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketingSystem.Api.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("tracking/{adsLinkId}/{data}")]
        [AllowAnonymous]
        public async Task<IActionResult> Tracking(string data, Guid adsLinkId)
        {
            var contact = new Contact
            {
                AdsLinkId = adsLinkId,
                Data = data
            };
            await _contactService.Add(contact);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Query(int skip = 0, int take = 10,[FromQuery] ContactFilterDto contactFilter = null)
        {
            var queryable = _contactService.Queryable();

            if (contactFilter?.Domain != null)
            {
                queryable = queryable.Where(x => x.UrlTracking.Domain.Contains(contactFilter.Domain));
            }

            if (contactFilter?.FromTime != DateTime.MinValue && contactFilter?.ToTime != DateTime.MinValue)
            {
                queryable = queryable.Where(x => x.CreatedDate.Date >= contactFilter.FromTime.Date && x.CreatedDate.Date <= contactFilter.ToTime.Date);
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmCampaign))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmCampaign.Contains(contactFilter.UtmCampaign));
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmAdset))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmAdset.Contains(contactFilter.UtmAdset));
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmAds))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmAds.Contains(contactFilter.UtmAds));
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmAgent))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmAgent.Contains(contactFilter.UtmAgent));
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmMedium))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmMedium.Contains(contactFilter.UtmMedium));
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmSource))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmSource.Contains(contactFilter.UtmSource));
            }

            if (contactFilter != null && !string.IsNullOrEmpty(contactFilter.UtmTeam))
            {
                queryable = queryable.Where(x => x.UrlTracking.UtmTeam.Contains(contactFilter.UtmTeam));
            }

            var list = new QueryResult<Contact>
            {
                Items = await queryable
                    .Include(x=>x.UrlTracking)
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(take)
                    .Select(x => new Contact()
                    {
                        Id = x.Id,
                        Data = x.Data,
                        AdsLinkId = x.AdsLinkId,
                        UrlTracking = x.UrlTracking,
                        CreatedDate = x.CreatedDate,
                        UpdatedDate = x.UpdatedDate
                    }).ToListAsync(),
                Count = await queryable.CountAsync()
            };
            return Ok(list);
        }
    }
}