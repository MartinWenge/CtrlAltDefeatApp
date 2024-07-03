namespace ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.Models;

public class Session
{
    public int Id {get; set;}

    public int WorkItemId {get; set;}

    public SessionState CurrentSessionState {get; set;}
}
