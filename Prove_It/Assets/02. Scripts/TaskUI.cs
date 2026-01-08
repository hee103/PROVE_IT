using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private Image gauge;
    [SerializeField] private float fillDuration = 30f;
    [SerializeField] private AlienController alienController;

    private float timer = 0f;
    private bool penaltyApplied = false;

    private void Start()
    {
        timer = 0f;
        gauge.fillAmount = 0f;
    }

    private void Update()
    {
        if (penaltyApplied) return;

        timer += Time.deltaTime;
        gauge.fillAmount = Mathf.Clamp01(timer / fillDuration);

        if (gauge.fillAmount >= 1f)
        {
            penaltyApplied = true;
            alienController.Penalty();
        }
    }
}
