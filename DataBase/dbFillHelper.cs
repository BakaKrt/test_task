using System;
using System.Collections.ObjectModel;
using System.Text;

namespace test_task.DataBase
{
    /// <summary>
    /// Помощник для заполнения БД данными. Наверное можно было сделать умнее и правильнее
    /// </summary>
    public static class DbFillHelper
    {
        private static readonly Random _random = new Random();
        private static int _counter = 0;

        private const int _namesSize = 7;
        private static readonly ReadOnlyCollection<string> _firstNames = new ReadOnlyCollection<string>(new string[_namesSize] { "Ivan", "Kolya", "Test", "Vova", "Kirieshka", "Erik", "Kirill" });
        private static readonly ReadOnlyCollection<string> _secondNames = new ReadOnlyCollection<string>(new string[_namesSize] { "Ivanov", "Test", "Koshka", "Kolyanskii", "Qwerty", "Bekker", "Da" });

        private static readonly ReadOnlyCollection<string> _emailPrefix = new ReadOnlyCollection<string>(new string[] { "subject", "test", "mymail" });
        private static readonly ReadOnlyCollection<string> _emailPostfixs = new ReadOnlyCollection<string>(new string[] { "@mail.ru", "@gmail.com", "@yandex.ru" });

        private static readonly int _numberMax = Int32.MaxValue;
        private static readonly int _minSalary = 1000;
        private static readonly int _maxSalary = 999999;

        private static readonly int _minYear = 2020;
        private static readonly int _maxYear = 2025;

        private static readonly int _minMonth = 1;
        private static readonly int _maxMonth = 12;


        /// <summary>
        /// Случайный рабочий
        /// </summary>
        /// <param name="count">Количество строк</param>
        /// <param name="jobs">Список работ</param>
        /// <param name="managers">Список менеджеров</param>
        /// <param name="departments">Список департаментов</param>
        /// <returns></returns>
        public static string GetRandomEmployee(int count, string[] jobs, string[] managers = null, string[] departments = null)
        {
            StringBuilder sb = new StringBuilder();

            int iter = 0;

            while (iter < count)
            {
                string randNumber = (8_000_000_00_00 - _random.Next(228, _numberMax) - 7_000_000_00_00).ToString()
                    .Insert(8, ".").Insert(6, ".").Insert(3, ".");

                sb.Append(
                    $"('{_firstNames[_random.Next(0, _namesSize)]}', " +            // FIRST_NAME
                    $"'{_secondNames[_random.Next(0, _namesSize)]}'," +             // LAST_NAME
                    $"'{_emailPrefix[_random.Next(0, _emailPrefix.Count)]}" +       // email prefix
                    $"{_counter}" +                                                 // email number
                    $"{_emailPostfixs[_random.Next(0, _emailPostfixs.Count)]}'," +  // email postfix
                    $"'{randNumber}'," +                                            // number
                    $"'{_random.Next(_minYear, _maxYear+1)}-{_random.Next(_minMonth, _maxMonth+1)}-15'," + // дата
                    $"'{jobs[_random.Next(0, jobs.Length)]}', " +                   // job
                    $"'{_random.Next(_minSalary, _maxSalary)}', " +                 // ЗП
                    $"NULL, ");                                                     // COMMISSION_PCT

                _counter++;

                if (managers != null) sb.Append($"'{_random.Next(1, managers.Length+1)}', ");
                else sb.Append($"NULL, ");

                if (departments != null) sb.Append($"'{_random.Next(1, departments.Length+1)}'");
                else sb.Append($"NULL");

                sb.Append(")");

                if (iter != count - 1)
                {
                    sb.Append(", \n");
                }
                iter++;
            }

            return sb.ToString();
        }
    }
}
