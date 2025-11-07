using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы REGIONS
    /// </summary>
    [Table("REGIONS")]
    public class Region
    {
        [Key]
        [Column("REGION_ID")]
        public int RegionId { get; set; }

        [Column("REGION_NAME")]
        [StringLength(25)]
        public string RegionName { get; set; } = string.Empty;

        /// <summary>
        /// COUNTRY_ID (LOCATIONS) -> REGION_ID (THIS)
        /// </summary>
        public virtual ICollection<Country> Countries { get; set; } = new List<Country>();
    }
}
