using System;
using System.Threading.Tasks;

namespace TrashMob.Shared.Persistence
{
    public interface IMapRepository
    {
        string GetMapKey();

        Task<double> GetDistanceBetweenTwoPoints(Tuple<double, double> pointA, Tuple<double, double> pointB, bool IsMetric = true);

        Task<string> GetTimeForPoint(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset);
    }
}
