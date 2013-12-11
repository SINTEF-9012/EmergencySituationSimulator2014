using System;
using System.Collections.Generic;
using System.Globalization;
using RestSharp;

namespace EmergencySituationSimulator2013.HereAPI
{
    class HereGeocoder
    {
        private static readonly RestClient Client =
            new RestClient("http://reverse.geocoder.cit.api.here.com");

        public IRestResponse<HereSearchResponse> Response { get; protected set; }
        public List<AddressType> Address { get; protected set; }

        public HereGeocoder(Location position)
        {
            var request = new RestRequest("/6.2/reversegeocode.json",
                Method.GET);

            // Application parameters
            request.AddParameter("app_id", HereConfig.AppId);
            request.AddParameter("app_code", HereConfig.AppCode);

            request.AddParameter("gen", "3");
            request.AddParameter("mode", "retrieveAreas");

            // First character of identifiers in uppercase
            request.AddParameter("jsonAttributes", "0");

            // And the waypoints
            request.AddParameter("prox", String.Format(
                CultureInfo.InvariantCulture, "{0:F},{1:F}",
                position.lat, position.lng));

            // Magic
            Response = Client.Execute<HereSearchResponse>(request);
            
            Address = new List<AddressType>();

            try
            {
                foreach (var view in Response.Data.Response.View)
                {
                    foreach (var result in view.Result)
                    {
                        foreach (var location in result.Location)
                        {
                            foreach (var address in location.Address)
                            {
                                Address.Add(address);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("HereGeocoder doesn't work :P", e);
            }
        }
    }
}
