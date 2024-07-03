using ctrlAltDefeatApp.Data.Models;
using Newtonsoft.Json;

namespace ctrlAltDefeatApp.Data.Entities;

public class Workitem
{
    [JsonProperty("")]
    public int Id {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public string WorkItemType {get; set;}
    public string State {get; set;}
    public string IterationPath {get; set;}

    public double? OriginalEstimate {get; set;}
    public double? RemainingWork {get; set;}
    public double? CompletedWork {get; set;}

    public Workitem(){
        this.Id = new Random().Next();
        this.Title = "";
        this.Description = "";
        this.WorkItemType = "";
        this.State = "";
        this.IterationPath = "";
        this.OriginalEstimate = 0f;
        this.RemainingWork = 0f;
        this.CompletedWork = 0f;
    }

    public Workitem(
        int _id, string _title, string _description, string _workitemtype, string _state, string _iterationpath,
        double _origionalestimate, double _remainingwork, double _completedwork
    ){
        this.Id = _id;
        this.Title = _title;
        this.Description = _description;
        this.WorkItemType = _workitemtype;
        this.State = _state;
        this.IterationPath = _iterationpath;
        this.OriginalEstimate = _origionalestimate;
        this.RemainingWork = _remainingwork;
        this.CompletedWork = _remainingwork;
    }

    public Workitem(WorkItemResponse response){
        this.Id = response.WorkItemId;
        this.Title = string.IsNullOrEmpty(response.Fields.Title) ? "" : response.Fields.Title;
        this.Description = string.IsNullOrEmpty(response.Fields.Description) ? (
            string.IsNullOrEmpty(response.Fields.ReproSteps) ? "" : response.Fields.ReproSteps
            ) : response.Fields.Description;
        this.WorkItemType = string.IsNullOrEmpty(response.Fields.WorkItemType) ? "" : response.Fields.WorkItemType;
        this.State = string.IsNullOrEmpty(response.Fields.State) ? "" : response.Fields.State;
        this.IterationPath = string.IsNullOrEmpty(response.Fields.IterationPath)? "kein Sprint zugeordnet" : response.Fields.IterationPath;

        this.OriginalEstimate = response.Fields.OriginalEstimate;
        this.RemainingWork = response.Fields.RemainingWork;
        this.CompletedWork = response.Fields.CompletedWork;
    }
}
