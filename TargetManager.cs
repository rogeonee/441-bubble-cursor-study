using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private int numDistractors;
    [SerializeField] private StudyBehavior studyBehavior;

    private List<Target> targetList = new();
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        SpawnStartTarget(); // Initially spawn the start target
    }

    // Spawn the start target at the center of the screen
    public void SpawnStartTarget()
    {
        // Clear previous targets
        ClearTargets();

        // Spawn start target at center of screen
        Vector3 centerPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10f));
        GameObject startTarget = Instantiate(targetPrefab, centerPosition, Quaternion.identity, transform);
        Target targetScript = startTarget.GetComponent<Target>();
        targetScript.SetTargetType(TargetType.Start);
        targetList.Add(targetScript);
        // Debug.Log("Start target spawned at center position.");
    }

    public void StartNextPhase()
    {
        SpawnTrialTargets();
    }

    public void StartNewTrial()
    {
        Debug.Log("TM: StartNewTrial fired");
        ClearTargets();
        SpawnStartTarget();
    }

    public void OnGoalTargetSelected()
    {
        Debug.Log("TM: OnGoalTargetSelected, delegating to SB");
        studyBehavior.NextTrial();  // StudyBehavior decides if the next trial should start or the study should end
    }

    // Spawn trial targets including the goal and distractors
    public void SpawnTrialTargets()
    {
        // Clear previous targets
        ClearTargets();

        // Generate positions for all targets (goal + distractors)
        List<Vector3> points = GenerateRandomPoints();
        // Debug.Log($"Number of points generated: {points.Count}");

        for (int i = 0; i < points.Count; i++)
        {
            GameObject newTarget = Instantiate(targetPrefab, points[i], Quaternion.identity, transform);
            Target targetScript = newTarget.GetComponent<Target>();

            if (i == 0) // The first target is the goal target
            {
                targetScript.SetTargetType(TargetType.Goal);
                // Debug.Log($"Goal target spawned at position: {points[i]}");
            }
            else // Remaining targets are distractors
            {
                targetScript.SetTargetType(TargetType.Distractor);
                // Debug.Log($"Distractor target spawned at position: {points[i]}");
            }

            targetList.Add(targetScript);
        }
    }


    // Clear all existing targets from the scene
    private void ClearTargets()
    {
        foreach (var target in targetList)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
            }
        }
        targetList.Clear();
    }

    // Generate random points on the screen for targets
    List<Vector3> GenerateRandomPoints()
    {
        List<Vector3> pointList = new();
        int numTargets = numDistractors + 1; // +1 to include the goal target

        // Radius threshold to prevent overlap, set based on target size
        float minDistanceBetweenTargets = 1.0f; // Adjust this value depending on the size of your targets

        for (int i = 0; i < numTargets; i++)
        {
            Vector3 randomWorldPoint;

            // Attempt to find a valid non-overlapping position
            bool validPointFound = false;
            int maxAttempts = 100; // Prevent infinite loops by limiting attempts
            int attempts = 0;

            do
            {
                float randomX = Random.Range(0, Screen.width);
                float randomY = Random.Range(0, Screen.height);
                float z = 10f; // Set z to avoid camera occlusion

                Vector3 randomScreenPoint = new(randomX, randomY, z);
                randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);

                // Check if this point overlaps with any existing point
                validPointFound = true; // Assume valid unless overlap is found
                foreach (var point in pointList)
                {
                    float distance = Vector3.Distance(randomWorldPoint, point);
                    if (distance < minDistanceBetweenTargets)
                    {
                        validPointFound = false; // Point is too close to an existing one
                        break;
                    }
                }

                attempts++;

            } while (!validPointFound && attempts < maxAttempts);

            if (!validPointFound)
            {
                Debug.LogWarning($"Could not find non-overlapping position after {maxAttempts} attempts. Continuing with remaining points.");
            }

            pointList.Add(randomWorldPoint);
            Debug.Log($"Generated point {i}: {randomWorldPoint}");
        }

        return pointList;
    }


}