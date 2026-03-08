/* eslint-disable no-console */
import { ContextPlugin } from 'ckeditor5';

export default class MyCustomContextPlugin extends ContextPlugin {
  static get pluginName() {
    return 'MyCustomContextPlugin';
  }

  init() {
    console.info('MyCustomContextPlugin was initialized');

    this.context.editors.on('add', (evt, editor) => {
      console.info('[MyCustomContextPlugin] Editor added:', editor);
    });

    this.context.editors.on('remove', (evt, editor) => {
      console.info('[MyCustomContextPlugin] Editor removed:', editor);
    });
  }
}
