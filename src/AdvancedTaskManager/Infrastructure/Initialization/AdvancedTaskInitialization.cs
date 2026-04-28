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

        public void Initialize(InitializationEngine context)
        {
            var configuration = ServiceLocator.Current.GetInstance<IOptions<AdvancedTaskManagerOptions>>();
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
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            foreach (var contentType in contentTypeRepository.List().Where(x => x.IsAvailable))
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
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            foreach (var contentType in contentTypeRepository.List().Where(x => x.IsAvailable))
                DeletePropertyDefinition(contentType, ContentApprovalDeadlinePropertyName);
        }

        private void CreateUpdatePropertyDefinition(
            ContentType contentType,
            string propertyDefinitionName,
            string helperText,
            string editCaption,
            Type propertyDefinitionType,
            string tabName,
            int? propertyOrder,
            bool addContentApprovalDeadlineProperty)
        {
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var tabDefinitionRepository = ServiceLocator.Current.GetInstance<ITabDefinitionRepository>();
            var propertyDefinitionTypeRepository = ServiceLocator.Current.GetInstance<IPropertyDefinitionTypeRepository>();

            var writableContentType = contentType.CreateWritableClone() as ContentType;
            if (writableContentType == null) return;

            var existingPropDef = writableContentType.PropertyDefinitions
                .FirstOrDefault(x => x.Name.Equals(propertyDefinitionName, StringComparison.OrdinalIgnoreCase));

            if (existingPropDef == null)
            {
                if (!addContentApprovalDeadlineProperty) return;

                var newPropDef = new PropertyDefinition
                {
                    ContentTypeID = writableContentType.ID,
                    DisplayEditUI = true,
                    DefaultValueType = DefaultValueType.None,
                    Name = propertyDefinitionName,
                    EditCaption = editCaption,
                    HelpText = helperText,
                    Type = propertyDefinitionTypeRepository.Load(propertyDefinitionType)
                };

                if (!string.IsNullOrEmpty(tabName))
                {
                    lock (Lock)
                    {
                        newPropDef.Tab = tabDefinitionRepository.Load(tabName);
                    }
                }

                if (propertyOrder.HasValue)
                    newPropDef.FieldOrder = propertyOrder.Value;

                writableContentType.PropertyDefinitions.Add(newPropDef);
            }
            else
            {
                existingPropDef.DisplayEditUI = addContentApprovalDeadlineProperty;

                if (!string.IsNullOrEmpty(editCaption))
                    existingPropDef.EditCaption = editCaption;

                if (!string.IsNullOrEmpty(helperText))
                    existingPropDef.HelpText = helperText;

                if (propertyOrder.HasValue)
                    existingPropDef.FieldOrder = propertyOrder.Value;

                if (!string.IsNullOrEmpty(tabName))
                {
                    lock (Lock)
                    {
                        existingPropDef.Tab = tabDefinitionRepository.Load(tabName);
                    }
                }
            }

            contentTypeRepository.Save(writableContentType);
        }

        private void DeletePropertyDefinition(ContentType contentType, string propertyDefinitionName)
        {
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

            var writableContentType = contentType.CreateWritableClone() as ContentType;
            if (writableContentType == null) return;

            var propDef = writableContentType.PropertyDefinitions
                .FirstOrDefault(x => x.Name.Equals(propertyDefinitionName, StringComparison.OrdinalIgnoreCase));

            if (propDef != null)
            {
                writableContentType.PropertyDefinitions.Remove(propDef);
                contentTypeRepository.Save(writableContentType);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Required
        }
    }
}
