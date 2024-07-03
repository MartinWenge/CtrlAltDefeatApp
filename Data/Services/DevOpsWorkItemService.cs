namespace ctrlAltDefeatApp.Data.Services;

using System.Net.Http.Headers;
using System.Text;
using ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.Models;
using ctrlAltDefeatApp.Data.DatabaseContext;
using Newtonsoft.Json;

public class DevOpsWorkItemService
{
    private readonly HttpClient httpClient;
    private IConfiguration configuration;

    private CtrlAltDefeatDatabaseContext databaseContext;

    private string DevOpsUsername {get; set;}
    private string AccessToken {get; set;}

    public DevOpsWorkItemService(IConfiguration config, CtrlAltDefeatDatabaseContext context){
        configuration = config;
        databaseContext = context;

        string? username = configuration.GetValue<string>("AzureDevOpsSettings:Username");
        string? token = configuration.GetValue<string>("AzureDevOpsSettings:Token");

        this.DevOpsUsername = string.IsNullOrEmpty(username) ? "" : username;
        this.AccessToken = string.IsNullOrEmpty(token) ? "" : token;

        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://tfs.pta.de/tfs/PTA-DD/CtrlAltDefeat/_apis/");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", DevOpsUsername, AccessToken)
            ))
        );
    }

    public async Task<List<Workitem>> getListOfClosedWorkitems(List<int> listOfWorkitemIds){
        List<Workitem> workitems = [];

        string listOfWorkitems = string.Join(",", listOfWorkitemIds);

        HttpRequestMessage requestList = new HttpRequestMessage(HttpMethod.Get, $"wit/workitems?ids={listOfWorkitems}&api-version=5.1");
        HttpResponseMessage responseList = await httpClient.SendAsync(requestList);

        if(responseList.IsSuccessStatusCode)
        {
            string responseListString = await responseList.Content.ReadAsStringAsync();
            WorkItemListResponse? responseListObject = JsonConvert.DeserializeObject<WorkItemListResponse>(responseListString);
            if (responseListObject != null){
                foreach(WorkItemResponse workItemFromServer in responseListObject.WorkItems)
                {
                    if(!IsNotClosed(workItemFromServer)){
                        workitems.Add(new Workitem(workItemFromServer));
                    }
                }
            }
        }

        return workitems;
    }

    public async Task<List<Workitem>> getListOfEstimateableWorkitems(Guid userId){
        List<Workitem> workitems = [];

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "wit/wiql?api-version=5.1");
        string jsonRequestBody = "{\"query\": \"SELECT [System.Id] FROM workitems WHERE ([System.TeamProject] = 'CtrlAltDefeat') ORDER BY [System.ChangedDate] DESC\"}";
        request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            WiqlWorkitemListResponse? responseObject = JsonConvert.DeserializeObject<WiqlWorkitemListResponse>(responseBody);
            if(responseObject != null && responseObject.WorkItemIds != null)
            {
                var listOfWorkitems = string.Join(",", responseObject.WorkItemIds.Select(x => x.Id.ToString()).ToArray());

                HttpRequestMessage requestList = new HttpRequestMessage(HttpMethod.Get, $"wit/workitems?ids={listOfWorkitems}&api-version=5.1");
                HttpResponseMessage responseList = await httpClient.SendAsync(requestList);

                if(responseList.IsSuccessStatusCode)
                {
                    string responseListString = await responseList.Content.ReadAsStringAsync();
                    WorkItemListResponse? responseListObject = JsonConvert.DeserializeObject<WorkItemListResponse>(responseListString);
                    if (responseListObject != null){
                        foreach(WorkItemResponse workItemFromServer in responseListObject.WorkItems)
                        {
                            //perform some checks
                            if(IsOfTypeBugOrTask(workItemFromServer) && IsNotClosed(workItemFromServer) && IsWithoutCompletedWork(workItemFromServer)){
                                workitems.Add(new Workitem(workItemFromServer));
                            }
                        }
                    }
                }
            }
        }

        if (workitems.Count != 0){
            List<Estimates> estimates = databaseContext.Estimates.Where(x => x.UserId == userId).ToList();
            foreach(Estimates estimate in estimates){
                Session? session = databaseContext.Sessions.FirstOrDefault(x => x.Id == estimate.Session);
                if (session != null)
                {
                    Workitem? wItem = workitems.FirstOrDefault(x => x.Id == session.WorkItemId);
                    if(wItem != null)
                    {
                        workitems.Remove(wItem);
                    }
                }
            }
        }

        return workitems;
    }

    public async Task<Workitem> getWorkItemById(int workitemId){
        Workitem workitem = new Workitem();
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"wit/workItems/{workitemId}?api-version=5.1" );
        HttpResponseMessage response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            WorkItemResponse? responseObject = JsonConvert.DeserializeObject<WorkItemResponse>(responseBody);
            if(responseObject != null){
                workitem = new Workitem(responseObject);
            }
        }
        
        return workitem;
    }

    public async Task<bool> updateWorkItemEstimates(int workItemId, double newEstimate){
        bool status = false;

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, $"wit/workitems/{workItemId}?api-version=5.1");
        string patchPayload = $"[{{\"op\":\"test\",\"path\":\"/id\",\"value\":\"{workItemId}\"}},";
        patchPayload += $"{{\"op\":\"add\",\"path\":\"/fields/Microsoft.VSTS.Scheduling.OriginalEstimate\",\"value\":\"{newEstimate}\"}},";
        patchPayload += $"{{\"op\":\"add\",\"path\":\"/fields/Microsoft.VSTS.Scheduling.RemainingWork\",\"value\":\"{newEstimate}\"}}]";

        request.Content = new StringContent(patchPayload, Encoding.UTF8, "application/json-patch+json");

        HttpResponseMessage response = await httpClient.SendAsync(request);
        if(response.IsSuccessStatusCode){
            status = true;
        }

        return status;
    }

    private bool IsOfTypeBugOrTask(WorkItemResponse workItem){
        bool itemShouldBeAdded = false;
        string? itemType = workItem.Fields.WorkItemType;
        if(itemType != null && (itemType  == "Task" || itemType == "Bug"))
        {
            itemShouldBeAdded = true;
        }
        return itemShouldBeAdded;
    }

    private bool IsNotClosed(WorkItemResponse workItem)
    {
        bool itemShouldBeAdded = false;
        string? state = workItem.Fields.State;
        if(!string.IsNullOrEmpty(state) && state != "Closed"){
            itemShouldBeAdded = true;
        }

        return itemShouldBeAdded;
    }

    private bool IsWithoutCompletedWork(WorkItemResponse workItem){
        bool itemShouldBeAdded = false;
        double? completed = workItem.Fields.CompletedWork;
        if(completed == null || (completed < 0.000001)){
            itemShouldBeAdded = true;
        }

        return itemShouldBeAdded;
    }

    

}
