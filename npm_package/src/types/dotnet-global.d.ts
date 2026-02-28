export {};

declare global {
  // eslint-disable-next-line vars-on-top
  var DotNet: {
    createJSObjectReference: <T extends object>(obj: T) => object;
    disposeJSObjectReference: (ref: object) => void;
  };

  // eslint-disable-next-line ts/consistent-type-definitions
  interface GlobalThis {
    DotNet: typeof DotNet;
  }
}
