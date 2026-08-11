<!--@component
Provides a turbo style svg gradient by targeting html Id "turbo-edge-gradient" - needs to be used at root component like App.svelte before the gradient is used.
-->

<svg>
  <defs>
    <linearGradient id="turbo-edge-gradient">
      <stop offset="0%" stop-color="#2a8af6" />
      <stop offset="100%" stop-color="#e84fb5" />
    </linearGradient>
  </defs>
</svg>
