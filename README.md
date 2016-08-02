# Exploring Microservices
Sample code for the 'Exploring Microservices' talk.

###To run on a local Service Fabric cluster:
- Create an Azure storage account
- Create a Document db account
- Update credentials in ContosoPizzaApp\ApplicationParameters\Local.xml
- Run ContosoPizzaApp.sfproj
- Wait till all the partitions of Store service are ready
- Execute a POST request to http://localhost:8085/admin/seed/store to upload some store data. No body needed
- Execute a POST request to http://localhost:8085/admin/seed/product to upload some product data. No body needed
- Browse to http://localhost:8080/ to launch the UI

##Architecture
![Contoso Pizza Architecture](https://30store.blob.core.windows.net:443/public/ContosoPizza.png "Contoso Pizza Architecture")
