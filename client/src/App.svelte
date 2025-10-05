<script>
    import "./app.css";
    import Workflows from "./workflows/Workflows.svelte";
    import { Icons } from "$lib/components/ui/icons";
    import { ModeWatcher } from "mode-watcher";
    import { QueryClient, QueryClientProvider } from '@tanstack/svelte-query'

    const queryClient = new QueryClient();
</script>

<div style:height="100vh" style:width="100vw" class="relative">
    <QueryClientProvider client={queryClient}>
        <ModeWatcher />
        <Icons/>
        <Workflows/>
    </QueryClientProvider>
</div>
