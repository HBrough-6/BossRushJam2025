using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGolemSettings", menuName = "Boss/New Golem Settings")]
public class GolemSettings : ScriptableObject
{
    [SerializeField]
    private GameObject hazardAreaPrefab, rockPrefab; //Private just so we can make sure we don't accidently overwrite these settings in game
    [SerializeField]
    private float leftRightSlamMagnitude, bigSlamMagnitude;

    public GameObject HazardAreaPrefab => hazardAreaPrefab;
    public GameObject RockPrefab => rockPrefab;
    public float LeftRightSlamMagnitude => leftRightSlamMagnitude;
    public float BigSlamMagnitude => bigSlamMagnitude;
}
