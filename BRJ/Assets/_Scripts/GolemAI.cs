/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/17/2025
 * UPDATED: 01/17/2025 | BY: Chase Morgan  | COMMENTS: Added Class
 * FILE DESCRIPTION: Golem AI class to handle most things that Golem will need to do
 */
using ChaseMorgan.Strategy;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class GolemAI : BossAI
{
    [SerializeField]
    private GameObject m_playerReference;
    [SerializeField]
    private BossCollider[] m_leftArms, m_rightArms;
    [SerializeField]
    private BossCollider m_eye;

    private BossMoveset m_currentMoveset;

    [Header("Settings"), SerializeField]
    private GolemSettings m_settings;

    private RockLiftStrategy m_rockLift;
    private SlamLeftStrategy m_slamLeft;
    private SlamRightStrategy m_slamRight;
    private BigSlamStrategy m_bigSlam;
    private RollStrategy m_roll;
    private AirstrikeStrategy m_airstrike;
    private SwipeLeftStrategy m_swipeLeft;
    private SwipeRightStrategy m_swipeRight;

    private ChargeStrategy m_leap;
    private SpinStrategy m_spin;
    private SandstormStrategy m_sandstorm;

    private bool m_changeMoveset;
    private float m_attackInterval = 5.0f;
    private float m_crystalHealth = 100f;

    protected override void Awake()
    {
        base.Awake();
        m_rockLift = new RockLiftStrategy(this, m_playerReference.transform, m_settings.HazardAreaPrefab, m_settings.RockPrefab);
        m_strategies.Add(m_rockLift);
        m_slamLeft = new SlamLeftStrategy(this, m_settings.HazardAreaPrefab, m_settings.LeftSlamOffset, m_settings.LeftRightSlamMagnitude, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_slamLeft);
        m_slamRight = new SlamRightStrategy(this, m_settings.HazardAreaPrefab, m_settings.RightSlamOffset, m_settings.LeftRightSlamMagnitude, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_slamRight);
        m_bigSlam = new BigSlamStrategy(this, m_settings.HazardAreaPrefab, m_settings.BigSlamOffset, m_settings.BigSlamMagnitude, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_bigSlam);
        m_roll = new RollStrategy(this, m_playerReference.transform, m_settings.MaxRollTime, m_settings.RollSpeed, m_settings.ChooseTargetDirectionOnce, m_settings.RollUpdateFrequency, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_roll);
        m_airstrike = new AirstrikeStrategy(this, m_playerReference.transform, m_settings.HazardAreaPrefab, m_settings.StrikeMagnitude, m_settings.StrikeAirTime, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_airstrike);
        m_swipeLeft = new SwipeLeftStrategy(this, m_leftArms, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_swipeLeft);
        m_swipeRight = new SwipeRightStrategy(this, m_rightArms, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_swipeRight);
        m_sandstorm = new SandstormStrategy(this, m_settings.SandstormPrefab, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_sandstorm);
        m_spin = new SpinStrategy(this, m_playerReference.transform, m_settings.PlayerTag, (o) => { Debug.Log("Collided with " + o[1].ToString()); });
        m_strategies.Add(m_spin);
    }

    public IEnumerator ExposeCrystal()
    {
        m_eye.GetComponent<SphereCollider>().enabled = true;
        m_eye.onTriggerEnter += (c) => { m_crystalHealth -= 25f; }; //Double the damage that the player deals when we have a hook for it

        yield return new WaitUntil(() => m_crystalHealth <= 0);

        m_crystalHealth = 100f;

        m_eye.onTriggerEnter -= (c) => { m_crystalHealth -= 25f; };
        m_eye.GetComponent<SphereCollider>().enabled = false;
    }

    public void ChangePhase(int phase)
    {
        CurrentPhase = (BossState)phase;

        m_attackInterval = 5.0f / phase;

        UpdateMoveset();
    }

    private void UpdateMoveset()
    {
        foreach (BossMoveset moves in m_movesets)
        {
            if (moves.activePhase == CurrentPhase)
            {
                m_currentMoveset = moves;
                break;
            }
        }

        m_changeMoveset = true;
    }

    public override void StartBossFight()
    {
        base.StartBossFight();

        ChangePhase(1);

        StartCoroutine(BossFight());
    }

    private IEnumerator BossFight()
    {
        if (m_currentMoveset == null) yield break;

        while (!m_changeMoveset)
        {
            bool finished = false;
            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.25f) //If over .25f, do a combo
            {
                StartCoroutine(ApplyCombo(m_currentMoveset.combos[UnityEngine.Random.Range(0, m_currentMoveset.combos.Length)], () => finished = true));
            }
            else
            {
                StrategyEnumerator.strategies.TryGetValue(m_currentMoveset.attacks[UnityEngine.Random.Range(0, m_currentMoveset.attacks.Length)], out Type strat);
                MethodInfo info = typeof(Client).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => {
                    return m.Name == nameof(ApplyStrategy) &&
                                m.IsGenericMethodDefinition &&
                                m.GetParameters().Length == 1 &&
                                m.GetParameters()[0].ParameterType == typeof(UnityAction);
                });

                if (info != null)
                {
                    MethodInfo gen = info.MakeGenericMethod(strat);

                    MethodInfo disable = typeof(Client).GetMethod(nameof(DisableStrategy), BindingFlags.Public | BindingFlags.Instance);
                    disable = disable.MakeGenericMethod(strat);
                    UnityAction callback = () => { finished = true; disable.Invoke(this, null); };

                    Debug.Log(gen);

                    gen.Invoke(this, new object[] { callback });
                }
            }

            yield return new WaitUntil(() => finished);

            yield return new WaitForSeconds(m_attackInterval);
        }

        m_changeMoveset = false;
        StartCoroutine(BossFight());
    }

    public void ToggleRollCollider(bool b)
    {
        //UPDATE WHEN WE GET ACTUAL COLLIDER
        if (b)
        {
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<SphereCollider>().enabled = true;
            GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = true;
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_activeStrategies.Contains(m_roll))
        {
            m_roll.Collision(collision);
        }
        else if (m_activeStrategies.Contains(m_airstrike))
        {
            m_airstrike.Collision(collision);
        }
        else if (m_activeStrategies.Contains(m_spin))
        {
            m_spin.Collision(collision);
        }
    }

    private void OnGUI()
{
        if (GUILayout.Button("Begin AI"))
        {
            StartBossFight();
        }

        if (GUILayout.Button("Roll"))
        {
            ApplyStrategy<RollStrategy>(() => DisableStrategy<RollStrategy>());
        }
} 
}
