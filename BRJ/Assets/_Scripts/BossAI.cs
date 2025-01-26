using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/11/2025
 * UPDATED: 01/14/2025 | BY: Chase Morgan  | COMMENTS: Added NavMesh compatibility
 * FILE DESCRIPTION: Parent Boss AI class to handle most things that bosses will need to do
 */

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class BossAI : AIBehaviour , ISubject
{
    protected AudioSource m_audioSource;
    protected BossState m_state = 0;

    [SerializeField]
    protected BossMoveset[] m_movesets;

    [Header("Audio")]
    [SerializeField]
    protected AudioClip m_deathAudio;

    protected List<IStrategy> m_baseStrategies = new();
    public BossState CurrentPhase
    {
        get => m_state;
        set
        {
            m_state = value;
            BossEventBus.Publish(m_state);
            NotifyObservers();
        }
    }

    public override float Health
    {
        get => base.Health;
        set
        {
            base.Health = value;
            NotifyObservers();
        }
    }


    /// <summary>
    /// Play audio for the boss
    /// </summary>
    /// <param name="clip">The clip to play</param>
    /// <param name="overrideClip">If the clip should replace the current permanent clip</param>
    public virtual void PlayAudio(AudioClip clip, bool overrideClip = false)
    {
        if (m_audioSource == null || clip == null) return;

        if (overrideClip)
        {
            m_audioSource.clip = clip;
            m_audioSource.Play();
        }
        else
        {
            m_audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Play audio for the boss
    /// </summary>
    /// <param name="clip">The clip to play</param>
    public virtual void PlayAudio(AudioClip clip) //This allows us to expose it to the inspector unlike the other method
    {
        if (m_audioSource == null || clip == null) return;

        m_audioSource.PlayOneShot(clip);
    }

    public ArrayList Observers { get; set; } = new();

    public virtual void Attach(IObserver observer)
    {
        Observers.Add(observer);
    }

    public virtual void Detach(IObserver observer)
    {
        Observers.Remove(observer);
    }
    
    public virtual void NotifyObservers()
    {
        foreach (IObserver o in Observers)
        {
            o.Notify(this);
        }
    }

    public virtual void StartBossFight()
    {
        CurrentPhase = BossState.PhaseOne;
    }

    public virtual IEnumerator ApplyCombo(BossCombo combo)
    {
        List<Type> strats = new();

        foreach (StrategyEnum e in combo.combo)
        {
            if (StrategyEnumerator.strategies.TryGetValue(e, out Type strat))
            {
                strats.Add(strat);
            }
        }

        bool finished = false;

        for (int index = 0; index < strats.Count; index++) //Since we need to convert from variable to type we need to use reflection to invoke a generic method. 
        {
            MethodInfo info = typeof(Client).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => { //Gets the first method that matches these parameters
                return m.Name == nameof(ApplyStrategy) &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType == typeof(UnityAction);
            });

            if (info != null)
            {
                MethodInfo gen = info.MakeGenericMethod(strats[index]);

                //We again have to use reflection to call this generic function to disable the strategy
                //It's easier when we do this through code and not expose strategy interfaces, but it makes it harder for designers to create unique bosses
                MethodInfo disable = typeof(Client).GetMethod(nameof(DisableStrategy), BindingFlags.Public | BindingFlags.Instance);
                disable = disable.MakeGenericMethod(strats[index]);
                UnityAction callback = () => { finished = true; disable.Invoke(this, null); };

                Debug.Log(gen);

                gen.Invoke(this, new object[] { callback });
            }
            yield return new WaitUntil(() => finished);
            finished = false;

            if (combo.timings.Length < index)
                yield return new WaitForSeconds(combo.timings[index]);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        MoveStrategy move = new MoveStrategy(this, Camera.main.transform);
        m_baseStrategies.Add(move);
        m_strategies.Add(move);
        m_audioSource = GetComponent<AudioSource>(); //Should never be null since we require it on the class
    }

    protected override void Death()
    {
        base.Death();

        PlayAudio(m_deathAudio);
    }
}
