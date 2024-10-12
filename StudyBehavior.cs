using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Serializable]
public class TrialConditions
{
    public float amplitude;
    public float targetSize;
    public float EWToW_Ratio;
}

public class StudySettings
{
    public List<float> targetSizes;
    public List<float> targetAmplitudes;
    public List<float> EWToW_Ratio;
    public CursorType cursorType;
}

public enum CursorType
{
    PointCursor = 0,
    BubbleCursor = 1
}

public class StudyBehavior : MonoBehaviour
{
    public static StudyBehavior Instance { get; private set; }
    public TrialConditions CurrentTrial => blockSequence[currentTrialIndex];
    public StudySettings StudySettings => studySettings;

    public int ParticipantID
    {
        get => participantID;
        set => participantID = value;
    }

    private int participantID;
    private int totalTrials;
    private StudySettings studySettings;
    [SerializeField] private int repetitions = 1;
    private List<TrialConditions> blockSequence = new();
    [SerializeField] private TargetManager targetManager;

    private float timer = 0f;
    private int misClick = 0;
    private int currentTrialIndex = 0;

    private string[] header =
    {
        "PID",
        "CT",
        "A",
        "W",
        "EWW",
        "MT",
        "MissedClicks"
    };

    private void Awake()
    {
        // Initialize studySettings with hardcoded values
        studySettings = new StudySettings
        {
            // cursorType = CursorType.PointCursor,               // Set cursor type
            cursorType = CursorType.BubbleCursor,
            targetSizes = new List<float> { 50f, 100f },       // Set target sizes
            targetAmplitudes = new List<float> { 200f, 400f }, // Set amplitudes
            EWToW_Ratio = new List<float> { 1f, 1.5f }         // Set EW/W ratios
        };

        CreateBlock();
        totalTrials = 5; // Set trials
    }

    private void Start()
    {
        Debug.Log("SB: Start - LogHeader");
        LogHeader();
        CreateBlock();
        targetManager = FindObjectOfType<TargetManager>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void NextTrial()
    {
        LogData();
        Debug.Log($"SB: NextTrial called, currentTrialIndex: {currentTrialIndex}");
        currentTrialIndex++;

        if (currentTrialIndex >= totalTrials)
        {
            Debug.Log("EndScene should be loaded.");
            SceneManager.LoadScene("EndScreen");
        }
        else
        {
            Debug.Log("Proceed to the next trial.");
            targetManager.StartNewTrial();  // Notify TargetManager to start a new trial
        }
    }

    private void CreateBlock()
    {
        for (int i = 0; i < repetitions; i++)
        {
            foreach (float EW in studySettings.EWToW_Ratio)
            {
                foreach (float size in studySettings.targetSizes)
                {
                    foreach (float amp in studySettings.targetAmplitudes)
                    {

                        blockSequence.Add(new TrialConditions()
                        {
                            amplitude = amp,
                            targetSize = size,
                            EWToW_Ratio = EW,
                        });
                    }
                }
            }
        }
        blockSequence = YatesShuffle(blockSequence);
    }

    private void LogHeader()
    {
        CSVManager.AppendToCSV(header);
    }

    private void LogData()
    {
        string[] data =
        {
            participantID.ToString(),
            studySettings.cursorType.ToString(),
            blockSequence[currentTrialIndex].amplitude.ToString(),
            blockSequence[currentTrialIndex].targetSize.ToString(),
            blockSequence[currentTrialIndex].EWToW_Ratio.ToString(),
            timer.ToString(),
            misClick.ToString()
        };
        CSVManager.AppendToCSV(data);
        timer = 0f;
        misClick = 0;
    }

    public void HandleMisClick()
    {
        misClick++;
    }

    public void SetParticipantID(int ID)
    {
        participantID = ID;
    }

    private static List<T> YatesShuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }
}
