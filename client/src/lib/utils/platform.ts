export const isCompatiblePlatform =
    () => typeof window !== 'undefined' &&
        typeof localStorage !== 'undefined' &&
        typeof indexedDB !== 'undefined';
