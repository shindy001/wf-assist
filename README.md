# Workflow Assist

Workflow Assist is an simple app for execution of work/task specified by workflows. It is expected to run alongside your server/app during `development` and contain workflows somewhat related to your server/app like creation of test data (seeding) with auto login, execution of specific actions or cleaning actions.

<img width="2467" height="1359" alt="Image" src="https://github.com/user-attachments/assets/ed5640c7-7a25-44aa-9605-7a8a9d0573ca" />

## How To Use
App (server + UI client) is expected to be deployed and released as nuget package (when MVP is done). Can also be started with dev server directly from the repository.

## Current state of app features
- Workflows
  - [x] Execution engine (server)
  - [x] Topology sorter (server)
  - [x] Workflow management (CRUD)
  - [x] Nodes drag and drop to canvas
  - [x] Node result references in other nodes (referenced node must run before the node where it is used) - i.e. reference to node1 json result with access token property `#{node:1}.access_token`
  - [x] Request node (able to do http actions - GET, POST, etc.)
  - [x] Header node (can set custom headers to http client used during workflow, i.e. authorization header for authentication)
  - [ ] Result node - printing some result or part of result to console or some dialog when workflow ends
  - [ ] Openapi spec support (upload spec file) - dynamicly create nodes according to actions in openapi spec (spec file should also be saved and nodes grouped according to api name)
  - [ ] Meta workflow - run multiple dependent workflows (i.e. `create data workflow` depends on `login workflow` which should hence execute first)
  - [ ] Custom nodes (idea) - maybe a DB specific node for direct seeding via sql, to execute a specific program/app on host or maybe some kind of script support for direct scripting nodes
- Executions
  - [x] Workflow execution
  - [x] Start/End notifications (via SSE)
  - [ ] Executions overview (results + workflow snapshot)
  - [ ] Execution animation (animation of the execution, node after node)
- Variables
  - [ ] Variables overview + crud
  - [ ] Ability to reference variable in workflows (workflows will need special resolver) like `#{var:[varName]}`

## Technology stack
#### Backend
  - aspnetcore web api
  - SQLite (DB)
#### FrontEnd
  - Svelte SPA
  - Svelteflow (workflow UI engine)

## Dev Requirements
- `Visual Studio 2026` or `Rider` that supports .net 10
- .net 10 SDK

## How to run
1. Open `WFAssist.slnx`
1. Select `[server + client] start` configuration (Rider only) and run it or run server and client separately.
