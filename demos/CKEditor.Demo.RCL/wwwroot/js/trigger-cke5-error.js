import { EditorsRegistry } from 'ckeditor5-blazor';

export function triggerCKE5Error() {
  setTimeout(() => {
    const err = new Error('foo');

    err.context = EditorsRegistry.the.getItem(null);
    err.is = () => true;

    throw err;
  });
}
