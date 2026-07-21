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
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.Web;
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
        private readonly ISiteDefinitionRepository _siteDefinitionRepository;
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;
        private readonly AdvancedTaskManagerOptions _configuration;
        private readonly ILogger _logger;

        private const string ContentApprovalDeadlinePropertyName = "ATM_ContentApprovalDeadline";
        private const int MaxFetchCount = 2000;

        private static readonly HashSet<string> ValidStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inreview", "approved", "" };
        private static readonly HashSet<string> ValidContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "page", "block", "asset", "" };

        public AdvancedTaskController(
            IApprovalRepository approvalRepository,
            IContentRepository contentRepository,
            IContentTypeRepository contentTypeRepository,
            IApprovalEngine approvalEngine,
            LocalizationService localizationService,
            IChangeTaskHelper changeTaskHelper,
            IUIHelper helper,
            ILanguageBranchRepository languageBranchRepository,
            ISiteDefinitionRepository siteDefinitionRepository,
            ISiteDefinitionResolver siteDefinitionResolver,
            IOptions<AdvancedTaskManagerOptions> options,
            INotificationHandler notificationHandler)
        {
            _approvalRepository = approvalRepository;
            _contentRepository = contentRepository;
            _contentTypeRepository = contentTypeRepository;
            _approvalEngine = approvalEngine;
            _localizationService = localizationService;
            _changeTaskHelper = changeTaskHelper;
            _helper = helper;
            _languageBranchRepository = languageBranchRepository;
            _siteDefinitionRepository = siteDefinitionRepository;
            _siteDefinitionResolver = siteDefinitionResolver;
            _notificationHandler = notificationHandler;
            _configuration = options.Value;
            _logger = LogManager.GetLogger(typeof(AdvancedTaskController));
        }

        public async Task<IActionResult> Index(int? page, string language, string status, string contentType, string siteId, string contentFilter)
        {
            // Whitelist validation for filter params
            var safeStatus = ValidStatuses.Contains(status ?? "") ? (status ?? "inreview") : "inreview";
            var safeContentType = ValidContentTypes.Contains(contentType ?? "") ? (contentType ?? "") : "";
            var safeSiteId = !string.IsNullOrEmpty(siteId) && Guid.TryParse(siteId, out _) ? siteId : "";
            var safeContentFilter = string.IsNullOrEmpty(contentFilter) ? "" : contentFilter.Trim().Substring(0, Math.Min(contentFilter.Trim().Length, 200));

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !string.IsNullOrEmpty(a.FullName) && a.FullName.Contains("EPiServer.ChangeApproval"));
            if (assemblies.Any())
                ViewBag.ChangeApproval = true;

            var viewModel = new AdvancedTaskIndexViewData(LanguageBranches(language), _configuration, GetSiteOptions(safeSiteId))
            {
                QueryString = HttpContext.Request.QueryString.ToString(),
                PageNumber = page ?? 1,
                SelectedStatus = safeStatus,
                SelectedContentType = safeContentType,
                SelectedSiteId = safeSiteId,
                ContentFilterText = safeContentFilter
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
                viewModel.HasPublishAccess = true;
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
                    approvalData.ApprovalComment = "Approved Through Advanced Task Manager";

                await ApproveContent(approvalData.TaskValues, approvalData.ApprovalComment, approvalData.PublishContent, approvalData.ScheduledPublishDate);
            }
            return Json("Ok");
        }

        /// <summary>
        /// Returns a grouped list of content items in review, for the dependency-approval dropdown.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContentTasksList(string language)
        {
            var languageBranches = LanguageBranches(language);
            var selectedLanguage = languageBranches.FirstOrDefault(x => x.Selected);
            if (selectedLanguage?.Language == null)
                return Json(Array.Empty<object>());

            var query = new ApprovalQuery
            {
                Status = ApprovalStatus.InReview,
                Language = new CultureInfo(selectedLanguage.Language.LanguageID),
                Reference = new Uri("content:"),
            };

            var isAdminUser = _helper.IsAdminUser();
            if (!isAdminUser)
                query.Username = PrincipalAccessor.Current.Identity?.Name;

            var allFromRepo = await _approvalRepository.ListAsync(query, 0, MaxFetchCount);
            var items = new List<(int ContentId, string Name, string Type)>();

            foreach (var task in allFromRepo.PagedResult)
            {
                if (!(task is ContentApproval approval)) continue;
                _contentRepository.TryGet(approval.ContentLink, out IContent content);
                if (content == null || !(content is PageData)) continue;

                items.Add((content.ContentLink.ID, content.Name, "Page"));
            }

            var result = items
                .OrderBy(i => i.Name)
                .Select(i => new { contentId = i.ContentId, name = i.Name });

            return Json(result);
        }

        /// <summary>
        /// Returns all approval IDs matching the current filters, for client-side "select all across pages".
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTaskIds(string language, string status, string contentType, string siteId, string contentFilter)
        {
            var safeStatus = ValidStatuses.Contains(status ?? "") ? (status ?? "inreview") : "inreview";
            var safeContentType = ValidContentTypes.Contains(contentType ?? "") ? (contentType ?? "") : "";
            if (!string.IsNullOrEmpty(siteId) && !Guid.TryParse(siteId, out _))
                return BadRequest();
            var safeSiteId = siteId ?? "";
            var safeContentFilter = string.IsNullOrEmpty(contentFilter) ? "" : contentFilter.Trim().Substring(0, Math.Min(contentFilter.Trim().Length, 200));

            var languageBranches = LanguageBranches(language);
            var selectedLanguage = languageBranches.FirstOrDefault(x => x.Selected);
            if (selectedLanguage?.Language == null)
                return Json(Array.Empty<int>());

            var query = BuildApprovalQuery(safeStatus, selectedLanguage.Language.LanguageID);

            var allFromRepo = await _approvalRepository.ListAsync(query, 0, MaxFetchCount);
            var ids = new List<int>();

            foreach (var task in allFromRepo.PagedResult)
            {
                if (!(task is ContentApproval approval)) continue;

                if (!string.IsNullOrEmpty(safeContentType) || !string.IsNullOrEmpty(safeSiteId) || !string.IsNullOrEmpty(safeContentFilter))
                {
                    _contentRepository.TryGet(approval.ContentLink, out IContent content);
                    if (content == null) continue;

                    if (!string.IsNullOrEmpty(safeContentType) && !MatchesContentType(content, safeContentType)) continue;
                    if (!string.IsNullOrEmpty(safeContentFilter) && !MatchesContentFilter(content, safeContentFilter)) continue;
                    if (!string.IsNullOrEmpty(safeSiteId) && !MatchesSite(content, safeSiteId)) continue;
                }

                ids.Add(task.ID);
            }

            return Json(ids);
        }

        /// <summary>
        /// Returns all site start pages as the top-level root nodes for the page hierarchy tree.
        /// Falls back to ContentReference.StartPage when no sites are configured.
        /// </summary>
        [HttpGet]
        public IActionResult GetRootPages()
        {
            var sites = _siteDefinitionRepository.List().OrderBy(s => s.Name).ToList();
            var result = new List<object>();

            foreach (var site in sites)
            {
                if (ContentReference.IsNullOrEmpty(site.StartPage)) continue;
                _contentRepository.TryGet(site.StartPage, out PageData startPage);
                if (startPage == null) continue;

                result.Add(new
                {
                    contentId = startPage.ContentLink.ID,
                    name = site.Name,
                    hasChildren = _contentRepository.GetChildren<PageData>(site.StartPage).Any()
                });
            }

            if (!result.Any() && !ContentReference.IsNullOrEmpty(ContentReference.StartPage))
            {
                _contentRepository.TryGet(ContentReference.StartPage, out PageData startPage);
                if (startPage != null)
                    result.Add(new
                    {
                        contentId = startPage.ContentLink.ID,
                        name = startPage.Name,
                        hasChildren = _contentRepository.GetChildren<PageData>(ContentReference.StartPage).Any()
                    });
            }

            return Json(result);
        }

        /// <summary>
        /// Returns immediate child pages of the given parent for the page hierarchy tree.
        /// </summary>
        [HttpGet]
        public IActionResult GetPageChildren(int parentId, string language)
        {
            if (parentId <= 0) return Json(Array.Empty<object>());
            var parentRef = new ContentReference(parentId);

            IEnumerable<PageData> children;
            try
            {
                children = _contentRepository.GetChildren<PageData>(parentRef);
            }
            catch
            {
                children = Enumerable.Empty<PageData>();
            }

            var result = children
                .Select(p => new
                {
                    contentId = p.ContentLink.ID,
                    name = p.Name,
                    hasChildren = _contentRepository.GetChildren<PageData>(p.ContentLink).Any()
                })
                .ToList();

            return Json(result);
        }

        /// <summary>
        /// Approves all pending blocks and media referenced by the specified content item for the given languages.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApprovePageDependencies([FromBody] PageDependencyData data)
        {
            if (data == null || data.ContentId <= 0)
                return BadRequest();

            if (string.IsNullOrEmpty(data.ApprovalComment))
                data.ApprovalComment = "Approved Through Advanced Task Manager";

            // Validate that languages are actual enabled branches
            var enabledLanguageIds = _languageBranchRepository.ListEnabled()
                .Select(l => l.LanguageID)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var selectedLanguages = (data.Languages ?? Array.Empty<string>())
                .Where(l => !string.IsNullOrEmpty(l) && enabledLanguageIds.Contains(l))
                .Distinct()
                .ToList();

            if (!selectedLanguages.Any())
            {
                var first = _languageBranchRepository.LoadFirstEnabledBranch();
                if (first != null) selectedLanguages.Add(first.LanguageID);
            }

            _contentRepository.TryGet(new ContentReference(data.ContentId), out IContent content);
            if (content == null)
                return Json(new { success = false, message = "Content not found." });

            var dependencyRefs = GetContentDependencies(content);
            if (!dependencyRefs.Any())
                return Json(new { success = false, message = "No block or media dependencies found for this content." });

            var dependencyIds = dependencyRefs.Select(r => r.ID).ToHashSet();
            var isAdminUser = _helper.IsAdminUser();
            var approvedCount = 0;

            foreach (var lang in selectedLanguages)
            {
                var query = new ApprovalQuery
                {
                    Status = ApprovalStatus.InReview,
                    Language = new CultureInfo(lang),
                    Reference = new Uri("content:"),
                };
                if (!isAdminUser)
                    query.Username = PrincipalAccessor.Current.Identity?.Name;

                var allApprovals = await _approvalRepository.ListAsync(query, 0, MaxFetchCount);

                foreach (var task in allApprovals.PagedResult)
                {
                    if (!(task is ContentApproval approval)) continue;
                    if (!dependencyIds.Contains(approval.ContentLink.ID)) continue;

                    try
                    {
                        await _approvalEngine.ForceApproveAsync(task.ID, PrincipalAccessor.Current.Identity?.Name, data.ApprovalComment);
                        approvedCount++;

                        if (data.PublishContent)
                        {
                            _contentRepository.TryGet(approval.ContentLink, out IContent depContent);
                            if (depContent != null && _helper.CanUserPublish(depContent))
                            {
                                try
                                {
                                    IContent clone = depContent switch
                                    {
                                        PageData page => page.CreateWritableClone(),
                                        BlockData block => block.CreateWritableClone() as IContent,
                                        ImageData image => image.CreateWritableClone() as IContent,
                                        MediaData media => media.CreateWritableClone() as IContent,
                                        _ => null
                                    };

                                    if (clone != null)
                                        _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.Publish);
                                }
                                catch (Exception pubEx)
                                {
                                    _logger.Error($"ATM - Error publishing dependency {approval.ContentLink.ID}", pubEx);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"ATM - Error approving dependency task {task.ID}", ex);
                    }
                }
            }

            return Json(new { success = true, approved = approvedCount });
        }

        private async Task<List<ContentTask>> GetContentTasks(AdvancedTaskIndexViewData model)
        {
            var selectedLanguage = model.LanguageBranchList.FirstOrDefault(x => x.Selected);
            if (selectedLanguage?.Language == null)
                return new List<ContentTask>();

            var query = BuildApprovalQuery(model.SelectedStatus, selectedLanguage.Language.LanguageID);

            bool hasExtraFilters = !string.IsNullOrEmpty(model.SelectedContentType)
                || !string.IsNullOrEmpty(model.SelectedSiteId)
                || !string.IsNullOrEmpty(model.ContentFilterText);

            IEnumerable<Approval> approvals;

            if (hasExtraFilters)
            {
                var allList = await _approvalRepository.ListAsync(query, 0, MaxFetchCount);
                approvals = allList.PagedResult;
            }
            else
            {
                var pagedList = await _approvalRepository.ListAsync(query, (model.PageNumber - 1) * model.PageSize, model.PageSize);
                approvals = pagedList.PagedResult;
                model.TotalItemsCount = Convert.ToInt32(pagedList.TotalCount);
            }

            var allFilteredTasks = new List<ContentTask>();

            foreach (var task in approvals)
            {
                if (!(task is ContentApproval approval)) continue;

                _contentRepository.TryGet(approval.ContentLink, out IContent content);
                if (content == null) continue;

                // Apply in-memory filters when needed
                if (hasExtraFilters)
                {
                    if (!string.IsNullOrEmpty(model.SelectedContentType) && !MatchesContentType(content, model.SelectedContentType)) continue;
                    if (!string.IsNullOrEmpty(model.ContentFilterText) && !MatchesContentFilter(content, model.ContentFilterText)) continue;
                    if (!string.IsNullOrEmpty(model.SelectedSiteId) && !MatchesSite(content, model.SelectedSiteId)) continue;
                }

                var customTask = new ContentTask
                {
                    ApprovalId = task.ID,
                    DateTime = task.ActiveStepStarted,
                    StartedBy = task.StartedBy,
                    URL = approval.ContentLink.GetEditUrl(selectedLanguage.Language.LanguageID),
                    CanUserPublish = _helper.CanUserPublish(content),
                    ContentReference = content.ContentLink,
                    ContentName = content.Name,
                    ContentType = GetTypeContent(content),
                };

                SetContentTypeAndIcon(customTask, content);
                SetDeadlineInfo(customTask, content, model);
                SetSiteInfo(customTask, content);

                var id = content.ContentLink.ID.ToString();
                customTask = await _notificationHandler.GetNotifications(id, customTask, true);

                allFilteredTasks.Add(customTask);
            }

            if (hasExtraFilters)
            {
                model.TotalItemsCount = allFilteredTasks.Count;
                return allFilteredTasks
                    .Skip((model.PageNumber - 1) * model.PageSize)
                    .Take(model.PageSize)
                    .ToList();
            }

            return allFilteredTasks;
        }

        private ApprovalQuery BuildApprovalQuery(string status, string languageId)
        {
            var approvalStatus = string.Equals(status, "approved", StringComparison.OrdinalIgnoreCase)
                ? ApprovalStatus.Approved
                : ApprovalStatus.InReview;

            var query = new ApprovalQuery
            {
                Status = approvalStatus,
                Language = new CultureInfo(languageId),
                Reference = new Uri("content:"),
            };

            var isAdminUser = _helper.IsAdminUser();
            if (!isAdminUser)
                query.Username = PrincipalAccessor.Current.Identity?.Name;

            return query;
        }

        private bool MatchesContentType(IContent content, string contentType) =>
            contentType switch
            {
                "page" => content is PageData,
                "block" => content is BlockData,
                "asset" => content is ImageData || content is MediaData,
                _ => true
            };

        private bool MatchesContentFilter(IContent content, string filter)
        {
            if (int.TryParse(filter, out var filterId))
                return content.ContentLink.ID == filterId;
            return content.Name.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesSite(IContent content, string siteId)
        {
            if (!Guid.TryParse(siteId, out var siteGuid)) return true;
            try
            {
                var site = _siteDefinitionResolver.GetByContent(content.ContentLink, false, false);
                return site?.Id == siteGuid;
            }
            catch
            {
                return false;
            }
        }

        private void SetContentTypeAndIcon(ContentTask customTask, IContent content)
        {
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
        }

        private void SetDeadlineInfo(ContentTask customTask, IContent content, AdvancedTaskIndexViewData model)
        {
            if (!model.AddContentApprovalDeadlineProperty) return;

            var propertyData = content.Property.Get(ContentApprovalDeadlinePropertyName) ?? content.Property[ContentApprovalDeadlinePropertyName];
            if (propertyData == null) return;

            DateTime.TryParse(propertyData.ToString(), out var dateValue);
            if (dateValue == DateTime.MinValue || string.IsNullOrEmpty(customTask.Type)) return;

            customTask.Deadline = dateValue;
            var days = DateTime.Now.CountDaysInRange(dateValue);

            if (days == 0)
                customTask.WarningColor = "red";
            else if (days > 0 && days < _configuration.WarningDays)
                customTask.WarningColor = "green";
        }

        private void SetSiteInfo(ContentTask customTask, IContent content)
        {
            try
            {
                var site = _siteDefinitionResolver.GetByContent(content.ContentLink, false, false);
                if (site != null)
                {
                    customTask.SiteName = site.Name;
                    customTask.SiteId = site.Id;
                }
            }
            catch
            {
                // non-critical: site info is for display only
            }
        }

        private async Task ApproveContent(string values, string approvalComment, bool publishContent, DateTime? scheduledPublishDate)
        {
            if (string.IsNullOrEmpty(values)) return;

            var ids = values.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

            foreach (var id in ids)
            {
                int.TryParse(id, out var approvalId);
                if (approvalId == 0) continue;

                try
                {
                    var approval = await _approvalRepository.GetAsync(approvalId);
                    await _approvalEngine.ForceApproveAsync(approvalId, PrincipalAccessor.Current.Identity?.Name, approvalComment);

                    if (approval is ContentApproval contentApproval && publishContent)
                    {
                        _contentRepository.TryGet(contentApproval.ContentLink, out IContent content);
                        if (content == null || !_helper.CanUserPublish(content)) continue;

                        try
                        {
                            IContent clone = content switch
                            {
                                PageData page => page.CreateWritableClone(),
                                BlockData block => block.CreateWritableClone() as IContent,
                                ImageData image => image.CreateWritableClone() as IContent,
                                MediaData media => media.CreateWritableClone() as IContent,
                                _ => null
                            };

                            if (clone == null) continue;

                            // Apply scheduled publish date when provided and in the future
                            if (scheduledPublishDate.HasValue && scheduledPublishDate.Value > DateTime.UtcNow)
                            {
                                if (clone is IVersionable versionable)
                                    versionable.StartPublish = scheduledPublishDate.Value;
                            }

                            _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.Publish);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"ATM - Error publishing content after approval for approvalId: {approvalId} & contentId: {contentApproval.ContentLink.ID}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"ATM - Error approving content for approvalId: {approvalId}", ex);
                }
            }
        }

        /// <summary>
        /// Collects all block and media content references from a content item's properties.
        /// Checks property values by type rather than property wrapper class names for portability across CMS versions.
        /// </summary>
        private List<ContentReference> GetContentDependencies(IContent content)
        {
            var seenIds = new HashSet<int>();
            var refs = new List<ContentReference>();

            foreach (var prop in content.Property)
            {
                try
                {
                    if (prop.Value is ContentArea area)
                    {
                        foreach (var item in area.Items)
                        {
                            if (!ContentReference.IsNullOrEmpty(item.ContentLink) && seenIds.Add(item.ContentLink.ID))
                                refs.Add(item.ContentLink);
                        }
                    }
                    else if (prop.Value is ContentReference cr && !ContentReference.IsNullOrEmpty(cr))
                    {
                        if (seenIds.Add(cr.ID))
                            refs.Add(cr);
                    }
                }
                catch
                {
                    // skip properties that can't be read
                }
            }

            // Filter to blocks and media only
            var filteredRefs = new List<ContentReference>();
            foreach (var r in refs)
            {
                _contentRepository.TryGet(r, out IContent depContent);
                if (depContent is BlockData || depContent is ImageData || depContent is MediaData)
                    filteredRefs.Add(r);
            }

            return filteredRefs;
        }

        private async Task<List<ContentTask>> GetChangeApprovalTasks(AdvancedTaskIndexViewData model)
        {
            var query = new ApprovalQuery
            {
                Status = ApprovalStatus.InReview,
                Reference = new Uri("changeapproval:")
            };

            var isAdminUser = _helper.IsAdminUser();
            if (!isAdminUser)
                query.Username = PrincipalAccessor.Current.Identity?.Name;

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
                            _contentRepository.TryGet(new ContentReference(contentId), out content);
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

        private List<SiteOption> GetSiteOptions(string selectedSiteId)
        {
            return _siteDefinitionRepository.List()
                .OrderBy(s => s.Name)
                .Select(s => new SiteOption
                {
                    SiteId = s.Id.ToString(),
                    SiteName = s.Name,
                    Selected = s.Id.ToString().Equals(selectedSiteId, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
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
                contentName = "";

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
                else if (!ContentReference.IsNullOrEmpty(ContentReference.StartPage))
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
