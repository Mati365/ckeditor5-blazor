#!/usr/bin/env node
import { cpSync, existsSync, mkdirSync, rmSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { exit } from 'node:process';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const projectRoot = resolve(__dirname, '..');
const distDir = resolve(projectRoot, 'npm_package/dist');
const wwwrootDir = resolve(projectRoot, 'src/CKEditor.Blazor/wwwroot/ckeditor5-blazor');

console.log('📦 Copying dist/ to wwwroot/ckeditor5-blazor...');
console.log('  Source:', distDir);
console.log('  Destination:', wwwrootDir);

if (!existsSync(distDir)) {
  console.error('❌ dist/ directory does not exist. Run the npm package build first.');
  exit(1);
}

if (existsSync(wwwrootDir)) {
  rmSync(wwwrootDir, { recursive: true, force: true });
}

mkdirSync(wwwrootDir, { recursive: true });
cpSync(distDir, wwwrootDir, { recursive: true });

console.log('✅ Copied dist/ to wwwroot/ckeditor5-blazor successfully.');
