using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IrishRailApi
{
    public class IrishRailApiService : IIrishRailApiService
    {
        private readonly string _apiBaseUrl = "http://api.irishrail.ie/realtime/realtime.asmx";
        private readonly IRestClient _restClient;

        public IrishRailApiService()
        {
            _restClient = new RestClient(_apiBaseUrl);
        }

        public async Task<ArrayOfObjStation> GetAllStationsAsync()
        {
            return await GetAsync<ArrayOfObjStation>("getAllStationsXML", null);
        }

        public async Task<ArrayOfObjStationData> GetStationDataByStationCodeAsync(string stationCode)
        {
            return await GetAsync<ArrayOfObjStationData>("getStationDataByCodeXML", new Dictionary<string, string> { { "StationCode", stationCode } });
        }

        private async Task<T> GetAsync<T>(string path, Dictionary<string, string> parameters)
        {
            var request = new RestRequest(path, Method.GET);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value);
                }
            }

            var response = await ExecuteRequestAsync<T>(request);
            return response;
        }

        private async Task<T> ExecuteRequestAsync<T>(RestRequest request)
        {

            //if (UseProxy)
            //{
            //    client.Proxy = new WebProxy(ProxyAddress, ProxyPort);
            //}

            IRestResponse response = await _restClient.ExecuteGetTaskAsync(request);

            var content = response.Content; // raw content as string
            var serializer = new DotNetXmlDeserializer();
            var myResponse = serializer.Deserialize<T>(response);
            return myResponse;
        }
    }
}
