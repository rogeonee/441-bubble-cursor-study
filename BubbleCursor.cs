using System.Collections.Generic;
using UnityEngine;

public class BubbleCursor : MonoBehaviour
{
    [SerializeField] private float defaultRadius = 0.5f; // Minimum radius
    [SerializeField] private float maxRadius = 5f;       // Maximum radius
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera mainCam;
    private List<Collider2D> detectedTargets = new();
    private Collider2D closestTarget;
    private Collider2D previousTarget;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Update cursor position based on mouse position
        UpdateCursorPosition();

        // Detect all targets within maxRadius range
        Physics2D.OverlapCircle(transform.position, maxRadius, contactFilter, detectedTargets);

        // Find the closest target within the detected targets
        FindClosestTarget();

        // Adjust the bubble cursor size based on the closest target
        AdjustCursorSize();

        // Handle mouse click selection
        HandleSelection();
    }

    private void UpdateCursorPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;  // Set to avoid occlusion by camera
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);
    }

    private void FindClosestTarget()
    {
        if (detectedTargets.Count == 0)
        {
            UnHoverPreviousTarget();
            closestTarget = null;
            return;
        }

        float closestDistance = float.MaxValue;
        Collider2D nearestTarget = null;

        foreach (var target in detectedTargets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestTarget = target;
            }
        }

        closestTarget = nearestTarget;

        // Highlight the closest target
        if (closestTarget != previousTarget)
        {
            UnHoverPreviousTarget();
            HoverTarget(closestTarget);
        }
    }

    private void AdjustCursorSize()
    {
        if (closestTarget != null)
        {
            // Get the distance to the closest target and adjust the radius accordingly
            float distanceToTarget = Vector2.Distance(transform.position, closestTarget.transform.position);
            float newRadius = Mathf.Clamp(distanceToTarget, defaultRadius, maxRadius);
            transform.localScale = Vector3.one * newRadius; // Scale the cursor based on the distance
        }
        else
        {
            // If no targets are detected, reset to default size
            transform.localScale = Vector3.one * defaultRadius;
        }
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) && closestTarget != null)
        {
            SelectTarget(closestTarget);
        }
    }

    private void HoverTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnHoverEnter();
        }
    }

    private void UnHoverPreviousTarget()
    {
        if (previousTarget != null && previousTarget.TryGetComponent(out Target target))
        {
            target.OnHoverExit();
        }
        previousTarget = closestTarget;
    }

    private void SelectTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnSelect();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
    }
}
