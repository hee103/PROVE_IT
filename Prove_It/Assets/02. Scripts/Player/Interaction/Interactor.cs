using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool CanInteract(GameObject interactor);
    void Interact(GameObject interactor);
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask interactMask;

    private IInteractable current;

    private void Awake()
    {
        if (origin == null) origin = Camera.main.transform;
    }

    private void Update()
    {
        current = Scan();
    }

    private IInteractable Scan()
    {
        if (Physics.Raycast(origin.position, origin.forward, out var hit, distance, interactMask, QueryTriggerInteraction.Collide))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract(gameObject))
                return interactable;
        }
        return null;
    }

    public void OnInteract()
    {
        if (!enabled) return;
        current?.Interact(gameObject);
    }
}
