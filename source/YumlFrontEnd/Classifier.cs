﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuml
{
    /// <summary>
    /// Generally, you can think of a classifier as a class, 
    /// but technically a classifier is a more general term that 
    /// refers to the other three types above as well.
    /// See documentation here:
    /// https://www.ibm.com/developerworks/rational/library/content/RationalEdge/sep04/bell/
    /// </summary>
    public class Classifier : IVisible
    {
        private NameMixin _name = new NameMixin();
        private IVisible _visible = new VisibleMixin();
        private PropertyList _properties = new PropertyList();

        public Classifier(string name)
        {
            Name = name;
        }
       
        public bool IsVisible
        {
            get { return _visible.IsVisible; }

            set { _visible.IsVisible = value; }
        }
        public string Name
        {
            get { return _name.Name; }
            internal set { _name.Name = value; }
        }

        public PropertyList Properties { get { return _properties; } internal set { _properties = value; } }

        public override string ToString() => _name.ToString();

        /// <summary>
        /// true if the class is an interface, otherwise false
        /// </summary>
        public bool IsInterface { get; internal set; }
        public void ShowOrHideAllProperties(bool showOrHide) => _properties.ShowOrHideAllProperties(showOrHide);
        public Property CreateProperty(string name, Classifier type) => _properties.CreateProperty(name, type);
        public IEnumerator<Property> GetEnumerator() => _properties.GetEnumerator();
    }
}
