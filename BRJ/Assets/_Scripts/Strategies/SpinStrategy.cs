using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpinStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private UnityEvent m_callback = new();
    private Transform m_target;
    private Animator m_animator;
    //private AnimationStateController m_controller;
    private event Action<object[]> m_onCollision;
    private string m_tag;
    private int m_spinIterations, m_spinIndex;
    private float m_speed;

    public StrategyMaxRange MaxRange { get; set; }


    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
        m_client.StartCoroutine(Spin());
    }

    public SpinStrategy(Client client, Transform target, string tag, Action<object[]> onCollision, float spinSpeed = 25f, int spinIterations = 25)
    {
        m_client = client;
        m_target = target;

        m_animator = client.GetComponent<Animator>();

        m_onCollision += onCollision;
        m_tag = tag;
        m_spinIterations = spinIterations;
        m_speed = spinSpeed;

        foreach (AnimationStateController controller in m_animator.GetBehaviours<AnimationStateController>())
        {
            if (controller.Label == "Spin")
            {
                controller.events[0].unityEvent.AddListener(() => { m_spinIndex++; });
                controller.onStateExit += () => { m_spinIndex = 0; m_client.GetComponent<Rigidbody>().velocity = new Vector3(0, m_client.GetComponent<Rigidbody>().velocity.y, 0); };
            }
        }
    }

    private IEnumerator Spin()
    {
        m_animator.SetTrigger("spinOn");
        yield return new WaitUntil(() => m_spinIndex > 0); //Wait for first spin to get started with moving
        while (m_spinIndex < m_spinIterations)
        {
            Vector3 vel = m_speed * (m_target.position - m_client.transform.position).normalized;
            m_client.GetComponent<Rigidbody>().velocity = new Vector3(vel.x, m_client.GetComponent<Rigidbody>().velocity.y, vel.z);
            yield return null;
        }

        StopSpin();

    }

    private void StopSpin()
    {
        m_animator.SetTrigger("spinOff");
        m_callback.Invoke();
    }

    public void Collision(Collision collision)
    {
        m_onCollision?.Invoke(new object[] { GetType().Name, collision.gameObject });

        if (collision.transform.CompareTag(m_tag))
        {
            StopSpin();
        }
    }
}