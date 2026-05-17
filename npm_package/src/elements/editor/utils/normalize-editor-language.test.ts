import { describe, expect, it } from 'vitest';

import { normalizeEditorLanguage } from './normalize-editor-language';

describe('normalizeEditorLanguage', () => {
  describe('when lang is falsy', () => {
    it('returns default object with "en" for empty string', () => {
      expect(normalizeEditorLanguage('')).toEqual({ ui: 'en', content: 'en' });
    });

    it('returns default object with "en" for null', () => {
      expect(normalizeEditorLanguage(null as any)).toEqual({ ui: 'en', content: 'en' });
    });

    it('returns default object with "en" for undefined', () => {
      expect(normalizeEditorLanguage(undefined as any)).toEqual({ ui: 'en', content: 'en' });
    });
  });

  describe('when lang is a string', () => {
    it('returns object with ui and content set to the given language', () => {
      expect(normalizeEditorLanguage('pl')).toEqual({ ui: 'pl', content: 'pl' });
    });

    it('handles "en" language code correctly', () => {
      expect(normalizeEditorLanguage('en')).toEqual({ ui: 'en', content: 'en' });
    });

    it('handles "de" language code correctly', () => {
      expect(normalizeEditorLanguage('de')).toEqual({ ui: 'de', content: 'de' });
    });
  });

  describe('when lang is an object', () => {
    it('returns the object unchanged when ui and content are the same', () => {
      const lang = { ui: 'pl', content: 'pl' };
      expect(normalizeEditorLanguage(lang)).toEqual(lang);
    });

    it('returns the object unchanged when ui and content differ', () => {
      const lang = { ui: 'pl', content: 'en' };
      expect(normalizeEditorLanguage(lang)).toEqual(lang);
    });

    it('returns the exact same object reference', () => {
      const lang = { ui: 'fr', content: 'en' };
      expect(normalizeEditorLanguage(lang)).toBe(lang);
    });
  });
});
