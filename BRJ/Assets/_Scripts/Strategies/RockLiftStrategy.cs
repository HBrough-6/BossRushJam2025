using ChaseMorgan.Strategy;
using System.Collections;
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

    private GameObject m_rockInstance, m_hazardAreaInstance;
    public void Disable()
    {
        m_isActive = false;
        m_callback.RemoveAllListeners();
        m_rockInstance.transform.position = Vector3.zero;
        m_hazardAreaInstance.transform.position = Vector3.zero;
        m_rockInstance.SetActive(false);
        m_hazardAreaInstance.SetActive(false);
    }

    public void Execute(Client client, UnityAction callback = null)
    {
        m_isActive = true;
        m_callback.AddListener(callback);
        m_client.StartCoroutine(RockLift());
    }

    public RockLiftStrategy(Client client, Transform target, GameObject hazardAreaPrefab, GameObject rockPrefab)
    {
        m_client = client;
        m_target = target;
        m_hazardAreaPrefab = hazardAreaPrefab;
        m_rockPrefab = rockPrefab;

        m_rockInstance = Object.Instantiate(m_rockPrefab);
        m_rockInstance.SetActive(false);
        m_rockInstance.name = "Rock_Instance_" + GetType().Name;

        m_hazardAreaInstance = Object.Instantiate(m_hazardAreaPrefab);
        m_hazardAreaInstance.SetActive(false);
        m_hazardAreaInstance.name = "Hazard_Area_Instance_" + GetType().Name;
    }

    private IEnumerator RockLift()
    {      
        m_rockInstance.SetActive(true);
        Rigidbody rb = m_rockInstance.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        m_hazardAreaInstance.SetActive(true);
        Debug.Log("RockLift");
        float time = 0.0f;
        float duration = 2.0f;
        Vector3 goal = Vector3.zero;
        Physics.Raycast(m_client.transform.position + (m_client.transform.forward.normalized * 2.0f) + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity);
        Vector3 start = hit.point - (m_rockInstance.transform.localScale / 2);
        Vector3 hazardVel = Vector3.zero;

        //Lift up the rock
        while (m_isActive && time < duration)
        {
            goal = m_target.position;
            m_hazardAreaInstance.transform.position = Vector3.SmoothDamp(m_hazardAreaInstance.transform.position, new Vector3(goal.x, hit.point.y, goal.z), ref hazardVel, 0.2f);
            m_rockInstance.transform.position = Vector3.Lerp(start, start + (Vector3.up * 5.0f), time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        start = m_rockInstance.transform.position;
        goal = m_hazardAreaInstance.transform.position;

        rb.isKinematic = false;
        rb.velocity = CalculateVelocity(start, goal);

        yield return new WaitUntil(() => m_rockInstance.transform.position.y < goal.y);

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

    ~RockLiftStrategy()
    {
        Object.Destroy(m_hazardAreaInstance);
        Object.Destroy(m_rockInstance);
    }
}