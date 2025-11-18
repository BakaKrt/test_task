using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using test_task.DataBase;

/* Не совсем понял задание с T-SQL и MSSQL. Видимо смысл был такой: (выполнением указанных запросов на языке T-SQL, используя драйвер MSSQL) ИЛИ (PostgreSQL)
 * Я написал на MSSQL
 * Не скрываю, частично пользовался ИИ. Уже работал с WinForms, а вот с MSSQL не приходилось.
 */

namespace test_task
{
    public class MsSQLDataService : IDisposable
    {
        private bool _disposed = false;
        private SqlConnection _connection;
        private readonly string _pathToMaster;
        private readonly string _pathToDb;
        private readonly string _dbName;


        public MsSQLDataService(string dbName, string pathToDb, string pathToMaster)
        {
            this._dbName = dbName;
            this._pathToDb = pathToDb;
            this._pathToMaster = pathToMaster;
        }

        /// <summary>
        /// Инициализация БД, проверка на существование (включая создание бд при её отсутствии и наполнению данными)
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            this._connection = new SqlConnection(_pathToMaster);

            try
            {
                Console.WriteLine($"Пытаемся подключиться к {_pathToMaster}");
                await _connection.OpenAsync();
                Console.WriteLine("Подключились");

                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine($"Не удалось подключиться к {_pathToMaster}");
                    _connection.Close();
                    _connection.Dispose();
                    _disposed = true;
                    return;
                }

                bool isTargetDbExists = await IsDbExist(_dbName, _connection);

                if (!isTargetDbExists) // если бд не существует, создаём и добавляем таблицы
                {
                    Console.WriteLine($"Требуемой бд '{_dbName}' не существует, создаём");
                    await CreateDatabaseIfNotExistsAsync();
                    await ChangeDb(this._pathToDb);
                    await CreateTablesIfNotExistsAsync();
                    await FillTablesIfEmptyAsync();
                }
                else
                {
                    await ChangeDb(this._pathToDb);
                }

                Console.WriteLine($"Инициализация бд успешна");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _connection.Close();
                _connection.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Проверка БД на существование
        /// </summary>
        /// <param name="dbName">Имя бд</param>
        /// <param name="con">Подключение</param>
        /// <returns>true, если БД существует, иначе false</returns>
        private async Task<bool> IsDbExist(string dbName, SqlConnection con)
        {
            SqlCommand allTables = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{dbName}';", con);
            int res = (int)(await allTables.ExecuteScalarAsync());

            if (res == 0) return false;
            return true;
        }


