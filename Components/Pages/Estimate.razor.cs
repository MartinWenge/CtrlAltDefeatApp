using ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.Models;
using ctrlAltDefeatApp.Data.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ctrlAltDefeatApp.Components.Pages;

partial class Estimate
{
    [Parameter]
    public string UserId { get; set; }

    [Parameter]
    public string WorkItemId { get; set; }

    [Inject]
    SessionService SessionService { get; set; }

    [Inject]
    EstimateService EstimateService { get; set; }

    [Inject]
    UserService UserService { get; set; }


    [Inject]
    XperiencePointsService XpService { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    DevOpsWorkItemService DevOpsWorkItemService { get; set; }

    private Session currentSession;
    private Workitem currentWorkItem;
    private Estimates currentEstimate;
    private double debugAverage;
    private int newXP;
    private int nextLevelXp;

    private bool showFirework = false;
    private bool showNewLevel = false;
    private bool workItemEstimateable = true;
    private User currentUser;

    Dictionary<(int, int), List<string>> messages = new Dictionary<(int, int), List<string>>
    {
        {(0, 1), new List<string> { "Reicht noch nicht mal zum Kaffee trinken!", "Da bleibt nicht mal Zeit für einen Kaffee!", "Kurz und knackig!", "Schnell erledigt!", "Nicht mal eine Stunde Arbeit!" }},
        {(1, 4), new List<string> { "Recht Übersichtlich!", "Nicht viel, aber immerhin etwas!", "Wenig Zeit, viel zu tun!", "Einfach zu schaffen!", "Nur ein kurzes Projekt!" }},
        {(4, 8), new List<string> { "Dafür brauchst du einen ganzen Tag? - Was soll dein Projektleiter davon halten...", "Hoffentlich hat dein Chef Geduld!", "Ein paar Stunden Arbeit!", "Eine halbe Tagesaufgabe!", "Keine leichte Aufgabe!" }},
        {(8, 16), new List<string> { "Ein Job fürs Wochenende.", "Da geht noch was, aber heute wird das wohl nicht mehr fertig...", "Mehr als ein ganzer Tag Arbeit!", "Eine echte Herausforderung!", "Braucht mindestens einen Tag!" }},
        {(16, 32), new List<string> { "Den Task würde ich splitten, wenn ich du wäre.", "Da hast du dir aber was vorgenommen!", "Das wird eine Herausforderung!", "Eine große Aufgabe!", "Fürchte, das wird dauern!" }},
        {(32, int.MaxValue), new List<string> { "Wenn du dir da nicht zu viel vorgenommen hast...", "Das wird ein langer Weg!", "Ein Mammutprojekt!", "Ein riesiges Unterfangen!", "Wird Monate dauern!" }}
    };

    Random random = new Random();

    protected override async Task OnInitializedAsync()
    {
        int worItemIdAsInt = int.Parse(WorkItemId);
        currentSession = SessionService.DoesSessionForWorkItemExits(worItemIdAsInt);
        if (currentSession == null)
        {
            currentSession = SessionService.CreateSessionForWorkItem(worItemIdAsInt);
        }
        currentEstimate = new Estimates
        {
            Session = currentSession.Id,
            UserId = Guid.Parse(UserId),
            currentEstimateState = EstimateState.Open
        };

        currentWorkItem = await DevOpsWorkItemService.getWorkItemById(worItemIdAsInt);
        if( (currentWorkItem != null) && ((currentWorkItem.State == "Closed") || (currentWorkItem.CompletedWork != null &&  currentWorkItem.CompletedWork > 0.000001))){
            workItemEstimateable = false;
        }
        debugAverage = EstimateService.GetCurrentAverage(currentSession);
        currentUser = UserService.GetUserById(UserId);
    }

    private void EstimateWorkitem()
    {
        newXP = XpService.Points();
        UserService.AddXPToUser(UserId, newXP);
        string calculatedLevel = XpService.GetLevel(currentUser);
        if (calculatedLevel != currentUser.Level)
        {
            UserService.SetNewLevel(currentUser, calculatedLevel);
            showNewLevel = true;
        }
        EstimateService.SaveEstimate(currentEstimate);
        EstimateService.SaveAverageToWorkItem(currentSession);
        nextLevelXp = XpService.GetNextLevelThreshold(currentUser);
        showFirework = true;
    }

    private void NavigateToHome()
    {
        
        NavigationManager.NavigateTo($"/home/{UserId}");
    }

    private string ValidateEstimatedTime(decimal value)
    {
        if (value < 0.25m)
        {
            return "Der Wert muss größer 0 sein."; // The value must be greater than or equal to 0.25
        }
        return null; // Return null if validation passes
    }






}
