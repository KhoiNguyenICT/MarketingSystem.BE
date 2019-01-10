using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using MarketingSystem.Common.Attributes;

namespace MarketingSystem.Model.Entities
{
    public class Asset: BaseEntity
    {
        [Required]
        public string FileName { get; set; }

        [StringLength(50)]
        public string FileExtension { get; set; }

        public long FileSize { get; set; }

        [Required]
        public string FilePath { get; set; }

        [NotMapped]
        [ResolveUrl]
        public string FileUrl => Path.GetFileName(FilePath);
    }
}