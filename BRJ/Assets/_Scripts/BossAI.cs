using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class BossAI : AIBehaviour , ISubject
{
    protected BossState m_state = 0;

    [SerializeField]
    protected BossMoveset[] m_movesets;

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
            MethodInfo info = typeof(Client).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => {
                return m.Name == nameof(ApplyStrategy) &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType == typeof(UnityAction);
            });

            if (info != null)
            {
                MethodInfo gen = info.MakeGenericMethod(strats[index]);
                UnityAction callback = () => { finished = true; };
                Debug.Log(gen);

                gen.Invoke(this, new object[] { callback });
            }
            yield return new WaitUntil(() => finished);
            finished = false;
            yield return new WaitForSeconds(combo.timings[index]);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        MoveStrategy move = new MoveStrategy(this, Camera.main.transform);
        m_baseStrategies.Add(move);
        m_strategies.Add(move);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Attempt Combo One"))
        {
            StartCoroutine(ApplyCombo(m_movesets[0].combos[0]));
        }

        if (GUILayout.Button("Disable move strategy"))
        {
            DisableStrategy<MoveStrategy>();
        }
    }
}
