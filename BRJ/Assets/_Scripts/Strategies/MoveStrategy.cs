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
    private bool m_slowingDown = false;
    private NavMeshAgent m_agent;

    private UnityEvent m_event = new();

    private Vector3 m_rotVel;

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
        m_event?.Invoke();
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
        m_slowingDown = false;
        client.StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        //bool db = false;
        while (m_active)
        {
            /*if (Mathf.Abs(Vector3.Distance(new Vector3(m_client.transform.position.x, 0, m_client.transform.position.z),
                                 new Vector3(m_target.position.x, 0, m_target.position.z))) <= MaxCloseRange)
            {
                if (!db)
                {
                    db = true;
                    //m_client.StartCoroutine(SlowDown());
                }

                yield return null;
                continue;
            }

            db = false; */


            if (m_client.transform.rotation != Quaternion.LookRotation(m_target.position - m_client.transform.position, Vector3.up))
                m_client.transform.rotation =
                    Quaternion.Euler(m_client.transform.rotation.x,
                                     Vector3.SmoothDamp(m_client.transform.rotation.eulerAngles,
                                                        Quaternion.LookRotation(m_target.position - m_client.transform.position, Vector3.up).eulerAngles,
                                                        ref m_rotVel,
                                                        0.125f).y, 
                                     m_client.transform.rotation.z);

            Debug.Log("Moving");

            m_agent.destination = m_target.transform.position;
            m_agent.speed = Speed;

            //Vector3 velocity = Vector3.ClampMagnitude(100 * Speed * Time.deltaTime * (m_target.transform.position - m_client.transform.position).normalized, Speed);
            //m_rigid.velocity = new Vector3(velocity.x, m_rigid.velocity.y, velocity.z);

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
