using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedTaskManager.Infrastructure.Mapper
{
    public class ViewModelMapper
    {
        private readonly IDictionary<Type, Mapping> _typeMappings = new Dictionary<Type, Mapping>();

        public ViewModelMapper Add<TSource, TModel>()
          where TSource : class
          where TModel : class, new()
        {
            return Add((Action<TSource, TModel>)null);
        }

        private ViewModelMapper Add<TSource, TModel>(Action<TSource, TModel> afterMapAction) where TSource : class where TModel : class, new()
        {
            _typeMappings.Add(typeof(TSource), new Mapping<TSource, TModel>()
            {
                AfterMapAction = afterMapAction
            });
            return this;
        }

        public object Map<TSource>(TSource source)
        {
            var type = source.GetType();
            var mapping = GetMapping(type);
            if (mapping == null)
                return null;

            var instance = mapping.CreateInstance();
            var dictionary = instance.GetType().GetProperties().ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var property in type.GetProperties())
                if (dictionary.TryGetValue(property.Name, out var propertyInfo) && propertyInfo.CanWrite && property.PropertyType == propertyInfo.PropertyType)
                    propertyInfo.SetValue(instance, property.GetValue(source));
            mapping.ExecuteAfterMappingAction(source, instance);
            return instance;
        }

        private Mapping GetMapping(Type sourceType)
        {
            return !_typeMappings.TryGetValue(sourceType, out var mapping) ? null : mapping;
        }

        private abstract class Mapping
        {
            public abstract object CreateInstance();

            public abstract void ExecuteAfterMappingAction(object source, object model);
        }

        private class Mapping<TSource, TModel> : Mapping
          where TSource : class
          where TModel : class, new()
        {
            public Action<TSource, TModel> AfterMapAction { get; set; }

            public override object CreateInstance()
            {
                return new TModel();
            }

            public override void ExecuteAfterMappingAction(object source, object model)
            {
                AfterMapAction?.Invoke((TSource)source, (TModel)model);
            }
        }
    }
}