[![Build Status](https://dev.azure.com/wtwd/Ease%20Maker/_apis/build/status/Repository.Pattern.Azure.Storage.Table?branchName=master)](https://dev.azure.com/wtwd/Ease%20Maker/_build/latest?definitionId=3&branchName=master)

# Introduction 
Repository pattern implementation for microsoft azure storage table.
It is probably not the fastest one but maybe one of the most simple.
The tradeof is the use of dynamic in order to map from model objects. to table entites.

This implementation is base on: 
[https://github.com/Crokus/azure-table-storage-repo](Crokus azure table storage repository)

This implementation is leverating:
[https://github.com/mgravell/fast-member](Mgravell Fast member library)

## IMPORTANT: 
This implementation is based on the las version of WindowsAzure.Storage (9.3.3) 
The next versions of azure storage client libraries include the table support under cosmosDb and does not currently support net standard.

# Getting Started
Just get it and build it. I was using VS 2019

The idea is to have a separate IRepository for each domain object. 
The domain object will know nothing about the storage layer and the effort to build the storage layer is minimum.

The tradeof of being so easy to use is the speed and the supported types, that we will try to improve with the time.
For the first version only the Azure Storage Table types are supported:
[https://docs.microsoft.com/en-us/rest/api/storageservices/understanding-the-table-service-data-model](Understanding the Table Service Data Model)


# Build and Test
Building is easy since there it not special dependecies, 
but in order to run the tests you have to manually launch the Microsoft Azure Storage Emulator.

Once you have the storage emulator up and running, just run the tests.
With the test you have an idea of how to use it and what are the limitations.

