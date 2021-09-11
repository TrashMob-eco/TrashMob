
namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Threading.Tasks;

    public interface IMapRepository
    {
        string GetMapKey();

        Task<double> GetDistanceBetweenTwoPoints(Tuple<double, double> pointA, Tuple<double, double> pointB, bool IsMetric = true);

        Task<string> GetTimeForPoint(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset);
    }
}
