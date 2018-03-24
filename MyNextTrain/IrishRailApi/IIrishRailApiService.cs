using System.Threading.Tasks;

namespace IrishRailApi
{
    public interface IIrishRailApiService
    {
        Task<ArrayOfObjStation> GetAllStationsAsync();
        Task<ArrayOfObjStationData> GetStationDataByStationCodeAsync(string stationCode);
    }
}