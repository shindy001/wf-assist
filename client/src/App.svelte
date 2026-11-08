<script>
  import "./app.css";
  import { Toaster } from "$lib/components/ui/toaster";
  import { connectToMessageHub } from "$lib/serverMessageHub";
  import HiddenResources from "$lib/HiddenResources.svelte";
  import Router from "svelte-spa-router";
  import routes from "./routes/routes";

  await connectToMessageHub();
</script>

<HiddenResources />
<Toaster />
<Router {routes} />
