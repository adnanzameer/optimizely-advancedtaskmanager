# Changelog

All notable changes to this project will be documented in this file.

## [3.1.0]

### Added

- **Advanced filtering** — filter the content approval task list by status (In Review / Ready to Publish), content type (Page / Block / Asset/Media), site (multi-site support), and by content ID or name.
- **Active filter badges** — when filters are applied, badge pills appear below the filter bar showing each active filter with a one-click clear action.
- **Select all across pages** — after selecting all items on the current page a banner appears offering to select all items matching the current filters across all pages, not just the visible page.
- **Scheduled publishing** — when "Publish selected content after approval" is enabled, editors can optionally schedule a future date and time for the publish to take effect.
- **Approve blocks & media on a page** — an *Approve Page Dependencies* button in the filter bar opens a modal with a lazy-loading page hierarchy tree. Editors navigate the tree to select a page, choose one or more languages, and approve all pending block and media dependencies in one action. An optional *Publish approved content after approval* checkbox publishes the items immediately after approval.
- **Site column** — when multiple sites are configured the task table gains a Site column showing which site each content item belongs to.

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