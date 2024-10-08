using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private BubbleCursor bubbleCursor;
    private StudyBehavior studyBehavior;
    private int participantID;
    private void Awake()
    {
        bubbleCursor = FindObjectOfType<BubbleCursor>();
        studyBehavior = FindObjectOfType<StudyBehavior>();
        CSVManager.SetFilePath(studyBehavior.StudySettings.cursorType.ToString());
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SetCursor(studyBehavior.StudySettings.cursorType);
    }
    public void SetCursor(CursorType cursor)
    {
        if (cursor == CursorType.PointCursor)
        {
            // If it's a Point Cursor, we can shrink the bubbleCursor to behave as a point
            bubbleCursor.gameObject.SetActive(false);  // Disable Bubble Cursor

            // You can create a simple PointCursor logic here if needed
        }
        else if (cursor == CursorType.BubbleCursor)
        {
            bubbleCursor.gameObject.SetActive(true);  // Enable Bubble Cursor
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

        // Load the next scene (StudyScene)
        SceneManager.LoadScene("StudyScene");
    }
}