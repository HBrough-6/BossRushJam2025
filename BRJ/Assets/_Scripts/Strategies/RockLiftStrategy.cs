using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RockLiftStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private GameObject m_rockPrefab, m_hazardAreaPrefab;
    private Transform m_target;
    private UnityEvent m_callback = new();

    private GameObject m_rockInstance, m_hazardAreaInstance;
    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
        m_client.StartCoroutine(RockLift());
    }

    public RockLiftStrategy(Client client, Transform target, GameObject hazardAreaPrefab, GameObject rockPrefab)
    {
        m_client = client;
        m_target = target;
        m_hazardAreaPrefab = hazardAreaPrefab;
        m_rockPrefab = rockPrefab;

        m_rockInstance = Object.Instantiate(m_rockPrefab);
        m_rockInstance.SetActive(false);
        m_rockInstance.name = "Rock_Instance_" + GetType().Name;

        m_hazardAreaInstance = Object.Instantiate(m_hazardAreaPrefab);
        m_hazardAreaInstance.SetActive(false);
        m_hazardAreaInstance.name = "Hazard_Area_Instance_" + GetType().Name;
    }

    private IEnumerator RockLift()
    {       
        float time = 0.0f;
        float duration = 2.0f;
        Vector3 goal = Vector3.zero;
        Physics.Raycast(m_client.transform.position + (m_client.transform.forward.normalized * 2.0f), Vector3.down, out RaycastHit hit, Mathf.Infinity);
        Vector3 start = hit.point;
        Vector3 hazardVel = Vector3.zero;

        //Lift up the rock
        while (m_isActive || time > duration)
        {
            goal = m_target.position;
            m_hazardAreaInstance.transform.position = Vector3.SmoothDamp(m_hazardAreaInstance.transform.position, goal, ref hazardVel, 0.25f);
            m_rockInstance.transform.position = Vector3.Lerp(start, start + (Vector3.up * 5.0f), time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        m_hazardAreaInstance.transform.position = goal;

        time = 0.0f;
        duration = 3.0f;

        start = m_rockInstance.transform.position;

        //Throw rock using interpolation
        while (m_isActive || time > duration)
        {

            time += Time.deltaTime;
            yield return null;
        }
    }

    ~RockLiftStrategy()
    {
        Object.Destroy(m_hazardAreaInstance);
        Object.Destroy(m_rockInstance);
    }
}