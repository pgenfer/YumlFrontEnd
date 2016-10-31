﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Yuml;
using Yuml.Command;
using YumlFrontEnd.editor.ViewModel;


namespace YumlFrontEnd.editor
{
    /// <summary>
    /// base class for view models that represent a list of items.
    /// </summary>
    /// <typeparam name="TDomain">Type of domain objects within the list</typeparam>
    internal class ListViewModelBase<TDomain> : PropertyChangedBase
    {
        private ViewModelFactory<TDomain> _viewModelFactory;
        /// <summary>
        /// every list view model has an expandable button for
        /// the child items. An additional flag controls
        /// whether this button should be visible or not
        /// </summary>
        private readonly ExpandableMixin _expandable = new ExpandableMixin();

        private readonly ChangeVisibilityMixin _visibility;
        
        /// <summary>
        /// sub items in this list
        /// </summary>
        private readonly BindableCollectionMixin<SingleItemViewModelBase<TDomain>> _items =
           new BindableCollectionMixin<SingleItemViewModelBase<TDomain>>();
    
        protected readonly IListCommandContext<TDomain> _commands;

        public BindableCollection<SingleItemViewModelBase<TDomain>> Items => _items.Items;

        public void New()
        {
            // execute the command
            _commands.New.CreateNew();
            // update the list of items to reflect the changes
            UpdateItemList();
        }

        /// <summary>
        /// updates the list of available items by calling the query which is part of the
        /// command context
        /// </summary>
        protected virtual void UpdateItemList()
        {
            Items.Clear();
           
            // create single view models for every domain object
            foreach (var domainObject in _commands.All.Get())
            {
                // command context (as base interface, must be cast to concrete type)
                var singleCommandContextInstance = _commands.GetCommandsForSingleItem(domainObject);
                // use constructor to create the item and cast it to the correct type
                var singleViewModel = _viewModelFactory.CreateViewModelForSingleItem(singleCommandContextInstance);
                singleViewModel.Init(domainObject, _viewModelFactory,this);
                Items.Add(singleViewModel);
            }

            // update the expand flag every time the list changes
            NotifyOfPropertyChange(nameof(CanExpand));
        }

        protected ListViewModelBase(IListCommandContext<TDomain> commands)
        {
            _commands = commands;
            _visibility = new ChangeVisibilityMixin(commands.Visibility);
        }

        /// <summary>
        /// initializes this view model. Will be called by the factory 
        /// that is responsible for creating this object.
        /// </summary>
        /// <param name="factory">factory that was used to create this object.
        /// Can be used to create further viewmodels</param>
        internal void Init(ViewModelFactory<TDomain> factory)
        {
            _viewModelFactory = factory;
            _expandable.PropertyChanged += (_, e) => NotifyOfPropertyChange(e.PropertyName);
            UpdateItemList();
        }

        public bool IsExpanded => _expandable.IsExpanded;
        public void ExpandOrCollapse() => _expandable.ExpandOrCollapse();
        /// <summary>
        /// controls whether the expand button is visible
        /// </summary>
        public virtual bool CanExpand => Items.Any();
        public void RemoveItem(SingleItemViewModelBase<TDomain> item) => _items.RemoveItem(item);

        public bool IsVisible => _visibility.IsVisible;

        public void ShowOrHide()
        {
            _visibility.ShowOrHide();
            NotifyOfPropertyChange(nameof(IsVisible));
        }
    }
}