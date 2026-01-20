using UnityEngine;

public class InteractPoint : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactMask;
    [SerializeField] private float range = 3f;
    [SerializeField] private InteractUI ui;

    [SerializeField] private AlienController alienController;

    private InteractUI currentUI;

    private void OnEnable()
    {
        alienController.InteractPressed += TryInteract;
    }

    private void OnDisable()
    {
        alienController.InteractPressed -= TryInteract;
    }

    private void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

        // 레이 시각화 (맞으면 초록, 안 맞으면 빨강)
        if (Physics.Raycast(ray, out RaycastHit hit, range, interactMask, QueryTriggerInteraction.Collide))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

            var hitUI = hit.collider.GetComponentInParent<InteractUI>();
            Debug.Log(hitUI);

            if (hitUI != currentUI)
            {
                ClearCurrent();
                currentUI = hitUI;
                currentUI?.Show();
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red);
            ClearCurrent();
        }
    }

    private void TryInteract()
    {
        if (currentUI == null) return;
        currentUI.Interact();
    }

    private void ClearCurrent()
    {
        if (currentUI == null) return;
        currentUI.Hide();
        currentUI = null;
    }
}
