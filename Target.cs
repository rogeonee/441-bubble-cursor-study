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
    private StudyBehavior studyBehavior;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        targetManager = FindObjectOfType<TargetManager>();
        studyBehavior = FindObjectOfType<StudyBehavior>();
    }

    // Set the type of the target (Start, Goal, Distractor)
    public void SetTargetType(TargetType type)
    {
        targetType = type;
        UpdateColor();
    }

    // Update the color based on its type
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

        if (targetType == TargetType.Start)
        {
            targetManager.StartNextPhase();  // Progress to the next phase if it's the start target
        }
        else if (targetType == TargetType.Distractor)
        {
            targetManager.OnMissClick();
        }
        else if (targetType == TargetType.Goal)
        {
            targetManager.OnGoalTargetSelected();  // Notify the TargetManager the goal target selected
        }
    }

    public IEnumerator DestroyGameObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}