/* eslint-disable no-console */
import { Plugin } from 'ckeditor5';

export default class MyCustomPlugin extends Plugin {
  static get pluginName() {
    return 'MyCustomPlugin';
  }

  init() {
    console.info('MyCustomPlugin was initialized');
  }
}
