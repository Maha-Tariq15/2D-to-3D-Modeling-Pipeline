using UnityEngine;
using UnityEngine.InputSystem;

public class PickupSystem : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform holdPoint;
    public float pickupRadius = 1.5f;

    [Header("Animation")]
    public Animator animator;
    public string pickupTriggerName = "Pickup";

    private GameObject heldObject;
    private GameObject pendingPickup;
    private bool isPickingUp;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // 🚫 Block ALL input while pickup animation is running
            if (isPickingUp)
                return;

            if (heldObject == null)
                TryPickup();
            else
                Drop();
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Pickup"))
            {
                pendingPickup = hit.gameObject;
                isPickingUp = true;

                animator.ResetTrigger(pickupTriggerName);
                animator.SetTrigger(pickupTriggerName);

                break;
            }
        }
    }

    // 🔥 CALLED BY ANIMATION EVENT (HAND TOUCHES OBJECT)
    public void AttachPickup()
    {
        if (pendingPickup == null)
            return;

        heldObject = pendingPickup;
        pendingPickup = null;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    // 🔥 CALLED BY ANIMATION EVENT (END OF ANIMATION)
    public void FinishPickup()
    {
        isPickingUp = false;
    }

    void Drop()
    {
        if (heldObject == null)
            return;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        heldObject.transform.SetParent(null);
        heldObject = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
