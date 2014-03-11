﻿using Nemo.Attributes;
using Nemo.Id;
using Nemo.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nemo.Configuration.Mapping
{
    public class PropertyMap<T, U> : IPropertyMap
                   where T : class, IDataEntity
    {
        private readonly ReflectedProperty _property;
        private readonly Expression<Func<T, U>> _selector;
        private bool _not;

        internal PropertyMap(Expression<Func<T, U>> selector, bool not = false)
        {
            _selector = selector;
            _not = not;

            var property = (PropertyInfo)((MemberExpression)selector.Body).Member;
            _property = new ReflectedProperty(property, readAttributes: false);
        }

        public PropertyMap<T, U> Not
        {
            get
            {
                return new PropertyMap<T, U>(_selector, true);
            }
        }

        public PropertyMap<T, U> PrimaryKey(int position = 0)
        {
            _property.IsPrimaryKey = _not ? false : true;
            _property.KeyPosition = position;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> CacheKey()
        {
            _property.IsCacheKey = _not ? false : true;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> CacheParameter()
        {
            _property.IsCacheParameter = _not ? false : true;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Generated(Type generator = null)
        {
            if (generator != null && typeof(IIdGenerator).IsAssignableFrom(generator))
            {
                _property.Generator = generator;
                _property.IsAutoGenerated = false;
            }
            else
            {
                _property.Generator = null;
                _property.IsAutoGenerated = _not ? false : true;
            }
            _not = false;
            return this;
        }

        public PropertyMap<T, U> References<V>(int position = 0)
            where V : class, IDataEntity
        {
            _property.Parent = typeof(V);
            _property.RefPosition = position;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Parameter(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            _property.ParameterName = name;
            _property.Direction = direction;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Persistent()
        {
            _property.IsPersistent = _not ? false : true;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Selectable()
        {
            _property.IsSelectable = _not ? false : true;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Serializable()
        {
            _property.IsSerializable = _not ? false : true;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Sorted<V>()
            where V : class, IComparer
        {
            if (_property.IsListInterface)
            {
                _property.Sorted = new SortedAttribute { ComparerType = typeof(V) };
            }
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Sorted()
        {
            if (_property.IsListInterface)
            {
                _property.Sorted = new SortedAttribute();
            }
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Distinct<V>()
        {
            if (_property.IsListInterface && typeof(IEqualityComparer<>).MakeGenericType(_property.ElementType).IsAssignableFrom(typeof(V)))
            {
                _property.Distinct = new DistinctAttribute { EqualityComparerType = typeof(V) };
            }
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Distinct()
        {
            if (_property.IsListInterface)
            {
                _property.Distinct = new DistinctAttribute();
            }
            _not = false;
            return this;
        }

        public PropertyMap<T, U> Column(string name)
        {
            _property.MappedColumnName = name;
            _not = false;
            return this;
        }

        public PropertyMap<T, U> SourceProperty(string name)
        {
            _property.MappedPropertyName = name;
            _not = false;
            return this;
        }

        ReflectedProperty IPropertyMap.Property
        {
            get
            {
                return _property;
            }
        }
    }
}
