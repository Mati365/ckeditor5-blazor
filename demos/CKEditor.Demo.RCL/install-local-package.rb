#!/usr/bin/env ruby
# frozen_string_literal: true

require 'fileutils'
require 'optparse'

options = { premium: false }

OptionParser.new do |parser|
  parser.on('--premium', 'Install premium assets') { options[:premium] = true }
end.parse!

script_dir = File.expand_path(__dir__)
repo_root = File.expand_path('../..', script_dir)
local_feed = File.join(repo_root, '.tmp', 'local-nuget-feed')
project_file = File.join(script_dir, 'CKEditor.Demo.RCL.csproj')
wwwroot_ckeditor = File.join(script_dir, 'wwwroot', 'ckeditor5')

# Clean wwwroot/ckeditor5 to force asset reinstallation
FileUtils.rm_rf(wwwroot_ckeditor) if Dir.exist?(wwwroot_ckeditor)
puts "🧹 Cleaned wwwroot/ckeditor5"

# Generate unique version
version = "1.0.0-local.#{Time.now.to_i}"
FileUtils.mkdir_p(local_feed)

puts "📦 Packing CKEditor.Blazor (#{version})..."
abort 'Pack failed' unless system('dotnet', 'pack', File.join(repo_root, 'src', 'CKEditor.Blazor', 'CKEditor.Blazor.csproj'),
                                   '-c', 'Debug', '-o', local_feed, "-p:PackageVersion=#{version}")

abort "ERROR: Package not found" unless File.exist?(File.join(local_feed, "CKEditor.Blazor.#{version}.nupkg"))

puts "🔨 Building RCL with packaged version #{version}..."
abort 'Build failed' unless system('dotnet', 'build', project_file, '--no-cache',
                                    '--source', local_feed, '--source', 'https://api.nuget.org/v3/index.json',
                                    "-p:CKEditorBlazorPackageVersion=#{version}",
                                    '-p:CKEditorInstallAssets=true',
                                    "-p:CKEditorInstallPremiumAssets=#{options[:premium]}")

wwwroot = File.join(script_dir, 'wwwroot')
if Dir.exist?(wwwroot)
  count = Dir.glob(File.join(wwwroot, '**', '*')).count { |f| File.file?(f) }
  puts "✅ Done! Copied #{count} files to wwwroot"
else
  puts "⚠️  No assets copied"
end
