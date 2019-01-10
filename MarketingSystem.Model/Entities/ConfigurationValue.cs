using System;
using System.ComponentModel.DataAnnotations;

namespace MarketingSystem.Model.Entities
{
    public class ConfigurationValue
    {
        [Key]
        [Required]
        [StringLength(255)]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}