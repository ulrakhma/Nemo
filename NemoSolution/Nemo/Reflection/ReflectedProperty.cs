﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Nemo.Attributes;
using Nemo.Fn;

namespace Nemo.Reflection
{
    internal class ReflectedProperty
    {
        internal ReflectedProperty(PropertyInfo property, int position = -1)
        {
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
            IsPersistent = Maybe<bool>.Empty;
            IsSelectable = Maybe<bool>.Empty;
            IsSimpleList = Reflector.IsSimpleList(property.PropertyType);
            IsBusinessObject = Reflector.IsBusinessObject(property.PropertyType);
            Type elementType;
            IsBusinessObjectList = Reflector.IsBusinessObjectList(property.PropertyType, out elementType);
            ElementType = elementType;
            if (IsBusinessObjectList)
            {
                IsList = true;
                IsListInterface = property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
            }
            else
            {
                IsList = Reflector.IsList(property.PropertyType);
                if (IsList)
                {
                    ElementType = Reflector.ExtractCollectionElementType(property.PropertyType);
                    IsListInterface = property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
                }
            }
            IsSimpleType = Reflector.IsSimpleType(property.PropertyType);
            IsTypeUnion = Reflector.IsTypeUnion(property.PropertyType);
            IsTuple = Reflector.IsTuple(property.PropertyType);
            IsNullableType = Reflector.IsNullableType(property.PropertyType);
            MappedColumnName = MapColumnAttribute.GetMappedColumnName(property);
            MappedPropertyName = MapPropertyAttribute.GetMappedPropertyName(property);
            CanWrite = property.CanWrite;
            CanRead = property.CanRead;
            Position = position;

            if (IsListInterface)
            {
                Sorted = property.GetCustomAttributes(typeof(SortedAttribute), false).Cast<SortedAttribute>().FirstOrDefault();
                Distinct = property.GetCustomAttributes(typeof(DistinctAttribute), false).Cast<DistinctAttribute>().FirstOrDefault();
            }

            var items = property.GetCustomAttributes(true).OfType<PropertyAttribute>();
            foreach(var item in items)
            {
                if (item is PrimaryKeyAttribute)
                {
                    IsPrimaryKey = true;
                }
                else if (item is Generate.UsingAttribute)
                {
                    Generator = ((Generate.UsingAttribute)item).Type;
                }
                else if (item is Generate.NativeAttribute)
                {
                    IsAutoGenerated = true;
                }
                else if (item is ReferencesAttribute)
                {
                    Parent = ((ReferencesAttribute)item).Parent;
                }
                else if (item is CacheKeyAttribute)
                {
                    IsCacheKey = true;
                }
                else if (item is ParameterAttribute)
                {
                    ParameterName = ((ParameterAttribute)item).Name;
                    Direction = ((ParameterAttribute)item).Direction;
                }
                else if (item is PersistentAttribute)
                {
                    IsPersistent = ((PersistentAttribute)item).Value;
                }
                else if (item is SelectableAttribute)
                {
                    IsSelectable = ((SelectableAttribute)item).Value;
                }
            }

            if (!IsPersistent.HasValue)
            {
                IsPersistent = true;
            }

            if (!IsSelectable.HasValue)
            {
                IsSelectable = true;
            }
        }

        public bool IsSimpleList
        {
            get;
            private set;
        }

        public bool IsBusinessObjectList
        {
            get;
            private set;
        }

        public bool IsListInterface
        {
            get;
            private set;
        }

        public bool IsBusinessObject
        {
            get;
            private set;
        }

        public bool IsSimpleType
        {
            get;
            private set;
        }

        public bool IsTypeUnion
        {
            get;
            private set;
        }

        public bool IsTuple
        {
            get;
            private set;
        }

        public Maybe<bool> IsPersistent
        {
            get;
            private set;
        }

        public bool IsPrimaryKey
        {
            get;
            private set;
        }

        public bool IsAutoGenerated
        {
            get;
            private set;
        }

        public Type Generator
        {
            get;
            private set;
        }

        public Type Parent
        {
            get;
            private set;
        }

        public Maybe<bool> IsSelectable
        {
            get;
            private set;
        }
        
        public bool IsCacheKey
        {
            get;
            private set;
        }

        public string ParameterName
        {
            get;
            private set;
        }

        public ParameterDirection Direction
        {
            get;
            private set;
        }

        public string PropertyName
        {
            get;
            private set;
        }

        public Type PropertyType
        {
            get;
            private set;
        }

        public string MappedColumnName
        {
            get;
            private set;
        }

        public string MappedPropertyName
        {
            get;
            private set;
        }

        public bool CanWrite
        {
            get;
            private set;
        }

        public bool CanRead
        {
            get;
            private set;
        }

        public Type ElementType
        {
            get;
            private set;
        }

        public bool IsNullableType
        {
            get;
            private set;
        }

        public bool IsList
        {
            get;
            private set;
        }

        public SortedAttribute Sorted
        {
            get;
            private set;
        }

        public DistinctAttribute Distinct
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }
    }
}