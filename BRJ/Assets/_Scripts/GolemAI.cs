/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/17/2025
 * UPDATED: 01/17/2025 | BY: Chase Morgan  | COMMENTS: Added Class
 * FILE DESCRIPTION: Golem AI class to handle most things that Golem will need to do
 */
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GolemAI : BossAI
{
    [Header("Settings"), SerializeField]
    private GolemSettings m_settings;

    protected override void Awake()
    {
        base.Awake();
        RockLiftStrategy rockLift = new RockLiftStrategy(this, Camera.main.transform, m_settings.HazardAreaPrefab, m_settings.RockPrefab);
        m_strategies.Add(rockLift);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Attempt Test Combo"))
        {
            StartCoroutine(ApplyCombo(m_movesets[0].combos[0]));
        }
    }
}
