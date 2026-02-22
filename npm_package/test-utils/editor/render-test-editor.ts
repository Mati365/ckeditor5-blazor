import type { EditorSnapshot } from './create-editor-snapshot';

import { vi } from 'vitest';

import { html } from '../html';
import { createEditorSnapshot } from './create-editor-snapshot';

/**
 * Renders the editor component in the DOM.
 */
export function renderTestEditor(
  snapshot: Partial<EditorSnapshot> = {},
  {
    withInput,
    interactive = true,
    container = document.body,
  }: Options = {},
): HTMLElement {
  const fullSnapshot: EditorSnapshot = {
    ...createEditorSnapshot(),
    ...snapshot,
  };

  const component = html.tag('cke5-editor', {
    'data-cke-editor-id': fullSnapshot.editorId,
    'data-cke-preset': JSON.stringify(fullSnapshot.preset),
    'data-cke-context-id': fullSnapshot.contextId,
    'data-cke-editable-height': fullSnapshot.editableHeight,
    'data-cke-save-debounce-ms': fullSnapshot.saveDebounceMs,
    'data-cke-language': JSON.stringify(fullSnapshot.language),
    'data-cke-content': JSON.stringify(fullSnapshot.content),
    ...fullSnapshot.watchdog && {
      'data-cke-watchdog': '',
    },
    ...interactive && {
      'data-cke-interactive': 'true',
    },
  });

  const elements = [component];

  if (!['multiroot', 'decoupled'].includes(fullSnapshot.preset.editorType)) {
    component.appendChild(
      html.div({
        id: `${fullSnapshot.editorId}_editor`,
      }),
    );
  }

  if (withInput) {
    const value = (
      typeof fullSnapshot.content === 'object'
        ? (fullSnapshot.content['main'] || '')
        : (fullSnapshot.content || '')
    );

    elements.push(
      html.input({
        type: 'hidden',
        id: `${fullSnapshot.editorId}_input`,
        value,
      }),
    );
  }

  const wrapper = html.div({}, ...elements);

  container.appendChild(wrapper);

  if (vi.isFakeTimers()) {
    vi.advanceTimersToNextTimer();
  }

  return component;
}

type Options = {
  withInput?: boolean;
  interactive?: boolean;
  container?: HTMLElement;
};
