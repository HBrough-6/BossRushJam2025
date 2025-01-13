using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct HealthEvent //These will fire methods if a certain threshold of health is met. These can be set either in the inspector or via code.
{
    public UnityEvent healthEvent;
    public int healthTrigger;
    public bool isActive;

    /// <summary>
    /// Makes a new HealthEvent with multiple actions added.
    /// </summary>
    /// <param name="actions">The UnityActions that will be fired at the certain health trigger</param>
    /// <param name="trigger">The health trigger</param>
    public HealthEvent(UnityAction[] actions, int trigger)
    {
        healthEvent = new UnityEvent();
        foreach (UnityAction action in actions)
        {
            healthEvent.AddListener(action);
        }
        healthTrigger = trigger;
        isActive = true;
    }

    /// <summary>
    /// Makes a new HealthEvent with one action added.
    /// </summary>
    /// <param name="action">The UnityAction that will be fired at the certain health trigger</param>
    /// <param name="trigger">The health trigger</param>
    public HealthEvent(UnityAction action, int trigger)
    {
        healthEvent = new UnityEvent();
        healthEvent.AddListener(action);
        healthTrigger = trigger;
        isActive = true;
    }
}

public abstract class AIBehaviour : Client
{
    [SerializeField]
    protected float m_health = 100f;
    [SerializeField]
    protected float m_maxHealth = 100f;
    [SerializeField]
    protected bool m_canDie = true;

    [SerializeField]
    protected List<HealthEvent> m_healthEvents = new List<HealthEvent>();

    public UnityEvent onDeath = new UnityEvent();

    public virtual float Health
    {
        get => m_health;
        set
        {
            m_health = Mathf.Clamp(value, 0, m_maxHealth);

            if (m_canDie && m_health <= 0)
            {
                Death();
            }

            m_healthEvents.ForEach(e =>
            {
                if (e.isActive && m_health <= e.healthTrigger)
                {
                    e.healthEvent?.Invoke();
                    e.isActive = false;
                }
            });
        }
    }

    public List<HealthEvent> HealthEvents => m_healthEvents;

    protected virtual void Death()
    {
        onDeath?.Invoke();
    }

    public virtual void Reset()
    {
        m_healthEvents.ForEach((e) => { e.isActive = true; }); //Reactivate health events for when this class is reused (if using object pooling or reviving the AI)
    }

    protected virtual void Awake()
    {
        if (m_maxHealth < m_health)
        {
            m_maxHealth = m_health;
        }
    }
}
