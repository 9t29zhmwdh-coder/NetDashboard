# Changelog

All notable changes to NetDashboard will be documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

## [0.1.9] - 2026-07-11

### Fixed

- Removed an eszett and em-dashes/en-dashes across the repo (TEMPLATE_NOTES.md, MainWindow.xaml.cs, GETTING_STARTED.md/.de.md, SKELETON.md, Services/NetworkService.cs, CONTRIBUTING.md), including the DE/EN/FR in-app help text. Swiss German orthography.

## [0.1.8] - 2026-07-11

### Added

- Documented Dual-Licensing assessment (Community-only) in ROADMAP.md.

## [0.1.7] - 2026-07-11

### Fixed

- Replaced the unmonitored security@raystudio.ch email in SECURITY.md with a GitHub Security Advisory link, matching the rest of the portfolio.

## [0.1.6] - 2026-07-11

### Fixed

- Updated actions/checkout and actions/setup-dotnet to their latest major versions in CI, since GitHub is deprecating the Node.js 20 runtime and older action versions were being forced onto Node 24 and crashing during post-run cleanup.

## [0.1.5] - 2026-07-11

### Fixed

- Corrected README hero section: only the title image and title stay centered, tagline and badges are now left aligned like the rest of the document

## [0.1.4] - 2026-07-10

### Fixed

- Removed em-dashes from CHANGELOG.md date headers, replaced with plain hyphens

## [0.1.3] - 2026-07-10

### Fixed

- Added missing `<Version>` element to NetDashboard.csproj so the assembly version tracks the release tag (was previously untracked in code)

## [0.1.2] - 2026-07-10

### Fixed

- Removed a duplicate "New here? -> beginners guide" callout from README.md and README.de.md (was shown twice in both)

## [0.1.0] - 2026-06-15
### Added
- Initial import: WPF dark-theme UI (.NET 8)
- Network diagnostics module
- Mail toolkit (SMTP/IMAP)
- Multi-language support (DE/EN/FR)
