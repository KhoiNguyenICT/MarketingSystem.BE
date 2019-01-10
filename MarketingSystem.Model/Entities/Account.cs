using System;
using System.ComponentModel.DataAnnotations.Schema;
using MarketingSystem.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MarketingSystem.Model.Entities
{
    public class Account : IdentityUser<Guid>, IEntity
    {
        public string FullName { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}