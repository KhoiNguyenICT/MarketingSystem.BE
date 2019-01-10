using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MarketingSystem.Model.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        [StringLength(250)]
        public string Description { get; set; }
    }
}