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

        if (Physics.Raycast(ray, out RaycastHit hit, range, interactMask, QueryTriggerInteraction.Collide))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

            // 1. 무엇에 맞았는지 이름 확인
            Debug.Log($"Hit Object: {hit.collider.name}");

            var hitUI = hit.collider.GetComponentInParent<InteractUI>();

            // 2. UI 컴포넌트를 찾았는지 확인
            if (hitUI == null)
            {
                Debug.LogWarning("Raycast hit something, but no InteractUI found in parents!");
            }

            if (hitUI != currentUI)
            {
                ClearCurrent();
                currentUI = hitUI;
                currentUI?.Show();
            }
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
