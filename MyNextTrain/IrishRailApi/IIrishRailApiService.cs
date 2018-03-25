using System.Threading.Tasks;

namespace IrishRailApi
{
    public interface IIrishRailApiService
    {
        Task<ArrayOfObjStation> GetAllStationsAsync();
        Task<ArrayOfObjStation> GetAllStationsWithTypeAsync(string stationType);

        Task<ArrayOfObjStationData> GetStationDataByStationCodeAsync(string stationCode);
    }
}