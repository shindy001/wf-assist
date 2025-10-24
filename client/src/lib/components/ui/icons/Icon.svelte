<!--@component
use an svg icon that was provided through 'Icons.svelte' - see the component for available icons
-->

<script lang="ts">
  import type { ClassValue } from "svelte/elements";

  const defaultSize = 24;

  let props: {
    name: string;
    size?: number;
    class?: ClassValue;
  } = $props();
</script>

<svg
  class={["icon", props.class]}
  width={props.size ?? defaultSize}
  height={props.size ?? defaultSize}
>
  <use href="#{props.name}" />
</svg>

<style>
  .icon {
    position: relative;
    overflow: hidden;
    vertical-align: middle;
    -o-object-fit: contain;
    object-fit: contain;
    -webkit-transform-origin: center center;
    transform-origin: center center;
    stroke-linecap: round;
    stroke-linejoin: round;
    fill: none;
  }
</style>
