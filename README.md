# Bubble Cursor Study

## Study Settings

In StudyBehavior, Awake() method - adjust numbers here if needed

## Point / Bubble Switch

- In StudyBehavior, Awake() method - comment / uncomment needed
- In CVSManager update file path - comment / uncomment needed
- Copy / paste needed cursor from Start Scene to Study Scene

## In CSV file when submitting delete duplicate header

## Scenes structure

### Start Scene

- Canvas
- GameManager (doesn't destroy on load)
- StudyBehavior
- BubbleCursor
- PointCursor

### Study Scene

- TargetManager
- StudyBehavior
- **Either** Bubble or Point Cursor, not both

### End Screen

- Canvas
- MenuManager
