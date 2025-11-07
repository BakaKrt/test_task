using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы JOB_HISTORY
    /// </summary>
    [Table("JOB_HISTORY")]
    public class JobHistory
    {
        [Key]
        [Column("EMPLOYEE_ID")]
        public int EmployeeId { get; set; }

        [Key]
        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        [Column("END_DATE")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Column("JOB_ID")]
        [StringLength(10)]
        public string JobId { get; set; } = null;

        [Column("DEPARTMENT_ID")]
        public int? DepartmentId { get; set; } = null;

        /// <summary>
        /// EMPLOYEE_ID (THIS) -> EMPLOYEE_ID (EMPLOYEES)
        /// </summary>
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null;

        /// <summary>
        /// JOB_ID (THIS) -> JOB_ID (JOBS)
        /// </summary>
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null;

        /// <summary>
        /// DEPARTMENT_ID (THIS) -> DEPARTMENT_ID (DEPARTMENTS)
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null;
    }
}
