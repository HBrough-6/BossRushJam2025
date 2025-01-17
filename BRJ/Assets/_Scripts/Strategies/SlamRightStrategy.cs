using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlamRightStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private UnityEvent m_callback = new();

    public float SlamMagnitude { get; set; }
    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;

        if (callback != null)
            m_callback.AddListener(callback);

        client.StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return null;
        m_callback?.Invoke();
    }

    public SlamRightStrategy(Client client, float slamMagnitude = 3.0f)
    {
        m_client = client;
        SlamMagnitude = slamMagnitude;
    }
}
