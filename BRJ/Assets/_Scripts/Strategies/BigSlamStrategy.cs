using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BigSlamStrategy : IStrategy
{
    private Client m_client;
    private GameObject m_hazardAreaPrefab;
    private UnityEvent m_callback = new();
    private GameObject m_hazardAreaInstance;
    private string m_tag = string.Empty;
    private event Action<object[]> m_onCollision;
    private Animator m_animator;
    private Vector3 m_offset;
    private float m_magnitude;
    private AnimationStateController m_controller;
    public StrategyMaxRange MaxRange { get; set; } = StrategyMaxRange.Small;


    public void Disable()
    {
        m_callback.RemoveAllListeners();
        m_hazardAreaInstance.transform.position = Vector3.zero;
        m_hazardAreaInstance.SetActive(false);
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_callback.AddListener(callback);
        m_animator.SetTrigger("slamBig");
    }

    public BigSlamStrategy(Client client, GameObject hazardAreaPrefab, Vector3 offset, float slamMagnitude, string tag, Action<object[]> onCollision = null)
    {
        m_client = client;
        m_animator = client.GetComponent<Animator>();
        foreach (AnimationStateController control in m_animator.GetBehaviours<AnimationStateController>())
        {
            if (control.Label == "Big Slam")
            {
                m_controller = control;
                m_controller.onStateExit += StateExit;
                m_controller.onStateUpdateTimeStamped += PlaceHazard;

                m_controller.events.ForEach(e =>
                {
                    if (e.eventName == "Slam")
                    {
                        e.unityEvent ??= new();

                        e.unityEvent.AddListener(Slam);
                    }
                });
                break;
            }
        }

        m_hazardAreaPrefab = hazardAreaPrefab;

        m_hazardAreaInstance = UnityEngine.Object.Instantiate(m_hazardAreaPrefab);
        m_hazardAreaInstance.SetActive(false);
        m_hazardAreaInstance.name = "Hazard_Area_Instance_" + GetType().Name;

        m_tag = tag;
        m_offset = offset;
        m_magnitude = slamMagnitude;

        if (onCollision != null)
            m_onCollision += onCollision;
    }

    private void Slam()
    {
        Vector3 pos = m_offset;
        pos = m_client.transform.TransformPoint(pos);
        Collider[] colliders = Physics.OverlapSphere(pos, m_magnitude);

        foreach (Collider c in colliders)
        {
            if (c.CompareTag(m_tag))
            {
                m_onCollision?.Invoke(new object[] { GetType().Name, c });
            }
        }
    }

    private void PlaceHazard(AnimatorStateInfo info, float time)
    {
        Vector3 offset = m_offset;
        offset = m_client.transform.TransformPoint(offset);

        Physics.Raycast(offset + Vector3.up * 3, Vector3.down, out RaycastHit hit, Mathf.Infinity);

        m_hazardAreaInstance.SetActive(true);

        m_hazardAreaInstance.transform.position = new Vector3(offset.x, hit.collider != null ? hit.point.y : 0, offset.z);

        m_hazardAreaInstance.transform.localScale = Vector3.Lerp(new Vector3(0.01f, 1f, 0.01f), new Vector3(1f * m_magnitude, 1f, 1f * m_magnitude), time * 3f / info.length);
    }

    private void StateExit()
    {
        m_hazardAreaInstance.SetActive(false);
        m_hazardAreaInstance.transform.position = Vector3.zero;
        m_callback.Invoke();
    }

    ~BigSlamStrategy()
    {
        UnityEngine.Object.Destroy(m_hazardAreaInstance);

        m_onCollision = null;
    }
}
