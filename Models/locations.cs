using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы LOCATIONS
    /// </summary>
    [Table("LOCATIONS")]
    public class Location
    {
        [Key]
        [Column("LOCATION_ID")]
        public int LocationId { get; set; }

        [Column("STREET_ADDRESS")]
        [StringLength(40)]
        public string StreetAddress { get; set; } = string.Empty;

        [Column("POSTAL_CODE")]
        [StringLength(12)]
        public string PostalCode { get; set; } = string.Empty;

        [Column("CITY")]
        [StringLength(30)]
        public string City { get; set; } = string.Empty; // NOT NULL в БД

        [Column("STATE_PROVINCE")]
        [StringLength(25)]
        public string StateProvince { get; set; } = string.Empty;

        [Column("COUNTRY_ID")]
        [StringLength(2)] // CHAR(2)
        public string CountryId { get; set; } = string.Empty; // Nullable, так как в БД может быть NULL

        /// <summary>
        /// COUNTRY_ID (THIS) -> REGION_ID (REGIONS)
        /// </summary>
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; } = null;

        /// <summary>
        /// LOCATION_ID (THIS) -> LOCATION_ID (DEPARTMENTS)
        /// </summary>
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
