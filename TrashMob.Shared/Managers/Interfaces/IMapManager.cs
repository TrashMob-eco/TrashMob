namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IMapManager
    {
        string GetMapKey();

        Task<double> GetDistanceBetweenTwoPointsAsync(Tuple<double, double> pointA, Tuple<double, double> pointB,
            bool IsMetric = true);

        Task<string> GetTimeForPointAsync(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset);

        Task<Address> GetAddressAsync(double latitude, double longitude);
    }
}