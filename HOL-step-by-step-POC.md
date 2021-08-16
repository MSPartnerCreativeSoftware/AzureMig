#HOL step-by step- POC
>This document will briefly walk through how Microservice can be deployed  into Kubernets and scaled as a POC .


[[_TOC_]]

##Solution Architecture

![MicrosoftTeams-image.png](/attachments/MicrosoftTeams-image-57c2a0e9-663a-4e0d-917c-122265a6b91a.png)

**Note:** Amedius Client is mocked by SOAP service client hosted at [http://soapservice.eastus.azurecontainer.io/Service.asmx](http://soapservice.eastus.azurecontainer.io/Service.asmx)

##Requirements

1. [Azure Account](https://portal.azure.com/)
1. Valid subscription
1. Visual code 
1. Azure DevOps Account
1. NET Core 3.1 SDK
1. [Helm](https://helm.sh)



##Exercise 0 -Setting Up Resources
This part covers the setup part of Azure resources and DevOps Project. First clone this repository to your local machine. 
 

###Provision required Resources on Azure Portal 
Goto [Azure Portal](https://portal.azure.com/)

#### 1. Create Resource group
<IMG  src="https://github.com/microsoft/MCW-App-modernization/raw/master/Hands-on%20lab/media/azure-services-resource-groups.png"  alt="Resource groups is highlighted in the Azure services list."/>


<IMG  src="https://github.com/microsoft/MCW-App-modernization/blob/master/Hands-on%20lab/media/resource-groups-add.png?raw=true"  alt="resource-groups-add.png"/>


Create new _Resource Group_ named **sla-rg** (or anyname) and select region as west US 2 or  East us


![image.png](/.attachments/image-0e49628f-0e0b-46f9-8ed6-c98af31ef592.png)
>_Note: Remeber this name, it will be needed next steps_ 

And `Click Review+Create` -> `Create` .

#### 2. Create ACR -Azure Container Registry

![image.png](/.attachments/image-7ac0fa0f-fa97-4692-9f57-abd8e323e0f4.png)

- Open the newly created resource group and click `+Add ` 
- Search for "container registry" and select `Azure container registry` - > `Create` .

- Create new _Container Registry_ named **slaacr** (or anyname).
![image.png](/.attachments/image-2da92afd-21d6-4e07-aa43-b0e614ad9362.png)

>_Note: Remeber this name, it will be needed next steps_

- And `Click Review+Create` -> `Create` .

- After resource is created goto resource. Note down the **Login server** name.


#### 3. Create AKS- Azure Kubernets Service Cluster
**Follow these steps when creating Kubernetes Cluster.**
- Create new _Kubernetes Service_ named **slaaks** (or anyname) in the same resource group. 
![image.png](/.attachments/image-bddd2861-6e3e-4f9a-a148-c1f770c382c5.png)
>_Note: Remeber this name, it will be needed next steps_

- switch to Authentication Tab and select **System-assigned managed identity** as Authentication method.

![image.png](/.attachments/image-dd0263cf-8cb0-4592-9fc8-bb5a15182246.png)

- Next Integrations tab select created container registry in "Container registry" field.

![image.png](/.attachments/image-ee04c40e-ff2c-41ab-bfa1-6fb97beeeb61.png)

- Then `Review + create`->`Create` .


### 4. Create a new Project in Azure DeOps
- First go to [Azure DevOps](https://dev.azure.com/).
> Create a DevOps account if you don't have one using the same email you used for Azure portal.
- If you don't have a organization created by default create one by clicking on the `New Organization` and follow the instructions .
- open the `Organization` .
- Then click on **+New Project** to create new project. Give **SLA** as Project Name, select **Private** and click **Create**.

![image.png](/.attachments/image-2067d7ea-9eee-4041-b449-e7afa9c21eff.png)


### 5. Import Lab resources to Azure DevOps

> In ths task you will Import two services in cloned repository (in `starter-files` folder) to created DevOps project.

#### 1. Import **PayLaterService**
Create new repository named **PayLaterService** in DevOps Project (_Uncheck add a README_).

Now goto `starter-files/PayLaterService` folder in cloned repository and  open folder in terminal. Then use following commands  inside `PaylaterService` folder.
>**Note : ** Do not delete the .gitignore or .dockerignore files .

```bash
git init

git add .

git commit -m "initial commit"
```

Then use Commands in _Push an existing repository from command line_ to import.

![Annotation 2020-12-07 170114.png](/.attachments/Annotation%202020-12-07%20170114-c3e405e9-7f7d-45cd-a16d-98c77a15a6c8.png)
 

#### 2. Import **PSSService**
Create new repository named **PSSService** in DevOps project. Goto `starter-files/PSSServices` cloned repository and follow the same process to import the repository.


### 3. Create Service Connection On Azure DevOps
**Note:**
> For this task use the exact service name given on the instruction as the same was used in pipeline .yaml files  .

1. Now goto SLA project in DevOps. 
2. Goto **Project Settings**. (At the bottom left corner) .
3. Select **Service connections** under Pipelines section.
4. Click `New service connection` .
5. Select type **Docker Registry** and select Azure container Registry at the top .
6. Select the subscription you are using for this lab and from the dropdown select previously created docker registry and give **slaacr** as service connection name.

**Note:**
> Dont forget to select `Grant permission to all pipelines`  .

![image.png](/.attachments/image-4ad64ec4-8507-4f6a-abde-c85b62f7c324.png)

7. Save.

8. Now Lets create a new service connection for for AKS cluster.
9. Select type "Kubernetes" and next.
10. Select _Azure Subscription_ as Authentication method and select the subscription you used for the lab then  select previous created kubernetes cluster ,set `Namespace` to `Default`and name the service connection as **slaaks** .
**Note:**
> Dont forget to select `Grant permission to all pipelines`  .

![image.png](/.attachments/image-3799cfb7-0f7b-419e-b2fd-c9cac8ef24e2.png)

11. Save.

##Exercise 1 - Configure SOAP endpoint to PSSService

###Test SOAP service end
To make the exercise easier we have already hosted the SOAP client as a container instance at Azure .You can check the service end by visiting [SOAP client](http://soapservice.eastus.azurecontainer.io/Service.asmx)

###Connect SOAP service endpoint to PSSService

**Note:** _If you like to practice seting up connection with SOAP client manually skip to [Exercise 2](#exercise-2)_

1. Open starter-files/PSSService folder . 
1. Open PSSService.Sln {on Visual Code} 
1. on Solution Explorer Navigate to `PSService-> Connected Services -> Service Reference1` and open  `ConnectedService.json` .
![image.png](/.attachments/image-7027bfb7-7063-4706-a25f-ac65183ee891.png)

1. on  `ConnectedService.json` update 'ExtendedData.inputs' value on line 6 with the SOAP client URL ( http://soapservice.eastus.azurecontainer.io/Service.asmx) .

1. save `ConnectedService.json` . The updated `ConnectedService.json` should now look like below 
![image.png](/.attachments/image-17d1cecf-ea4e-4f6a-90a5-d259272be4b4.png)

1. Right Click on `ServiceReference1` and click `Update Microsoft WCF Web Service Reference Provider... .
![image.png](/.attachments/image-f39c20a9-4dac-4932-9c62-a2abb511afa0.png)

1. run the service locally by pressing `Ctrl+F5`  or clicking on the run method on top ribbon(select PSSservice from drop down) .
![image.png](/.attachments/image-170d07d3-25cd-4516-98da-1b0832c3d29b.png)

1. Now a new browser window will be opened and a `json` object is returned
`{"id":12,"firstName":"Nilaan","lastName":"Logan","bookingDate":"2020-12-07T12:42:27.1698983+05:30","status":"Success"}` 
1. commit and push your changes to remote .

_Optionally if you prefer to setup the SOAP endpoint by your self follow the below steps in Exercise 2 else skip to [Exercise 3](#exercise-3)

##Exercise 2

1. First on Solution Explorer Navigate to `PSService-> Connected Services  and right click on `Service Reference1` and select delete .(Confirm if any warning prompted) .
![image.png](/.attachments/image-7027bfb7-7063-4706-a25f-ac65183ee891.png)

1. Now right click on `Connected Services` and click `Add Connected Services`

1. scroll below and On the `Other Services` section click on the “Microsoft WCF Web Service Reference Provider” and wait for the “Configure WCF Web Service Reference” wizard to open.
1. In the “Configure WCF Web Service Reference” window, specify the URI of the `SOAP service` (http://soapservice.eastus.azurecontainer.io/Service.asmx) as shown in Figure 
![image.png](/.attachments/image-df3c1381-849f-4781-8f59-774412313e0d.png)

1. Click Go to view the available services(Optional).Optionally you can provide  a `**namespace** ` but we leave that to the default value `**ServiceReference1**`

1. Click Next . 

1. In the next window, you can specify the data type options. We’ll skip that step here. and Click Next

1. In the next window, you can specify the access level. Let it be public (the default).

1. Click Finish.

1. Wait until the process complete .It will take ~1 min to auto generate the files 

1. Explore the Service created under Connected services

1. Optional : If you have used any other namespace on Service Connection creation Go to Controller - > BookingDetailController.cs and replace any reference to ServiceReference1 with the namespace you have used .
>**Note :** _Controllers are not auto genrated_ 
1. commit and push your changes to remote.

## Exercise 3 - Configure Pipelines on Azure DevOps

This section show how to configure a pipeline to build and  deploy to AKS cluster. We have used [helm charts](https://helm.sh) to manage deployments in AKS cluster.

> _All the requierd resources are included in each repository._(under charts folder)

This diagram shows high level view of this pipeline and AKS cluster.

![SLA diagram.png](/.attachments/SLAdiagram-daf79d44-9a69-45f5-adb1-7f922c5538e3.png)
### 0. update Helm charts
1. Open PSService on Visual Code and expand Chart folder - > open Value.yaml and update `image.repository` inline 7 with your repository details

```yaml
image:
  repository: <Your Container registry name>.azurecr.io/pssservice
  pullPolicy: IfNotPresent
  # Overrides the image tag whose default is the chart appVersion.
  tag: "latest"
```
2. save the changes
3. Open PayLaterService on Visual Code and expand Chart folder - > open Value.yaml and update `image.repository` inline 7 with your repository details
```yaml
image:
  repository: <Your Container registry name>.azurecr.io/pssservice
  pullPolicy: IfNotPresent
  # Overrides the image tag whose default is the chart appVersion.
  tag: "latest"
```
4. save changes
5. commit and push your changes to remote .

### 1. Pipeline for **PSSService**
1. Goto PSSService in DevOps project repos.
2. Click **Set up build** in top right corner. 
![image.png](/.attachments/image-d2123f55-c6f4-49d9-aca5-f2688c388b9a.png)
3. Select **Existing Azure Pipelies YAML file**.
4. Find and select `azure-pipelines.yaml` file from the repo.
5. In review update **containerRegistry** in variables section if you have used different ACR service connection name on Exercise 1.
6. Save and run.
7. Now the service will be deployed in AKS cluster.

> _You can view pipeline progress in Pipelines section_

### 2. Pipeline for **PayLaterService**
1. Goto PSSService in DevOps project repos.
2. Click **Set up build** in top right corner. 
![image.png](/.attachments/image-d2123f55-c6f4-49d9-aca5-f2688c388b9a.png)
3. Select **Existing Azure Pipelies YAML file**.
4. Find and select `azure-pipelines.yaml` file from the repo.
5. In review update **containerRegistry** in variables section if you have used different ACR service connection name on Exercise 1.
6. Save and run.
7. Now the service will be deployed in AKS cluster.
> _You can view pipeline progress in Pipelines section_

After deployment is completed you can goto **slaaks** resource in [Azure Portal](https://portal.azure.com/) and then goto **Services and Ingresses**.

![image.png](/.attachments/image-b7abd4ad-be0c-4524-aac6-760fb2ceeab6.png)

This is the api end of **PayLaterService**.

Try this link in browser `http://<ip address>/api/bookingstatus/{id}` replacing id .

If every thing setup correctly you will get a response like this.
```
{"id":12,"clientId":12,"status":"Success"}
```

**Note :** Here Only the Paylater Service exposed to external parties ,PSSService can only be accessed by kubernetes internal services .

