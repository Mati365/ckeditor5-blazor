import { afterEach, beforeEach, describe, expect, it } from 'vitest';

import { createUIPartBlazorInterop } from './create-ui-part-blazor-interop';

describe('createUIPartBlazorInterop', () => {
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

    createUIPartBlazorInterop(element);

    expect(element.getAttribute('data-cke-interactive')).toBe('true');
  });

  it('unmount should not throw', () => {
    const interop = createUIPartBlazorInterop(element);

    expect(() => interop.unmount()).not.toThrow();
  });
});
