using Newtonsoft.Json;
using System.Collections.Generic;


namespace ParseSQLTool.Database
{
    public class SearchReportWoodDeal
    {
        public List<DataWoodTransation> content { get; set; }
        [JsonProperty("total")]
        public int total { get; set; }
        [JsonProperty("number")]
        public int number { get; set; }
        [JsonProperty("size")]
        public int size { get; set; }
        [JsonProperty("overallBuyerVolume")]
        public double overallBuyerVolume { get; set; }
        [JsonProperty("overallSellerVolume")]
        public double overallSellerVolume { get; set; }
        [JsonProperty("__typename")]
        public string __typename { get; set; }
    }
}
