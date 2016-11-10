﻿using NUnit.Framework;
using Yuml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuml.Test
{
    public class ImplementationListTest : TestBase
    {
        private ImplementationList _implementationList;
        private readonly Classifier _classifier = new Classifier("class");
        private readonly Classifier _interface = new Classifier("interface") {IsInterface = true};

        protected override void Init()
        {
            _implementationList = new ImplementationList();
        }


        [TestDescription(
            "Ensure that the implementation list of a class is not empty if there is an interface implementation")]
        public void ImplementationList_WithInterface_NotEmpty()
        {
            _implementationList.AddInterfaceToList(new Implementation(_classifier, _interface));
            var implementations = _implementationList.FindImplementationsOfInterface(_interface);

            Assert.IsTrue(implementations.IsNotEmpty());
        }
    }
}