# Breaking Changes

This document lists all breaking changes in Marventa Framework versions.

## Version 3.2.0

**Release Date:** 2024-09-30

### ✅ No Breaking Changes

Version 3.2.0 is fully backward compatible with v3.1.x. All changes are additive.

**New Features:**
- Added LoggingBehavior for MediatR pipeline
- Added TransactionBehavior for automatic transaction management
- Enhanced CQRS configuration options

**Migration:** No code changes required. Simply upgrade the package version.

---

## Version 3.1.0

**Release Date:** 2024-12-20

### ⚠️ Minor Breaking Changes

#### 1. Namespace Reorganization

**Impact:** Medium - All interface imports need updating

**Before:**
```csharp
using Marventa.Framework.Interfaces;
```

**After:**
```csharp
using Marventa.Framework.Core.Interfaces.Data;
using Marventa.Framework.Core.Interfaces.Messaging;
```

**Affected Types:** 88 types across 17 namespaces

**Fix:** Use IDE's "Find and Replace" or "Fix all" feature

#### 2. BaseDbContext Constructor

**Impact:** High - All DbContext implementations affected

**Before:**
```csharp
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
{
}
```

**After:**
```csharp
public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ITenantContext tenantContext)
    : base(options, tenantContext)
{
}
```

**Fix:** Add `ITenantContext` parameter and register in DI:
```csharp
services.AddScoped<ITenantContext, TenantContext>();
```

Or disable multi-tenancy:
```csharp
options.EnableMultiTenancy = false;
```

---

## Version 3.0.0

**Release Date:** 2024-12-18

### 🚨 Major Breaking Changes

#### 1. Complete Namespace Restructuring

**Impact:** High - Affects all projects

**Changed Namespaces:**
- `Marventa.Framework.Core` → Organized into sub-namespaces
- `Marventa.Framework.Infrastructure` → Organized by feature
- `Marventa.Framework.Application` → Added CQRS support

**Fix:** Global find and replace or use migration tool

#### 2. Repository Pattern Changes

**Impact:** Medium - Custom repositories need updates

**Before:**
```csharp
public class ProductRepository : IRepository<Product>
{
    // Full implementation required
}
```

**After:**
```csharp
public class ProductRepository : BaseRepository<Product>
{
    // Only custom methods needed
}
```

**Fix:** Inherit from BaseRepository and remove boilerplate code

#### 3. Configuration API Changes

**Impact:** Medium - Startup configuration needs updates

**Before:**
```csharp
services.AddMarventaServices(configuration);
```

**After:**
```csharp
builder.Services.AddMarventaFramework(configuration, options =>
{
    options.EnableLogging = true;
    options.EnableCaching = true;
    // ... configure features
});
```

**Fix:** Update Program.cs with new configuration API

#### 4. Removed Deprecated APIs

**Impact:** Low - Only affects users of deprecated features

**Removed:**
- `LegacyRepository` - Use `BaseRepository`
- `OldConfigurationMethod` - Use `AddMarventaFramework`
- `DeprecatedServiceExtensions` - Use new extensions

**Fix:** Update to recommended alternatives

---

## Version 2.8.0 → 3.0.0 Migration Checklist

Use this checklist when migrating from v2.8.0 to v3.0.0:

- [ ] Update NuGet package to v3.0.0
- [ ] Update all namespace imports
- [ ] Update DbContext to inherit from BaseDbContext
- [ ] Add ITenantContext to DbContext constructor
- [ ] Update repository implementations
- [ ] Update Program.cs configuration
- [ ] Remove deprecated API usage
- [ ] Run tests
- [ ] Review and test multi-tenancy features
- [ ] Update documentation

---

## How to Handle Breaking Changes

### 1. Read Release Notes

Always check [CHANGELOG.md](../../CHANGELOG.md) before upgrading.

### 2. Test in Development First

```bash
# Create a branch for upgrade
git checkout -b upgrade-to-v3

# Update package
dotnet add package Marventa.Framework --version 3.0.0

# Run tests
dotnet test
```

### 3. Use IDE Refactoring Tools

Most IDEs can help with:
- Namespace updates
- Method signature changes
- Deprecated API warnings

### 4. Incremental Migration

If possible, migrate incrementally:
- v2.8.0 → v3.0.1 (bug fixes)
- v3.0.1 → v3.1.0 (new features)
- v3.1.0 → v3.2.0 (enhancements)

### 5. Reach Out for Help

If stuck:
- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 🐛 [Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)

---

## Deprecation Policy

We follow semantic versioning:

**Major Version (X.0.0):**
- Breaking changes allowed
- 6-month deprecation notice when possible

**Minor Version (X.Y.0):**
- New features, no breaking changes
- Deprecated features marked with `[Obsolete]`

**Patch Version (X.Y.Z):**
- Bug fixes only
- No breaking changes

---

## Future Breaking Changes

### Planned for v4.0.0 (Q4 2026)

The following changes are planned for v4.0.0:

1. **Minimum .NET Version**: .NET 9.0 (dropping .NET 8.0 support)
2. **C# Version**: C# 13 features
3. **API Modernization**: Simplify configuration API
4. **Performance**: Breaking changes for 30% performance improvement

**Timeline:**
- Q1 2026: Preview release
- Q2 2026: Beta release
- Q3 2026: RC release
- Q4 2026: Final release

---

## Version Support Policy

| Version | Support Status | End of Support |
|---------|---------------|----------------|
| 3.2.x   | ✅ Active     | TBD            |
| 3.1.x   | ✅ Security   | Dec 2025       |
| 3.0.x   | ✅ Security   | Jun 2025       |
| 2.x     | ❌ Ended      | Dec 2024       |
| 1.x     | ❌ Ended      | Jun 2024       |

**Active Support:** New features, bug fixes, security patches
**Security Support:** Security patches only
**Ended:** No support

---

## Questions?

- Review [Migration Guide](v2-to-v3.md)
- Check [FAQ](../../README.md#-faq)
- Ask in [Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)