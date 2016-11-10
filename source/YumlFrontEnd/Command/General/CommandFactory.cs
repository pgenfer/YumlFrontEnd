﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Yuml.DomainObject;
using Yuml.Service;

namespace Yuml.Command
{
    /// <summary>
    /// command factory is used to retrieve
    /// the available commands for a given domain object
    /// </summary>
    public class CommandFactory
    {
        /// <summary>
        /// stores all factory functions for creating list commands. The functions are
        /// stored in an untyped way but will be cast if necessary
        /// </summary>
        private readonly Dictionary<Type,Delegate> _listCommandCreatorFunctions = 
            new Dictionary<Type, Delegate>();
        /// <summary>
        /// dictionary that stores functions for creating commands for single domain objects.
        /// </summary>
        private readonly Dictionary<Type, Delegate> _singleCommandCreatorFunctions =
            new Dictionary<Type, Delegate>();

        public CommandFactory(ClassifierDictionary classifiers,
            DeletionService deletionService,
            IRelationService relationService,
            IValidateNameService propertyValidationService,
            IMethodNameValidationService methodValidationService,
            MessageSystem messageSystem)
        {
            // classifier list
            RegisterFactoryFuncForListCommands<Classifier>(
                _ => new ClassifierListCommandContext(classifiers, messageSystem));
            // property list
            RegisterFactoryFuncForListCommands<Property>(
                x => new PropertyListCommandContext((PropertyList)x, classifiers, messageSystem));
            // method list
            RegisterFactoryFuncForListCommands<Method>(
                x => new MethodListCommandContext((MethodList)x,classifiers,messageSystem));
            // associations
            RegisterFactoryFuncForListCommands<Relation>(
                x => new AssociationListCommandContext((ClassifierAssociationList)x,classifiers,messageSystem));
            // implementations
            RegisterFactoryFuncForListCommands<Implementation>(
                x => new InterfaceListCommandContext((ImplementationList)x,classifiers,messageSystem));
            // TODO: parameters

            // classifier
            RegisterFactoryFuncForSingleCommands<Classifier>(
                x => new ClassifierSingleCommandContext(x,classifiers,deletionService,relationService,messageSystem));
            // property
            RegisterFactoryFuncForSingleCommands<Property>(
                x => new PropertySingleCommandContext(x,classifiers,propertyValidationService,messageSystem));
            // method
            RegisterFactoryFuncForSingleCommands<Method>(
                x => new MethodSingleCommandContext(x,methodValidationService,messageSystem));
            // association
            RegisterFactoryFuncForSingleCommands<Relation>(
                x => new SingleAssociationCommands(x,classifiers,messageSystem));
            // implementation
            RegisterFactoryFuncForSingleCommands<Implementation>(
                x => new SingleInterfaceCommandContext(x,classifiers,messageSystem));
            // TODO: parameter
        }

        /// <summary>
        /// registration method to register a factory function for a specific type.
        /// </summary>
        /// <typeparam name="TDomain">type of the domain</typeparam>
        /// <param name="factoryFunc">function to create the list commands for a given domain list</param>
        private void RegisterFactoryFuncForListCommands<TDomain>(
            Func<BaseList<TDomain>,IListCommandContext<TDomain>> factoryFunc)
            where TDomain : IVisible
        {
            _listCommandCreatorFunctions[typeof(TDomain)] = factoryFunc;
        }

        /// <summary>
        /// register a factory function for creating commands for a single domain object
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="factoryFunc"></param>
        private void RegisterFactoryFuncForSingleCommands<TDomain>(
            Func<TDomain, ISingleCommandContext<TDomain>> factoryFunc)
            where TDomain : class
        {
            _singleCommandCreatorFunctions[typeof(TDomain)] = factoryFunc;
        }

        public IListCommandContext<TDomain> GetListCommands<TDomain>(BaseList<TDomain> list) 
            where TDomain : IVisible
        {
            Delegate commandFunc;
            if (_listCommandCreatorFunctions.TryGetValue(typeof(TDomain), out commandFunc))
                return ((Func<BaseList<TDomain>, IListCommandContext<TDomain>>)commandFunc)(list);
            throw new NotImplementedException(
                $"Don't know how to create commands for a list of type {typeof(TDomain).Name}");
        }

        public ISingleCommandContext<TDomain> GetSingleCommands<TDomain>(TDomain domainObject)
        {
            Delegate commandFunc;
            if (_singleCommandCreatorFunctions.TryGetValue(typeof(TDomain), out commandFunc))
                return ((Func<TDomain, ISingleCommandContext<TDomain>>)commandFunc)(domainObject);
            throw new NotImplementedException(
                $"Don't know how to create commands for a single instance of type {typeof(TDomain).Name}");
        }
    }
}