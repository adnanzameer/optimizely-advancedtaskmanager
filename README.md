# Advanced Task Manager for EPiServer CMS

While Advanced Task Manager for EpiServeris a freely available module, if you'd like to express your support, consider treating me to a coffee on Ko-fi:

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/U7U2STV29)

## Description
[![Platform](https://img.shields.io/badge/Platform-.NET%204.7.1-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/Episerver%20-%2011-orange.svg?style=flat)](https://world.episerver.com/cms/)

## Optimizely CMS 12?
**For ASP.NET 5+ and Episerver/Optimizely 12+ see: https://github.com/adnanzameer/optimizely-advancedtaskmanager**

Advanced Task Management for EpiServer, providing enhanced oversight of the administration for Awaiting Review tasks in Content Approval and Change Approval processes.

 ## Overview
Currently, the User notification and Tasks (Awaiting Review) are disconnected and provide very basic information to the editors.

![Tasks pane](assets/docsimages/image001.png)

![User notification](assets/docsimages/image003.png)

The idea behind this project is two-fold. The first purpose is to build a more versatile Approval Sequence task management gadget (for Content & Change Awaiting Review tasks) to extend the information available to the editors. The second purpose is to combine user notifications with tasks and empower Approvers to act on their tasks within a single interface.  

![Advanced Task Manager - Content Approval](assets/docsimages/image0052.png)

![Advanced Task Manager - Change Approval](assets/docsimages/change-approval.png)

The gadget provides the following information about the task to the Editor:

### Change Approval

* Content Name
* Content Type
* Type
* Submitted Date/Time
* Started By
* Deadline (optional)

### Content Approval

* Content Name
* Content Type
* Type
* Submitted Date/Time
* Started By

The list of current features are as follow:
* [Task ordering](#task-ordering)
* [Deadline field for the content approval](#deadline-field-for-the-content-approval-optional)
* [User notifications associated with the task](#user-notifications-associated-with-the-task)
* [Approve entire approval sequence](#approve-entire-approval-sequence)
* [Publish content after approval](#publish-content-after-approval)
* [Change approval task information](#change-approval-task-information)

Some features are disabled by default, but you can decide which ones are enabled by Configuring features in the **Web.config**.

## Task ordering

The gadget gives editors an option to sort through all the tasks with status Awaiting Review by the following columns:
* Order tasks by Content name
* Order tasks by content type
* Order tasks by type
* Order tasks by time/date
* Order tasks by a user who submitted the request
* Order task by the deadline

![Advanced Task Manager](assets/docsimages/image007.gif)

## Deadline field for the approval sequence (Optional)

![Deadline field for the approval sequence](assets/docsimages/image008.png)

The deadline property is a date/time property that allows editors to set priority against the content (Page or Block) so that the Approvers are aware of the priority ahead of approval.

The deadline property functionality is disabled by default and can be enabled (if required) by adding the following **<appSettings>** element of the **Web.config**.

![Enable deadline](assets/docsimages/image012.png)

By enabling the ```Content approval deadline``` The property ```Content approval deadline``` will be added in all  PageTyes and BlockTypes under ```Content Approval``` Tab.

![Enable approval sequence deadline](assets/docsimages/image014.png)

The deadline property has three states in the gadget:

* **Warning**

The ```Warning``` state, highlighted in green informs approvers of the task that needs attention to be approved. The duration of the ```Warning``` state is 4 days by default. It means if the content deadline is within 4 days ```deadline row``` will be highlighted in green.

You can set the duration of the ```Warning``` state by adding the **<appSettings>** element of the **Web.config**.

![Warning notification duration](assets/docsimages/image010.png)

* **Danger**

The ```Danger``` state highlighted in red indicates the deadline date/time has passed.

* **Normal**

The ```Normal``` state is not represented by any color as it shows there is still time for Approvers to prioritize the task.

Setting the **<appSettings> ATM:EnableContentApprovalDeadline** element of the **Web.config** to **false** will hide the property and Tab from CMS editor UI.
 
If you want to delete the property from the CMS, add the following **<appSettings>** element of the **Web.config**

![Disable deadline](assets/docsimages/image016.png)

Please note that the **ATM:DeleteContentApprovalDeadlineProperty** will only trigger if **ATM:EnableContentApprovalDeadline** element is set to **false**.

## User notifications associated with the task
 
![User notification with task](assets/docsimages/image018.png)
 
The gadget allows user notifications associated with the task to be ‘read’ and enable the notification icon to be more useful as opposed to accumulating notifications.

When the editors open or refresh the gadget, all user tasks with unread notifications are highlighted and the notifications then are marked as read automatically.  It means in the editor notification section the notifications will be marked as read. 
 
![User notification](assets/docsimages/image020.png)
 
## Approve entire approval sequence (only for Content Approval)
 
The gadget informs the editor to approve the entire Content Approval Sequence of single or multiple contents. Comment field is required.
 
![Approve entire approval sequence](assets/docsimages/image022.png)
 
## Publish content after approval
 
If the editor has published rights for the content approval, the option for ```Publish selected content after approval``` will be enabled that allows the editor to publish the content after approval.

If the editor has published rights for some of the content after approving all content then only content the editor can publish will be published. The warning messages will appear against the content which the editor cannot publish.

![Publish content after approval](assets/docsimages/image024.png)

## Change approval task information

 The table row of the change approval task information acts like an accordion. On clicking on the row the change approval task details will show (and hide). 

 There are four types of Change approval tasks.

 * Securty setting change

 ![Securty setting change](assets/docsimages/ca-security.png)

 * Moving content change

 ![Moving content change](assets/docsimages/ca-moving.png)

 * Language setting change 

 ![Language setting change](assets/docsimages/ca-language.png)

 * Expiration date setting change

 ![Expiration date setting change](assets/docsimages/ca-expire.png)

## Configuring features

To turn on or off one or more features, use the following **<appSettings>** elements of **Web.config**. By way of an example set false on the feature that should not be available.

![Configuring features](assets/docsimages/image026.png)

## Install 

```Install-Package AdvancedTaskManager```

https://nuget.episerver.com/package/?id=AdvancedTaskManager

Add the ```Advanced Task Manager``` gadget in the dashboard

![Advanced task manager gadet](assets/docsimages/image028.png)

## Contributing

If you can help please do so by contributing to the package! Reach out package maintainer for additional details if needed.

# Update 2.2.0

* New tab and better UI for Change Approval tasks.

# Update 2.0.0

* Change Approval tasks will show along with the Content Approval tasks.
* Support for all content type tasks in CMS. Now editors can view, approve and publish, Episerver Forms, ImageData & MediaData. 
* Bug fixes for pagination and performance improvements.
