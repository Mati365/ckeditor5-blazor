import { markElementAsInteractive } from '../shared';

/**
 * Creates a simple interop layer for CKEditor 5 context to set the interactive attribute.
 *
 * @param element - The root HTML element of the context component, used to identify
 * the context instance and attach necessary attributes.
 */
export function createContextBlazorInterop(element: HTMLElement) {
  markElementAsInteractive(element);

  return {
    unmount() {},
  };
}
