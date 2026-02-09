#!/usr/bin/env node
import { cpSync, existsSync, mkdirSync, rmSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import process from 'node:process';
import { fileURLToPath } from 'node:url';

import { watch } from 'chokidar';

const __dirname = dirname(fileURLToPath(import.meta.url));
const projectRoot = resolve(__dirname, '..');
const distDir = resolve(projectRoot, 'npm_package/dist');
const wwwrootDir = resolve(projectRoot, 'src/CKEditor.Blazor/wwwroot/ckeditor5-blazor');

console.log('🔍 Watching for changes in:', distDir);
console.log('📦 Copying to:', wwwrootDir);

function copyDist() {
  try {
    if (existsSync(wwwrootDir)) {
      rmSync(wwwrootDir, { recursive: true, force: true });
    }
    mkdirSync(wwwrootDir, { recursive: true });
    cpSync(distDir, wwwrootDir, { recursive: true });

    console.log('✅ Copied dist/ to wwwroot/ckeditor5-blazor');
  }
  catch (error) {
    console.error('❌ Error copying files:', error.message);
  }
}

const debouncedCopyDist = debounce(copyDist, 300);

copyDist();

const watcher = watch(distDir, {
  persistent: true,
  ignoreInitial: true,
  awaitWriteFinish: {
    stabilityThreshold: 100,
    pollInterval: 50,
  },
});

watcher
  .on('add', debouncedCopyDist)
  .on('change', debouncedCopyDist)
  .on('unlink', debouncedCopyDist)
  .on('error', (error) => {
    console.error('❌ Watcher error:', error);
  });

console.log('👀 Watcher started. Press Ctrl+C to stop.');

// Handle graceful shutdown
process.on('SIGINT', () => {
  console.log('\n👋 Stopping watcher...');
  watcher.close();
  process.exit(0);
});

function debounce(func, wait) {
  let timeout;

  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };

    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}
