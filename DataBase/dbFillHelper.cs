using System;
using System.Collections.ObjectModel;
using System.Text;

namespace test_task.DataBase
{
    public static class DbFillHelper
    {
        private static readonly Random _random = new Random();
        private static int _counter = 0;

        private const int size = 7;
        private static readonly ReadOnlyCollection<string> firstNames = new ReadOnlyCollection<string>(new string[size] { "Ivan", "Kolya", "Test", "Vova", "Kirieshka", "Erik", "Kirill" });
        private static readonly ReadOnlyCollection<string> secondNames = new ReadOnlyCollection<string>(new string[size] { "Ivanov", "Test", "Koshka", "Kolyanskii", "Qwerty", "Bekker", "Da" });

        private static readonly ReadOnlyCollection<string> emailPrefix = new ReadOnlyCollection<string>(new string[] { "subject", "test", "mymail" });
        private static readonly ReadOnlyCollection<string> emailPostfixs = new ReadOnlyCollection<string>(new string[] { "@mail.ru", "@gmail.com", "@yandex.ru" });

        private static readonly int numberMax = Int32.MaxValue;
        private static readonly int minSalary = 1000;
        private static readonly int maxSalary = 999999;

        private static readonly int minYear = 2020;
        private static readonly int maxYear = 2025;

        private static readonly int minMonth = 1;
        private static readonly int maxMonth = 12;


        public static string GetRandomEmployee(int count, string[] jobs, string[] managers = null, string[] departments = null)
        {
            StringBuilder sb = new StringBuilder();

            int iter = 0;

            while (iter < count)
            {
                string randNumber = (8_000_000_00_00 - _random.Next(228, numberMax) - 7_000_000_00_00).ToString()
                    .Insert(8, ".").Insert(6, ".").Insert(3, ".");

                sb.Append(
                    $"('{firstNames[_random.Next(0, size)]}', " +                   // FIRST_NAME
                    $"'{secondNames[_random.Next(0, size)]}'," +                    // LAST_NAME
                    $"'{emailPrefix[_random.Next(0, emailPrefix.Count)]}" +         // email prefix
                    $"{_counter}" +                                                 // email number
                    $"{emailPostfixs[_random.Next(0, emailPostfixs.Count)]}'," +    // email postfix
                    $"'{randNumber}'," +                                            // number
                    $"'{_random.Next(minYear, maxYear+1)}-{_random.Next(minMonth, maxMonth+1)}-15'," + // дата
                    $"'{jobs[_random.Next(0, jobs.Length)]}', " +                   // job
                    $"'{_random.Next(minSalary, maxSalary)}', " +                   // ЗП
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
