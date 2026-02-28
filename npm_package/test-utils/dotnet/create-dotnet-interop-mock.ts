import type { DotNetInterop } from '../../src/types';

import { vi } from 'vitest';

export function createDotNetInteropMock(): DotNetInterop {
  return {
    invokeMethodAsync: vi.fn(() => Promise.resolve(undefined)),
  };
}
