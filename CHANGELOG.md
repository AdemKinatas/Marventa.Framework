# Changelog

All notable changes to Marventa Framework will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.1.0] - 2024-12-20

### Added
- **BaseDbContext**: Enterprise-grade DbContext with automatic features
  - Automatic audit tracking (CreatedDate, UpdatedDate)
  - Soft delete with global query filters (IsDeleted)
  - Multi-tenancy support with automatic tenant isolation
  - Domain event dispatching before save
  - Configurable override for custom domain event publishing
- **Interface Organization**: Reorganized 88 types into 17 domain-specific namespaces
  - Data/ - Repository, UnitOfWork, DatabaseSeeder
  - Messaging/ & Messaging/Outbox/ - Message bus, handlers, transactional messaging
  - Sagas/ - Saga orchestration (9 files)
  - Projections/ - CQRS read models (6 files)
  - Storage/ - Storage, CDN, file processing
  - Services/ - Email, SMS, Search, Logger (10 files)
  - MultiTenancy/ - Tenant management (9 files)
  - Caching/ - Cache services
  - Security/ - JWT, encryption, tokens (5 files)
  - Events/ - Domain and integration events
  - HealthCheck/ - Health monitoring
  - Idempotency/ - Idempotent operations
  - DistributedSystems/ - Distributed locks, correlation
  - Analytics/, Http/, MachineLearning/, BackgroundJobs/, Configuration/, Validation/

### Changed
- **Documentation**: Comprehensive README updates
  - Added BaseDbContext usage examples with full setup guide
  - Updated architecture diagram to show all layers
  - Added interface organization tree (88 types, 17 namespaces)
  - Enhanced extension methods documentation with all new features
  - Added Quick Start guide with BaseDbContext integration
- **Namespace Updates**: 75+ files updated to use new organized namespaces
  - `Marventa.Framework.Core.Interfaces.Data`
  - `Marventa.Framework.Core.Interfaces.Messaging`
  - `Marventa.Framework.Core.Interfaces.Sagas`
  - `Marventa.Framework.Core.Interfaces.MultiTenancy`
  - And 13 more domain-specific namespaces

### Improved
- **Code Organization**: Better discoverability with domain-based namespace structure
- **Clean Architecture**: Enhanced implementation following SOLID principles
- **Developer Experience**: Clearer interface grouping and comprehensive examples

### Technical Details
- Split 12 multi-interface files into 80+ focused files
- Organized 71 interfaces, 13 classes, and 4 enums
- Zero breaking changes - fully backward compatible
- Build: 0 errors, 0 warnings

## [3.0.1] - 2024-12-19

### Fixed
- **NuGet Package**: Fixed dependency resolution issue
  - All sub-assemblies (Core, Domain, Application, Infrastructure, Web) now properly embedded
  - No more "Could not load file or assembly" errors
  - Clean installation experience

### Technical Details
- Added PrivateAssets="all" to all project references
- Configured TfmSpecificPackageFile for both net8.0 and net9.0
- All DLLs now included in lib/net8.0 and lib/net9.0 folders

## [3.0.0] - 2024-12-18

### Breaking Changes
- Major version release with significant architectural improvements
- Interface reorganization following Clean Architecture principles
- Namespace changes for better code organization
- Removed deprecated APIs and legacy code

### Added
- Production-ready CDN services (Azure, AWS, CloudFlare)
- Saga pattern for distributed transactions
- Enhanced repository pattern with caching
- Unified middleware pipeline
- Full multi-tenancy support
- 47+ modular features

### Changed
- Complete Clean Architecture implementation
- SOLID principles throughout
- Comprehensive documentation
- Real-world usage examples

### Migration Guide
Users upgrading from v2.x need to:
- Update interface references
- Update namespace imports
- Review removed APIs
- Test thoroughly before production deployment

## [2.8.0] - 2024-11-15

### Added
- Core utilities and base classes
- Initial framework structure

### Changed
- Enhanced core functionality
- Improved performance

---

For complete documentation and migration guides, see [README.md](README.md).