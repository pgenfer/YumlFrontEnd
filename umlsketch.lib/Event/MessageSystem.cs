using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DomainEventHandlers = System.Collections.Generic.List<UmlSketch.Event.DomainEventHandler>;

namespace UmlSketch.Event
{
    /// <summary>
    /// stores information about a handler that reacts on a domain event
    /// </summary>
    public class DomainEventHandler
    {
        /// <summary>
        /// the instance that holds the event handler (used for unsubscribing)
        /// is optional and can be omitted.
        /// </summary>
        public object Subscriber { get; }
        /// <summary>
        /// the event handler which will be executed when the domain event raises
        /// </summary>
        public Action<IDomainEvent> EventHandler { get; }

        public DomainEventHandler(object subscriber, Action<IDomainEvent> eventHandler)
        {
            Subscriber = subscriber;
            EventHandler = eventHandler;
        }

    }

    /// <summary>
    /// message infrastructure based on publisher/subscriber pattern.
    /// </summary>
    public class MessageSystem
    {
        /// <summary>
        /// event is fired whenever a domain object
        /// publishes an domain event.
        /// </summary>
        public event Action<IDomainEvent> DomainModelChanged;
        
        /// <summary>
        /// The message system is organized in the following way:
        /// A dictionary contains all senders and for every sender
        /// the domain events that are fired by the sender. For
        /// these events, a list of event handlers is stored that reacts on this 
        /// specific domain event. 
        /// </summary>
        private readonly Dictionary<object, Dictionary<Type, DomainEventHandlers>> _eventHandlers =
            new Dictionary<object, Dictionary<Type, DomainEventHandlers>>();

        public virtual void Publish<T>(object sender, T domainEvent) where T : IDomainEvent
        {
            Dictionary<Type, DomainEventHandlers> handlersForSource;
            if (_eventHandlers.TryGetValue(sender, out handlersForSource))
            {
                DomainEventHandlers handlersForEvent;
                if (handlersForSource.TryGetValue(typeof(T), out handlersForEvent))
                {
                    // copy handlers to new list, because an event could add new handlers
                    // which would change our list
                    var tmp = handlersForEvent.ToList();
                    tmp.ForEach(x => x.EventHandler(domainEvent));
                }
            }
            // notify observers that the domain model was changed.
            DomainModelChanged?.Invoke(domainEvent);
        }

        /// <summary>
        /// subscrive a domain event handler.
        /// The handler reacts only for the specific event and only
        /// if it is raised by the given source.
        /// </summary>
        /// <typeparam name="T">type of the domain event to watch for.</typeparam>
        /// <param name="source">Source object that must fire the event</param>
        /// <param name="eventHandler">event handler that will be executed
        /// when the event is fired</param>
        /// <param name="subscriber">optional, the subscription object. Can be stored with the event handler</param>
        public void Subscribe<T>(object source, Action<T> eventHandler, object subscriber = null) where T : IDomainEvent =>
            Subscribe(source, typeof(T), x => eventHandler((T)x), subscriber);

        /// <summary>
        /// non generic version of subscription method.
        /// </summary>
        /// <param name="source">source object which should be observed</param>
        /// <param name="domainEventType">type of domain event</param>
        /// <param name="eventHandler">event handler which should be fired</param>
        /// <param name="subscriber"></param>
        public void Subscribe(
            object source,
            Type domainEventType,
            Action<IDomainEvent> eventHandler,
            object subscriber = null)
        {
            Dictionary<Type, DomainEventHandlers> handlersForSource;
            if (!_eventHandlers.TryGetValue(source, out handlersForSource))
            {
                handlersForSource = new Dictionary<Type, DomainEventHandlers>();
                _eventHandlers.Add(source, handlersForSource);
            }
            DomainEventHandlers handlersForEvent;
            if (!handlersForSource.TryGetValue(domainEventType, out handlersForEvent))
            {
                handlersForEvent = new DomainEventHandlers();
                handlersForSource.Add(domainEventType, handlersForEvent);
            }
            handlersForEvent.Add(new DomainEventHandler(subscriber, eventHandler));
        }

        public void Unsubscribe(object subscriber)
        {
            // iterate over all senders
            foreach (var handlersBySource in _eventHandlers.Values)
            {
                // iterate over all events that can be raised for this sender
                foreach (var handlersByDomainEvent in handlersBySource.Values)
                {
                    // go through all registered domain handlers
                    // if there is one for this subscriber, remove it
                    var handler = handlersByDomainEvent.FirstOrDefault(x => x.Subscriber == subscriber);
                    if (handler != null)
                        handlersByDomainEvent.Remove(handler);
                }
            }
        }

        /// <summary>
        /// removes all event handlers which are registered for this source
        /// object. Should be called after the source object was deleted.
        /// </summary>
        /// <param name="source"></param>
        protected void RemoveSource(object source) => _eventHandlers.Remove(source);

        /// <summary>
        /// uses reflection to read all event handlers from the subscriber
        /// and registers them for the source object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subscriber"></param>
        public void Subscribe(object source, object subscriber)
        {
            // get all methods that accepts a domain event as parameter
            var subscriptionMethodInfos = subscriber.GetType()
                .GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(x => x.GetParameters()
                    .Count(p => typeof(IDomainEvent).IsAssignableFrom(p.ParameterType)) == 1)
                .ToList();

            // register them for the current domain object
            foreach (var subscriptionMethodInfo in subscriptionMethodInfos)
            {
                // store the lambda that we use to execute the method, so we can unsubscribe later
                Action<IDomainEvent> subscription = x => subscriptionMethodInfo.Invoke(subscriber, new object[] { x });

                // we already know that there is only one parameter in this method
                // so no additional check is needed
                var eventType = subscriptionMethodInfo.GetParameters().Single().ParameterType;
                Subscribe(source, eventType, subscription, subscriber);
            }
        }

        /// <summary>
        /// helper method for publishing create events
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="createdDomainObject"></param>
        public virtual void PublishCreated<T>(object source, T createdDomainObject) 
            => Publish(source, new DomainObjectCreatedEvent<T>(createdDomainObject));

        /// <summary>
        /// helper method for publishing delete events
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainList"></param>
        /// <param name="deletedDomainObject"></param>
        public void PublishDeleted<T>(IEnumerable<T> domainList, T deletedDomainObject)
        {
            Publish(domainList, new DomainObjectDeletedEvent<T>(deletedDomainObject));
            // since the object does not exist any more, we
            // will also remove it as a source for messages
            RemoveSource(deletedDomainObject);
        }
    }
}