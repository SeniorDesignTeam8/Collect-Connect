using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    List<EventListener> eventListeners = new List<EventListener>();

    public void Raise()
    {
        foreach (EventListener e in eventListeners)
        {
            e.OnEventRaised();
        }
    }
    public void Register(EventListener listener)
    {
        if(! eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);

        }
    }
    public void DeRegister(EventListener listener)
    {
        if(eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }

}

