using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : StateMachineBehaviour
{
    [Serializable]
    public class AnimationEvent
    {
        [Range(0.0f, 1.0f)] public float eventPercent;
        public string eventName;
        [Tooltip("Optional if you want to fire an event when the state reaches this percent")] public UnityEvent unityEvent;

        [HideInInspector] public bool fired;
    }

    [SerializeField, Tooltip("Label this as the name of the state that will be executed.")]
    private string m_label;
    public string Label => m_label;
    public event Action onStateExit;
    public event Action onStateEnter;
    public event Action<AnimatorStateInfo> onStateUpdate;
    public event Action<AnimatorStateInfo, float> onStateUpdateTimeStamped;
    public List<AnimationEvent> events = new();

    private float m_time = 0.0f;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        onStateExit?.Invoke();
        m_time = 0.0f;
        events.ForEach(e => { e.fired = false; });
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        onStateUpdate?.Invoke(stateInfo);
        onStateUpdateTimeStamped?.Invoke(stateInfo, m_time);

        float currentPercent = m_time / stateInfo.length;

        if (currentPercent > 1) //Usually will be called if the state is looped
        {
            Debug.Log("Looping!");
            m_time = 0.0f;
            events.ForEach(e => { e.fired = false; });
        }

        events.ForEach(e => 
        {
            if (currentPercent >= e.eventPercent && !e.fired)
            {
                e.fired = true;
                e.unityEvent?.Invoke();
            }
        });

        m_time += Time.deltaTime;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        onStateEnter?.Invoke();
    }
}
