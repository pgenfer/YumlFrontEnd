﻿using UmlSketch.Command;
using UmlSketch.DomainObject;
using UmlSketch.Event;

namespace UmlSketch.Editor
{
    /// <summary>
    /// view model for handling a complete list of classifiers.
    /// A separate view model is created for every classifier
    /// </summary>
    internal class ClassifierListViewModel : ListViewModelBase<Classifier,ClassifierListCommandContext>
    {
        /// <summary>
        /// always hide the expand button in the class list
        /// TODO: later when we have relations, we could let the user expand/collapse the class list
        /// </summary>
        public override bool CanExpand => false;

        public void Collapse()
        {
            foreach (var item in Items)
                ((ClassifierViewModel) item).Collapse();
        }

        protected override SingleItemViewModelBaseSimple<Classifier> OnNewItemAdded(DomainObjectCreatedEvent<Classifier> itemCreated)
        {
            var newViewModel = base.OnNewItemAdded(itemCreated);
            ((ClassifierViewModel) newViewModel).IsExpanded = true;
            return newViewModel;
        }

        protected override void SubscribeToMessageSystem(BaseList<Classifier> domainList)
        {
            Context.MessageSystem.Subscribe(Context.Classifiers, this);
        }
    }
}
