using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlamLeftStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private GameObject m_hazardAreaPrefab;
    private UnityEvent m_callback = new();
    private GameObject m_hazardAreaInstance;
    private string m_tag = string.Empty;
    private event Action m_onCollision;

    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
        m_hazardAreaInstance.transform.position = Vector3.zero;
        m_hazardAreaInstance.SetActive(false);
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
    }

    public SlamLeftStrategy(Client client, GameObject hazardAreaPrefab, string tag = "", Action[] onCollision = null)
    {
        m_client = client;

        m_hazardAreaInstance = UnityEngine.Object.Instantiate(m_hazardAreaPrefab);
        m_hazardAreaInstance.SetActive(false);
        m_hazardAreaInstance.name = "Hazard_Area_Instance_" + GetType().Name;

        m_tag = tag;
        
        foreach (Action action in onCollision)
        {
            m_onCollision += action;
        }
    }

    ~SlamLeftStrategy()
    {
        UnityEngine.Object.Destroy(m_hazardAreaInstance);

        m_onCollision = null;
    }
}
