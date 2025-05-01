import App from './App.svelte';

import './app.css';
import {mount} from "svelte";
import {isCompatiblePlatform} from "./shared/platformUtils";

if (!isCompatiblePlatform()) {
  throw new Error("Unsupported platform. This application requires localStorage and indexDB to run hence cannot run outside of a browser.")
}

const app = mount(App, {
  target: document.getElementById('app')!,
});

export default app;
