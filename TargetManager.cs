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

    // Spawn the start target at the center of screen
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
        // Debug.Log("TM: OnGoalTargetSelected, delegating to SB");
        studyBehavior.NextTrial();  // StudyBehavior decides if the next trial should start
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

    // Spawn trial targets including the goal and distractors
    public void SpawnTrialTargets()
    {
        // Clear previous targets
        ClearTargets();

        // Spawn the goal target first at a random position
        Vector3 goalPosition = GenerateSinglePoint();
        GameObject goalTarget = Instantiate(targetPrefab, goalPosition, Quaternion.identity, transform);
        Target goalTargetScript = goalTarget.GetComponent<Target>();
        goalTargetScript.SetTargetType(TargetType.Goal);
        targetList.Add(goalTargetScript);
        Debug.Log($"Goal target spawned at position: {goalPosition}");

        // Place four distractors around the goal target
        float minOffset = 1.5f; // Min distance from the goal
        float maxOffset = 3.0f; // Max distance from the goal
        float offset = Random.Range(minOffset, maxOffset); // Random offset

        // Calculate positions for the four distractors
        Vector3[] fixedDistractorPositions = new Vector3[4];
        fixedDistractorPositions[0] = goalPosition + new Vector3(0, offset, 0);    // North
        fixedDistractorPositions[1] = goalPosition + new Vector3(0, -offset, 0);   // South
        fixedDistractorPositions[2] = goalPosition + new Vector3(offset, 0, 0);    // East
        fixedDistractorPositions[3] = goalPosition + new Vector3(-offset, 0, 0);   // West

        // Instantiate fixed distractors at the calculated positions
        for (int i = 0; i < fixedDistractorPositions.Length; i++)
        {
            Vector3 distractorPosition = fixedDistractorPositions[i];

            // Ensure distractor positions are within the screen bounds
            Vector3 screenPos = mainCamera.WorldToScreenPoint(distractorPosition);
            if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
            {
                // Debug.LogWarning($"Distractor position {i} is out of bounds, skipping.");
                continue; // Skip out-of-bounds distractors
            }

            GameObject distractorTarget = Instantiate(targetPrefab, distractorPosition, Quaternion.identity, transform);
            Target distractorScript = distractorTarget.GetComponent<Target>();
            distractorScript.SetTargetType(TargetType.Distractor);
            targetList.Add(distractorScript);
            // Debug.Log($"Distractor target spawned at position: {distractorPosition}");
        }

        // Generate additional random distractors if numDistractors is greater than 4
        int additionalDistractors = numDistractors - 4;
        if (additionalDistractors > 0)
        {
            List<Vector3> randomPoints = GenerateRandomPoints(additionalDistractors);
            foreach (Vector3 randomPoint in randomPoints)
            {
                GameObject distractorTarget = Instantiate(targetPrefab, randomPoint, Quaternion.identity, transform);
                Target distractorScript = distractorTarget.GetComponent<Target>();
                distractorScript.SetTargetType(TargetType.Distractor);
                targetList.Add(distractorScript);
                // Debug.Log($"Additional distractor spawned at position: {randomPoint}");
            }
        }
    }

    private Vector3 GenerateSinglePoint()
    {
        Vector3 randomWorldPoint;

        // Radius threshold to prevent overlap, set based on target size
        float minDistanceBetweenTargets = 2.0f; // Adjust this value depending on the size of your targets

        // Attempt to find a valid non-overlapping position
        bool validPointFound = false;
        int maxAttempts = 50; // Prevent infinite loops by limiting attempts
        int attempts = 0;

        do
        {
            float randomX = Random.Range(0, Screen.width);
            float randomY = Random.Range(0, Screen.height);
            float z = 10f; // Set z to avoid camera occlusion

            Vector3 randomScreenPoint = new Vector3(randomX, randomY, z);
            randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);

            // Check if this point overlaps with any existing point
            validPointFound = true; // Assume valid unless overlap is found
            foreach (var target in targetList)
            {
                float distance = Vector3.Distance(randomWorldPoint, target.transform.position);
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
            Debug.LogWarning($"Could not find non-overlapping position after {maxAttempts} attempts. Returning last generated point.");
        }

        return randomWorldPoint;
    }

    // Generate random points on the screen for targets
    List<Vector3> GenerateRandomPoints(int numberOfPoints)
    {
        List<Vector3> pointList = new();

        // Radius threshold to prevent overlap, set based on target size
        float minDistanceBetweenTargets = 1.5f; // Adjust this value depending on the size of your targets

        for (int i = 0; i < numberOfPoints; i++)
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

                Vector3 randomScreenPoint = new Vector3(randomX, randomY, z);
                randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);

                // Check if this point overlaps with any existing point
                validPointFound = true; // Assume valid unless overlap is found
                foreach (var target in targetList)
                {
                    float distance = Vector3.Distance(randomWorldPoint, target.transform.position);
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
            // Debug.Log($"Generated point {i}: {randomWorldPoint}");
        }

        return pointList;
    }
}