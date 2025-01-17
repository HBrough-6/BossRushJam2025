using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChargeStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private UnityEvent m_callback = new();
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

    public ChargeStrategy(Client client)
    {
        m_client = client;

    }
}