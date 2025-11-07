using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы JOBS
    /// </summary>
    [Table("JOBS")]
    public class Job
    {
        [Key]
        [Column("JOB_ID")]
        [StringLength(10)]
        public string JobId { get; set; } = string.Empty;

        [Column("JOB_TITLE")]
        [StringLength(35)]
        public string JobTitle { get; set; } = string.Empty;

        [Column("MIN_SALARY")]
        public int? MinSalary { get; set; } = null;

        [Column("MAX_SALARY")]
        public int? MaxSalary { get; set; } = null;

        /// <summary>
        /// JOB_ID (EMPLOYEES) -> JOB_ID (THIS)
        /// </summary>
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        /// <summary>
        /// JOB_ID (JOB_HISTORY) -> JOB_ID (THIS)
        /// </summary>
        public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();
    }
}
