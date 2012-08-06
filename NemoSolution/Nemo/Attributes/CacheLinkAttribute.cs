﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nemo.Caching
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CacheLinkAttribute : Attribute
    {
        public CacheLinkAttribute(Type type) : base() 
        {
            DependentType = type;
        }

        public Type DependentType { get; private set; }
        public string DependentParameter { get; set; }
        public string ValueProperty { get; set; }
    }
}