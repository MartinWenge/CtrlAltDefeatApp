using ctrlAltDefeatApp.Data.Entities;
using Microsoft.AspNetCore.Components;
using ctrlAltDefeatApp.Data.Services;
using MudBlazor;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Web;

namespace ctrlAltDefeatApp.Components.Pages;

partial class Home : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private XperiencePointsService XpService {get; set;}

    [Inject]
    private UserService UserService {get; set;}

    [Inject]
    private DevOpsWorkItemService devOpsService {get; set;}

    
    [Inject]
    private EstimateService EstimateService {get; set;}

    [Parameter]
    public string UserId { get; set; }

    List<Workitem> estimateableWorkitems {get; set;}

    private User currentUser;

    public string[]? iterations;
    public string[]? types;

    private int xpForNextLevel;
    private string initials;

    private List<User> highscoreListByXP;
    private List<User> highscoreListByAverage;
    private List<Estimates> userEstimates;

    protected override async Task OnInitializedAsync ()
    {
        currentUser = UserService.GetUserById(UserId);

        estimateableWorkitems = await devOpsService.getListOfEstimateableWorkitems(currentUser.Id);

        iterations = GetIterationPaths(estimateableWorkitems);
        types = GetTypes(estimateableWorkitems);
        
        xpForNextLevel = XpService.GetNextLevelThreshold(currentUser);
    
        initials= currentUser.Username.Substring(0, 2).ToUpper();

        int xpFromClosedWorkitems = await XpService.checkForFinishedWorkitems(currentUser.Id);

        if(xpFromClosedWorkitems > 0){
            UserService.SetConfirmXP(currentUser.Id, true);
            UserService.AddXPToUser(UserId, xpFromClosedWorkitems);
            string calculatedLevel = XpService.GetLevel(currentUser);
            if (calculatedLevel != currentUser.Level)
            {
                UserService.SetNewLevel(currentUser, calculatedLevel);
            }
        }

        UserService.UpdateAverageEstimateDifference(currentUser);
        highscoreListByXP = UserService.GetHighscoreListXP();
        highscoreListByAverage = UserService.GetHighscoreListAverageEstimation();
        userEstimates = EstimateService.GetAllClosedEstimatesForUser(currentUser.Id);
    }

    public int GetWonXp(double realEffort, double estimatedTime){
        return XpService.CalculatePoints(realEffort, estimatedTime);
    }

    public void resetConfirmNewXP(){
        UserService.SetConfirmXP(currentUser.Id, false);
    }

    public void OnChipClicked(Workitem workItem)
    {
        var workItemId = workItem.Id.ToString();
        NavigationManager.NavigateTo($"/estimate/{UserId}/{workItemId}");
    }

    public static string[] GetIterationPaths(List<Workitem> data)
    {
        string[] iterationPaths = data.Select(item => item.IterationPath).Distinct().ToArray();
        return iterationPaths;
    }

    public static string[] GetTypes(List<Workitem> data)
    {
        string[] types = data.Select(item => item.WorkItemType).Distinct().ToArray();
        return types;
    }

}
