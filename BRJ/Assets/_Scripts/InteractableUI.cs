using System.Collections;
using TMPro;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    public TMP_Text interactableText;
    private GameObject PopUpText;

    private void Awake()
    {
        PopUpText = transform.GetChild(0).gameObject;
    }

    public void SetTextActive(bool active)
    {
        PopUpText.SetActive(active);
    }

    public void SetTextInactiveOnTimer(int time, string text)
    {
        interactableText.text = text;
        StartCoroutine(TextInactiveOnDelay(time));
    }

    private IEnumerator TextInactiveOnDelay(int time)
    {
        yield return new WaitForSeconds(time);
        SetTextActive(false);
    }
}
