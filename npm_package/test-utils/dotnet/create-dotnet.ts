import { vi } from 'vitest';

export function createDotnet() {
  const registeredObjects = new Map<object, object>();

  return {
    createJSObjectReference: vi.fn((obj: object) => {
      const ref = { $ref: obj };

      registeredObjects.set(obj, ref);
      return ref;
    }),
    disposeJSObjectReference: vi.fn((obj: object) => {
      registeredObjects.delete(obj);
    }),
  };
}
