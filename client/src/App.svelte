<script>
  import "./app.css";
  import Workflows from "./workflows/Workflows.svelte";
  import { Icons } from "$lib/components/ui/icons";
  import { ModeWatcher } from "mode-watcher";
  import { TurboEdgeGradient } from "$lib/css";
</script>

<div class="size-full">
  <ModeWatcher />
  <Icons />
  <TurboEdgeGradient />
  <Workflows />
</div>
