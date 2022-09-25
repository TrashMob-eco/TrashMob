namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IMapRepository
    {
        string GetMapKey();

        Task<double> GetDistanceBetweenTwoPoints(Tuple<double, double> pointA, Tuple<double, double> pointB, bool IsMetric = true);

        Task<string> GetTimeForPoint(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset);

        Task<Address> GetAddress(double latitude, double longitude);
    }
}
