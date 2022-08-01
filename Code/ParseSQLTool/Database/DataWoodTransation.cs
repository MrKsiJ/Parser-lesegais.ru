using Newtonsoft.Json;
using System;

namespace ParseSQLTool.Database
{
    [System.Serializable]
    public class DataWoodTransation : IEquatable<DataWoodTransation>
    {
        [JsonProperty("sellerName")]
        public string sellerName { get; set; }
        [JsonProperty("sellerInn")]
        public string sellerInn { get; set; }
        [JsonProperty("buyerName")]
        public string buyerName { get; set; }
        [JsonProperty("buyerInn")]
        public string buyerInn { get; set; }
        [JsonProperty("woodVolumeBuyer")]
        public double woodVolumeBuyer { get; set; }
        [JsonProperty("woodVolumeSeller")]
        public double woodVolumeSeller { get; set; }
        [JsonProperty("dealDate")]
        public string dealDate { get; set; }
        [JsonProperty("dealNumber")]
        public string dealNumber { get; set; }
        [JsonProperty("__typename")]
        public string __typename { get; set; }

        public bool Equals(DataWoodTransation other)
        {
            return sellerName == other.sellerName &&
                sellerInn == other.sellerInn &&
                buyerName == other.buyerName &&
                buyerInn == other.buyerInn &&
                dealDate == other.dealDate &&
                dealNumber == other.dealNumber;
        }
    }
}
