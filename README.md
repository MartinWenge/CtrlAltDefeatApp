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
    ```bash
    git clone https://github.com/MartinWenge/CtrlAltDefeatApp.git
    ```
* add your Azure DevOps username and PAT providing access in [appsettings](appsettings.json)
    ```json
    "AzureDevOpsSettings":{
        "Username": "<add Azure DevOps username here>",
        "Token": "<add Azure DevOps PAT here>"
    },
    ```
* run the application
    ``` bash
    dotnet run
    ```

## Authors

  - Sebastian Christoph
  - Tom Donath
  - [Martin Wengenmayr](https://github.com/MartinWenge)

## License

This project is licensed under the [MIT license](LICENSE.md)

