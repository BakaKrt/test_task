using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_task.Models
{
    /// <summary>
    /// Модель для таблицы EMPLOYEES
    /// </summary>
    [Table("EMPLOYEES")]
    public class Employee
    {
        [Key]
        [Column("EMPLOYEE_ID")]
        public int EmployeeId { get; set; }

        [Column("FIRST_NAME")]
        [StringLength(20)]
        public string FirstName { get; set; } = string.Empty;

        [Column("LAST_NAME")]
        [StringLength(25)]
        public string LastName { get; set; } = null; // NOT NULL в БД

        [Column("EMAIL")]
        [StringLength(20)]
        public string Email { get; set; } = null; // NOT NULL в БД

        [Column("PHONE_NUMBER")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column("HIRE_DATE")]
        public DateTime HireDate { get; set; } = DateTime.Now; // NOT NULL в БД

        [Column("JOB_ID")]
        [StringLength(10)]
        public string JobId { get; set; } = string.Empty; // NOT NULL в БД

        [Column("SALARY")]
        public decimal? Salary { get; set; }

        [Column("COMMISSION_PCT")]
        public decimal? CommissionPct { get; set; }

        [Column("MANAGER_ID")]
        public int? ManagerId { get; set; }

        [Column("DEPARTMENT_ID")]
        public int? DepartmentId { get; set; }

        /// <summary>
        /// JOB_ID (THIS) -> JOB_ID (JOBS)
        /// </summary>
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null;

        /// <summary>
        /// Менеджер <br/>
        /// MANAGER_ID (DEPARTMENTS) -> EMPLOYEE_ID (THIS)
        /// </summary>
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }

        /// <summary>
        /// В каком департаменте<br/>
        /// DEPARTMENT_ID (THIS) -> DEPARTMENT_ID (DEPARTMENTS)
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        /// <summary>
        /// Подчинённые<br/>
        /// MANAGER_ID (THIS) -> EMPLOYEE_ID (THIS)
        /// </summary>
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        /// <summary>
        /// Где работал <br/>
        /// EMPLOYEE_ID (JOB_HISTORY) -> EMPLOTEE_ID (EMPLOYEES)
        /// </summary>
        public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();
    }
}
