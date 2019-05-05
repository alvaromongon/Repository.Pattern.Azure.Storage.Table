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
Just get it and build it. I was using VS 2017 Enterprise.

The idea is to have a separate IRepository for each domain object. 
The domain object will know nothing about the storage layer and the effort to build the storage layer is minimum.

# Build and Test
Take a look at the tests to understand how to use it.
