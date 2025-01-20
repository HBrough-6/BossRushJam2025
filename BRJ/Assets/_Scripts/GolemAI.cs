/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/17/2025
 * UPDATED: 01/17/2025 | BY: Chase Morgan  | COMMENTS: Added Class
 * FILE DESCRIPTION: Golem AI class to handle most things that Golem will need to do
 */
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class GolemAI : BossAI
{
    [SerializeField]
    private GameObject m_playerReference;

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
        m_swipeLeft = new SwipeLeftStrategy(this, null);
        m_strategies.Add(m_swipeLeft);
        m_swipeRight = new SwipeRightStrategy(this, null);
        m_strategies.Add(m_swipeRight);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Attempt Test Combo"))
        {
            StartCoroutine(ApplyCombo(m_movesets[0].combos[0]));
        }

        if (GUILayout.Button("Attempt Rock Lift"))
        {
            ApplyStrategy<RockLiftStrategy>(() => DisableStrategy<RockLiftStrategy>());
        }

        if (GUILayout.Button("Attempt Left Slam"))
        {
            ApplyStrategy<SlamLeftStrategy>(() => DisableStrategy<SlamLeftStrategy>());
        }

        if (GUILayout.Button("Attempt Right Slam"))
        {
            ApplyStrategy<SlamRightStrategy>(() => DisableStrategy<SlamRightStrategy>());
        }

        if (GUILayout.Button("Attempt Big Slam"))
        {
            ApplyStrategy<BigSlamStrategy>(() => DisableStrategy<BigSlamStrategy>());
        }

        if (GUILayout.Button("Attempt Roll"))
        {
            ApplyStrategy<RollStrategy>(() => DisableStrategy<RollStrategy>());
        }

        if (GUILayout.Button("Attempt Airstrike"))
        {
            ApplyStrategy<AirstrikeStrategy>(() => DisableStrategy<AirstrikeStrategy>());
        }

        if (GUILayout.Button("Attempt Swipe Left"))
        {
            ApplyStrategy<SwipeLeftStrategy>(() => DisableStrategy<SwipeLeftStrategy>());
        }

        if (GUILayout.Button("Attempt Swipe Right"))
        {
            ApplyStrategy<SwipeRightStrategy>(() => DisableStrategy<SwipeRightStrategy>());
        }
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
    }
}
