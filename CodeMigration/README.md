# Dot Net Code Migration LAB

## Guidelines
* Please clone this project to your local machine 
`https://github.com/MSPartnerCreativeSoftware/AzureMig`
* Open **CodeMigration** Project from visual studio and click on ***CodeMigration->Properties*** to identify the current target framework on the project
* From a terminal, run the following to install the try-convert. *(It’s a global tool, so you can run the command anywhere.)*
`dotnet tool install -g try-convert`
* If you have try-convert installed but need to upgrade to a newer version *(You must have version 0.7.212201 or later to use the Upgrade Assistant)*, execute the following
`dotnet tool update -g try-convert`
* We’re now ready to install the .NET Upgrade Assistant. To do so, execute the following from a terminal:
`dotnet tool install -g upgrade-assistant`
* After installing the .NET Upgrade Assistant, run it by navigating to the folder where your solution exists and entering the following command.
`dotnet tool upgrade -g upgrade-assistant`
* Navigate to the root of your solution and simply execute:
`upgrade-assistant upgrade SampleNetFrameworkAPI.sln`
* The tool executes and shows you the steps it will perform. For each step in the process, you can apply the next step in the process, skip it, see details or configure logging. Most of the time, you’ll want to select Apply next step. To save some time, you can press Enter to do this.
* Go back to visual studio and check the target framework is changed to ***.Net 5*** 

* You have successfully ported your project from .NET Framework to .NET 5
