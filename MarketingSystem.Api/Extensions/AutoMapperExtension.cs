using AutoMapper;
using System;

namespace MarketingSystem.Api.Extensions
{
    public static class AutoMapperExtension
    {
        private static IMapper _mapper;

        public static IMapper RegisterMap(this IMapper mapper)
        {
            _mapper = mapper;
            return mapper;
        }

        public static T To<T>(this object source)
        {
            return _mapper.Map<T>(source);
        }

        public static object To(this object source, Type destinationType)
        {
            return _mapper.Map(source, source.GetType(), destinationType);
        }

        public static T To<T>(this object source, Action<IMappingOperationOptions> opts)
        {
            return _mapper.Map<T>(source, opts);
        }

        public static T To<T>(this object source, T dest)
        {
            return _mapper.Map(source, dest);
        }

        private static void IgnoreUnmappedProperties(TypeMap map, IMappingExpression expr)
        {
            foreach (string propName in map.GetUnmappedPropertyNames())
            {
                if (map.SourceType.GetProperty(propName) != null)
                {
                    expr.ForSourceMember(propName, opt => opt.Ignore());
                }
                if (map.DestinationType.GetProperty(propName) != null)
                {
                    expr.ForMember(propName, opt => opt.Ignore());
                }
            }
        }

        public static void IgnoreUnmapped(this IProfileExpression profile)
        {
            profile.ForAllMaps(IgnoreUnmappedProperties);
        }

        public static void IgnoreUnmapped(this IProfileExpression profile, Func<TypeMap, bool> filter)
        {
            profile.ForAllMaps((map, expr) =>
            {
                if (filter(map))
                {
                    IgnoreUnmappedProperties(map, expr);
                }
            });
        }

        public static void IgnoreUnmapped(this IProfileExpression profile, Type src, Type dest)
        {
            profile.IgnoreUnmapped(map => map.SourceType == src && map.DestinationType == dest);
        }

        public static void IgnoreUnmapped<TSrc, TDest>(this IProfileExpression profile)
        {
            profile.IgnoreUnmapped(typeof(TSrc), typeof(TDest));
        }
    }
}