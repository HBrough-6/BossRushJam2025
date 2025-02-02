using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwipeRightStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private UnityEvent m_callback = new();
    private BossCollider[] m_arms;
    private event Action<object[]> OnCollision;
    private string m_tag;
    private bool m_triggered = false;
    private Animator m_animator;

    public StrategyMaxRange MaxRange { get; set; } = StrategyMaxRange.Small;

    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
        m_triggered = false;
    }

    public void Execute(Client client, UnityAction callback)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
        m_animator.SetTrigger("swipeRight");
    }

    public SwipeRightStrategy(Client client, BossCollider[] armColliders, string playerTag, Action<object[]> onCollision)
    {
        m_client = client;
        m_arms = armColliders;
        m_tag = playerTag;
        m_arms = armColliders;
        OnCollision += onCollision;

        foreach (var arm in armColliders)
        {
            arm.onTriggerEnter += ArmTriggered;
        }

        m_animator = client.GetComponent<Animator>();
        foreach (AnimationStateController control in m_animator.GetBehaviours<AnimationStateController>())
        {
            if (control.Label == "Swipe Right")
            {
                control.onStateExit += () => { m_callback.Invoke(); };
            }
        }
    }

    private void ArmTriggered(Collider other)
    {
        if (other.CompareTag(m_tag) && !m_triggered && m_isActive)
        {
            m_triggered = true;
            /*switch (m_client.GetType())
            {
                case Type t when t == typeof(GolemAI):
                    GolemAI golem = (GolemAI)m_client;
                    break;
            } */

            OnCollision?.Invoke(new object[] { GetType().Name, other });

            m_callback.Invoke();
        }
    }

    ~SwipeRightStrategy()
    {
        foreach (var arm in m_arms)
        {
            arm.onTriggerEnter -= ArmTriggered;
        }
    }
}