﻿using NUnit.Framework;
using Yuml.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Yuml.Service;
using static NSubstitute.Substitute;

namespace Yuml.Test
{
    
    public class MakeClassifierToInterfaceCommandTest : TestBase
    {
        private MessageSystem _messageSystem;
        private readonly Classifier _classifier = new Classifier("TestClass");
        private IRelationService _relationService;

        protected override void Init()
        {
            _messageSystem = For<MessageSystem>();
            _relationService = For<IRelationService>();
        }

        [TestDescription("Check that correct event is fired when changing interface to class")]
        public void ToggleInterfaceFlagTest_FromInterfaceToClass()
        {
            _classifier.IsInterface = true;
            var command = new MakeClassifierToInterfaceCommand(_classifier, _relationService);
            command.ToggleInterfaceFlag();
            _messageSystem.Received().Publish(
                 _classifier,
                 Arg.Any<InterfaceToClassEvent>());
        }

        [TestDescription("Check that correct event is fired when changing class to interface")]
        public void ToggleInterfaceFlagTest_FromClassToInterface()
        {
            _classifier.IsInterface = false;
            var command = new MakeClassifierToInterfaceCommand(_classifier, _relationService);
            command.ToggleInterfaceFlag();
            _messageSystem.Received().Publish(
                 _classifier,
                 Arg.Any<ClassToInterfaceEvent>());
        }
    }
}