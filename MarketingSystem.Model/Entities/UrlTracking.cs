using System;
using System.ComponentModel.DataAnnotations.Schema;
using MarketingSystem.Model.Interfaces;

namespace MarketingSystem.Model.Entities
{
    public class UrlTracking: BaseEntity
    {
        public string Name { get; set; }

        public string Domain { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmAdset { get; set; }
        public string UtmAds { get; set; }
        public string UtmAgent { get; set; }
        public string UtmMedium { get; set; }
        public string UtmSource { get; set; }
        public string UtmTeam { get; set; }
        public Guid CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual Account Account { get; set; }
    }
}