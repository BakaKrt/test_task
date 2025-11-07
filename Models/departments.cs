using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы DEPARTMENTS
    /// </summary>
    [Table("DEPARTMENTS")]
    public class Department
    {
        [Key]
        [Column("DEPARTMENT_ID")]
        public int DepartmentId { get; set; }

        [Column("DEPARTMENT_NAME")]
        [StringLength(30)]
        public string DepartmentName { get; set; } = string.Empty; // NOT NULL в БД

        [Column("MANAGER_ID")]
        public int? ManagerId { get; set; } // Nullable, так как в БД может быть NULL

        [Column("LOCATION_ID")]
        public int? LocationId { get; set; } // Nullable, так как в БД может быть NULL

        /// <summary>
        /// LOCATION_ID (LOCATIONS) -> LOCATION_ID (THIS)
        /// </summary>
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; } = null;

        /// <summary>
        /// MANAGER_ID (THIS) -> EMPLOYEE_ID (EMPLOYEES)
        /// </summary>
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; } = null;

        /// <summary>
        /// DEPARTMENT_ID (JOB_HISTORY) -> DEPARTMENT_ID (THIS)
        /// </summary>
        public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

        /// <summary>
        /// DEPARTMENT_ID (EMPLOYEES) -> DEPARTMENT_ID (THIS)
        /// </summary>
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
