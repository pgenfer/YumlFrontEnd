﻿namespace UmlSketch.Event
{
    /// <summary>
    /// event is fired when the user changes one interface in the list of interfaces
    /// </summary>
    public class ChangeInterfaceOfClassEvent : IDomainEvent
    {
        public ChangeInterfaceOfClassEvent(string oldInterfaceName, string newInterfaceName)
        {
            OldInterfaceName = oldInterfaceName;
            NewInterfaceName = newInterfaceName;
        }

        public string OldInterfaceName { get; }
        public string NewInterfaceName { get; }
    }
}
