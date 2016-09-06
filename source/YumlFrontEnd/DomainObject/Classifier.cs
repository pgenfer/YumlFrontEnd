﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Diagnostics.Contracts.Contract;

namespace Yuml
{
    /// <summary>
    /// Generally, you can think of a classifier as a class, 
    /// but technically a classifier is a more general term that 
    /// refers to the other three types above as well.
    /// See documentation here:
    /// https://www.ibm.com/developerworks/rational/library/content/RationalEdge/sep04/bell/
    /// </summary>
    public class Classifier : IVisible, INamed
    {
        private readonly NameMixin _name = new NameMixin();
        private readonly IVisible _visible = new VisibleMixin();

        /// <summary>
        /// only for testing, don't use in production
        /// </summary>
        public Classifier() { }

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
            set { _name.Name = value; }
        }

        /// <summary>
        /// internal setter is only used by serialization, should not be used by other production code
        /// </summary>
        public PropertyList Properties { get; internal set; } = new PropertyList();

        /// <summary>
        /// internal setter only used by serialization, should not be used by other production code
        /// </summary>
        public MethodList Methods { get; internal set; } = new MethodList();

        public override string ToString() => _name.ToString();

        /// <summary>
        /// true if the class is an interface, otherwise false
        /// </summary>
        public bool IsInterface { get; internal set; }
        public void ShowOrHideAllProperties(bool showOrHide) => Properties.ShowOrHideAllProperties(showOrHide);
        public Property CreateProperty(string name, Classifier type) => Properties.CreateProperty(name, type);
        public IEnumerator<Property> GetEnumerator() => Properties.GetEnumerator();

        public ClassWriter WriteTo(ClassWriter classWriter)
        {
            Requires(classWriter != null);

            classWriter.WithName(Name);
            Properties.WriteTo(classWriter);
            return classWriter;
        }

        public Method CreateMethod(string name, Classifier type) => Methods.CreateMethod(name, type);
        public Property CreateNewPropertyWithBestInitialValues(ClassifierDictionary systemClassifiers) => 
            Properties.CreateNewPropertyWithBestInitialValues(systemClassifiers);

        /// <summary>
        /// optional base class of this classifier
        /// </summary>
        public Classifier BaseClass { get; set; }
    }
}
