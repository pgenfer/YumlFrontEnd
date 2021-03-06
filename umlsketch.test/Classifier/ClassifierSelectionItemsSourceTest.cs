﻿using NUnit.Framework;
using System.Linq;
using UmlSketch.DomainObject;
using UmlSketch.Editor;
using UmlSketch.Event;
using static NSubstitute.Substitute;

namespace UmlSketch.Test
{
    public class ClassifierSelectionItemsSourceTest : TestBase
    {
        private MessageSystem _messageSystem;

        protected override void Init()
        {
            _messageSystem = For<MessageSystem>();
        }

        [TestDescription("Add new classifier to empty classifier selection list")]
        public void EmptyList_AddClassifier()
        {
            // Arrange
            var classifier = For<Classifier>();
            var classifierSelectionSource = new ClassifierSelectionItemsSource();
            // Act
            classifierSelectionSource.OnNewClassifierCreated(new DomainObjectCreatedEvent<Classifier>(classifier));
            // Assert => Item was added
            Assert.IsNotEmpty(classifierSelectionSource);
        }

        [TestDescription("Rename an item to a new name")]
        public void OnNameChangedTest()
        {
            // Arrange
            var classifier3 = new Classifier(3.ToString());
            var classifier2 = new Classifier(2.ToString());
            var classifiers = new ClassifierDictionary(classifier3, classifier2);
            var classifierSelectionSource = new ClassifierSelectionItemsSource(classifiers,_ => true,_messageSystem);
            // Act
            classifierSelectionSource.OnNameChanged(new NameChangedEvent(3.ToString(), 1.ToString()));
            // Assert: after renaming, item should be at first position
            Assert.IsTrue(classifierSelectionSource.First().Name == 1.ToString());
        }
    }
}