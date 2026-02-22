import { markElementAsInteractive } from '../shared';

/**
 * Creates a simple interop layer for CKEditor 5 UI part to set the interactive attribute.
 *
 * @param element - The root HTML element of the UI part component, used to identify
 * the UI part instance and attach necessary attributes.
 */
export function createUIPartBlazorInterop(element: HTMLElement) {
  markElementAsInteractive(element);

  return {
    unmount() {},
  };
}
