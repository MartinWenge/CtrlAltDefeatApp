using Microsoft.AspNetCore.Components;
using ctrlAltDefeatApp.Data.Models;
using ctrlAltDefeatApp.Data.Entities;
using ctrlAltDefeatApp.Data.DatabaseContext;
using ctrlAltDefeatApp.Data.Services;

namespace ctrlAltDefeatApp.Components.Pages;

public partial class Login : ComponentBase{

    [Inject]
    private CtrlAltDefeatDatabaseContext databaseContext {get; set;}

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private DevOpsWorkItemService devOpsService { get; set; }

    private LoginModel loginModel;

    private User user;
    private CtrlAltDefeatDatabaseContext? _context;
    private bool showError = false;
    private bool registerNewUser = false;

    protected override void OnInitialized ()
    {
        loginModel = new LoginModel();
    }

    private void UserLogin()
    {
        if (registerNewUser)
        {
            User newUser = new User { Username = loginModel.username, Password = loginModel.password, Id = Guid.NewGuid(), CurrentXP = 0, Level = "Neuling", ConfirmNewXP=false};
            databaseContext.Users.Add(newUser);
            databaseContext.SaveChanges();
        }
        var foundUser = GetUserFromDB();
        if(foundUser != null){
            NavigationManager.NavigateTo($"/home/{foundUser.Id}");
        }
        else{
             showError = true;
             loginModel.username = "";
             loginModel.password = "";
        }
    }

    private User GetUserFromDB(){
        user = databaseContext.Users.FirstOrDefault(x => x.Username == loginModel.username && x.Password == loginModel.password);
        return user;
    }

    public void ChangeFormPurpose(){
        registerNewUser = !registerNewUser;
    }

}


