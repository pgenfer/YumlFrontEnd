﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Diagnostics.Contracts.Contract;

namespace Yuml
{
    /// <summary>
    /// list of interfaces that are implemented by a classifier
    /// </summary>
    public class ImplementationList : BaseList<Implementation>
    {
        public Classifier Root { get; set; }

        /// <summary>
        /// returns all interfaces that are implemented.
        /// </summary>
        public IEnumerable<Classifier> ImplementedInterfaces => _list.Select(x => x.End.Classifier);


        /// <summary>
        /// adds a new implementation entry to the list. This entry will be a prototype
        /// where the end node can be changed by the user later.
        /// </summary>
        /// <param name="classifiers">classifier list used to obtain available interfaces</param>
        /// <returns></returns>
        public Implementation AddNewImplementation(ClassifierDictionary classifiers)
        {
            Requires(Root != null);

            Implementation newImplementation = null;
            // all interfaces except
            // 1. the ones which are already in the interface list
            // 2. the owner of the interfaces itself
            var availableInterfaces = classifiers
                .Where(x => x.IsInterface && x != Root)
                .Except(ImplementedInterfaces);
            var firstInterface = availableInterfaces.FirstOrDefault();
            // TODO: throw error if there is no interface anymore that we can add?
            if (firstInterface != null)
            {
                newImplementation = new Implementation(Root, firstInterface);
                AddExistingMember(newImplementation);
            }
            return newImplementation;
        }

        public void ReplaceInterface(Implementation implementation, Classifier newInterface)
        {
            implementation.End.Classifier = newInterface;
        }

        public void RemoveInterfaceFromList(Implementation implementation)
        {
            Requires(implementation != null);
            Requires(this.Contains(implementation));

            _list.Remove(implementation);
        }

        public void AddInterfaceToList(Implementation implementation)
        {
            Requires(implementation != null);
            Requires(!this.Contains(implementation));

            AddExistingMember(implementation);
        }

        public SubSet FindImplementationsOfInterface(Classifier @interface) =>
            @interface.IsInterface ? Filter(x => x.End.Classifier == @interface) : SubSet.Empty;

        internal void RemoveImplementationForInterface(Classifier @interface, MessageSystem messageSystem = null)
        {
            Requires(@interface != null);

            var implementation = _list.Single(x => x.End.Classifier == @interface);
            RemoveInterfaceFromList(implementation);
            messageSystem?.PublishDeleted(implementation);
        }
    }
}