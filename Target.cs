using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Start,
    Goal,
    Distractor
}

public class Target : MonoBehaviour
{
    private SpriteRenderer sprite;
    private bool onSelect;
    private TargetType targetType;
    private TargetManager targetManager;

    void Awake() // Use Awake instead of Start to initialize sprite earlier
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        targetManager = FindObjectOfType<TargetManager>(); // Find TargetManager instance
    }

    // Method to set the type of the target (Start, Goal, Distractor)
    public void SetTargetType(TargetType type)
    {
        targetType = type;
        UpdateColor();
        // Debug.Log($"Target type set to: {targetType}");
    }

    // Method to update the color of the target based on its type
    private void UpdateColor()
    {
        switch (targetType)
        {
            case TargetType.Start:
                sprite.color = Color.green;
                break;
            case TargetType.Goal:
                sprite.color = Color.red;
                break;
            case TargetType.Distractor:
                sprite.color = Color.gray;
                break;
        }
    }

    public void OnHoverEnter()
    {
        if (onSelect) return;
        sprite.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        if (onSelect) return;
        UpdateColor();
    }

    public void OnSelect()
    {
        onSelect = true;
        sprite.color = Color.green;
        Debug.Log($"Target selected: {targetType}");
        StartCoroutine(DestroyGameObject(0.1f));

        // If this is the Start target, call StartNextPhase from TargetManager
        if (targetType == TargetType.Start)
        {
            targetManager.StartNextPhase();
        }
        else if (targetType == TargetType.Goal)
        {
            targetManager.StartNewTrial();
        }
    }

    public IEnumerator DestroyGameObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}