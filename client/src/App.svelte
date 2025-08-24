<script>
    import "./app.css";
    import Workflows from "./workflows/Workflows.svelte";
    import { Icons } from "$lib/components/ui/icons";
    import { ModeWatcher } from "mode-watcher";
</script>

<div style:height="100vh" style:width="100vw" class="relative">
    <ModeWatcher />
    <Icons/>
    <Workflows/>
</div>