        /// <summary>
        /// Смена текущей базы данных на другую
        /// </summary>
        /// <param name="dbPath">Путь до БД</param>
        /// <returns>true, если смена удалась, иначе false</returns>
        private async Task<bool> ChangeDb(string dbPath)
        {
            try
            {
                this._connection.Close();
                this._connection.Dispose();

                this._connection = new SqlConnection(dbPath);
                await this._connection.OpenAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }

        /// <summary>
        /// Создаёт БД, если её нет
        /// </summary>
        /// <returns></returns>
        private async Task CreateDatabaseIfNotExistsAsync()
        {
            string createDbQuery = $@"CREATE DATABASE [{_dbName}];";

            using (var command = new SqlCommand(createDbQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Создаёт таблицы, если их нет
        /// </summary>
        /// <returns></returns>
        private async Task CreateTablesIfNotExistsAsync()
        {
            // Пример создания таблицы REGIONS
            string createRegionsTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='REGIONS' AND xtype='U')
            BEGIN
                CREATE TABLE REGIONS (
                    REGION_ID int IDENTITY(1,1) PRIMARY KEY,
                    REGION_NAME nvarchar(25)
                );
                PRINT 'Таблица REGIONS создана.';
            END";

            // Пример создания таблицы COUNTRIES
            string createCountriesTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='COUNTRIES' AND xtype='U')
            BEGIN
                CREATE TABLE COUNTRIES (
                    COUNTRY_ID char(2) PRIMARY KEY,
                    COUNTRY_NAME nvarchar(40),
                    REGION_ID int NULL,
                    FOREIGN KEY (REGION_ID) REFERENCES REGIONS(REGION_ID)
                );
                PRINT 'Таблица COUNTRIES создана.';
            END";

            // Пример создания таблицы LOCATIONS
            string createLocationsTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LOCATIONS' AND xtype='U')
            BEGIN
                CREATE TABLE LOCATIONS (
                    LOCATION_ID int IDENTITY(1,1) PRIMARY KEY,
                    STREET_ADDRESS nvarchar(40),
                    POSTAL_CODE nvarchar(12),
                    CITY nvarchar(30) NOT NULL,
                    STATE_PROVINCE nvarchar(25),
                    COUNTRY_ID char(2) NULL,
                    FOREIGN KEY (COUNTRY_ID) REFERENCES COUNTRIES(COUNTRY_ID)
                );
                PRINT 'Таблица LOCATIONS создана.';
            END";

            // Пример создания таблицы DEPARTMENTS
            string createDepartmentsTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DEPARTMENTS' AND xtype='U')
            BEGIN
                CREATE TABLE DEPARTMENTS (
                    DEPARTMENT_ID int IDENTITY(1,1) PRIMARY KEY,
                    DEPARTMENT_NAME nvarchar(30) NOT NULL,
                    MANAGER_ID int NULL,
                    LOCATION_ID int NULL,
                    FOREIGN KEY (LOCATION_ID) REFERENCES LOCATIONS(LOCATION_ID)
                    -- FOREIGN KEY (MANAGER_ID) будет добавлена позже, после создания EMPLOYEES
                );
                PRINT 'Таблица DEPARTMENTS создана.';
            END";

            // Пример создания таблицы JOBS
            string createJobsTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JOBS' AND xtype='U')
            BEGIN
                CREATE TABLE JOBS (
                    JOB_ID nvarchar(10) PRIMARY KEY,
                    JOB_TITLE nvarchar(35) NOT NULL,
                    MIN_SALARY int NULL,
                    MAX_SALARY int NULL
                );
                PRINT 'Таблица JOBS создана.';
            END";

            // Пример создания таблицы EMPLOYEES
            string createEmployeesTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EMPLOYEES' AND xtype='U')
            BEGIN
                CREATE TABLE EMPLOYEES (
                    EMPLOYEE_ID int IDENTITY(1,1) PRIMARY KEY,
                    FIRST_NAME nvarchar(20),
                    LAST_NAME nvarchar(25) NOT NULL,
                    EMAIL nvarchar(20) NOT NULL,
                    PHONE_NUMBER nvarchar(20),
                    HIRE_DATE datetime NOT NULL,
                    JOB_ID nvarchar(10) NOT NULL,
                    SALARY decimal(8,2) NULL,
                    COMMISSION_PCT decimal(2,2) NULL,
                    MANAGER_ID int NULL,
                    DEPARTMENT_ID int NULL,
                    FOREIGN KEY (JOB_ID) REFERENCES JOBS(JOB_ID),
                    FOREIGN KEY (MANAGER_ID) REFERENCES EMPLOYEES(EMPLOYEE_ID),
                    FOREIGN KEY (DEPARTMENT_ID) REFERENCES DEPARTMENTS(DEPARTMENT_ID)
                );
                PRINT 'Таблица EMPLOYEES создана.';
            END";

            // Пример создания таблицы JOB_HISTORY
            string createJobHistoryTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JOB_HISTORY' AND xtype='U')
            BEGIN
                CREATE TABLE JOB_HISTORY (
                    EMPLOYEE_ID int NOT NULL,
                    START_DATE datetime NOT NULL,
                    END_DATE datetime NOT NULL,
                    JOB_ID nvarchar(10) NOT NULL,
                    DEPARTMENT_ID int NULL,
                    PRIMARY KEY (EMPLOYEE_ID, START_DATE),
                    FOREIGN KEY (EMPLOYEE_ID) REFERENCES EMPLOYEES(EMPLOYEE_ID),
                    FOREIGN KEY (JOB_ID) REFERENCES JOBS(JOB_ID),
                    FOREIGN KEY (DEPARTMENT_ID) REFERENCES DEPARTMENTS(DEPARTMENT_ID)
                );
                PRINT 'Таблица JOB_HISTORY создана.';
            END";

            using (var command = new SqlCommand(createRegionsTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqlCommand(createCountriesTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqlCommand(createLocationsTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqlCommand(createDepartmentsTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqlCommand(createJobsTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqlCommand(createEmployeesTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            using (var command = new SqlCommand(createJobHistoryTableQuery, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // После создания EMPLOYEES можно добавить внешний ключ в DEPARTMENTS
            string addManagerFkToDepartments = @"
            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DEPARTMENTS_MANAGER')
            BEGIN
                ALTER TABLE DEPARTMENTS
                ADD CONSTRAINT FK_DEPARTMENTS_MANAGER FOREIGN KEY (MANAGER_ID) REFERENCES EMPLOYEES(EMPLOYEE_ID);
                PRINT 'Внешний ключ FK_DEPARTMENTS_MANAGER добавлен.';
            END";
            using (var command = new SqlCommand(addManagerFkToDepartments, _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            Console.WriteLine($"Дошли до конца создания ёпта");
        }

        /// <summary>
        /// Заполняем таблицы данными
        /// </summary>
        /// <returns></returns>
        private async Task FillTablesIfEmptyAsync()
        {
            try
            {

                #region РЕГИОНЫ
                string checkRegionsQuery = "SELECT COUNT(*) FROM REGIONS";
                using (var command = new SqlCommand(checkRegionsQuery, _connection))
                {
                    int regionCount = (int)await command.ExecuteScalarAsync();
                    if (regionCount == 0)
                    {
                        string insertRegionsQuery = @"
                    INSERT INTO REGIONS (REGION_NAME) VALUES
                    ('Europe'),
                    ('Asia'),
                    ('America'),
                    ('Australia and Oceania'),
                    ('Africa');";
                        using (var insertCommand = new SqlCommand(insertRegionsQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица REGIONS заполнена начальными данными.");
                        }
                    }
                }
                #endregion

                #region СТРАНЫ
                string checkCountriesQuery = "SELECT COUNT(*) FROM COUNTRIES";
                using (var command = new SqlCommand(checkCountriesQuery, _connection))
                {
                    int countryCount = (int)await command.ExecuteScalarAsync();
                    if (countryCount == 0)
                    {
                        string insertCountriesQuery = @"
                            INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME, REGION_ID) VALUES
                            ('RU', 'Russia', 1),
                            ('DE', 'Germany', 1),
                            ('FR', 'France', 1),
                            ('JP', 'Japan', 2),
                            ('US', 'United States of America', 3);";
                        using (var insertCommand = new SqlCommand(insertCountriesQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица COUNTRIES заполнена начальными данными.");
                        }
                    }
                }
                #endregion

                #region ЛОКАЦИИ
                string checkLocationsQuery = "SELECT COUNT(*) FROM LOCATIONS";
                using (var command = new SqlCommand(checkLocationsQuery, _connection))
                {
                    int locationCount = (int)await command.ExecuteScalarAsync();
                    if (locationCount == 0)
                    {
                        string insertLocationsQuery = @"
                        INSERT INTO LOCATIONS (STREET_ADDRESS, POSTAL_CODE, CITY, STATE_PROVINCE, COUNTRY_ID) VALUES
                            ('Maxim Gorky Avenue, 18b'  , '428001', 'Cheboksary', 'Chuvash Republic', 'RU'),
                            ('Chapaeva Street, 39V', '429530', 'Morgaushi', 'Chuvash Republic', 'RU'),
                            ('Moskovsky Prospect, 15'   , '428015', 'Cheboksary', 'Chuvash Republic', 'RU'),
                            ('Pirogov Street, 6'        , '428034', 'Cheboksary', 'Chuvash Republic', 'RU'),
                            ('Boris Semenovich Markov Street, 14', '428003', 'Cheboksary', 'Chuvash Republic', 'RU'),
                            ('Chapaeva Street, 52', '429530', 'Morgaushi', 'Chuvash Republic', 'RU'),
                            ('Chapaeva Street, 64', '429530', 'Morgaushi', 'Chuvash Republic', 'RU'),
                            ('50th Anniversary of October Street, 25', '429530', 'Morgaushi', 'Chuvash Republic', 'RU'),
                            ('2014 Jabberwocky Rd', '26192', 'Southlake', 'Texas', 'US'),
                            ('2011 Interiors Blvd', '99236', 'South San Francisco', 'California', 'US');";
                        using (var insertCommand = new SqlCommand(insertLocationsQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица LOCATIONS заполнена начальными данными.");
                        }
                    }
                }
                #endregion

                #region Работа
                string[] jobs = null;

                string checkJobsQuery = "SELECT COUNT(*) FROM JOBS";
                using (var command = new SqlCommand(checkJobsQuery, _connection))
                {
                    int jobCount = (int)await command.ExecuteScalarAsync();
                    if (jobCount == 0)
                    {
                        jobs = new string[6] { "PRESIDENT", "IT_PROG", "TEST", "TEST2", "TEST3", "MANAGER" };

                        string insertJobsQuery = $@"
                            INSERT INTO JOBS (JOB_ID, JOB_TITLE, MIN_SALARY, MAX_SALARY) VALUES
                            ('{jobs[0]}', 'President of the world', 20000, 40000),
                            ('{jobs[1]}', 'Just a programer, maybe', 4000, 999999),
                            ('{jobs[2]}', 'Something test', 4000, 77777),
                            ('{jobs[3]}', 'Another test pos', 4000, 88888),
                            ('{jobs[4]}', 'Another one test', 4000, 567567),
                            ('{jobs[5]}', 'Manager of the world', 5000, 789789);";
                        using (var insertCommand = new SqlCommand(insertJobsQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица JOBS заполнена начальными данными.");
                        }
                    }
                }
                #endregion

                #region Департаменты
                string[] departments = null;

                string checkDepartmentsQuery = "SELECT COUNT(*) FROM DEPARTMENTS";
                using (var command = new SqlCommand(checkDepartmentsQuery, _connection))
                {
                    int deptCount = (int)await command.ExecuteScalarAsync();
                    if (deptCount == 0)
                    {
                        departments = new string[8] { "Administration", "Marketing", "Purchasing", "Human Resources", "Shipping", "IT", "DAD", "Public Relations" };

                        string insertDepartmentsQuery = $@"
                            INSERT INTO DEPARTMENTS (DEPARTMENT_NAME, MANAGER_ID, LOCATION_ID) VALUES
                            ('{departments[0]}', NULL, 1),
                            ('{departments[1]}', NULL, 2),
                            ('{departments[2]}', NULL, 3),
                            ('{departments[3]}', NULL, 4),
                            ('{departments[4]}', NULL, 5),
                            ('{departments[5]}', NULL, 1),
                            ('{departments[6]}', NULL, 9),
                            ('{departments[7]}', NULL, 10);";
                        using (var insertCommand = new SqlCommand(insertDepartmentsQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица DEPARTMENTS заполнена начальными данными.");
                        }
                    }
                }
                #endregion

                #region РАБОТНИКИ
                string checkEmployeesQuery = "SELECT COUNT(*) FROM EMPLOYEES";
                using (var command = new SqlCommand(checkEmployeesQuery, _connection))
                {
                    int empCount = (int)await command.ExecuteScalarAsync();
                    if (empCount == 0)
                    {
                        const int managersCount = 20;
                        string randomEmployees = DbFillHelper.GetRandomEmployee(managersCount, jobs, departments: departments);

                        // ==== работники, у которых сами являются менеджерами ====
                        string insertFirstEmployeeQuery = $@"
                            INSERT INTO EMPLOYEES (FIRST_NAME, LAST_NAME, EMAIL, PHONE_NUMBER, HIRE_DATE, JOB_ID, SALARY, COMMISSION_PCT, MANAGER_ID, DEPARTMENT_ID)
                            VALUES
                        {randomEmployees};";

                        using (var insertCommand = new SqlCommand(insertFirstEmployeeQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                        }

                        // ==== работники, у которых есть менеджер ====

                        string[] managersID = new string[managersCount];
                        for (int i = 0; i < managersCount; i++) managersID[i] = (i + 1).ToString();

                        string randomEmployyesWithManager = DbFillHelper.GetRandomEmployee(60, jobs, managersID, jobs);

                        string insertOtherEmployeesQuery = $@"
                            INSERT INTO EMPLOYEES (FIRST_NAME, LAST_NAME, EMAIL, PHONE_NUMBER, HIRE_DATE, JOB_ID, SALARY, COMMISSION_PCT, MANAGER_ID, DEPARTMENT_ID)
                            VALUES
                        {randomEmployyesWithManager};";
                        using (var insertCommand = new SqlCommand(insertOtherEmployeesQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица EMPLOYEES заполнена начальными данными.");
                        }


                        // добавляем департаментам менеджеров
                        string updateDepartmentsQuery = @"
                            UPDATE DEPARTMENTS
                            SET MANAGER_ID = 1
                            WHERE DEPARTMENT_NAME = 'Administration';

                            UPDATE DEPARTMENTS
                            SET MANAGER_ID = 2
                            WHERE DEPARTMENT_NAME = 'IT';

                            UPDATE DEPARTMENTS
                            SET MANAGER_ID = 3
                            WHERE DEPARTMENT_NAME = 'DAD'";

                        using (var updateCommand = new SqlCommand(updateDepartmentsQuery, _connection))
                        {
                            await updateCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица DEPARTMENTS обновлена (установлены менеджеры).");
                        }

                        // обновляем привязку работников к департаменту
                        string updateEmployeesQuery = @"
                        UPDATE EMPLOYEES
                        SET DEPARTMENT_ID = 1
                        WHERE EMPLOYEE_ID = 1;

                        UPDATE EMPLOYEES
                        SET DEPARTMENT_ID = 2
                        WHERE EMPLOYEE_ID = 2;

                        UPDATE EMPLOYEES
                        SET DEPARTMENT_ID = 2
                        WHERE EMPLOYEE_ID = 3;";

                        using (var updateCommand = new SqlCommand(updateEmployeesQuery, _connection))
                        {
                            await updateCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица EMPLOYEES обновлена (установлены департаменты).");
                        }
                    }
                }
                #endregion

                #region история_работ
                string checkJobHistoryQuery = "SELECT COUNT(*) FROM JOB_HISTORY";
                using (var command = new SqlCommand(checkJobHistoryQuery, _connection))
                {
                    int historyCount = (int)await command.ExecuteScalarAsync();
                    if (historyCount == 0)
                    {
                        string insertJobHistoryQuery = @"
                            INSERT INTO JOB_HISTORY (EMPLOYEE_ID, START_DATE, END_DATE, JOB_ID, DEPARTMENT_ID) VALUES
                            (1, '1993-01-13', '1998-07-24', 'IT_PROG', 1),
                            (1, '1989-09-21', '1993-10-27', 'TEST', 2),
                            (2, '1993-10-28', '1997-03-15', 'IT_PROG', 1),
                            (2, '1987-09-17', '1993-06-17', 'TEST', 2);";
                        using (var insertCommand = new SqlCommand(insertJobHistoryQuery, _connection))
                        {
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine("Таблица JOB_HISTORY заполнена начальными данными.");
                        }
                    }
                }
                #endregion
            }
            catch
            {
                // если случилась ошибка, то дропаем её для новой попытки создания
                string dropDbToRetry =
                    "USE master;" +
                    "ALTER DATABASE test_task SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                    "DROP DATABASE test_task;";
                using (var command = new SqlCommand(dropDbToRetry, _connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Выполнить запрос согласно строке
        /// </summary>
        /// <param name="query">Сам запрос</param>
        /// <returns></returns>
        public async Task<DataSet> QueryByStringAsync(string query)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            using (var command = new SqlCommand(query, _connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // чтобы не блокировать поток UI, придётся запустить в отдельном потоке
                    await Task.Run(() => dataTable.Load(reader));
                }
            }

            dataSet.Tables.Add(dataTable);
            return dataSet;
        }


        public void Dispose()
        {
            if (this._connection.State == System.Data.ConnectionState.Open)
            {
                this._connection.Close();
                this._connection.Dispose();
                this._connection = null;
                this._disposed = true;
                Console.WriteLine($"Закрыли соединение с бд");
            }
        }
    }
}
