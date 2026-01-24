<script>
  import "./app.css";
  import Workflows from "./workflows/Workflows.svelte";
  import { Icons } from "$lib/components/ui/icons";
  import { ModeWatcher } from "mode-watcher";
  import { TurboEdgeGradient } from "$lib/css";
  import { Toaster } from "$lib/components/ui/toaster";
</script>

<div class="size-full">
  <ModeWatcher track={false} defaultMode={"dark"} />
  <Icons />
  <TurboEdgeGradient />
  <Workflows />
  <Toaster />
</div>
