/**
 * Custom error class for CKEditor5 Blazor-related errors.
 */
export class CKEditor5BlazorError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'CKEditor5BlazorError';
  }
}
