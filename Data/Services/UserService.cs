using ctrlAltDefeatApp.Data.DatabaseContext;
using ctrlAltDefeatApp.Data.Entities;

namespace ctrlAltDefeatApp.Data.Services;

public class UserService
{
      public UserService(CtrlAltDefeatDatabaseContext context, EstimateService estimateService){
        _context = context;
        _estimateService = estimateService;
    }
    
    private CtrlAltDefeatDatabaseContext _context;
    private EstimateService _estimateService;
    
    public void AddXPToUser(string userId, int XP){
        User user = _context.Users.First(u => u.Id == Guid.Parse(userId));
        user.CurrentXP += XP;
        _context.Users.Update(user);
        _context.SaveChangesAsync();
    }

    public void SetNewLevel(User user, string level){
        user.Level = level;
        _context.Users.Update(user);
        _context?.SaveChangesAsync();
    }

    public User GetUserById(string userId){
         var user = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(userId));
        if(user != null){
            return user;
        }
        else{
            return  null;
        }
    }

    public List<User>GetHighscoreListAverageEstimation(){
        var userList = _context.Users
        .OrderByDescending(user => user.currentAverage)
        .ToList();

        return userList;
    }

    public List<User>GetHighscoreListXP(){
        var userList = _context.Users
        .OrderByDescending(user => user.CurrentXP)
        .ToList();

        return userList;
    }

    public void SetConfirmXP(Guid userId, bool value = false){
        User user = _context.Users.First(x => x.Id == userId);
        user.ConfirmNewXP = value;
        _context.Users.Update(user);
        _context.SaveChangesAsync();
    }

    public void UpdateAverageEstimateDifference(User user){
        user.currentAverage = _estimateService.GetAverageEstimateTimeForUser(user.Id);
        _context.Users.Update(user);
        _context.SaveChangesAsync();
    }
}
