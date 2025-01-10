using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaseMorgan.Strategy;

public class TestStrategy1 : IStrategy
{
    private bool m_active = false;
    public void Disable()
    {
        m_active = false;
        Debug.Log("Test strategy 2 disabled!");
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
            Debug.Log("This is test strategy 2!");
            yield return null;
        }
    }
}
