using System;
using System.Collections.Generic;

public class MessageCenter
{
    private static MessageCenter instance;
    private static event EventHandler handleMessageEvent;
    private Dictionary<MessageType, EventHandler> listeners;

    public delegate void EventHandler(Message message);

    public static MessageCenter Instance
    {
        get
        {
            if (instance == null) instance = new MessageCenter();

            return instance;
        }
    }

    public MessageCenter()
    {
        listeners = new Dictionary<MessageType, EventHandler>();
    }

    public void RegisterListener(MessageType type, EventHandler eventHandler)
    {
        if (listeners.ContainsKey(type))
        {
            handleMessageEvent = listeners[type] as EventHandler;
            handleMessageEvent += eventHandler;
            listeners[type] = handleMessageEvent;
        }
        else
            listeners.Add(type, eventHandler);
    }

    public void UnregisterListener(MessageType type, EventHandler eventHandler)
    {
        if(listeners.ContainsKey(type))
        {
            handleMessageEvent = listeners[type] as EventHandler;
            handleMessageEvent -= eventHandler;
            listeners[type] = handleMessageEvent;
        }
    }

    public void Broadcast(Message message)
    {
        if(listeners.ContainsKey(message.Type))
        {
            handleMessageEvent = listeners[message.Type] as EventHandler;

			if (handleMessageEvent != null)
            	handleMessageEvent(message);
        }
    }
}
