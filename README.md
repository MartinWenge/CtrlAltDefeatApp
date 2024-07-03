# CtrlAltDefeat - a hackathon app

Gamification App for effort estimation. An Azure DevOps Board is used to provide the tasks. The app is used to estimate existing tasks and get rewards for estimations and the quality of estimations.

## Getting Started

These instructions will give you a copy of the project up and running on
your local machine for development and testing purposes.

### Prerequisites

Requirements for the software and other tools to build, test and push 
- [Set up dev environment for ASP.NET Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/startup?view=aspnetcore-8.0)
- [The project uses Mud Blazor Templates](https://www.mudblazor.com/getting-started/installation#online-playground)
- [An Azure DevOps project is required to have a source of tasks to estimate](https://learn.microsoft.com/en-us/azure/devops/organizations/projects/create-project?view=azure-devops&tabs=browser)

### Installing

* clone the repository
    git clone https://github.com/MartinWenge/CtrlAltDefeatApp.git
* add your Azure DevOps username and PAT providing access in [appsettings](appsettings.json)
    "AzureDevOpsSettings":{
        "Username": "<add Azure DevOps username here>",
        "Token": "<add Azure DevOps PAT here>"
    },
* run the application
    dotnet run

## Authors

  - Sebastian Christoph
  - Tom Donath
  - [Martin Wengenmayr](https://github.com/MartinWenge)

See also the list of
[contributors](https://github.com/PurpleBooth/a-good-readme-template/contributors)
who participated in this project.

## License

This project is licensed under the [CC0 1.0 Universal](LICENSE.md)
Creative Commons License - see the [LICENSE.md](LICENSE.md) file for
details
