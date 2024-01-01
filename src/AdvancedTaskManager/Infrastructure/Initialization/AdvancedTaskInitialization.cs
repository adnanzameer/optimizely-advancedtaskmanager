using System;
using System.Linq;
using AdvancedTaskManager.Infrastructure.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.Options;

namespace AdvancedTaskManager.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class AdvancedTaskInitialization : IInitializableModule
    {
        private const string ContentApprovalDeadlinePropertyName = "ATM_ContentApprovalDeadline";

        private static readonly object Lock = new();
        private IContentTypeRepository _contentTypeRepository;
        private ITabDefinitionRepository _tabDefinitionRepository;
        private IPropertyDefinitionRepository _propertyDefinitionRepository;
        private IPropertyDefinitionTypeRepository _propertyDefinitionTypeRepository;

        public void Initialize(InitializationEngine context)
        {
            _contentTypeRepository = context.Locate.Advanced.GetInstance<IContentTypeRepository>();
            _tabDefinitionRepository = context.Locate.Advanced.GetInstance<ITabDefinitionRepository>();
            _propertyDefinitionRepository = context.Locate.Advanced.GetInstance<IPropertyDefinitionRepository>();
            _propertyDefinitionTypeRepository = context.Locate.Advanced.GetInstance<IPropertyDefinitionTypeRepository>();
            var configuration = context.Locate.Advanced.GetInstance<IOptions<AdvancedTaskManagerOptions>>();

            var deleteContentApprovalDeadlineProperty = configuration.Value.DeleteContentApprovalDeadlineProperty;
            if (deleteContentApprovalDeadlineProperty)
            {
                DeleteMappingProperties();
            }
            else
            {
                var addContentApprovalDeadlineProperty = configuration.Value.AddContentApprovalDeadlineProperty;

                SetupMappingProperties(addContentApprovalDeadlineProperty);
            }
        }

        private void SetupMappingProperties(bool addContentApprovalDeadlineProperty)
        {
            foreach (var contentType in _contentTypeRepository.List().Where(x => x.IsAvailable))
                CreateUpdatePropertyDefinition(
                    contentType,
                    ContentApprovalDeadlinePropertyName,
                    "Content approval deadline property for Advanced Task Manager",
                    "Content approval deadline",
                    typeof(PropertyDate),
                    SystemTabNames.Settings, 100,
                    addContentApprovalDeadlineProperty);
        }

        private void DeleteMappingProperties()
        {
            foreach (var contentType in _contentTypeRepository.List().Where(x => x.IsAvailable))
                DeletePropertyDefinition(
                    contentType,
                    ContentApprovalDeadlinePropertyName);
        }
        private void CreateUpdatePropertyDefinition(ContentType contentType, string propertyDefinitionName, string helperText, string editCaption, Type propertyDefinitionType, string tabName, int? propertyOrder, bool addContentApprovalDeadlineProperty)
        {
            var propertyDefinition = GetPropertyDefinition(contentType, propertyDefinitionName);

            if (propertyDefinition == null)
            {
                if (propertyDefinitionType == null)
                    return;

                if (!addContentApprovalDeadlineProperty)
                    return;

                propertyDefinition = new PropertyDefinition();
            }
            else
            {
                propertyDefinition = propertyDefinition.CreateWritableClone();
            }

            propertyDefinition.ContentTypeID = contentType.ID;
            propertyDefinition.DisplayEditUI = true;
            propertyDefinition.DefaultValueType = DefaultValueType.None;

            propertyDefinition.DisplayEditUI = addContentApprovalDeadlineProperty;

            if (!string.IsNullOrEmpty(propertyDefinitionName))
                propertyDefinition.Name = propertyDefinitionName;

            if (!string.IsNullOrEmpty(editCaption))
                propertyDefinition.EditCaption = editCaption;

            if (!string.IsNullOrEmpty(helperText))
                propertyDefinition.HelpText = helperText;

            if (propertyDefinitionType != null)
                propertyDefinition.Type = _propertyDefinitionTypeRepository.Load(propertyDefinitionType);

            if (!string.IsNullOrEmpty(tabName))
            {
                var obj2 = Lock;
                lock (obj2)
                {
                    propertyDefinition.Tab = _tabDefinitionRepository.Load(tabName);
                }
            }

            if (propertyOrder.HasValue)
                propertyDefinition.FieldOrder = propertyOrder.Value;

            _propertyDefinitionRepository.Save(propertyDefinition);
        }

        private void DeletePropertyDefinition(ContentType contentType, string propertyDefinitionName)
        {
            var propertyDefinition = GetPropertyDefinition(contentType, propertyDefinitionName);

            if (propertyDefinition != null) _propertyDefinitionRepository.Delete(propertyDefinition);
        }

        private static PropertyDefinition GetPropertyDefinition(ContentType contentType, string propertyName, Type propertyDefinitionType = null)
        {
            var source = from pd in contentType.PropertyDefinitions
                         where pd.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                         select pd;
            if (propertyDefinitionType != null)
                source = from pd in source
                         where pd.Type.DefinitionType == propertyDefinitionType
                         select pd;
            return source.FirstOrDefault();
        }


        public void Uninitialize(InitializationEngine context)
        {
            //Required
        }
    }
}