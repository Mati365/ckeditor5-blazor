import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { waitForInteractiveAttribute } from './wait-for-interactive-attribute';

describe('waitForInteractiveAttribute', () => {
  let element: HTMLElement;

  beforeEach(() => {
    vi.useFakeTimers();
    element = document.createElement('div');
    document.body.appendChild(element);
  });

  afterEach(() => {
    document.body.removeChild(element);
    vi.restoreAllMocks();
    vi.useRealTimers();
  });

  it('should resolve immediately if the element already has the data-cke-interactive attribute', async () => {
    element.setAttribute('data-cke-interactive', '');

    const result = waitForInteractiveAttribute(element);

    await expect(result.promise).resolves.toBeUndefined();
  });

  it('should wait for the data-cke-interactive attribute to be added', async () => {
    const result = waitForInteractiveAttribute(element);
    const successSpy = vi.fn();

    // Attach a spy to the promise to track its resolution status
    void result.promise.then(successSpy);

    // 1. Ensure the promise is pending initially (flush microtasks)
    await vi.advanceTimersByTimeAsync(0);
    expect(successSpy).not.toHaveBeenCalled();

    // 2. Add the attribute
    element.setAttribute('data-cke-interactive', '');

    // 3. Ensure the promise resolves after the mutation
    await expect(result.promise).resolves.toBeUndefined();
    expect(successSpy).toHaveBeenCalled();
  });

  it('should NOT resolve if a different attribute is added', async () => {
    const result = waitForInteractiveAttribute(element);
    const successSpy = vi.fn();

    void result.promise.then(successSpy);

    // Add an irrelevant attribute
    element.setAttribute('data-other', '');

    // Advance time and flush tasks to give the observer a chance to react (incorrectly)
    await vi.advanceTimersByTimeAsync(100);

    // The promise should still be pending
    expect(successSpy).not.toHaveBeenCalled();
  });

  it('should disconnect the observer automatically when the attribute is added', async () => {
    // Spy on the native MutationObserver disconnect method
    const disconnectSpy = vi.spyOn(MutationObserver.prototype, 'disconnect');

    const result = waitForInteractiveAttribute(element);
    element.setAttribute('data-cke-interactive', '');

    await result.promise;

    expect(disconnectSpy).toHaveBeenCalled();
  });

  it('should allow disconnecting the observer manually and stop listening', async () => {
    const disconnectSpy = vi.spyOn(MutationObserver.prototype, 'disconnect');
    const result = waitForInteractiveAttribute(element);
    const successSpy = vi.fn();

    void result.promise.then(successSpy);

    // 1. Manually disconnect
    result.disconnect();
    expect(disconnectSpy).toHaveBeenCalled();

    // 2. Add the attribute AFTER disconnecting
    element.setAttribute('data-cke-interactive', '');

    // 3. Verify the promise remains pending (observer should be dead)
    await vi.advanceTimersByTimeAsync(100);
    expect(successSpy).not.toHaveBeenCalled();
  });
});
