﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace KenticoCloud.Deliver
{
    /// <summary>
    /// Class for querying Deliver API.
    /// </summary>
    public class DeliverClient
    {
        private readonly HttpClient httpClient;
        private readonly DeliverUrlBuilder urlBuilder;

        /// <summary>
        /// Constructor for production API.
        /// </summary>
        /// <param name="projectId">Project ID.</param>
        /// <remarks>Production API connects to your published content.</remarks>
        public DeliverClient(string projectId)
        {
            urlBuilder = new DeliverUrlBuilder(projectId);
            httpClient = new HttpClient();
        }


        /// <summary>
        /// Constructor for preview API.
        /// </summary>
        /// <param name="projectId">Project ID.</param>
        /// <param name="accessToken">Preview access token.</param>
        /// <remarks>Production API connects to your unpublished content.</remarks>
        public DeliverClient(string projectId, string accessToken)
        {
            urlBuilder = new DeliverUrlBuilder(projectId, accessToken);
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", accessToken));
        }


        /// <summary>
        /// Gets one content item by its codename. This method returns the whole response as JObject.
        /// </summary>
        /// <param name="itemCodename">Content item codename.</param>
        /// <param name="queryParams">Query parameters. For example: "elements=title" or "depth=0"."</param>
        public async Task<JObject> GetItemJsonAsync(string itemCodename, params string[] queryParams)
        {
            if (String.IsNullOrEmpty(itemCodename))
            {
                throw new ArgumentException("Entered item codename is not valid.", "itemCodename");
            }

            var url = urlBuilder.GetEndpointUrl(itemCodename, queryParams);
            return await GetDeliverResponseAsync(url);
        }


        /// <summary>
        /// Searches the content repository for items that match the criteria. This method returns the whole response as JObject.
        /// </summary>
        /// <param name="queryParams">Query parameters. For example: "elements=title" or "depth=0"."</param>
        public async Task<JObject> GetItemsJsonAsync(params string[] queryParams)
        {
            var url = urlBuilder.GetEndpointUrl(queryParams: queryParams);
            return await GetDeliverResponseAsync(url);
        }


        /// <summary>
        /// Gets one content item by its codename.
        /// </summary>
        /// <param name="itemCodename">Content item codename.</param>
        /// <param name="parameters">Query parameters.</param>
        public async Task<DeliverResponse> GetItemAsync(string itemCodename, IEnumerable<IFilter> parameters = null)
        {
            if (String.IsNullOrEmpty(itemCodename))
            {
                throw new ArgumentException("Entered item codename is not valid.", "itemCodename");
            }

            var url = urlBuilder.GetEndpointUrl(itemCodename, parameters);
            var response = await GetDeliverResponseAsync(url);

            return new DeliverResponse(response);
        }


        /// <summary>
        /// Searches the content repository for items that match the filter criteria.
        /// </summary>
        /// <param name="parameters">Query parameters.</param>
        public async Task<DeliverListingResponse> GetItemsAsync(IEnumerable<IFilter> parameters = null)
        {
            var url = urlBuilder.GetEndpointUrl(filters: parameters);
            var response = await GetDeliverResponseAsync(url);

            return new DeliverListingResponse(response);
        }


        private async Task<JObject> GetDeliverResponseAsync(string url)
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                return JObject.Parse(responseBody);
            }

            throw new DeliverException(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
