export function createRenameWorkflowCommand() {
    return async (currentName: string, newName: string)=> {
        if (currentName === newName) {
            return;
        }
        // TODO - send rename request to server - should unique name be required ???
    }
}