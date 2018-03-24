using System;
using Xunit;

namespace IrishRailApiIntegrationTest
{
    public class IrishRailTest
    {
        [Fact]
        public async void GetStations()
        {
            var api = new IrishRailApi.IrishRailApiService();
            var stations = await api.GetAllStationsAsync();
            Assert.NotNull(stations);
            Assert.NotEmpty(stations.Items);
        }

        [Theory]
        [InlineData("BROCK", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("FAKESTATION", false)]
        [InlineData("FFAKE", false)]
        public async void GetStationDataByStationCode(string stationCode, bool success)
        {
            var api = new IrishRailApi.IrishRailApiService();
            var stationData = await api.GetStationDataByStationCodeAsync(stationCode);
            Assert.NotNull(stationData);
            Assert.Equal(stationData.Items != null, success);
        }
    }
}
