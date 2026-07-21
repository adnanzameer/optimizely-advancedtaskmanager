# Advanced Task Manager for Optimizely

The **Advanced Task Manager for Optimizely** is a freely available module. If you find it beneficial, you can show your support by treating me to a coffee on [Ko-fi](https://ko-fi.com/U7U2STV29):

## Description
[![Platform](https://img.shields.io/badge/Platform-.NET%206-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![Platform](https://img.shields.io/badge/Optimizely-%2012-blue.svg?style=flat)](http://world.episerver.com/cms/)
[![Platform](https://img.shields.io/badge/EPiServer-%2012-orange.svg?style=flat)](http://world.episerver.com/cms/)

The **Advanced Task Manager for Optimizely** provides CMS editors with enhanced oversight of administration tasks related to Awaiting Review in Content Approval and Change Approval processes.

## Features

- **Advanced filtering:** Filter tasks by status, content type, site, and by content ID or name.
- **Select all across pages:** Select every item matching the current filters, not just those visible on the current page.
- **Approve entire approval sequence:** Approve the entire approval sequence in bulk or against individual tasks.
- **Publish content after approval:** Publish selected content immediately after approval, with rights-aware handling when only some items can be published.
- **Scheduled publishing:** Optionally schedule a future date and time for approved content to go live instead of publishing immediately.
- **Approve blocks & media on a page:** Open a modal from the filter bar, navigate the page hierarchy tree to pick a page, and approve all pending blocks and media it references in one action — with an optional publish step.
- **Task ordering:** Sort the task list by name, content type, type, submission date, started-by user, or deadline.
- **Deadline field:** Optional date/time property on pages and blocks that highlights time-sensitive tasks in the list with warning and danger states.
- **User notifications:** Tasks with unread notifications are highlighted automatically and marked as read on open.
- **Change approval task information:** View detailed change approval task data (security, move, language, and expiry changes) inline in the task list.

## Installation 

To install the Advanced Task Manager, begin by adding the NuGet package using [Optimizely NuGet](https://nuget.optimizely.com/):

`
dotnet add package AdvancedTaskManager
`

## Configuration

Add the AdvancedTaskManager handler in the `Startup.cs` within the `ConfigureServices` method. Here's an example with all available configurations:

```C#
public void ConfigureServices(IServiceCollection services)
{
    
    services.AddAdvancedTaskManager(o =>
    {
        o.DeleteChangeApprovalTasks = true; //Default true
        o.PageSize = 10; //Default 30
        o.AddContentApprovalDeadlineProperty = true; //Default false
        o.DeleteContentApprovalDeadlineProperty = true; //Default false
        o.WarningDays = 4; //Default 4;
        o.DateTimeFormat = "yyyy-MM-dd HH:mm"; //Default "yyyy-MM-dd HH:mm";
        o.DateTimeFormatUserFriendly = "MMM dd, yyyy, h:mm:ss tt"; //Default "MMM dd, yyyy, h:mm:ss tt";
    });

    OR

    services.AddAdvancedTaskManager();
    
...
}
```

In addition, the configuration can be read from the `appsettings.json`:

```Json
"AdvancedTaskManager": {
    "PageSize":  40, 
    "WarningDays": 6
    "DeleteChangeApprovalTasks": true,
    "PageSize": 10,
    "AddContentApprovalDeadlineProperty": true,
    "DeleteContentApprovalDeadlineProperty": true,
    "WarningDays": 4,
    "DateTimeFormat": "yyyy-MM-dd HH:mm",
    "DateTimeFormatUserFriendly": "MMM dd, yyyy, h:mm:ss tt""
}
```

The settings specified in the `appsettings.json` file will take precedence over any configurations defined in the Startup.


### Integrating Advanced Task Manager into the CMS

This solution incorporates an implementation of the `IMenuProvider`, guaranteeing the inclusion of the Advanced Task Manager administration pages in the CMS Admin menu titled `Adv. Task Manager.` No additional action is required, as Optimizely CMS will automatically scan and activate all instances of the `IMenuProvider`.

### Restricting Access to the Admin UI

By default, access to the Admin UI is limited to users with the `Administrators` role. The solution enhances this by granting access to the `Adv. Task Manager menu` for specific roles, including `CmsAdmins`, `Administrator`, `WebAdmins`, `WebEditors`, and `CMSEditors`. You have the flexibility to configure your authorization policy when registering the AdvancedTaskManager handler.

Example:

```C#
services.AddAdvancedTaskManager(o => { },
    policy =>
    {
        policy.RequireRole("MyRole");
    });
```

### Enabling Optimizely Opti ID Authentication

To integrate Optimizely Opti ID for authentication within your Optimizely CMS and other Optimizely One applications, it's necessary to define the `configurePolicy` for this module during your application startup. This involves a straightforward step of incorporating `policy.AddAuthenticationSchemes(OptimizelyIdentityDefaults.SchemeName);` into the `configurePolicy`, as illustrated in the example below.

```C#
services.AddAdvancedTaskManager(o => { },
    policy =>
    {
        policy.RequireRole("MyRole");
        policy.AddAuthenticationSchemes(OptimizelyIdentityDefaults.SchemeName);
    });
```

 ## Overview
Presently, the User notifications and Tasks (Awaiting Review) are disjointed, offering only fundamental information and limited control to the editors.

![Tasks pane](assets/docsimages/1-task-pane.png)

![Dashboard](assets/docsimages/2-dashboard.png)

![User Notification](assets/docsimages/3-notifications.png)

This project has two goals:

* **Empower editors:** Build a more versatile Approval Sequence tool for Content and Change Awaiting Review tasks, offering editors deeper insights.

* **Streamline approver workflow:** Combine user notifications with tasks within a single interface, allowing approvers to efficiently handle tasks individually or in bulk.

By default, Change Approval is integrated into the Approval Sequence in Optimizely CMS. Therefore, Advanced Task Manager seamlessly incorporates content approval tasks. If the [Change Approval](https://nuget.optimizely.com/package/?id=EPiServer.ChangeApproval) NuGet package is installed, the tool will automatically display two tabs in the left-side menu:

* Content Approval
* Change Approval

![Advanced Task Manager - Content Approval](assets/docsimages/content-approval.png)

![Advanced Task Manager - Change Approval](assets/docsimages/change-approval.png)

The tool furnishes the Editor with the following information regarding the task:

### Change Approval

* Name
* Content type
* Type
* Submitted (UTC)
* Started by
* Deadline (optional)

### Content Approval

* Name
* Content type
* Type
* Submitted (UTC)
* Started by

The list of current features are as follows:
* [Advanced filtering](#advanced-filtering)
* [Select all across pages](#select-all-across-pages)
* [Approve entire approval sequence](#approve-entire-approval-sequence-content-approval-only)
* [Publish content after approval](#publish-content-after-approval)
* [Scheduled publishing](#scheduled-publishing)
* [Approve blocks & media on a page](#approve-blocks--media-on-a-page)
* [Deadline field for content approval](#deadline-field-for-content-approval-sequence-optional)
* [User Notifications Linked to the Task](#user-notifications-linked-to-the-task)
* [Information for Change Approval Tasks](#information-for-change-approval-tasks)
* [Task ordering](#task-ordering)

Some features are disabled by default, but you can decide which ones are enabled by configuring options in `startup.cs` or `appsettings.json`.

### Advanced Filtering

The filter bar above the task list lets editors narrow down content approval tasks using any combination of:

- **Language** — filter by a specific language branch (existing behaviour).
- **Status** — show tasks that are *In Review* (default) or *Ready to Publish* (approved but not yet published).
- **Type** — show only *Pages*, *Blocks*, or *Assets / Media*.
- **Site** — in multi-site solutions, filter tasks to a specific site. The site dropdown is hidden on single-site installations.
- **Content search** — enter a content ID (integer) or part of the content name to find a specific item.

When one or more filters are active, badge pills appear below the filter bar. Each badge links directly to clearing that individual filter, and a **Clear all** link removes every filter at once. Navigating to a different page or language always resets pagination to page 1.

![Advanced Filtering](assets/docsimages/filtering.png)

### Select All Across Pages

Checking the header checkbox selects all items visible on the current page. When there are more results than the configured page size, a banner appears:

> All 30 items on this page are selected. **Select all N items matching current filters.**

![Select all pages](assets/docsimages/select-all-pages.png)

Clicking that link calls the server and loads every matching approval ID into the selection, across all pages. The approval action then covers the complete set. To start over, a **Clear selection** link is shown in the same banner.

![All pages selected](assets/docsimages/all-pages-selected.png)

### Approve Entire Approval Sequence (Content Approval Only)
The tool prompts the editor to approve the entire Content Approval Sequence for single or multiple contents. A comment in the comment field is mandatory.
 
![Approve entire approval sequence](assets/docsimages/content-approval-approve.png)

![Approve entire model](assets/docsimages/content-approval-model.png)
 
### Publish Content After Approval

If the editor possesses publishing rights for content approval, the option to `Publish selected content after approval` will be enabled, allowing the editor to publish the content post-approval.

If the editor holds publishing rights for only some of the content after approving all, only the content the editor can publish will be processed. Warning messages will be displayed for content that the editor cannot publish.

![Publish content after approval](assets/docsimages/content-approval-no-rights-publish.png)

![Publish content after approval](assets/docsimages/content-approval-approve-publish.png)

![Publish content after approval](assets/docsimages/content-approval-publish-model.png)

### Scheduled Publishing

When the **Publish selected content after approval** option is checked, an additional **Schedule publishing for a specific date and time** checkbox becomes available. Enabling it reveals a date/time picker. The selected datetime is sent alongside the approval request; Optimizely schedules the publish by setting `IVersionable.StartPublish` to the chosen time, so the content goes live automatically at that moment rather than immediately.

![Schedule](assets/docsimages/schedule.png)

### Approve Blocks & Media on a Page

An **Approve Page Dependencies** button sits in the filter bar above the task list. Clicking it opens a modal that lets editors approve all pending blocks and media files referenced by a specific page in a single action.

**How to use:**

1. Click the **Approve Page Dependencies** button in the filter bar.
2. Navigate the **page hierarchy tree** to find and select the target page. The tree lazy-loads child pages as you expand nodes.
3. Select one or more **languages** to process.
4. Optionally edit the approval comment.
5. Optionally check **Publish approved content after approval** to publish the blocks and media immediately after approving them (only shown when the current user has publish rights).
6. Click **Approve Dependencies**. A spinner is shown while the request is in progress.

![Approve Blocks & Media on a Page](assets/docsimages/approve-block-media-page.png)

The system inspects all `ContentArea` and `ContentReference` property values on the selected page, identifies any blocks and media currently awaiting approval for the chosen languages, and approves (and optionally publishes) them all. The number of items approved is shown inline. If any items were approved the page refreshes automatically after 1.5 seconds.

### Deadline Field for Content Approval Sequence (Optional)

![Deadline field for the approval sequence](assets/docsimages/change-approval-deadline.png)

The deadline property is a date/time attribute enabling editors to assign priority to content (Page or Block). This prioritization ensures that Approvers are informed about the urgency before approval.

By default, the functionality of the deadline property is deactivated and can be activated (if needed) by incorporating the following option in `startup.cs` or in `appsettings.json`.

```C#
services.AddAdvancedTaskManager(o =>
{
    o.AddContentApprovalDeadlineProperty = true;
});
```

```Json
"AdvancedTaskManager": {
    "AddContentApprovalDeadlineProperty":  true
}
```

By enabling the `Content approval deadline` The property `Content approval deadline` will be added in all PageTyes and BlockTypes under `Settings` Tab.

![Enable approval sequence deadline](assets/docsimages/deadline-property.png)

The deadline property within the tool operates in three states:

* **Warning**

The `Warning `state, highlighted in green, alerts approvers to tasks requiring prompt attention for approval. By default, the Warning state spans a duration of 4 days. This implies that if the content deadline is within 4 days, the `deadline row`` will be highlighted in green.

To customize the duration of the `Warning` state, you can adjust the settings by Configuring option in the `startup.cs` or in `appsettings.json`.

```C#
services.AddAdvancedTaskManager(o =>
{
    o.WarningDays = 8;
});
```

```Json
"AdvancedTaskManager": {
    "WarningDays":  8
}
```

* **Danger**

The `Danger` state, highlighted in red, indicates that the deadline date/time has already passed.

* **Normal**

The `Normal` state is not associated with any specific color, signifying that there is still ample time for Approvers to prioritize the task.

If you set the `AddContentApprovalDeadlineProperty` option in the `startup.cs` or `appsettings.json` to false, it will hide the property and tab from the CMS editor UI.
 
If you wish to remove the property from the CMS, include the following option in the `startup.cs` or `appsettings.json:

```C#
services.AddAdvancedTaskManager(o =>
{
    o.DeleteContentApprovalDeadlineProperty = true;
});
```

```Json
"AdvancedTaskManager": {
    "DeleteContentApprovalDeadlineProperty":  true
}
```

Please note that the **AddContentApprovalDeadlineProperty** will only trigger if **DeleteContentApprovalDeadlineProperty** element is set to **false**.

### User Notifications Linked to the Task
 
![User notification with task](assets/docsimages/notification.png)
 
The tool facilitates the reading of user notifications linked to the task, enhancing the usefulness of the notification icon, rather than accumulating notifications.

Upon the editors opening or refreshing the tool, tasks with unread notifications are highlighted, and the notifications are subsequently marked as read automatically. This ensures that in the editor notification section, the notifications will be acknowledged as read.

### Task ordering

The tool gives editors an option to sort through all the tasks with status Awaiting Review by the following columns:
* Order tasks by name
* Order tasks by content type
* Order tasks by type
* Order tasks by time/date
* Order tasks by a user who submitted the request
* Order task by the deadline

![Sort](assets/docsimages/sort.png)

## Information for Change Approval Tasks

The table row for change approval task information functions as an accordion. Clicking on the row will reveal (or hide) the details of the change approval task.

There are four types of Change approval tasks:

### Security Setting Change

![Securty setting change](assets/docsimages/security-change-approval.png)

### Moving Content Change
 
 ![Moving content change](assets/docsimages/moving-change-approval.png)

### Language Setting Change
 
 ![Language setting change](assets/docsimages/language-change-approval.png)

### Expiration Date Setting Change
 
 ![Expiration date setting change](assets/docsimages/expire-change-approval.png)

# Sandbox App
The sandbox application serves as a testing polygon for evaluating new features and addressing bug fixes in the package.

**ADMIN**

Username: sysadmin

Password: Sandbox#123

**EDITOR**

Username: editor

Password: Sandbox#123

**AUTHOR**

Username: author

Password: Sandbox#123

## Contributing

If you can help please do so by contributing to the package! Reach out package maintainer for additional details if needed.

## Feature requests

If you have new ideas or feature requests that can make this tool even more useful for you or the community, please feel free to open a new issue at the following link.

https://github.com/adnanzameer/optimizely-advancedtaskmanager/issues


## Optimizely (Episerver) CMS 11 Support
Visit the repository: [AdvancedTaskManager on GitHub](https://github.com/adnanzameer/AdvancedTaskManager)

## Changelog

[Changelog](CHANGELOG.md)
