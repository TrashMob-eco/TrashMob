namespace TrashMobMobile
{
    using System;
    using System.Collections.Generic;

    public class DateRanges
    {
        public const string AllTime = "All";
        public const string LastYear = "Last 12 Months";
        public const string LastSixMonths = "Last Six Months";
        public const string LastThreeMonths = "Last 90 Days";
        public const string LastMonth = "Last 30 Days";
        public const string LastWeek = "Last 7 Days";
        public const string Yesterday = "Yesterday";
        public const string Today = "Today";
        public const string Tomorrow = "Tomorrow";
        public const string ThisWeek = "Next 7 Days";
        public const string ThisMonth = "Next 30 Days";
        public const string ThisYear = "Next 12 Months";

        public static readonly Dictionary<string, Tuple<int, int>> UpcomingRangeDictionary = new()
        {
            { Today, new Tuple<int, int>(0, 0) },
            { Tomorrow, new Tuple<int, int>(1, 0) },
            { ThisWeek, new Tuple<int, int>(0, 7) },
            { ThisMonth, new Tuple<int, int>(0, 30) },
            { ThisYear, new Tuple<int, int>(0, 365) }
        };

        public static readonly Dictionary<string, Tuple<int, int>> CompletedRangeDictionary = new()
        {
            { AllTime, new Tuple<int, int>(-3650, 0) },
            { LastYear, new Tuple<int, int>(-365, 0) },
            { LastSixMonths, new Tuple<int, int>(-180, 0) },
            { LastThreeMonths, new Tuple<int, int>(-90, 0) },
            { LastMonth, new Tuple<int, int>(-30, 0) },
            { LastWeek, new Tuple<int, int>(-7, 0) },
            { Yesterday, new Tuple<int, int>(-1, 0) },
        };

        public static readonly Dictionary<string, Tuple<int, int>> CreatedDateRangeDictionary = new()
        {
            { AllTime, new Tuple<int, int>(-3650, 0) },
            { LastYear, new Tuple<int, int>(-365, 0) },
            { LastSixMonths, new Tuple<int, int>(-180, 0) },
            { LastThreeMonths, new Tuple<int, int>(-90, 0) },
            { LastMonth, new Tuple<int, int>(-30, 0) },
            { LastWeek, new Tuple<int, int>(-7, 0) },
            { Yesterday, new Tuple<int, int>(-1, 0) },
        };
    }
}
