export {};

declare global {
  const DotNet: {
    createJSObjectReference: <T extends object>(obj: T) => unknown;
    disposeJSObjectReference: (ref: unknown) => void;
  };
}
