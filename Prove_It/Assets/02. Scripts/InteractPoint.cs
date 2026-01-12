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
        if (Physics.Raycast(ray, out RaycastHit hit, range, interactMask))
        {
            //var ui = hit.collider.GetComponentInParent<InteractUI>();
            if (ui != currentUI)
            {
                ClearCurrent();
                currentUI = ui;
                currentUI?.Show();
            }
        }
        else
        {
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
