/**
 * Waits for the 'data-cke-interactive' attribute to be added to the element.
 */
export function waitForInteractiveAttribute(element: HTMLElement): WaitForInteractiveResult {
  if (element.hasAttribute('data-cke-interactive')) {
    return { promise: Promise.resolve(), disconnect: () => {} };
  }

  let observer: MutationObserver;

  const promise = new Promise<void>((resolve) => {
    observer = new MutationObserver((mutations) => {
      for (const mutation of mutations) {
        if (mutation.type === 'attributes' && mutation.attributeName === 'data-cke-interactive' && element.hasAttribute('data-cke-interactive')) {
          observer.disconnect();
          resolve();
          break;
        }
      }
    });

    observer.observe(element, { attributes: true });
  });

  return {
    promise,
    disconnect: () => observer?.disconnect(),
  };
}

/**
 * Result of waiting for the interactive attribute.
 */
export type WaitForInteractiveResult = {
  promise: Promise<void>;
  disconnect: () => void;
};
