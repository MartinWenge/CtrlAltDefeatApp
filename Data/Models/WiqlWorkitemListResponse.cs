using Newtonsoft.Json;

namespace ctrlAltDefeatApp.Data.Models;

public class WiqlWorkitemListResponse
{
    [JsonProperty("workItems")]
    public List<WorkItemId>? WorkItemIds { get; set;}
}

public class WorkItemId
{
    [JsonProperty("id")]
    public int Id { get; set;}
}
