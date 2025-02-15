import moment from 'moment';

export const getTodayTimerange = (date: Date = new Date()) =>
    `${moment(date).format('YYYY-MM-DD')}|${moment(date).format('YYYY-MM-DD')}`;

export const getTomorrowTimerange = (date: Date = new Date()) =>
    `${moment(date).add(1, 'd').format('YYYY-MM-DD')}|${moment(date).add(1, 'd').format('YYYY-MM-DD')}`;

export const getThisweekendTimerange = (date: Date = new Date()) =>
    `${moment(date).endOf('week').format('YYYY-MM-DD')}|${moment(date).endOf('week').add(1, 'd').format('YYYY-MM-DD')}`;

export const getAllUpcomingTimerange = (date: Date = new Date()) =>
    `${moment(date).format('YYYY-MM-DD')}|${moment(date).clone().add(10, 'y').format('YYYY-MM-DD')}`;

export const getUpcomingTimeranges = (date: Date = new Date()) => {
    return [
        { value: getTodayTimerange(date), label: 'Today' },
        { value: getTomorrowTimerange(date), label: 'Tomorrow' },
        { value: getThisweekendTimerange(date), label: 'This weekend' },
        { value: getAllUpcomingTimerange(date), label: 'All' },
    ];
};

export const getLastDaysTimerange = (n: number, date: Date = new Date()) =>
    `${moment(date).subtract(n, 'd').format('YYYY-MM-DD')}|${moment(date).format('YYYY-MM-DD')}`;

export const getLastMonthsTimerange = (n: number, date: Date = new Date()) =>
    `${moment(date).subtract(n, 'M').format('YYYY-MM-DD')}|${moment(date).format('YYYY-MM-DD')}`;

export const getAllCompletedTimerange = (date: Date = new Date()) =>
    `${moment(date).subtract(10, 'y').format('YYYY-MM-DD')}|${moment(date).format('YYYY-MM-DD')}`;

export const getCompletedTimeranges = (date: Date = new Date()) => {
    return [
        {
            value: getLastDaysTimerange(7),
            label: 'Last 7 days',
        },
        {
            value: getLastDaysTimerange(30),
            label: 'Last 30 days',
        },
        {
            value: getLastDaysTimerange(90),
            label: 'Last 90 days',
        },
        {
            value: getLastMonthsTimerange(6),
            label: 'Last 6 months',
        },
        {
            value: getLastMonthsTimerange(12),
            label: 'Last 12 months',
        },
        {
            value: getAllCompletedTimerange(),
            label: 'All time',
        },
    ];
};
