using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AdvancedTaskManager.Infrastructure;
using AdvancedTaskManager.Infrastructure.Cms;
using AdvancedTaskManager.Infrastructure.Configuration;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer;
using EPiServer.Approvals;
using EPiServer.Approvals.ContentApprovals;
using EPiServer.Authorization;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdvancedTaskManager.Features.AdvancedTask
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = Constants.PolicyName)]
    public class AdvancedTaskController : Controller
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IUIHelper _helper;
        private readonly IContentRepository _contentRepository;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly INotificationHandler _notificationHandler;
        private readonly IApprovalEngine _approvalEngine;
        private readonly LocalizationService _localizationService;
        private readonly IChangeTaskHelper _changeTaskHelper;

        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly AdvancedTaskManagerOptions _configuration;

        private readonly ILogger _logger;
        private const string ContentApprovalDeadlinePropertyName = "ATM_ContentApprovalDeadline";

        public AdvancedTaskController(IApprovalRepository approvalRepository, IContentRepository contentRepository, IContentTypeRepository contentTypeRepository, IApprovalEngine approvalEngine, LocalizationService localizationService, IChangeTaskHelper changeTaskHelper, IUIHelper helper, ILanguageBranchRepository languageBranchRepository, IOptions<AdvancedTaskManagerOptions> options, INotificationHandler notificationHandler)
        {
            _approvalRepository = approvalRepository;
            _contentRepository = contentRepository;
            _contentTypeRepository = contentTypeRepository;
            _approvalEngine = approvalEngine;
            _localizationService = localizationService;
            _changeTaskHelper = changeTaskHelper;
            _helper = helper;

            _languageBranchRepository = languageBranchRepository;
            _notificationHandler = notificationHandler;
            _configuration = options.Value;
            _logger = LogManager.GetLogger(typeof(AdvancedTaskController));
        }

        public async Task<IActionResult> Index(int? page, string language)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !string.IsNullOrEmpty(a.FullName) && a.FullName.Contains("EPiServer.ChangeApproval"));

            if (assemblies.Any())
            {
                ViewBag.ChangeApproval = true;
            }

            var viewModel = new AdvancedTaskIndexViewData(LanguageBranches(language), _configuration)
            {
                QueryString = HttpContext.Request.QueryString.ToString(),
                PageNumber = page ?? 1
            };

            await ChangeApprovalModel(viewModel);

            return View(viewModel);
        }


        private async Task ChangeApprovalModel(AdvancedTaskIndexViewData viewModel)
        {
            ViewBag.Page = "contentapproval";

            var contentTaskList = await GetContentTasks(viewModel);
            viewModel.ContentTaskList = contentTaskList;

            var count = contentTaskList.Count(x => x.CanUserPublish);
            if (count != 0)
            {
                viewModel.HasPublishAccess = true;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApproval(int? page)
        {
            var viewModel = new AdvancedTaskIndexViewData(LanguageBranches(string.Empty), _configuration)
            {
                QueryString = HttpContext.Request.QueryString.ToString(),
                PageNumber = page ?? 1
            };

            ViewBag.Page = "changeapproval";

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !string.IsNullOrEmpty(a.FullName) && a.FullName.Contains("EPiServer.ChangeApproval"));

            if (!assemblies.Any())
            {
                //delete all change approval tasks
                var deleteChangeApprovalTasks = _configuration.DeleteChangeApprovalTasks;
                if (deleteChangeApprovalTasks)
                {
                    var changeTasks = await GetChangeApprovalTasks(viewModel);
                    var ids = changeTasks.Select(contentTask => contentTask.ApprovalId).ToList();
                    await AbortTasks(ids);
                }

                await ChangeApprovalModel(viewModel);
                return View("Index", viewModel);
            }

            ViewBag.ChangeApproval = true;
            var approvalTasks = await GetChangeApprovalTasks(viewModel);
            viewModel.ContentTaskList = approvalTasks;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveContentTasks([FromBody] ApprovalData approvalData)
        {
            ViewBag.Page = "contentapproval";
            if (approvalData != null && !string.IsNullOrEmpty(approvalData.TaskValues))
            {
                if (string.IsNullOrEmpty(approvalData.ApprovalComment))
                {
                    approvalData.ApprovalComment = "Approved Through Advanced Task Manager";
                }

                await ApproveContent(approvalData.TaskValues, approvalData.ApprovalComment, approvalData.PublishContent);
            }
            return Json("Ok");
        }

        private async Task<List<ContentTask>> GetContentTasks(AdvancedTaskIndexViewData model)
        {
            var selectedLanguage = model.LanguageBranchList.FirstOrDefault(x => x.Selected);

            if (selectedLanguage?.Language != null)
            {
                var query = new ApprovalQuery
                {
                    Status = ApprovalStatus.InReview,
                    Language = new CultureInfo(selectedLanguage.Language.LanguageID),
                    Reference = new Uri("content:"),
                };


                var roles = await _helper.GetUserRoles();
                if (!roles.Contains(Roles.Administrators) && !roles.Contains(Roles.WebAdmins) &&
                    !roles.Contains(Roles.CmsAdmins))
                {
                    query.Username = PrincipalAccessor.Current.Identity?.Name;
                }

                var list = await _approvalRepository.ListAsync(query, (model.PageNumber - 1) * model.PageSize, model.PageSize);
                model.TotalItemsCount = Convert.ToInt32(list.TotalCount);

                var taskList = new List<ContentTask>();

                foreach (var task in list.PagedResult)
                {
                    var id = task.ID.ToString();

                    var customTask = new ContentTask
                    {
                        ApprovalId = task.ID,
                        DateTime = task.ActiveStepStarted,
                        StartedBy = task.StartedBy
                    };

                    if (task is ContentApproval approval)
                    {
                        _contentRepository.TryGet(approval.ContentLink, out IContent content);

                        if (content != null)
                        {
                            customTask.URL = approval.ContentLink.GetEditUrl(selectedLanguage.Language.LanguageID);
                            id = content.ContentLink.ID.ToString();
                            var canUserPublish = _helper.CanUserPublish(content);

                            customTask.CanUserPublish = canUserPublish;
                            customTask.ContentReference = content.ContentLink;
                            customTask.ContentName = content.Name;

                            customTask.ContentType = GetTypeContent(content);

                            if (content is PageData)
                            {
                                customTask.Type = "Page";
                                customTask.ContentIcon = "file-text";
                            }
                            else if (content is BlockData)
                            {
                                customTask.Type = "Block";
                                customTask.ContentIcon = "file-text";

                                if (!string.IsNullOrWhiteSpace(customTask.ContentType) && customTask.ContentType.Equals("Form container"))
                                {
                                    customTask.Type = "Form";
                                    customTask.ContentIcon = "file";
                                }
                            }
                            else if (content is ImageData)
                            {
                                customTask.Type = "Image";
                                customTask.ContentIcon = "image";
                            }
                            else if (content is MediaData)
                            {
                                customTask.Type = "Media";
                                customTask.ContentIcon = "youtube";

                                if (!string.IsNullOrWhiteSpace(customTask.ContentType) && customTask.ContentType.Equals("Video"))
                                {
                                    customTask.Type = "Video";
                                    customTask.ContentIcon = "film";
                                }
                            }

                            var enableContentApprovalDeadline = model.AddContentApprovalDeadlineProperty;
                            var warningDays = _configuration.WarningDays;

                            if (enableContentApprovalDeadline)
                            {
                                //Deadline Property of The Content
                                var propertyData = content.Property.Get(ContentApprovalDeadlinePropertyName) ?? content.Property[ContentApprovalDeadlinePropertyName];
                                if (propertyData != null)
                                {
                                    DateTime.TryParse(propertyData.ToString(), out var dateValue);
                                    if (dateValue != DateTime.MinValue && !string.IsNullOrEmpty(customTask.Type))
                                    {
                                        customTask.Deadline = dateValue;
                                        var days = DateTime.Now.CountDaysInRange(dateValue);

                                        if (days == 0)
                                        {
                                            customTask.WarningColor = "red";
                                        }
                                        else if (days > 0 && days < warningDays)
                                        {
                                            customTask.WarningColor = "green";
                                        }
                                    }
                                }
                            }
                        }

                        //Get Notifications
                        customTask = await _notificationHandler.GetNotifications(id, customTask, true);

                        taskList.Add(customTask);
                    }
                }

                return taskList;
            }

            return new List<ContentTask>();
        }

        private async Task ApproveContent(string values, string approvalComment, bool publishContent)
        {
            if (!string.IsNullOrEmpty(values))
            {
                var ids = values.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

                foreach (var id in ids)
                {
                    int.TryParse(id, out var approvalId);
                    if (approvalId != 0)
                    {
                        try
                        {
                            var approval = await _approvalRepository.GetAsync(approvalId);
                            await _approvalEngine.ForceApproveAsync(approvalId, PrincipalAccessor.Current.Identity?.Name, approvalComment);

                            if (approval is ContentApproval contentApproval)
                            {
                                _contentRepository.TryGet(contentApproval.ContentLink, out IContent content);
                                var canUserPublish = publishContent && _helper.CanUserPublish(content);
                                if (content != null && canUserPublish)
                                {
                                    try
                                    {

                                        switch (content)
                                        {
                                            case PageData page:
                                                {
                                                    var clone = page.CreateWritableClone();
                                                    _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.Publish);
                                                    break;
                                                }
                                            case BlockData block:
                                                {
                                                    var clone = block.CreateWritableClone() as IContent;
                                                    _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.Publish);
                                                    break;
                                                }
                                            case ImageData image:
                                                {
                                                    var clone = image.CreateWritableClone() as IContent;
                                                    _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.Publish);
                                                    break;
                                                }
                                            case MediaData media:
                                                {
                                                    var clone = media.CreateWritableClone() as IContent;
                                                    _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.Publish);
                                                    break;
                                                }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error("ATM - Error at Publishing content after approval for approvalId: " + approvalId + " & contentId: " + contentApproval.ContentLink.ID, ex);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("ATM -  Error at Approving content for approvalId: " + approvalId, ex);
                        }
                    }
                }
            }
        }

        private async Task<List<ContentTask>> GetChangeApprovalTasks(AdvancedTaskIndexViewData model)
        {
            var query = new ApprovalQuery
            {
                Status = ApprovalStatus.InReview,
                Reference = new Uri("changeapproval:")
            };

            var roles = await _helper.GetUserRoles();
            if (!roles.Contains(Roles.Administrators) && !roles.Contains(Roles.WebAdmins) &&
                !roles.Contains(Roles.CmsAdmins))
            {
                query.Username = PrincipalAccessor.Current.Identity?.Name;
            }

            var list = await _approvalRepository.ListAsync(query, (model.PageNumber - 1) * model.PageSize, model.PageSize);
            model.TotalItemsCount = Convert.ToInt32(list.TotalCount);

            var taskList = new List<ContentTask>();

            foreach (var task in list.PagedResult)
            {
                IContent content = null;
                var id = task.ID.ToString();

                var customTask = new ContentTask
                {
                    ApprovalId = task.ID,
                    DateTime = task.ActiveStepStarted,
                    StartedBy = task.StartedBy,
                    URL = new ContentReference(task.ID).GetEditUrl().Replace(".contentdata:", ".changeapproval:")
                };

                if (!(task is ContentApproval))
                {

                    var taskDetails = _changeTaskHelper.GetData(task.ID);

                    if (taskDetails != null)
                    {
                        customTask.Type = taskDetails.Type;
                        customTask.ContentName = taskDetails.Name;
                        customTask.Details = taskDetails.Details;
                    }

                    if (task.Reference != null && !string.IsNullOrEmpty(task.Reference.AbsolutePath))
                    {
                        var pageId = task.Reference.AbsolutePath.Replace("/", "");

                        int.TryParse(pageId, out var contentId);
                        if (contentId != 0)
                        {
                            _contentRepository.TryGet(new ContentReference(contentId), out content);
                        }
                    }

                    if (content != null)
                    {
                        customTask.ContentReference = content.ContentLink;
                        customTask.ContentType = GetTypeContent(content);
                    }

                    customTask = await _notificationHandler.GetNotifications(id, customTask, false);

                    taskList.Add(customTask);
                }
            }

            return taskList;
        }

        private async Task AbortTasks(List<int> ids)
        {
            await _approvalEngine.AbortAsync(ids, PrincipalAccessor.Current.Identity?.Name);
        }

        private string GetTypeContent(IContent content)
        {
            var contentName = "";

            var contentType = _contentTypeRepository.Load(content.GetType().BaseType);

            if (contentType != null)
            {
                contentName = contentType.DisplayName;
            }
            else
            {
                contentType = _contentTypeRepository.Load(content.GetType());

                if (contentType != null)
                {
                    if (!string.IsNullOrEmpty(contentType.DisplayName))
                        contentName = contentType.DisplayName;
                    else if (!string.IsNullOrEmpty(contentType.Name))
                        contentName = contentType.Name;
                }
            }

            if (string.IsNullOrWhiteSpace(contentName))
            {
                var memberInfo = content.GetType().BaseType;
                if (memberInfo != null)
                {
                    contentName = _localizationService.GetString("/contenttypes/" + memberInfo.Name.ToLower() + "/name", FallbackBehaviors.FallbackCulture);
                }
            }

            if (!string.IsNullOrWhiteSpace(contentName) && contentName.Contains("[Missing text"))
            {
                contentName = "";
            }

            return contentName;
        }

        private List<LanguageBranchOption> LanguageBranches(string language)
        {
            var toReturn = new List<LanguageBranchOption>();
            var enabledLanguages = _languageBranchRepository.ListEnabled();
            var firstEnabled = _languageBranchRepository.LoadFirstEnabledBranch();

            foreach (var languageBranch in enabledLanguages)
            {
                var languageBranchOption = new LanguageBranchOption
                {
                    Language = languageBranch,
                    Selected = false
                };

                if (!string.IsNullOrEmpty(language))
                {
                    languageBranchOption.Selected = languageBranch.LanguageID.Equals(language, StringComparison.OrdinalIgnoreCase);
                }
                else if (!ContentReference.IsNullOrEmpty(ContentReference.StartPage)) //get it from Start page
                {
                    _contentRepository.TryGet<IContent>(ContentReference.StartPage, languageBranch.Culture, out var startPage);

                    if (startPage != null && startPage.IsMasterLanguageBranch())
                    {
                        languageBranchOption.Selected = true;
                    }
                    else if (firstEnabled.LanguageID == languageBranch.LanguageID)
                    {
                        languageBranchOption.Selected = true;

                    }
                }
                else if (firstEnabled.LanguageID == languageBranch.LanguageID)
                {
                    languageBranchOption.Selected = true;
                }

                toReturn.Add(languageBranchOption);
            }

            return toReturn;
        }
    }
}