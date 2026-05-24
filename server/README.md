# WF Assist server

## AspNetCore.App
Contains WF Assist api and js client endpoint and service registrations that can be used in actual hosting app like ```_AspNetCore.Host (aspnetcore webapi)```.</br>
Will be published as part of the WF Assist nuget, these endpoints will be used by WF Assists client app for processing on backend.

## _AspNetCore.Host project (for debugging and auto generating openapi specifications only)
Empty host app that just maps WF Assist endpoints and uses openapi middleware.

## Shared
Project with shared infrastructure and utilities.

## Workflows
Module related to workflows - workflow storage and means of workflow execution.
