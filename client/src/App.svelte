<script>
  import "./app.css";
  import { Toaster } from "$lib/components/ui/toaster";
  import HiddenResources from "$lib/HiddenResources.svelte";
  import Router from "svelte-spa-router";
  import routes from "./routes";
</script>

<HiddenResources />
<Toaster />
<Router {routes} />
