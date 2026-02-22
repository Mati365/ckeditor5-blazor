import type { UISnapshot } from './create-ui-part-snapshot';

import { html } from '../html';
import { createUIPartSnapshot } from './create-ui-part-snapshot';

/**
 * Renders the UI part component in the DOM.
 */
export function renderTestUIPart(
  snapshot: Partial<UISnapshot> = {},
  {
    interactive = true,
  }: Options = {},
): HTMLElement {
  const fullSnapshot = {
    ...createUIPartSnapshot(snapshot.name),
    ...snapshot,
  };

  const el = html.tag('cke5-ui-part', {
    'data-cke-name': fullSnapshot.name,
    'data-cke-editor-id': fullSnapshot.editorId,
    ...interactive && {
      'data-cke-interactive': 'true',
    },
  });

  document.body.appendChild(el);
  return el;
}

type Options = {
  interactive?: boolean;
};
