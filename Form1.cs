using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_task
{
    public partial class Form1 : Form
    {
        private MsSQLDataService _dbService = null;
        private readonly Dictionary<int, string> _comboBoxToQuery = new Dictionary<int, string>();
        public Form1()
        {
            InitializeComponent();
            InitComboBox();
        }

        public async Task InitDb()
        {
            string dbName = "test_task";
            string pathToMaster = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";
            string pathToDb = $@"Server=(localdb)\mssqllocaldb;Database={dbName};Trusted_Connection=True;";
            _dbService = new MsSQLDataService(dbName, pathToDb, pathToMaster);

            await _dbService.InitAsync();
        }

        private async void MainForm_OnLoad(object sender, System.EventArgs e)
        {
            await InitDb();

            _comboBoxToQuery[0] = @"
                SELECT e.FIRST_NAME, e.LAST_NAME, e.EMAIL, e.PHONE_NUMBER
                FROM EMPLOYEES AS e
                    JOIN EMPLOYEES m ON e.MANAGER_ID = m.EMPLOYEE_ID
                WHERE e.HIRE_DATE BETWEEN '2023-01-01' AND '2025-01-01'
                AND YEAR(m.HIRE_DATE) = 2025;";

            _comboBoxToQuery[1] = @"SELECT e.FIRST_NAME, j.JOB_TITLE, d.DEPARTMENT_NAME
                FROM EMPLOYEES as e
                    JOIN JOBS j ON e.JOB_ID = j.JOB_ID
                    JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID;";

            _comboBoxToQuery[2] = @"SELECT TOP 1 l.CITY, SUM(e.SALARY) AS 'SUM'
                FROM EMPLOYEES as e
                    JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID
                    JOIN LOCATIONS l ON l.LOCATION_ID = d.LOCATION_ID
                    JOIN JOBS j ON j.JOB_ID = e.JOB_ID
                GROUP BY l.CITY
                ORDER BY SUM(e.SALARY)DESC;";

            _comboBoxToQuery[3] = @"SELECT e.FIRST_NAME, e.LAST_NAME, e.EMAIL, e.PHONE_NUMBER
                FROM EMPLOYEES AS e
                    JOIN EMPLOYEES m ON e.MANAGER_ID = m.EMPLOYEE_ID
                    JOIN JOBS j ON j.JOB_ID = e.JOB_ID
                WHERE MONTH(m.HIRE_DATE) = 3
                AND
                LEN(j.JOB_TITLE) > 16;";

            _comboBoxToQuery[4] = @"SELECT e.FIRST_NAME
                FROM EMPLOYEES AS e
	                JOIN DEPARTMENTS d ON d.DEPARTMENT_ID = e.DEPARTMENT_ID
	                JOIN LOCATIONS l ON l.LOCATION_ID = d.LOCATION_ID
	                JOIN COUNTRIES c ON c.COUNTRY_ID = l.COUNTRY_ID
	                JOIN REGIONS r ON r.REGION_ID = c.REGION_ID
                WHERE r.REGION_NAME = 'Europe'
                AND
                c.COUNTRY_NAME = 'Russia'";

            _comboBoxToQuery[5] = @"SELECT e.FIRST_NAME, e.LAST_NAME, d.*, j.*, l.STREET_ADDRESS, c.COUNTRY_NAME, r.REGION_NAME
                FROM EMPLOYEES AS e
                    JOIN JOBS j ON j.JOB_ID = e.JOB_ID
                    JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID
                    JOIN LOCATIONS l ON l.LOCATION_ID = d.LOCATION_ID
                    JOIN COUNTRIES c ON c.COUNTRY_ID = l.COUNTRY_ID
                    JOIN REGIONS r ON r.REGION_ID = c.REGION_ID";

            _comboBoxToQuery[6] = @"SELECT r.REGION_NAME, COUNT(e.EMPLOYEE_ID) AS EmployeeCount
                FROM REGIONS AS r
	                JOIN COUNTRIES c ON c.REGION_ID = r.REGION_ID
	                JOIN LOCATIONS l ON l.COUNTRY_ID = c.COUNTRY_ID
	                JOIN DEPARTMENTS d ON d.LOCATION_ID = l.LOCATION_ID
	                JOIN EMPLOYEES e ON e.DEPARTMENT_ID = d.DEPARTMENT_ID
                GROUP BY r.REGION_NAME;";

            _comboBoxToQuery[7] = @"SELECT
	                d.DEPARTMENT_NAME,
                    MIN(j.MIN_SALARY) AS 'Мин.ЗП',
                    MAX(j.MAX_SALARY) AS 'Макс.ЗП',
	                MIN(e.HIRE_DATE) AS 'Мин.время найма', MAX(e.HIRE_DATE) AS 'Макс.время найма',
	                COUNT(EMPLOYEE_ID) AS 'Кол-во сотрудников'
                FROM DEPARTMENTS AS d
                    JOIN EMPLOYEES e ON d.DEPARTMENT_ID = e.DEPARTMENT_ID
                    JOIN JOBS j ON j.JOB_ID = e.JOB_ID
                GROUP BY d.DEPARTMENT_NAME
                ORDER BY COUNT(EMPLOYEE_ID) DESC;";

            _comboBoxToQuery[8] = @"SELECT e.FIRST_NAME, e.LAST_NAME, LEFT(e.PHONE_NUMBER, 3) AS 'Номер'
                FROM EMPLOYEES AS e
                WHERE e.PHONE_NUMBER LIKE '___.___.__.__';";

            _comboBoxToQuery[9] = @"SELECT e.FIRST_NAME, e.LAST_NAME
                FROM DEPARTMENTS AS d
                    JOIN EMPLOYEES e ON e.DEPARTMENT_ID = d.DEPARTMENT_ID
                WHERE d.DEPARTMENT_NAME = 'DAD'";
        }

        private void InitComboBox()
        {
            string[] queries = new string[] {
                "1.Отобразить реквизиты сотрудников, менеджеры которых устроились на работу в 2025 году," +
                    "но при это сами эти работники устроились на работу между 2023м и 2025м годами",
                "2.Отобразить данные по сотрудникам: из какого департамента и какими текущими задачами занимается. " +
                    "Результат отобразить в трех полях: employees.first_name, jobs.job_title, departments.department_name",
                "3.Отобразить город, в котором сотрудники в сумме зарабатывают больше всех.",
                "4.Вывести все реквизиты сотрудников, менеджеры которых устроились на работу в марте месяце любого года и" +
                    "длина job_title этих сотрудников больше 16 символа",
                "5.Вывести реквизит first_name сотрудников, которые живут в Europe (region_name) и Russia (country_name)",
                "6.Получить детальную информацию о каждом сотруднике:First_name, Last_name, Departament, Job, Street, Country, Region",
                "7.Отразить регионы и количество сотрудников в каждом регионе",
                "8.Вывести информацию по департаменту department_name с минимальной и максимальной зарплатой," +
                    "с ранней и поздней датой найма на работу и с количеством сотрудников. Сортировать по количеству сотрудников (по убыванию)",
                "9.Получить список реквизитов сотрудников FIRST_NAME, LAST_NAME и первые три цифры от номера телефона," +
                    "если его номер в формате ХХХ.ХХХ.ХХХХ",
                "10.Вывести список сотрудников, которые работают в департаменте администрирования доходов" +
                    "(departments.department_name = 'DAD')" };


            PickQueryComboBox.Items.AddRange(queries);
        }

        private async void RunButton_OnClick(object sender, EventArgs e)
        {
            int index = PickQueryComboBox.SelectedIndex;
            if (index != -1)
            {
                if (_comboBoxToQuery.TryGetValue(index, out var query))
                {
                    this.dbResultDataGrid.DataSource = await _dbService.QueryByStringAsync(query);
                }
                else
                {
                    Console.WriteLine($"RunButton_OnClick: _comboBoxToAction.TryGetValue = false");
                }

            }
            else
            {
                Console.WriteLine($"RunButton_OnClick: Не выбран элемент комбо-бокса!");
            }
        }

        private void PickQueryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
