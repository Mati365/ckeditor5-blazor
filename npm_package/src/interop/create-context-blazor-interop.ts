/**
 * Creates a simple interop layer for CKEditor 5 context to set the interactive attribute.
 * @param contextId - The unique identifier of the CKEditor context instance.
 */
export function createContextBlazorInterop(contextId: string): void {
  const element = document.querySelector(`cke5-context[data-cke-context-id="${contextId}"]`) as HTMLElement | null;

  /* v8 ignore next if -- @preserve */
  if (!element) {
    throw new Error(`Context element with ID "${contextId}" not found.`);
  }

  element.setAttribute('data-cke-interactive', '');
}
