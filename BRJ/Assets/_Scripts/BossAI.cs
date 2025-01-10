using UnityEngine;

public class BossAI : BaseAI
{
    private void OnGUI()
    {
        if (GUILayout.Button("Add Test Strategy 1"))
        {
            ApplyStrategy(new TestStrategy());
        }
        else if (GUILayout.Button("Add Test Strategy 2"))
        {
            ApplyStrategy(new TestStrategy1());
        }

        if (GUILayout.Button("Disable test strategy 1"))
        {
            DisableStrategy<TestStrategy>();
        }
        else if (GUILayout.Button("Disable Test Strategy 2"))
        {
            DisableStrategy<TestStrategy1>();
        }
    }
}
