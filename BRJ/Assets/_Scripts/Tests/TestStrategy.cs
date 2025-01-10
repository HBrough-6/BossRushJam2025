using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaseMorgan.Strategy;

public class TestStrategy : IStrategy
{
    private bool m_active = false;
    public void Disable()
    {
        m_active = false;
        Debug.Log("Test strategy 1 disabled!");
    }

    public void Execute(Client client)
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
