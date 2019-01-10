using System;
using System.ComponentModel.DataAnnotations.Schema;
using MarketingSystem.Model.Interfaces;

namespace MarketingSystem.Model.Entities
{
    public class Contact : BaseEntity
    {
        public string Data { get; set; }

        public Guid AdsLinkId { get; set; }

        [ForeignKey("AdsLinkId")]
        public virtual UrlTracking UrlTracking { get; set; }
    }
}