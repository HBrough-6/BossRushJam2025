using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BossState
{
    PhaseZero, //This is an idle phase before the player will activate the boss
    PhaseOne,
    PhaseTwo,
    PhaseThree,
    PhaseFour,
    PhaseFive,
    PhaseSix,
    PhaseSeven,
    PhaseEight,
    PhaseNine,
    PhaseTen
}

public class BossEventBus
{
    private static readonly IDictionary<BossState, UnityEvent> Events = new Dictionary<BossState, UnityEvent>();

    public static void Subscribe(BossState eventType, UnityAction listener)
    {
        if (Events.TryGetValue(eventType, out UnityEvent thisEvent))
        {
            thisEvent?.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent?.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(BossState eventType, UnityAction listener)
    {
        if (Events.TryGetValue(eventType, out UnityEvent thisEvent))
        {
            thisEvent?.RemoveListener(listener);
        }
    }

    public static void Publish(BossState eventType)
    {
        if (Events.TryGetValue(eventType, out UnityEvent thisEvent))
        {
            thisEvent?.Invoke();
        }
    }
}
