using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// DELETE CURSOR THAT'S NOT NEEDED IN STUDY SCENE
// SWITCH BEHAVIOR TO TARGET CURSOR IN BOTH START AND STUDY SCENES

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private BubbleCursor bubbleCursor;
    private PointCursor pointCursor;
    private StudyBehavior studyBehavior;
    private int participantID;
    private void Awake()
    {
        bubbleCursor = FindObjectOfType<BubbleCursor>();
        //Debug.Log($"GN Awake: bubbleCursor: {bubbleCursor}");
        pointCursor = FindObjectOfType<PointCursor>();
        //Debug.Log($"GN Awake: pointCursor: {pointCursor}");

        studyBehavior = FindObjectOfType<StudyBehavior>();
        //CSVManager.SetFilePath(studyBehavior.StudySettings.cursorType.ToString());
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SetCursor(studyBehavior.StudySettings.cursorType);
    }

    public void SetCursor(CursorType cursor)
    {
        Debug.Log($"SetCursor: CursorType: {cursor}");

        // Deactivate both cursors initially
        if (bubbleCursor != null)
        {
            bubbleCursor.gameObject.SetActive(false);
        }

        if (pointCursor != null)
        {
            pointCursor.gameObject.SetActive(false);
        }

        // Activate the selected cursor
        if (cursor == CursorType.PointCursor)
        {
            Debug.Log("SetCursor: PointCursor selected");
            if (pointCursor != null)
            {
                pointCursor.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("PointCursor reference is null.");
            }
        }
        else if (cursor == CursorType.BubbleCursor)
        {
            Debug.Log("SetCursor: BubbleCursor selected");
            if (bubbleCursor != null)
            {
                bubbleCursor.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("BubbleCursor reference is null.");
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null);
        }
    }

    public void StartStudy()
    {
        if (inputField.text == string.Empty) return;
        participantID = int.Parse(inputField.text);
        studyBehavior.ParticipantID = participantID;

        SceneManager.LoadScene("StudyScene");
    }
}