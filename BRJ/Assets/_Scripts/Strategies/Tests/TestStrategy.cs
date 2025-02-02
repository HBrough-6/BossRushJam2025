using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaseMorgan.Strategy;
using UnityEngine.Events;

public class TestStrategy : IStrategy
{
    private bool m_active = false;
    public StrategyMaxRange MaxRange { get; set; } = StrategyMaxRange.Custom;

    public void Disable()
    {
        m_active = false;
        Debug.Log("Test strategy 1 disabled!");
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_active = true;
        client.StartCoroutine(DoStuff());
    }

    private IEnumerator DoStuff()
    {
        while (m_active)
        {
            Debug.Log("This is test strategy 1!");
            yield return null;
        }
    }
}
