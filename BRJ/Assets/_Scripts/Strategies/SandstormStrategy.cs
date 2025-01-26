using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class SandstormStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private UnityEvent m_callback = new();
    private Animator m_animator;
    private AnimationStateController m_controller;
    private GameObject m_sandstormPrefab;
    private GameObject m_sandstormInstance;
    private float m_speed, m_sandstormLength;
    private event Action<object[]> m_onCollision;
    private string m_tag;
    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
        m_client.StopCoroutine(Sandstorm());
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
        m_animator.SetTrigger("sandstorm");
    }

    public SandstormStrategy(Client client, GameObject sandstormPrefab, string tag, Action<object[]> onCollision, float speed = 2.5f, float sandstormLength = 3.5f)
    {
        m_client = client;
        m_animator = client.GetComponent<Animator>();
        m_sandstormPrefab = sandstormPrefab;
        m_speed = speed;
        m_sandstormLength = sandstormLength;
        m_onCollision += onCollision;
        m_tag = tag;

        m_sandstormInstance = UnityEngine.Object.Instantiate(sandstormPrefab);
        m_sandstormInstance.SetActive(false);
        m_sandstormInstance.name = "Sandstorm_Instance_" + GetType().Name;
        m_sandstormInstance.GetComponent<BossCollider>().onTriggerEnter += Triggered;

        foreach (AnimationStateController control in m_animator.GetBehaviours<AnimationStateController>())
        {
            if (control.Label == "Sandstorm")
            {
                m_controller = control;

                m_controller.events.ForEach(e =>
                {
                    if (e.eventName == "Start")
                    {
                        e.unityEvent ??= new();

                        e.unityEvent.AddListener(() => { m_client.StartCoroutine(Sandstorm()); });
                    }
                });
                break;
            }
        }
    }

    private void Triggered(Collider other)
    {
        m_onCollision?.Invoke(new object[] { GetType().Name, other });
    }

    private IEnumerator Sandstorm()
    {
        m_sandstormInstance.transform.position = m_client.transform.position;
        m_sandstormInstance.SetActive(true);
        Vector3 forward = m_client.transform.forward;
        float time = 0.0f;
        while (time < m_sandstormLength)
        {
            m_sandstormInstance.transform.position += m_speed * Time.deltaTime * forward;
            time += Time.deltaTime;
            yield return null;
        }

        m_sandstormInstance.SetActive(false);
        m_callback.Invoke();
    }
}
