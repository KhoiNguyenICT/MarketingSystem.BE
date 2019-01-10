using MarketingSystem.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarketingSystem.Core.Resolvers
{
    public class CoreContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly string _hostPrefixUrl;

        public CoreContractResolver(string hostPrefixUrl)
        {
            _hostPrefixUrl = hostPrefixUrl;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            foreach (var prop in props)
            {
                if (prop.PropertyType != typeof(string) && !typeof(IEnumerable<string>).IsAssignableFrom(prop.PropertyType)) continue;
                var pi = type.GetProperty(prop.UnderlyingName);
                if (pi?.GetCustomAttribute(typeof(ResolveAssetUrlAttribute), true) != null)
                {
                    prop.ValueProvider = new ResolvedUrlValueProvider(prop.ValueProvider, pi, _hostPrefixUrl);
                }
            }

            return props;
        }

        private class ResolvedUrlValueProvider : IValueProvider
        {
            private readonly PropertyInfo _targetProperty;
            private readonly IValueProvider _baseProvider;
            private readonly string _prefix;

            public ResolvedUrlValueProvider(IValueProvider baseProvider, PropertyInfo targetProperty, string prefix)
            {
                _targetProperty = targetProperty;
                _prefix = prefix;
                _baseProvider = baseProvider;
            }

            public object GetValue(object target)
            {
                var value = _targetProperty.GetValue(target);
                switch (value)
                {
                    case null:
                        return _baseProvider.GetValue(target);
                    case string _:
                        return AddPrefix(value as string);
                }
                return ((IEnumerable<string>)value).Select(AddPrefix);
            }

            public void SetValue(object target, object value)
            {
                switch (value)
                {
                    case null:
                        _baseProvider.SetValue(target, null);
                        break;
                    case string _:
                        _targetProperty.SetValue(target, RemovePrefix((string)value));
                        break;
                    case IList<string> _:
                        _targetProperty.SetValue(target, ((IEnumerable<string>)value).Select(RemovePrefix).ToList());
                        break;
                    default:
                        _targetProperty.SetValue(target, ((IEnumerable<string>)value).Select(RemovePrefix).ToArray());
                        break;
                }
            }

            private string AddPrefix(string value)
            {
                if (string.IsNullOrWhiteSpace(value) || value == "/")
                    return value;
                return $"{_prefix}/{string.Join("/", value.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries))}";
            }

            private string RemovePrefix(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return value;
                return value.Replace($"{_prefix}/", "");
            }
        }
    }
}