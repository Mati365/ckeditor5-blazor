import type { EditorType } from '../typings';

import { CKEditor5BlazorError } from '../../../ckeditor5-blazor-error';

/**
 * Returns the constructor for the specified CKEditor5 editor type.
 *
 * @param type - The type of the editor to load.
 * @returns A promise that resolves to the editor constructor.
 */
export async function loadEditorConstructor(type: EditorType) {
  const PKG = await import('ckeditor5');

  const editorMap = {
    inline: PKG.InlineEditor,
    balloon: PKG.BalloonEditor,
    classic: PKG.ClassicEditor,
    decoupled: PKG.DecoupledEditor,
    multiroot: PKG.MultiRootEditor,
  } as const;

  const EditorConstructor = editorMap[type];

  if (!EditorConstructor) {
    throw new CKEditor5BlazorError(`Unsupported editor type: ${type}`);
  }

  return EditorConstructor;
}
