<script>
  import "./app.css";
  import Workflows from "./workflows/Workflows.svelte";
  import { Icons } from "$lib/components/ui/icons";
  import { ModeWatcher } from "mode-watcher";
  import { TurboEdgeGradient } from "$lib/css";
  import { Toaster } from "svelte-sonner";
</script>

<div class="size-full">
  <ModeWatcher />
  <Icons />
  <TurboEdgeGradient />
  <Workflows />
  <Toaster theme="dark" position="top-right" duration={5000} expand={true} />
</div>
