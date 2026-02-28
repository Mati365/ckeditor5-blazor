import { afterEach, beforeEach, describe, expect, it } from 'vitest';

import { createContextBlazorInterop } from './create-context-blazor-interop';

describe('createContextBlazorInterop', () => {
  let element: HTMLElement;

  beforeEach(() => {
    element = document.createElement('ckeditor5-editable');
    document.body.appendChild(element);
  });

  afterEach(() => {
    element.remove();
  });

  it('should mark the element as interactive after initialization', () => {
    expect(element.hasAttribute('data-cke-interactive')).toBe(false);

    createContextBlazorInterop(element);

    expect(element.getAttribute('data-cke-interactive')).toBe('true');
  });

  it('unmount should not throw', () => {
    const interop = createContextBlazorInterop(element);

    expect(() => interop.unmount()).not.toThrow();
  });
});
