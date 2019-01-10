using System;

namespace MarketingSystem.Model.Interfaces
{
    public interface ITracking
    {
        string Domain { get; set; }
        string UtmCampaign { get; set; }
        string UtmAdset { get; set; }
        string UtmAds { get; set; }
        string UtmAgent { get; set; }
        string UtmMedium { get; set; }
        string UtmSource { get; set; }
        string UtmTeam { get; set; }
        Guid AdsLinkId { get; set; }
    }
}