namespace ctrlAltDefeatApp.Data.Services;
using ctrlAltDefeatApp.Data.DatabaseContext;
using ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.Models;

public class EstimateService
{

    public EstimateService(CtrlAltDefeatDatabaseContext context, DevOpsWorkItemService devOpsService){
        _context = context;
        _devOpsService = devOpsService;
    }

    private CtrlAltDefeatDatabaseContext _context;
    private DevOpsWorkItemService _devOpsService;

    private List<Estimates>? AllEstimatesPerSession;
    

    public void SaveEstimate(Estimates estimateToSave){
    
        _context?.Estimates.Add(estimateToSave);
       _context?.SaveChangesAsync();
    }

    public void UpdateEstimate(Estimates estimateToUpdate){
        _context.Estimates.Update(estimateToUpdate);
        _context.SaveChangesAsync();
    }

    public double GetCurrentAverage(Session session){
        double average = 0;

        AllEstimatesPerSession= _context.Estimates.Where(e => e.Session == session.Id ).ToList();

        foreach(Estimates estimate in AllEstimatesPerSession){
            average += estimate.EstimatedTime;
        }

        average = average / AllEstimatesPerSession.Count;
        return average;
    }
    public void SaveAverageToWorkItem(Session session){
        double average = GetCurrentAverage(session);
        _devOpsService.updateWorkItemEstimates(session.WorkItemId, average);
    }

    public List<Estimates> GetAllClosedEstimatesForUser(Guid userId){
         List<Estimates> estimates = _context?.Estimates.Where(x => x.UserId == userId && x.currentEstimateState == EstimateState.SessionClosed).ToList();
         return estimates;
    }

    public List<Estimates> GetAllOpenEstimatesForUser(Guid userId){
         List<Estimates> estimates = _context?.Estimates.Where(x => x.UserId == userId && x.currentEstimateState == EstimateState.Open).ToList();
         return estimates;
    }


    public double GetAverageEstimateTimeForUser(Guid userId){
        List<Estimates> estimates = GetAllClosedEstimatesForUser(userId);

        double averagePrecision;
        if (estimates == null || estimates.Count == 0){
            return 0;
        }else{
            double precisionSum = 0;
            foreach(Estimates estimate in estimates){
                // je näher an 100 %, desto besser
                double precision = 0.0;
                if(estimate.EstimatedTime <= estimate.RealEffort){
                    precision = 100 - ((Math.Abs(estimate.EstimatedTime - estimate.RealEffort)/estimate.RealEffort)*100);
                } else {
                    precision = 100 - ((Math.Abs(estimate.RealEffort - estimate.EstimatedTime)/estimate.EstimatedTime)*100);
                } 
                precisionSum += precision;
            }
            averagePrecision = precisionSum / estimates.Count;
        }
        return averagePrecision;
    }

    public Estimates GetEstimateFromSession(Session session)
    {
        Estimates estimate = _context.Estimates.First(x => x.Session == session.Id);
        return estimate;
    }
}