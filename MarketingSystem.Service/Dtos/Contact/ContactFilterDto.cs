using System;
using MarketingSystem.Model.Entities;

namespace MarketingSystem.Service.Dtos.Contact
{
    public class ContactFilterDto: BaseDto
    {
        public string Data { get; set; }

        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string Domain { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmAdset { get; set; }
        public string UtmAds { get; set; }
        public string UtmAgent { get; set; }
        public string UtmMedium { get; set; }
        public string UtmSource { get; set; }
        public string UtmTeam { get; set; }
        public string AdsLink { get; set; }
    }
}