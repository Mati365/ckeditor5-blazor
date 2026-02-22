import type { Snapshot } from './create-context-snapshot';

import { html } from '../html';
import { createContextSnapshot } from './create-context-snapshot';

/**
 * Renders the context component in the DOM.
 */
export function renderTestContext(
  snapshot: Partial<Snapshot> = {},
  {
    container = document.body,
    interactive = true,
  }: Options = {},
): HTMLElement {
  const fullSnapshot: Snapshot = {
    ...createContextSnapshot(),
    ...snapshot,
  };

  const component = html.tag('cke5-context', {
    'data-cke-context-id': fullSnapshot.contextId,
    'data-cke-context': JSON.stringify(fullSnapshot.context),
    'data-cke-language': JSON.stringify(fullSnapshot.language),
    ...interactive && {
      'data-cke-interactive': 'true',
    },
  });

  container.appendChild(component);

  return component;
}

type Options = {
  container?: HTMLElement;
  interactive?: boolean;
};
