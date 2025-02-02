using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AirstrikeStrategy : IStrategy
{
    private Client m_client;
    private Transform m_target;
    private float m_magnitude, m_airTime;
    private string m_tag;
    private GameObject m_hazardAreaPrefab;
    private UnityEvent m_callback = new();
    private GameObject m_hazardAreaInstance;
    private bool m_airBound;
    private Vector3 hazardVel = Vector3.zero;
    private event Action<object[]> m_onCollision;

    public StrategyMaxRange MaxRange { get; set; }

    public void Disable()
    {
        m_callback.RemoveAllListeners();
        if (m_airBound)
        {
            m_client.GetComponent<NavMeshAgent>().enabled = true;
            m_client.GetComponent<Rigidbody>().isKinematic = false;
            m_client.transform.position = m_hazardAreaInstance.transform.position;
            m_airBound = false;
        }

        m_hazardAreaInstance.transform.position = Vector3.zero;
        m_hazardAreaInstance.SetActive(false);
    }

    public void Execute(Client client, UnityAction callback)
    {
        m_callback.AddListener(callback);
        m_client.StartCoroutine(AirAttack());
    }

    private IEnumerator AirAttack()
    {
        m_airBound = true;
        float time = 0.0f, upDur = 1.0f;
        Vector3 start = m_client.transform.position;
        Vector3 end = m_client.transform.position + (Vector3.up * 50);
        Rigidbody rb = m_client.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        NavMeshAgent agent = rb.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        //FIX ONCE WE HAVE ACTUAL HITBOX
        CapsuleCollider c = m_client.GetComponent<CapsuleCollider>();
        //c.isTrigger = true;

        while(time < upDur)
        {
            m_client.transform.position = Vector3.Lerp(start, end, time / upDur);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0.0f;


        m_hazardAreaInstance.SetActive(true);
        m_hazardAreaInstance.transform.position = m_target.transform.position;

        while (time < m_airTime)
        {
            Physics.Raycast(m_target.transform.position + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity);
            Vector3 goal = m_target.position;
            m_hazardAreaInstance.transform.localScale = Vector3.Lerp(new Vector3(0.01f, 1f, 0.01f), new Vector3(1f * m_magnitude, 1f, 1f * m_magnitude), time / m_airTime);
            m_hazardAreaInstance.transform.position = Vector3.SmoothDamp(m_hazardAreaInstance.transform.position, new Vector3(goal.x, hit.point.y, goal.z), ref hazardVel, 0.2f);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0.0f;
        yield return new WaitForSeconds(1);

        m_client.transform.position = new Vector3(m_hazardAreaInstance.transform.position.x, m_client.transform.position.y, m_hazardAreaInstance.transform.position.z);
        start = m_client.transform.position;
        end = m_hazardAreaInstance.transform.position;

        while (time < upDur)
        {
            m_client.transform.position = Vector3.Lerp(start, end, time / upDur);
            time += Time.deltaTime;
            yield return null;
        }

        rb.isKinematic = false;
        agent.enabled = true;
        m_airBound = false;
        m_callback.Invoke();
    }

    public void Collision(Collision collision)
    {
        Debug.Log(collision.transform.tag);
        if (collision.transform.CompareTag(m_tag))
        {
            m_onCollision?.Invoke(new object[] { GetType().Name, collision });
        }
    }

    public AirstrikeStrategy(Client client, Transform target, GameObject hazardAreaPrefab, float magnitude, float airTime, string tag, Action<object[]> onCollision = null)
    {
        m_client = client;
        m_target = target;
        m_magnitude = magnitude;
        m_airTime = airTime;
        m_tag = tag;

        if (onCollision != null)
            m_onCollision += onCollision;

        m_hazardAreaPrefab = hazardAreaPrefab;

        m_hazardAreaInstance = UnityEngine.Object.Instantiate(m_hazardAreaPrefab);
        m_hazardAreaInstance.SetActive(false);
        m_hazardAreaInstance.name = "Hazard_Area_Instance_" + GetType().Name;

        MaxRange = StrategyMaxRange.None;
    }
}