# WF Assist server

## ServerBootstrapper
Contains WF Assist client and server bootstrapping code (js client endpoint and service registrations that can be used in actual hosting app like `_AspNetCore.Host`).</br>
Will be published as part of the WF Assist nuget.

## _AspNetCore.Host project (for debugging and auto generating open api specifications only)
Empty host app that just maps WF Assist endpoints and uses open api middleware.

## Shared
Project with shared infrastructure and utilities.

## Workflows
Module related to workflows - workflow storage and means of workflow execution.

## Executions
Module related to executions - execution management and storage.
