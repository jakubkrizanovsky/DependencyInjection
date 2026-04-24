## [1.0.0] - 2026-04-17
### First Release
- Dependency Injection Framework

## [1.1.0] - 2026-04-23
### Unique Services
- Added support for making services unique

## [1.1.1] - 2026-04-23
### Fix
- Fix discovery when returning to global context scene

## [1.2.0] - 2026-04-24
### Improvements and Refactor
- Persistent services are now always registered at the global context
- ServiceProxy can now make services persistent
- Some minor API changes, manual service registration now done directly with DependencyInjector

## [1.2.1] - 2026-04-24
### Fixes
- Fix nullpointer when resolving global context during load
- Fix persistent services duplication
- Fix null coalescing usage in LazyInject
