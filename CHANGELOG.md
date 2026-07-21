# Changelog

All notable changes to this project will be documented in this file.

## [4.1.0]

### Added

- **Advanced filtering** — filter the content approval task list by status (In Review / Ready to Publish), content type (Page / Block / Asset/Media), site (multi-site support), and by content ID or name.
- **Active filter badges** — when filters are applied, badge pills appear below the filter bar showing each active filter with a one-click clear action.
- **Select all across pages** — after selecting all items on the current page a banner appears offering to select all items matching the current filters across all pages, not just the visible page.
- **Scheduled publishing** — when "Publish selected content after approval" is enabled, editors can optionally schedule a future date and time for the publish to take effect.
- **Approve blocks & media on a page** — an *Approve Page Dependencies* button in the filter bar opens a modal with a lazy-loading page hierarchy tree rooted at each configured site. Editors navigate the tree (or enter a content ID directly) to select a page, choose one or more languages, and approve all pending block and media dependencies in one action. An optional *Publish approved content after approval* checkbox publishes the items immediately after approval.
- **Site column** — when multiple sites are configured the task table gains a Site column showing which site each content item belongs to.

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