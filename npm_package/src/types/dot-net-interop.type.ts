/**
 * Represents the .NET interop helper for communication with Blazor.
 */
export type DotNetInterop = {
  invokeMethodAsync: <T = void>(methodName: string, ...args: any[]) => Promise<T>;
};
