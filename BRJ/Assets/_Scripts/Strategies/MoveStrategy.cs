using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/12/2025
 * UPDATED: 01/12/2025 | BY: Chase Morgan  | COMMENTS: Added class
 * FILE DESCRIPTION: MoveStrategy to allow for an AIBehvaiour to move
 */

public class MoveStrategy : IStrategy
{
    private Client m_client;
    private Transform m_target;
    private Rigidbody m_rigid;
    private bool m_active = false;
    private NavMeshAgent m_agent;

    private UnityEvent m_event = new();

    private Vector3 m_rotVel;

    public StrategyMaxRange MaxRange { get; set; }


    public float Speed { get; set; } //This allows systems to update the speed at runtime incase we need to speed up/down the client
    public float MaxCloseRange { get; set; }

    public MoveStrategy(Client client, Transform target, float speed = 5.0f, float maxCloseRange = 5.0f)
    {
        m_client = client;
        m_target = target;
        Speed = speed;
        MaxCloseRange = maxCloseRange;

        m_rigid = client.GetComponent<Rigidbody>();
        m_agent = client.GetComponent<NavMeshAgent>();

        m_agent.stoppingDistance = maxCloseRange;
    }

    public void Disable()
    {
        m_active = false;
        //m_client.StartCoroutine(SlowDown());
        //m_event?.Invoke();
        m_agent.isStopped = true;
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_event.RemoveAllListeners();
        if (callback != null)
        {
            m_event.AddListener(callback);
        }

        m_active = true;
        client.StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        //bool db = false;
        while (m_active)
        {



            m_client.transform.rotation = Quaternion.Lerp(m_client.transform.rotation, Quaternion.LookRotation(m_target.position - m_client.transform.position, Vector3.up), Time.deltaTime * 5);

            m_agent.destination = m_target.transform.position;
            m_agent.speed = Speed;

            if (Vector3.Distance(m_client.transform.position, m_target.transform.position) <= MaxCloseRange)
            {
                m_event.Invoke();
                m_active = false;
                yield return new WaitUntil(() => m_active);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /*private IEnumerator SlowDown()
    {
        if (m_slowingDown) yield break;
        Debug.Log("Slowing down");
        m_slowingDown = true;
        Vector3 currentVel = m_rigid.velocity;
        float time = 0.0f;
        while (time < 0.125f && m_active)
        {
            m_rigid.velocity = Vector3.Lerp(currentVel, new Vector3(0, m_rigid.velocity.y, 0), time / 0.125f);
            time += Time.deltaTime;
            yield return null;
        }

        if (!m_active) //if the client slowed down successfully and wasn't interrupted
        {
            m_rigid.velocity = new Vector3(0, m_rigid.velocity.y, 0);
        }

        m_slowingDown = false;
    } */
}
