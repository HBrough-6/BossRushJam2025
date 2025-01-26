using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGolemSettings", menuName = "Boss/New Golem Settings")]
public class GolemSettings : ScriptableObject
{
    [Header("General")]
    [SerializeField]
    private GameObject hazardAreaPrefab; //Private just so we can make sure we don't accidently overwrite these settings in game
    [SerializeField]
    private string playerTag;
    [SerializeField]
    private float dangerZoneUpdateFrequency = 0.25f;

    [Header("Slam Attacks")] //I have to separate these fields because of how Unity serializes fields alongside headers
    [SerializeField, Min(0.0f)]
    private float leftRightSlamMagnitude;
    [SerializeField, Min(0.0f)]
    private float leftRightSlamDamage;
    [SerializeField, Min(0.0f)]
    private float bigSlamMagnitude;
    [SerializeField, Min(0.0f)]
    private float bigSlamDamage;
    [SerializeField]
    private Vector3 leftSlamOffset, rightSlamOffset, bigSlamOffset;

    [Header("Rock Lift")]
    [SerializeField]
    private GameObject rockPrefab;

    [Header("Roll")]
    [SerializeField]
    private bool chooseTargetDirectionOnce = true;
    [SerializeField, Min(0.0f)]
    private float maxRollTime = 5f, rollSpeed = 10f, rollUpdateFrequency = .25f, rollDamage;

    [Header("Airstrike")]
    [SerializeField, Min(0.0f)]
    private float strikeMagnitude = 10f;
    [SerializeField, Min(0.0f)]
    private float strikeAirTime = 4.0f;

    [Header("Sandstorm")]
    [SerializeField]
    private GameObject sandstormPrefab;

    public GameObject HazardAreaPrefab => hazardAreaPrefab;
    public string PlayerTag => playerTag;
    public float DangerZoneUpdateFrequency => dangerZoneUpdateFrequency;
    public GameObject RockPrefab => rockPrefab;
    public float LeftRightSlamMagnitude => leftRightSlamMagnitude;
    public float LeftRightSlamDamage => leftRightSlamDamage;
    public float BigSlamMagnitude => bigSlamMagnitude;
    public float BigSlamDamage => bigSlamDamage;
    public Vector3 LeftSlamOffset => leftSlamOffset;
    public Vector3 RightSlamOffset => rightSlamOffset;
    public Vector3 BigSlamOffset => bigSlamOffset;
    public bool ChooseTargetDirectionOnce => chooseTargetDirectionOnce;
    public float MaxRollTime => maxRollTime;
    public float RollSpeed => rollSpeed;
    public float RollUpdateFrequency => rollUpdateFrequency;
    public float RollDamage => rollDamage;
    public float StrikeMagnitude => strikeMagnitude;
    public float StrikeAirTime => strikeAirTime;
    public GameObject SandstormPrefab => sandstormPrefab;
}
