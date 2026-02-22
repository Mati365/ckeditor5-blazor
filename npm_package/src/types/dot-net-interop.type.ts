/**
 * Represents the .NET interop helper for communication with Blazor.
 */
export type DotNetInterop = {
  invokeMethodAsync: (methodName: string, ...args: any[]) => Promise<void>;
};
