using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RockLiftStrategy : IStrategy
{
    private bool m_isActive = false;
    private Client m_client;
    private GameObject m_rockPrefab, m_hazardAreaPrefab;
    private Transform m_target;
    private UnityEvent m_callback = new();
    private Transform m_instanceContainer;
    private float m_areaMagnitude;

    public StrategyMaxRange MaxRange { get; set; }


    private List<GameObject> m_rockInstances = new(), m_hazardAreaInstances = new();
    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
        m_rockInstances.ForEach(rock => { rock.transform.position = Vector3.zero; rock.SetActive(false); });
        m_client.StopCoroutine(((GolemAI)m_client).ExposeCrystal());
        m_hazardAreaInstances.ForEach(hazard => { hazard.transform.position = Vector3.zero; hazard.SetActive(false); });
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
        m_client.StartCoroutine(RockLift());
        m_client.StartCoroutine(((GolemAI)m_client).ExposeCrystal());
    }

    public RockLiftStrategy(Client client, Transform target, GameObject hazardAreaPrefab, GameObject rockPrefab, float rockAreaMagnitude = 15, int rockCount = 15)
    {
        m_client = client;
        m_target = target;
        m_hazardAreaPrefab = hazardAreaPrefab;
        m_rockPrefab = rockPrefab;
        m_areaMagnitude = rockAreaMagnitude;

        m_instanceContainer = new GameObject("Instance_Container_" + GetType().Name).transform;

        for(int index = 0; index < rockCount; index++)
        {
            GameObject rock = Object.Instantiate(m_rockPrefab);
            rock.SetActive(false);
            rock.name = "Rock_" + index + "_Instance" + GetType().Name;
            rock.transform.parent = m_instanceContainer;

            GameObject hazard = Object.Instantiate(m_hazardAreaPrefab);
            hazard.SetActive(false);
            hazard.name = "Hazard_Area_" + index + "_Instance" + GetType().Name;
            hazard.transform.parent = m_instanceContainer;

            m_rockInstances.Add(rock);
            m_hazardAreaInstances.Add(hazard);
        }
    }

    private IEnumerator RockLift()
    {
        Debug.Log("RockLift");

        float time = 0.0f;
        float duration = 2.0f;

        int arrayLength = Mathf.Min(m_rockInstances.Count, m_hazardAreaInstances.Count);

        Vector3[] goals = new Vector3[arrayLength];
        Vector3[] hazardVels = new Vector3[arrayLength];
        Vector3[] starts = new Vector3[arrayLength];
        RaycastHit[] hits = new RaycastHit[arrayLength];
        Rigidbody[] rbs = new Rigidbody[arrayLength];

        Physics.Raycast(m_client.transform.position + (m_client.transform.forward * 2), Vector3.down, out RaycastHit ground, Mathf.Infinity);

        for (int index = 0; index < arrayLength; index++)
        {
            GameObject rock = m_rockInstances[index];
            GameObject hazard = m_hazardAreaInstances[index];

            rock.SetActive(true);
            rbs[index] = rock.GetComponent<Rigidbody>();
            rbs[index].isKinematic = true;

            hazard.SetActive(true);


            Vector3 randPoint = Random.insideUnitCircle * m_areaMagnitude;
            Vector3 scaledPos = m_client.transform.position + randPoint;

            Physics.Raycast(new Vector3(scaledPos.x, 0, scaledPos.y) + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity);
            hits[index] = hit;
            starts[index] = ground.point;
            goals[index] = hit.point - (rock.transform.localScale / 2);
        }

        //Lift up the rock
        while (m_isActive && time < duration)
        {
            for (int index = 0; index < arrayLength; index++)
            {
                GameObject rock = m_rockInstances[index];
                GameObject hazard = m_hazardAreaInstances[index];
                hazard.transform.position = Vector3.SmoothDamp(hazard.transform.position, new Vector3(goals[index].x, ground.point.y, goals[index].z), ref hazardVels[index], 0.2f);
                rock.transform.position = Vector3.Lerp(starts[index], starts[index] + Vector3.up * 5.0f, time / duration);
            }

            time += Time.deltaTime;
            yield return null;
        }

        for (int index = 0; index < arrayLength; index++)
        {
            starts[index] = m_rockInstances[index].transform.position;
            goals[index] = m_hazardAreaInstances[index].transform.position;

            rbs[index].isKinematic = false;
            rbs[index].velocity = CalculateVelocity(starts[index], goals[index]);
        }

        Vector3 lastPos = Vector3.zero;

        Transform lastRock = null;

        foreach (GameObject h in m_hazardAreaInstances)
        {
            if (lastPos == Vector3.zero)
            {
                lastRock = m_rockInstances[m_hazardAreaInstances.IndexOf(h)].transform;
                lastPos = h.transform.position;
                continue;
            }

            if (Vector3.Distance(lastPos, m_client.transform.position) < Vector3.Distance(h.transform.position, m_client.transform.position))
            {
                lastRock = m_rockInstances[m_hazardAreaInstances.IndexOf(h)].transform;
                lastPos = h.transform.position;
            }
        }

        yield return new WaitUntil(() => lastRock.position.y < lastPos.y);

        m_callback?.Invoke();
    }

    private Vector3 CalculateVelocity(Vector3 start, Vector3 goal)
    {
        //Debug.Log(goal.y + "\n" + start.y + "\n" + (goal.y - start.y));
        Vector3 vec = Vector3.zero;

        Vector3 horDist = new Vector3(goal.x - start.x, 0, goal.z - start.z);
        float mag = horDist.magnitude;

        float vertDist = goal.y - start.y;

        float time = mag / Mathf.Sqrt(-2 * Physics.gravity.y * Mathf.Abs(vertDist));
        float vy = (vertDist / time) - (0.5f * Physics.gravity.y * time);
        Vector3 horVec = horDist.normalized * (mag / time);

        vec = new Vector3(horVec.x, vy, horVec.z);

        return vec;
    }
}