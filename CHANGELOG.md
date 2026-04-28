# Changelog

All notable changes to this project will be documented in this file.

## [4.0.0]

### Changed

- Upgraded target framework from .NET 6 to .NET 10
- Upgraded all Optimizely/EPiServer packages from CMS 12 to CMS 13 (13.0.0)
- Upgraded `X.PagedList` from 8.1.0 to 10.5.9
- Added explicit `Newtonsoft.Json` 13.0.3 package reference (no longer a transitive dependency in CMS 13)
- Replaced removed `ModuleResourceResolver.Instance` (internal API) with `IModuleResourceResolver` resolved via `ServiceLocator`
- Replaced removed `IPropertyDefinitionRepository.Save/Delete` (obsoleted as errors in CMS 13) with writable content type clone pattern via `IContentTypeRepository.Save`
- Refactored `AdvancedTaskInitialization` to use `ServiceLocator.Current.GetInstance<T>()` inside `Initialize()` (CMS 13 instantiates `IInitializableModule` before the DI container is built)
- Removed `[ServiceConfiguration]` attribute from `ChangeApprovalDynamicDataStoreFactory`; registered explicitly via `services.AddSingleton<>()`
- Replaced `@Html.CreatePlatformNavigationMenu()` / `@Html.ApplyPlatformNavigation()` with `<platform-navigation />` tag helper in shell layout
- Fixed shell layout content area width by applying `epi-pn-navigation--fixed-adjust` CSS class (CMS 13 sets `body { display: flex }`)
- Updated MSBuild targets path from `build\net6.0\` to `build\net10.0\`

### Known Limitations

- **Change Approval tab is unavailable in CMS 13.** The `EPiServer.ChangeApproval` package declares a hard dependency on `EPiServer.CMS.UI < 13.0.0` and no CMS 13-compatible version has been published. The tab will be restored automatically once Optimizely ships a compatible release.

## [3.0.0]

### Changed

- Migrated package to .NET 5.0

## [2.3.0]

### Changed

* Implemented .NET Framework 4.7.1 update, incorporating code improvements and resolving bugs.

## [2.2.0]

### Changed

* New tab and better UI for Change Approval tasks.

## [2.0.0]

### Changed

* Change Approval tasks will show along with the Content Approval tasks.
* Support for all content type tasks in CMS. Now editors can view, approve and publish, Episerver Forms, ImageData & MediaData. 
* Bug fixes for pagination and performance improvements.