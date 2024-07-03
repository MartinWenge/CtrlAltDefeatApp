using Newtonsoft.Json;

namespace ctrlAltDefeatApp.Data.Models;

public class WorkItemListResponse
{
    [JsonProperty("value")]
    public List<WorkItemResponse> WorkItems {get; set;}
}

public class WorkItemResponse
{
    [JsonProperty("id")]
    public int WorkItemId { get; set;}

    [JsonProperty("fields")]
    public WorkItemFields Fields {get; set;}
}

public class WorkItemFields
{
    [JsonProperty("System.Title")]
    public string? Title {get; set;}

    [JsonProperty("Microsoft.VSTS.TCM.ReproSteps")]
    public string? ReproSteps {get; set;}

    [JsonProperty("System.Description")]
    public string? Description {get; set;}

    [JsonProperty("System.WorkItemType")]
    public string? WorkItemType {get; set;}

    [JsonProperty("System.State")]
    public string? State {get; set;}

    [JsonProperty("System.IterationPath")]
    public string? IterationPath {get; set;}

    [JsonProperty("Microsoft.VSTS.Scheduling.OriginalEstimate")]
    public double? OriginalEstimate {get; set;}

    [JsonProperty("Microsoft.VSTS.Scheduling.RemainingWork")]
    public double? RemainingWork {get; set;}

    [JsonProperty("Microsoft.VSTS.Scheduling.CompletedWork")]
    public double? CompletedWork {get; set;}
}
