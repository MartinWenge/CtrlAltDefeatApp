namespace ctrlAltDefeatApp.Data.Entities;

public class User
{
    public Guid Id {get; set;}
    public string Username {get; set;}
    public string Password {get;set;}
    public string Level {get; set;}
    public int CurrentXP {get; set;}  
    public double currentAverage {get; set;}
    public bool ConfirmNewXP {get; set;}
}
