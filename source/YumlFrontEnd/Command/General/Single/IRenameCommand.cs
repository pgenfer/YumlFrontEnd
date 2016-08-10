﻿using System.Diagnostics.Contracts;
using static System.Diagnostics.Contracts.Contract;

namespace Yuml.Command
{
    /// <summary>
    /// generic command used to rename a domain object.
    /// </summary>
    [ContractClass(typeof(RenameCommandContract))]
    public interface IRenameCommand
    {
        void Rename(string newName);
        ValidationResult CanRenameWith(string newName);
    }

    /// <summary>
    /// contract definition for rename command
    /// </summary>
    [ContractClassFor(typeof(IRenameCommand))]
    internal abstract class RenameCommandContract : IRenameCommand
    {
        /// <summary>
        /// rename can only be executed when the new name is set
        /// </summary>
        /// <param name="newName"></param>
        void IRenameCommand.Rename(string newName) => Requires(!string.IsNullOrEmpty(newName));
        ValidationResult IRenameCommand.CanRenameWith(string newName) => default(ValidationResult);
    }
}