using Newtonsoft.Json;

namespace ChildImmunizationCare_Parent.Models
{
    public class ParentRequest
    {
     
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
