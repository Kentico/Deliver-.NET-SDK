﻿using Kentico.Kontent.Delivery.Abstractions.SharedModels;
using Newtonsoft.Json;

namespace Kentico.Kontent.Delivery.SharedModels
{
    /// <inheritdoc/>
    public sealed class Pagination : IPagination
    {
        /// <inheritdoc/>
        public int Skip { get; }

        /// <inheritdoc/>
        public int Limit { get; }

        /// <inheritdoc/>
        public int Count { get; }

        /// <inheritdoc/>
        public int? TotalCount { get; }

        /// <inheritdoc/>
        public string NextPageUrl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pagination"/> class with information from a response.
        /// </summary>
        [JsonConstructor]
        internal Pagination(int skip, int limit, int count, int? total_count, string next_page)
        {
            Skip = skip;
            Limit = limit;
            Count = count;
            TotalCount = total_count;
            NextPageUrl = next_page;
        }
    }
}