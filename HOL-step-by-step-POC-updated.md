#HOL step-by step- POC
>This document will briefly walk through how Microservice can be deployed  into Kubernets and scaled as a POC .


[[_TOC_]]

##Solution Architecture

![MicrosoftTeams-image.png](/.attachments/MicrosoftTeams-image-57c2a0e9-663a-4e0d-917c-122265a6b91a.png)


**Note:** Amedius Client is mocked by SOAP service client hosted at [https://samplesoapservice.azurewebsites.net/Service.asmx](https://samplesoapservice.azurewebsites.net/Service.asmx)

##Requirements

1. [Azure Account](https://portal.azure.com/)
1. Valid subscription
1. Visual Studio 
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

By following the same steps above create another  _Kubernetes Service_ named **slaaks-prod** (or anyname) in the same resource group. 

Once both AKS are created open azure cloud shell by clicking the `>_ ` icon on top bar.
then select bash and enter following command replacing myresourceGroup and myAKSCluster with your resourcegroup name and aks cluster name, to enable http routing (do this for both the AKS)
```bash
az aks enable-addons --resource-group <myResourceGroup> --name <myAKSCluster> --addons http_application_routing
```

Now run below command and not down the output (HTTPApplicationRoutingZoneName)
you will get somthing like `cfff4b3c88544e628252.eastus.aksapp.io`
```bash
az aks show --resource-group testing-rg --name slaaks1 --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName -o table
```
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

![Annotation 2020-12-07 170114.png](/.attachments/Annotation2-c3e405e9-7f7d-45cd-a16d-98c77a15a6c8.png)
 

#### 2. Import **PSSService**
Create new repository named **PSSService** in DevOps project. Goto `starter-files/PSSServices` cloned repository and follow the same process to import the repository.


### 3. Create Service Connections On Azure DevOps
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
12. similarly create a service conneciton for `AKS-prod`  and name it `slaaks-prod`

##Exercise 1 - Configure SOAP endpoint to PSSService

###Test SOAP service end
To make the exercise easier we have already hosted the SOAP client as a container instance at Azure .You can check the service end by visiting [SOAP client](https://samplesoapservice.azurewebsites.net/Service.asmx)

###Connect SOAP service endpoint to PSSService

**Note:** _If you like to practice seting up connection with SOAP client manually skip to [Exercise 2](#exercise-2)_

1. Open starter-files/PSSService folder . 
1. Open PSSService.Sln {on Visual Studio} 
1. on Solution Explorer Navigate to `PSService-> Connected Services -> Service Reference1` and open  `ConnectedService.json` .
![image.png](/.attachments/image-7027bfb7-7063-4706-a25f-ac65183ee891.png)

1. on  `ConnectedService.json` update 'ExtendedData.inputs' value on line 6 with the SOAP client URL ( https://samplesoapservice.azurewebsites.net/Service.asmx) .

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
1. In the “Configure WCF Web Service Reference” window, specify the URI of the `SOAP service` (https://samplesoapservice.azurewebsites.net/Service.asmx) as shown in Figure 
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
1. Open PSService on Visual Studio and expand Chart folder - > open values.yaml and update `image.repository` inline 7 with your repository details

```yaml
image:
  repository: <Your Container registry name>.azurecr.io/pssservice
  pullPolicy: IfNotPresent
  # Overrides the image tag whose default is the chart appVersion.
  tag: "latest"
```
2. save the changes
3. Open PayLaterService on Visual Studio and expand Chart folder - > open values.yaml and update `image.repository` inline 7 with your repository details
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
1. Goto PayLaterService in DevOps project repos.
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

## Exercise 4 - Configure Release Pipeline
In this exercise we are going to configure release pipelines for PayLaterService and PSSService. Release pipelines usually used for production deployment and but it only has UI based pipelines.

*****Note: In this exercise we only demonstrate deploying in to production AKS cluster. But in a real application this pipeline may contains may include multiple stages.***

1. Goto DevOps project.
2. Goto **Releases** under Pipelines section.
3. Click on **New Pipeline**.
![Annotation 2020-12-09 162225.png](/.attachments/new-pipeline.png) 

4. Select **Empty Job** on opened tab.

5. Click on Artifacts **Add** and select the build we created in build pipelines.
![image.png](/.attachments/image-f089e254-24b7-45d0-b661-570e4f93aea0.png)
> We are not going to use these files in this exercise, but we need the variables that are imported with the build files for next steps. _For more info about pipeline variables check this [link](https://docs.microsoft.com/en-us/azure/devops/pipelines/release/variables?view=azure-devops&tabs=batch)_

6. Add another new artifact and select **PSSService** repo from Azure Repos Git.
![image.png](/.attachments/image-7c65b425-0d35-4d63-879a-4a6a42f31e9a.png)
> The helm charts in the repo is used for helm deployment.

*****Note: Make sure "_PSSService_Build" is set as "Primary Artifact". You can check this by selecting artifact and then in the tab opened click "..." icon in top right corner.***

7. Next In Stages section goto Tasks/Jobs section.
![image.png](/.attachments/image-bdb829a7-de50-4967-a3ad-915f7db7815e.png)

6. Select **Agent Job** and change its "Agent Specification" to "Ubuntu <latest>".
 > Step 6 is not required

7. Click **+** sign on **Agent Job** to add new job.

8. Select **Helm tool installer** from the selection and click on **Add**.
![image.png](/.attachments/image-014c3b07-ca93-45e2-8cd8-8f6ff5128d88.png)

9. We can use its default setting for this example.

10. Again similar to _Step 7_ select **Package and deploy Helm charts** and click on **Add**.

11. In **Package and deploy Helm charts** change content,
Set `Connection Type` to `Kubernetes Service Connection`.
Set `Kubernetes Service Connection` to your prod. AKS service connection.
Set `Command` to `Upgrade`.
Set `Chart Type` to `File Path`.
Set `Chart Path` to you repo's `charts/pssservice` folder.
Set `Release Name` to `pssservice`.
Set `Set Values` to `image.tag=$(Build.BuildId)`
Check `Install if release not present.` and `Wait`.
![image.png](/.attachments/image-4dc47faf-7c84-4ecd-bd92-f813ab5c3cb4.png)
![image.png](/.attachments/image-a9d8cb24-436f-43f9-a2b4-b4fe5e38ce37.png)

> **Set Values** is used to override values.yml in helm charts.
> **$(Build.BuildId)** is a variable imported with the artifact(in _Step 5_).

12. Save and then click **Create Release** to run the pipeline.
*****Special Note: Release pipelines use the docker image build in build pipeline. Therefore make sure build pipelines is completed before running Release Pipelines. This is only a issue in this exercise. Can be easily prevented in real scenarios.***

13. Now follow the same process for **PayLaterService** accordingly. There is a one different step, **Package and deploy Helm charts** -> **Set Values** should be changed to,
```
image.tag=$(Build.BuildId),ingress.hosts[0].host="paylater.<HTTPApplicationRoutingZoneName>",ingress.hosts[0].paths={"/"}
```
> Update _<HTTPApplicationRoutingZoneName>_ with zoneName got for the production AKS cluster.


14. **Create Release** for PayLaterServices.

### Some Additional Features
1. Triggers (Lighting icon)
Triggers can be added to artifacts and stages.
![image.png](/.attachments/image-c1789868-646c-4464-8d23-41fe9eb9a792.png)

2. Pre-deployment approvals (Person icon)
Can be added to each stage.
![image.png](/.attachments/image-62ef5032-d69b-42af-a072-6d0b1330c2ef.png)

3. Post-deployment approvals

4. And many more. Feel free to try them out as well.

**Check out [Azure Docs](https://docs.microsoft.com/en-us/azure/devops/pipelines/release/?view=azure-devops) for more details.** 



## Exercise 5 - Now lets introduce ingress and API management to the environment
1. open starter-files/PaylaterService
1. go to charts/PaylaterService and open values.yaml
2. overide service and ingress values as follow replacing HTTPApplicationRoutingZoneName with the value you noted down earlier.
```yaml
service:
  type: ClusterIP
  port: 80

ingress:
  enabled: true
  annotations:
    kubernetes.io/ingress.class: addon-http-application-routing
    # kubernetes.io/tls-acme: "true"
  hosts:
    - host: paylater.<HTTPApplicationRoutingZoneName>
      paths:
        - /
  tls: []
  #  - secretName: chart-example-tls
  #    hosts:
  #      - chart-example.local
```
> **Note:** remember the host address you will need this is below exercise.

3. save and commit the changes .
4. push changes to remote .
5. Now new build will be automaticlly triggered and on Successful compile new build will be deployed to our developemnt environment `slaaks ` cluster .

> **Note:** If the build fails please update the `paylaterservice/templates/ingress.yaml` `apiVersion` as from *v1beta1* to *v1* and change the backend service as below from *line 36.*

```yaml
apiVersion: networking.k8s.io/v1
```
```yaml
            backend:
              service:
                name: {{ $fullName }}
                port: {{ $svcPort }}
                  number: {{ $svcPort }}
            pathType: ImplementationSpecific
```

  your final `ingress.yaml` should be as follows,

  ```yaml
{{- if .Values.ingress.enabled -}}
{{- $fullName := include "PayLaterService.fullname" . -}}
{{- $svcPort := .Values.service.port -}}
{{- if semverCompare ">=1.14-0" .Capabilities.KubeVersion.GitVersion -}}
apiVersion: networking.k8s.io/v1
{{- else -}}
apiVersion: extensions/v1
{{- end }}
kind: Ingress
metadata:
  name: {{ $fullName }}
  labels:
    {{- include "PayLaterService.labels" . | nindent 4 }}
  {{- with .Values.ingress.annotations }}
  annotations:
    {{- toYaml . | nindent 4 }}
  {{- end }}
spec:
  {{- if .Values.ingress.tls }}
  tls:
    {{- range .Values.ingress.tls }}
    - hosts:
        {{- range .hosts }}
        - {{ . | quote }}
        {{- end }}
      secretName: {{ .secretName }}
    {{- end }}
  {{- end }}
  rules:
    {{- range .Values.ingress.hosts }}
    - host: {{ .host | quote }}
      http:
        paths:
          {{- range .paths }}
          - path: {{ . }}
            backend:
              service:
                name: {{ $fullName }}
                port:
                  number: {{ $svcPort }}
            pathType: ImplementationSpecific
          {{- end }}
    {{- end }}
  {{- end }}
```

Now go to `slaaks` cluster using azure portal and connect to it using Azure CLI. Then type `kubectl delete pod paylaterservice` to delete the running pod. You may try to run the build again now from Azure Devops.

6. once the build is complete go to Release under Pipelines on side menu.
7. click on `create release ` ,(this step is not nessary if the release pipeline trigger is set to automatic) 
8. If you have enable pre deployment approval an email will be sent to the person you have assigned earlier  , once he appove the release it will be deployed (to the production environment).


9. Now lets introduce API management service to handle permission ,
10. go back th portal.azure.com
11. on top search bar type "API management service" and create new Api Management service .
12. On the API Management service blade, enter the following:
   - Name: Enter sla-api.
   - Subscription: Select the subscription you are using for this hands-on lab.
   - Resource Group: Select the resource group  you are using for this hands-on lab.
   - Location: Select the location you are using for resources in this hands-on lab.
   - Organization name: Enter SLA
   - Administrator email: Enter an email add that can receive API Management admin notifications.
   - Pricing tier: Select Developer (No SLA).
   - Enable Application Insights: Uncheck this box.

13. click Create
14. once the resource is created In the Azure portal, navigate to your API Management Service by selecting it from the list of resources under your resource group.
15. On the API Management service select the APIs blade, and then select + Add API and select OpenAPI.
16. A dialog to Create from OpenAPI specification is displayed. Select Full to expand the options that need to be entered.

<IMG src="https://raw.githubusercontent.com/microsoft/MCW-App-modernization/master/Hands-on%20lab/media/e8-t1-create-api-dialog.png">

17. open a new tab and go to the ingress host address you have created earlier (paylater.cfff4b3c88544e628252.eastus.aksapp.io). You will get "Page cant be found" error
18. add `/swagger` to the path (paylater.cfff4b3c88544e628252.eastus.aksapp.io/swagger) . Now you will be redirected to swgger api documentaion page .
19. On the Swagger page for your API App, right-click on the swagger/v1/swagger.json file link just below the PayLaterService API title, and select Copy link address.
<IMG src="https://github.com/microsoft/MCW-App-modernization/raw/master/Hands-on%20lab/media/swagger-copy-json-link-address.png">
20. Return to the API Management Create from OpenAPI specification dialog, and enter the following and click create.
   - OpenAPI specification: Paste the copied link address from your Swagger page.
   - Display name: This is automatically populated from the Swagger definition.
   - Name: This is automatically populated from the Swagger definition.
   - URL scheme: Choose Both.
   - Products: Select the Unlimited tag by clicking the field and selecting it from the dropdown list.
21. Next, select the Settings tab. On the Settings tab, enter the URL of your API App, starting with http://. (http://paylater.cfff4b3c88544e628252.eastus.aksapp.io) and set the URL scheme to http . Make sure Unlimited tag is selected on Products
22. Notedown the auto generated base url which will be used to access paylater service .(http://sla-api.azure-api.net0)
23. save the changes .
24. now go to poratal overview under Developer options and open `Developer Portal (Leggacy)`
25. on Developer poratal go to API tab and open Paylater Service API you created.
26. you can use the `Try it` option to call test the api.
27. The same API managemnt can be used to mange multiple API end points (Can mange both developemnt and production environment ) . EXplore the possiblities .
**Note : here we have not restricted the direct call to the API end (paylater.<HTTPApplicationRoutingZoneName>.eastus.azurecr.io) , but in real production environment it can be restricted to accept only the request comming through API manager .

#END