import { isEmptyObject } from '../../src/shared';
import { DEFAULT_TEST_EDITOR_ID } from '../editor';
import { html } from '../html';

/**
 * Renders the editable component in the DOM.
 */
export function renderTestEditable(
  {
    withInput,
    interactive = true,
    rootName = 'main',
    content = null,
    editorId = DEFAULT_TEST_EDITOR_ID,
    rootAttributes = {},
    rootModelElementName: rootModelElement,
    saveDebounceMs,
  }: Options = {},
): HTMLElement {
  const element = html.tag(
    'cke5-editable',
    {
      ...editorId && {
        'data-cke-editor-id': editorId,
      },
      'data-cke-root-name': rootName,
      'data-cke-content': content,
      'data-cke-save-debounce-ms': saveDebounceMs,
      ...rootModelElement && {
        'data-cke-root-model-element-name': rootModelElement,
      },
      ...!isEmptyObject(rootAttributes) && {
        'data-cke-root-attributes': JSON.stringify(rootAttributes),
      },
      ...interactive && {
        'data-cke-interactive': 'true',
      },
    },
    ...[
      html.div({ 'data-cke-editable-content': '' }),
      ...withInput
        ? [html.input({ type: 'text' })]
        : [],
    ],
  );

  const wrapper = html.div({}, element);

  // The structure expected for decoupled editor might just be the element itself.
  // But append to body.
  document.body.appendChild(wrapper);

  return element;
}

type Options = {
  /**
   * Render HTML input.
   */
  withInput?: boolean;

  /**
   * Rendering in interactive mode.
   */
  interactive?: boolean;

  /**
   * The ID of the editor instance this editable belongs to.
   */
  editorId?: string;

  /**
   * The name of the root element in the editor.
   */
  rootName?: string;

  /**
   * The attributes that should be applied to the root element.
   */
  rootAttributes?: Record<string, string>;

  /**
   * The name of the root model element.
   */
  rootModelElementName?: string | null;

  /**
   * The initial content value for the editable.
   */
  content?: string | null;

  /**
   * The debounce time in milliseconds for saving changes.
   */
  saveDebounceMs?: number;
};
