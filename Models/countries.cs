using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы COUNTRIES
    /// </summary>
    [Table("COUNTRIES")]
    public class Country
    {
        [Key]
        [Column("COUNTRY_ID")]
        [StringLength(2)] // CHAR(2)
        public string CountryId { get; set; } = string.Empty;

        [Column("COUNTRY_NAME")]
        [StringLength(40)]
        public string CountryName { get; set; } = string.Empty;

        [Column("REGION_ID")]
        public int? RegionId { get; set; } // Nullable, так как в БД может быть NULL

        /// <summary>
        /// REGION_ID (THIS) -> REGION_ID (REGIONS)
        /// </summary>
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; } = null;

        /// <summary>
        /// COUNTRY_ID (LOCATIONS) -> COUNTRY_ID (THIS)
        /// </summary>
        public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
