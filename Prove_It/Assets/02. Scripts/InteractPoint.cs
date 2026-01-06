
using UnityEngine;

public class InteractPoint : MonoBehaviour
    
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactMask;
    [SerializeField] private GameObject interactButton;
    [SerializeField] private float range = 3f;

    private InteractUI currentUI;
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, interactMask))
        {
            InteractUI ui = hit.collider.GetComponentInParent<InteractUI>();
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

    void ClearCurrent()
    {
        if (currentUI != null)
        {
            currentUI.Hide();
            currentUI = null;
        }
    }
}
