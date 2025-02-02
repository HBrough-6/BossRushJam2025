using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;

    int maxStamina;

    public void SetMaxStamina(int maxStamina)
    {
        slider.maxValue = maxStamina;
        slider.value = maxStamina;
    }

    public void SetCurrentStamina(int currentStamina)
    {
        slider.value = currentStamina;
    }
}
