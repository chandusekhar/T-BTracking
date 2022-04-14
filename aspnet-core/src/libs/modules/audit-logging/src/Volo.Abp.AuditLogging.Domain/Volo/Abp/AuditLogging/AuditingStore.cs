using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.Follows;
using BugTracking.Issues;
using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Volo.Abp.AuditLogging
{
    public class AuditingStore : IAuditingStore, ITransientDependency
    {
        public ILogger<AuditingStore> Logger { get; set; }

        protected IAuditLogRepository AuditLogRepository { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; }
        private readonly IUserRepository _userRepository;
        protected AbpAuditingOptions Options { get; }
        //
        private readonly ITMTCurrentUser _tmtCurrentUser;

        private readonly IIssueRepository _issueRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly ILocalEventBus _localEventBus;
        public AuditingStore(
            ILocalEventBus localEventBus,
            IAssigneeRepository assigneeRepository,
            ICommentRepository commentRepository,
            ICategoryRepository categoryRepository,
            IProjectRepository projectRepository,
            IStatusRepository statusRepository,
            IIssueRepository issueRepository,
            IUserRepository userRepository,
            ITMTCurrentUser tmtCurrentUser,
            IAuditLogRepository auditLogRepository,
            IGuidGenerator guidGenerator,
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<AbpAuditingOptions> options
            )
        {
            _localEventBus = localEventBus;
            _assigneeRepository = assigneeRepository;
            _commentRepository = commentRepository;
            _categoryRepository = categoryRepository;
            _projectRepository = projectRepository;
            _statusRepository = statusRepository;
            _issueRepository = issueRepository;
            _userRepository = userRepository;
            _tmtCurrentUser = tmtCurrentUser;
            AuditLogRepository = auditLogRepository;
            GuidGenerator = guidGenerator;
            UnitOfWorkManager = unitOfWorkManager;
            Options = options.Value;

            Logger = NullLogger<AuditingStore>.Instance;
        }

        public virtual async Task SaveAsync(AuditLogInfo auditInfo)
        {
            if (!Options.HideErrors)
            {
                await SaveLogAsync(auditInfo);
                return;
            }

            try
            {
                await SaveLogAsync(auditInfo);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Could not save the audit log object: " + Environment.NewLine + auditInfo.ToString());
                Logger.LogException(ex, LogLevel.Error);
            }
        }

        public PropertiesChangeDto changeIdToName(PropertiesChangeDto property)
        {
            List<string> enumTypeList = new List<string> { "Priority", "IssueLevel" };

            List<string> listPriority = Enum.GetValues(typeof(Priority))
                                .Cast<Priority>()
                                .Select(v => v.ToString())
                                .ToList();
            List<string> listIssueLevel = Enum.GetValues(typeof(Level))
                                .Cast<Level>()
                                .Select(v => v.ToString())
                                .ToList();

            if (enumTypeList.Contains(property.PropertyName))
            {
                switch (property.PropertyName)
                {
                    case "Priority":
                        property.OriginalValue = property.OriginalValue != null ? listPriority[property.OriginalValue.To<int>()] : null;
                        property.NewValue = property.NewValue != null ? listPriority[property.NewValue.To<int>()] : null;
                        break;
                    case "IssueLevel":
                        property.OriginalValue = property.OriginalValue != null ? listIssueLevel[property.OriginalValue.To<int>()] : null;
                        property.NewValue = property.NewValue != null ? listIssueLevel[property.NewValue.To<int>()] : null;
                        break;
                }
            }
            else if (property.PropertyName != "Id" && (property.PropertyName.Contains("Id") || property.PropertyName.Contains("ID")))
            {
                string propertyName = property.PropertyName.Replace("Id", "").Replace("ID", "");
                string newValue = property.NewValue;
                string originalValue = property.OriginalValue;

                switch (propertyName)
                {
                    case "Project":
                        if (originalValue != null)
                        {
                            property.OriginalValue = _projectRepository.GetAsync(originalValue.To<Guid>()).Result.Name;
                        }
                        if (newValue != null)
                        {
                            property.NewValue = _projectRepository.GetAsync(newValue.To<Guid>()).Result.Name;
                        }
                        property.PropertyName = "ProjectName";
                        break;
                    case "Status":
                        if (originalValue != null)
                        {
                            property.OriginalValue = _statusRepository.GetAsync(originalValue.To<Guid>()).Result.Name;
                        }
                        if (newValue != null)
                        {
                            property.NewValue = _statusRepository.GetAsync(newValue.To<Guid>()).Result.Name;
                        }
                        property.PropertyName = "StatusName";
                        break;
                };
            }
            return property;
            
        }

        public void setExtendPropsForId(string propertyName, string Id, AuditLogInfo auditInfo)
        {
            if (Id != null)
            {
                Guid parseId = Id.To<Guid>();
                propertyName = propertyName.Replace("Id", "").Replace("ID", "");
                switch (propertyName)
                {
                    case "Project":
                        Project project = _projectRepository.GetAsync(parseId).Result;
                        auditInfo.SetProperty($"{propertyName}ID", project.Id)
                            .SetProperty($"{propertyName}Name", project.Name);
                        break;
                    case "Status":
                        Status status = _statusRepository.GetAsync(parseId).Result;
                        auditInfo.SetProperty($"{propertyName}ID", status.Id)
                            .SetProperty($"{propertyName}Name", status.Name)
                            .SetProperty($"{propertyName}Color", status.NzColor);
                        break;
                };
            }
        }

        //Return Issue id to call event to store update issues' relateions into issue
        public string setExtraProperties(AuditLogInfo auditInfo)
        {
            string issueId_returned = String.Empty;
            //Get entity type of entity changes which related to Issue
            List<string> entityType = new List<string>
            {
                typeof(Issue).FullName,
                typeof(Comment).FullName
            };

            //Get issue properties
            List<string> issueProps = typeof(IssuePropertiesDto).GetProperties().Select(x => x.Name).ToList();

            //Get list properties changeable
            List<string> issuePropsChangeable = typeof(IssuePropertiesChangeableDto).GetProperties().Select(x => x.Name).ToList();
            List<string> commentPropsChangeable = typeof(CommentPropertiesChangeableDto).GetProperties().Select(x => x.Name).ToList();
            List<string> concatProsChangeable = issuePropsChangeable.Union(commentPropsChangeable).ToList();

            //Get list entity changes info which have props changable
            List<EntityChangeInfo> entityChangeInfos = auditInfo.EntityChanges
                        .Where(
                            entityChange =>
                                entityType.Contains(entityChange.EntityTypeFullName) && entityChange.PropertyChanges.Any(prop => concatProsChangeable.Contains(prop.PropertyName))
                        ).ToList();

            //Set properties based entity changes info above
            entityChangeInfos.ForEach(entityChange =>
            {
                //Find issue Id in props changeable
                var getIssuePropertyInPropsChangeable = entityChange.PropertyChanges.FirstOrDefault(prop => prop.PropertyName == "IssueID");
                var issueIdInPropertiesChangeable = getIssuePropertyInPropsChangeable?.NewValue?.Replace("\"", "").To<Guid>() ?? getIssuePropertyInPropsChangeable?.OriginalValue?.Replace("\"", "").To<Guid>();

                //If null, find in current entityChange which is Issue type
                Guid? issueId = issueIdInPropertiesChangeable ?? (entityChange.EntityTypeFullName == typeof(Issue).FullName ? entityChange.EntityId.To<Guid>() : null);
                //Try to get issue id on Put comment
                //if (entityChange.ChangeType != 0 && entityChange.EntityTypeFullName == typeof(Comment).FullName)
                //{
                //    issueId = _commentRepository.FirstOrDefault(c => c.Id.ToString() == entityChange.EntityId).IssueID;
                //}
                Issue issue;
                //&& entityChange.EntityTypeFullName.Contains("Issue")
                //|| auditInfo.HttpMethod == "PUT" || auditInfo.HttpMethod == "DELETE"
                //Get current issue states
                if ((auditInfo.HttpMethod=="POST" || auditInfo.HttpMethod == "PUT" || auditInfo.HttpMethod == "DELETE"))
                {
                    issue = null;
                }
                else
                {
                    issue = _issueRepository.FirstOrDefault(x => x.Id == issueId);
                }
                #region POST ISSUE CASE
                //If cannot find out issue at all entityChange => issue is not exist => get it from audit created
                if (issue == null)
                {
                    entityChange.PropertyChanges.ForEach(prop =>
                    {
                        if (issueProps.Contains(prop.PropertyName))
                        {
                            if (prop.PropertyName.Contains("ID") || prop.PropertyName.Contains("Id"))
                            {
                                if (auditInfo.HttpMethod != "DELETE")
                                {
                                    setExtendPropsForId(prop.PropertyName, prop.NewValue.Replace("\"", ""), auditInfo);
                                }
                            }
                            else
                            {
                                if(auditInfo.HttpMethod != "DELETE")
                                {
                                    auditInfo.SetProperty(prop.PropertyName, prop.NewValue.Replace("\"", "") == "null" ? null : prop.NewValue.Replace("\"", ""));
                                }
                            }
                        }
                    });
                    auditInfo.SetProperty("Id", entityChange.EntityId);
                    //return issueId of this function
                    issueId_returned = entityChange.EntityId;
                }
                #endregion POST ISSUE CASE

                #region PUT/DELETE CASE
                //Issue is existed
                else
                {
                    //Get all properties of Issue and set it for audit log extraproperties
                    issueProps.ForEach(i =>
                    {
                        if (i != "Id" && (i.Contains("ID") || i.Contains("Id")))
                        {
                            setExtendPropsForId(i, issue.GetType().GetProperty(i).GetValue(issue)?.ToString()?.Replace("\"", ""), auditInfo);
                        }
                        else
                        {
                            auditInfo.SetProperty(i, issue.GetType().GetProperty(i).GetValue(issue, null));
                        }
                    });

                    //Get entityChangeProps for using update methods below
                    var entityChangeProps = entityChange.PropertyChanges.Where(prop => concatProsChangeable.Contains(prop.PropertyName)).ToList();

                    //UPDATE Properties when it changed for Audit Extraproperties (Option)
                    #region UPDATE Properties when it changed for Audit Extraproperties (Option)
                    entityChangeProps.ForEach(prop =>
                    {
                        //////////////////////////DONT UPDATE ISSUE ON DELETE
                        if (!(entityChange.EntityTypeFullName == typeof(Issue).FullName && entityChange.ChangeType.To<int>() == 2))
                        {
                            issueProps.ForEach(i =>
                            {
                                if (i == prop.PropertyName)
                                {
                                    if (i != "Id" && (i.Contains("ID") || i.Contains("Id")))
                                    {
                                        setExtendPropsForId(i, prop.NewValue?.Replace("\"", ""), auditInfo);
                                    }
                                    else
                                    {
                                        auditInfo.SetProperty(i, prop.NewValue?.Replace("\"", "") == "null" ? null : prop.NewValue?.Replace("\"", ""));
                                    }
                                }
                            });
                        }
                    });
                    #endregion

                    /////Store changes (NOT ADD FINISH DATE)
                    #region Store changes (NOT ADD FINISH DATE)
                    var listChanges = entityChangeProps.Where(propChange => propChange.PropertyName != "FinishDate").Select(prop =>
                    {
                        PropertiesChangeDto newObj = new PropertiesChangeDto
                        {
                            PropertyName = prop.PropertyName,
                            OriginalValue = prop.OriginalValue?.Replace("\"", ""),
                            NewValue = prop.NewValue?.Replace("\"", ""),
                        };
                        return changeIdToName(newObj);
                    }).ToList();
                    var json = JsonSerializer.Serialize<List<PropertiesChangeDto>>(listChanges);
                    auditInfo.SetProperty("ListChange", json);
                    #endregion

                    //return issueId of this function
                    issueId_returned = issue.Id.ToString();
                }
                #endregion PUT/DELETE CASE
            });
            //return issueId of this function
            return issueId_returned.ToString();
        }

        protected virtual async Task SaveLogAsync(AuditLogInfo auditInfo)
        {
            string auditId = String.Empty;
            string issue_id = String.Empty;
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                var userId = _tmtCurrentUser.Id;
                if (userId != null)
                {
                    if (!Regex.IsMatch(auditInfo.HttpStatusCode.ToString(), @"40\d") && !Regex.IsMatch(auditInfo.HttpStatusCode.ToString(), @"50\d"))
                    {
                        try
                        {
                            var user = await _userRepository.GetAsync(userId);
                            try
                            {
                                issue_id = setExtraProperties(auditInfo);
                            }
                            catch { }
                            var audit = new AuditLog(GuidGenerator, auditInfo, user);
                            auditId = audit.Id.ToString();

                            await AuditLogRepository.InsertAsync(audit);
                        }
                        catch { }
                        await uow.CompleteAsync();
                    }
                }
            }
            if (issue_id != String.Empty)
            {
                await _localEventBus.PublishAsync(new AuditLogEventDto
                {
                    IssueId = issue_id,
                    AuditLogId = auditId
                });
            }

        }
    }
}