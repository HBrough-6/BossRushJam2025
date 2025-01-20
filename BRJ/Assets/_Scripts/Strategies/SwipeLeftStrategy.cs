using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwipeLeftStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private UnityEvent m_callback = new();
    private BossCollider[] m_arms;
    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
    }

    public SwipeLeftStrategy(Client client, BossCollider[] armColliders)
    {
        m_client = client;
        m_arms = armColliders;
    }
}
