import type { EditorLanguage } from '../typings';

/**
 * Normalize editor language into object.
 *
 * @param lang Editor language.
 * @returns Language object.
 */
export function normalizeEditorLanguage(lang: EditorLanguage | string): EditorLanguage {
  if (!lang) {
    return {
      ui: 'en',
      content: 'en',
    };
  }

  if (typeof lang === 'string') {
    return {
      ui: lang,
      content: lang,
    };
  }

  return lang;
}
