using ChaseMorgan.Strategy;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RollStrategy : IStrategy
{
    private Client m_client;
    private UnityEvent m_callback = new();
    private string m_tag = string.Empty;
    private event Action<object[]> m_onCollision;
    private Animator m_animator;
    private float m_maxTime, m_speed;
    private Transform m_target;
    private float m_updateFrequency;

    private float m_timeSinceLastUpdate = 0.0f;

    public void Disable()
    {
        m_callback.RemoveAllListeners();
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_callback.AddListener(callback);
        m_animator.SetTrigger("rollOn");
    }

    public RollStrategy(Client client, Transform target, float maxTime, float speed, bool chooseTargetDirectionOnce, float updateFrequency = 0.0f, string tag = "Untagged", Action<object[]> onCollision = null)
    {
        m_client = client;
        m_target = target;
        m_maxTime = maxTime;
        m_speed = speed;
        m_updateFrequency = updateFrequency;
        m_tag = tag;

        if (onCollision != null)
            m_onCollision += onCollision;

        m_animator = client.GetComponent<Animator>();
        foreach (AnimationStateController control in m_animator.GetBehaviours<AnimationStateController>())
        {
            if (control.Label == "Roll")
            {
                if (!chooseTargetDirectionOnce)
                {
                    control.onStateUpdate += UpdateRoll;
                }

                control.onStateEnter += StartRoll;
            }
            else if (control.Label == "EndRoll")
            {
                control.onStateExit += StateExit;
            }
        }
    }

    private void StartRoll()
    {
        Vector3 direction = (m_target.transform.position - m_client.transform.position).normalized;

        switch (m_client.GetType())
        {
            case Type t when t == typeof(GolemAI):
                ((GolemAI)m_client).ToggleRollCollider(true);
                break;
        }
        
        m_client.StartCoroutine(StopRoll());

        m_client.GetComponent<Rigidbody>().velocity = direction * m_speed;
    }

    private IEnumerator StopRoll()
    {
        yield return new WaitForSeconds(m_maxTime);
        m_animator.SetTrigger("rollOff");
    }

    private void UpdateRoll(AnimatorStateInfo info)
    {
        if (m_timeSinceLastUpdate > m_updateFrequency)
        {
            m_timeSinceLastUpdate = 0.0f;
            Vector3 direction = (m_target.transform.position - m_client.transform.position).normalized;
            m_client.GetComponent<Rigidbody>().velocity = direction * m_speed;
        }

        m_timeSinceLastUpdate += Time.deltaTime;
    }

    private void StateExit()
    {
        switch (m_client.GetType())
        {
            case Type t when t == typeof(GolemAI):
                ((GolemAI)m_client).ToggleRollCollider(false);
                break;
        }

        m_client.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_client.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        m_timeSinceLastUpdate = 0.0f;
        m_client.StopCoroutine(StopRoll()); //If we leave the state early we want to stop the coroutine so it doesn't fire causing the next roll to stop prematuraly

        m_callback.Invoke();
    }

    public void Collision(Collision collision)
    {
        Debug.Log(collision.transform.tag);
        if (collision.transform.CompareTag(m_tag))
        {
            m_onCollision?.Invoke(new object[] { GetType().Name, collision });
            m_callback?.Invoke();
            m_animator.SetTrigger("rollOff");
        }
        else //Check if it is a wall
        {
            if (collision.collider.bounds.max.y > m_client.GetComponent<SphereCollider>().bounds.min.y)
            {
                Debug.Log("Hit Wall!");
                m_callback?.Invoke();
                m_animator.SetTrigger("rollOff");
            }
        }
    }
}