namespace ctrlAltDefeatApp.Data.Services;
using ctrlAltDefeatApp.Data.DatabaseContext;
using ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.Models;

public class SessionService
{

    public SessionService(CtrlAltDefeatDatabaseContext context){
        _context = context;
    }

    private CtrlAltDefeatDatabaseContext _context;

    public Session DoesSessionForWorkItemExits(int workItemId){
    
        var session = _context.Sessions.FirstOrDefault(s => s.WorkItemId == workItemId);
        return session;
    }

    public Session CreateSessionForWorkItem(int workItemId){
        Session newSession = new Session{
            WorkItemId = workItemId,
            CurrentSessionState = SessionState.Open
            };
       _context?.Sessions.Add(newSession);
       _context?.SaveChangesAsync();

       return newSession;
    }

    public Session GetSessionFromEstimate(Estimates estimate){
        Session session = _context.Sessions.First(x => x.Id == estimate.Session);
        return session;
    }

    public Session GetSessionFromWorkItem(Workitem workitem){
        Session session = _context.Sessions.First(x => x.WorkItemId == workitem.Id);
        return session;
    }

    public int GetWorkItemIdFromSessionId(int sessionId){
        Session session = _context.Sessions.First(x => x.Id == sessionId);
        return session.WorkItemId;
    }
}
