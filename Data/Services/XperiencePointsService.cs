using ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.Services;
using ctrlAltDefeatApp.Data.DatabaseContext;


namespace ctrlAltDefeatApp.Data.Services;

public class XperiencePointsService
{
    public XperiencePointsService(UserService userService, EstimateService estimateService, DevOpsWorkItemService devOpsService, SessionService sessionService){
        _userService = userService;
        _estimateService = estimateService;
        _devOpsService = devOpsService;
        _sessionService = sessionService;
    }

    private UserService _userService;
    private EstimateService _estimateService;
    private DevOpsWorkItemService _devOpsService;
    private SessionService _sessionService;

    private string[] levelNames = {
        "Neuling", "Anfänger", "Einsteiger", "Lernender", "Schüler",
        "Fortgeschrittener", "Kenner", "Erfahrener", "Experte", "Meister",
        "Proficient", "Virtuose", "Fachmann", "Spezialist", "Gelehrter",
        "Kundiger", "Könner", "Experte", "Guru", "Weiser",
        "Koryphäe", "Veteran", "Profi", "Sachverständiger", "Experte",
        "Genie", "Wunderkind", "Visionär", "Pionier", "Legende"
    };
    private int[] xpThresholds = {
        0, 20, 50, 100, 200,
        3000, 4000, 5000, 7000, 10000,
        15000, 20000, 25000, 30000, 35000,
        40000, 45000, 50000, 60000, 70000,
        80000, 90000, 100000, 120000, 140000,
        160000, 180000, 200000, 250000, 300000,
        350000
    };

   public string GetLevel(User user)
    {

    int currentXp = user.CurrentXP;
    for (int i = 0; i < xpThresholds.Length; i++)
    {
        if (currentXp < xpThresholds[i])
        {
            return levelNames[i - 1];
        }
    }

    return levelNames[levelNames.Length - 1]; // Genie, wenn XP alle Grenzen überschreitet
}

public int GetNextLevelThreshold(User user)
{
    int currentXp = user.CurrentXP;

    // Aktuellen Level-Index finden
    int currentLevelIndex = 0;
    for (int i = 0; i < xpThresholds.Length; i++)
    {
        if (currentXp < xpThresholds[i])
        {
            currentLevelIndex = i;
            break;
        }
    }

    // Nächstes Level ermitteln
    int nextLevelIndex = currentLevelIndex ;
    if (nextLevelIndex >= xpThresholds.Length)
    {
        // Bereits höchstes Level erreicht
        return xpThresholds[xpThresholds.Length - 1]; // Höchste Punktstufe zurückgeben
    }

    // Punktstufe des nächsten Levels zurückgeben
    return xpThresholds[nextLevelIndex];
}

    public int CalculatePoints(double completedWork, double guessedEstimate)
    {
        // Calculate the absolute difference between completed work and guessed estimate
        double difference = Math.Abs(completedWork - guessedEstimate);

        // Calculate the relative difference between completed work and guessed estimate
        double relDifference = difference / completedWork;

        // Calculate points using an exponential function
        double points = 100 * Math.Exp(-(Math.Log(100))*relDifference);

        // Ensure points are within the range [0, 100]
        points = Math.Max(0, Math.Min(100, points));

        // Round points to the nearest integer
        return (int)Math.Round(points);
    }

    public int Points(int amount)
    {   
        return amount;
    }

    // Overloaded method without the 'amount' parameter
    public int Points()
    {
        int standard = 10;
        return standard;
    }

    public async Task<int> checkForFinishedWorkitems(Guid userId){
        int newXP = 0;
        List<Estimates> openEstimates = _estimateService.GetAllOpenEstimatesForUser(userId);
        List<int> workitemIds = [];

        if(openEstimates != null){
            foreach(Estimates estimate in openEstimates){
                workitemIds.Add(_sessionService.GetSessionFromEstimate(estimate).WorkItemId);
            }
        }

        List<Workitem> closedWorkItems = await _devOpsService.getListOfClosedWorkitems(workitemIds);

        if(closedWorkItems != null)
        {
            foreach(Workitem closedWorkItem in closedWorkItems){
                Session session = _sessionService.GetSessionFromWorkItem(closedWorkItem);
                Estimates estimateForXp = _estimateService.GetEstimateFromSession(session);
                estimateForXp.RealEffort = closedWorkItem.CompletedWork != null ? closedWorkItem.CompletedWork.Value : 0.0;
                estimateForXp.currentEstimateState = Models.EstimateState.SessionClosed;
                _estimateService.UpdateEstimate(estimateForXp);
                newXP += this.CalculatePoints(estimateForXp.RealEffort, estimateForXp.EstimatedTime);
            }
        }

        return newXP;
    }
}
