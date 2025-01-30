using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class NPCLocationInfo {
    public Vector3 position;
    public bool isSpotTaken;
}

[System.Serializable]
public class NPCLocationInfoList {
    public List<NPCLocationInfo> idleLocationsList;
}

public class NPCManager : Audible {
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask fireDetectionLayer;
    [SerializeField] public float NPCHeight = 1.75f;
    [SerializeField] public float NPCRadius = 0.3f;

    [SerializeField] private int timeChangeIdle = 15;
    [SerializeField] private int changeIdleVariance = 3;
    [SerializeField] private int timeBeforeMove = 60;
    [SerializeField] private int moveVariance = 30;

    [SerializeField] private float NPCWalkSpeedMultiplier = 1.5f;
    [SerializeField] private float NPCRunningSpeedMultiplier = 5.5f;
    [SerializeField] private float NPCTurnSpeed = 6f;

    [SerializeField] private int pointsBeforeOpenDoor = 3; // how many tiles away will the door open on an NPCs path
    [SerializeField] private float timeSpentOpenByNPC = 1.5f;

    [SerializeField] private float crossFadeDuration = 1.5f;

    [SerializeField] public bool onPhone;
    [SerializeField] public bool isMan;

    public int NPCLocationIndex;
    public int previousNPCLocationIndex = -1; // set the value as negative since null isnt a thing
    public int tentativeNPCLocationIndex;

    public bool isCurrentlyPathfinding = false;
    public bool isIdle = true;
    public bool isWalking = false;
    public bool isRunning = false;
    private GameObject lastDoorBlocked;

    private IEnumerator activeMouvementCoroutine = null;

    [SerializeField] private string originalLocationsFilePath = "/Scripts/originalNPCLocations.json";
    [SerializeField] private string NPCLocationsFilePath = "/Scripts/NPCLocations.json";

    private string[] NPCStateNames = {"Exercise_warmingUp_170f",
                                        "idle_phoneTalking_180f",
                                        "idle_f_1_150f",
                                        "idle_f_2_190f",
                                        "idle_phoneTalking_180f",
                                        "idle_selfcheck_1_300f",
                                        "idle_m_1_200f",
                                        "idle_m_2_220f",
                                        "dance_afro_56f",
                                        "dance_f_flossing_48f",
                                        "dance_hype_100f",
                                        "dance_m_flossing_40f",
                                        "dance_riverdance_60f",
                                        "locom_m_jogging_30f",
                                        "locom_f_phoneWalking_40f",
                                        "locom_f_running_20f",
                                        "locom_f_slowWalk_40f",
                                        "locom_m_basicWalk_30f",
                                        "locom_m_phoneWalking_40f",
                                        "locom_m_running_20f",
                                        "locom_m_slowWalk_40f"};
    private AnimationClip[] clips;
    private Animator animator;

    [SerializeField] public AudioSource NPCAudioSource;
    [SerializeField] public AudioClip runAudio;
    [SerializeField] public AudioClip walkAudio;
    [SerializeField] public float walkOffset = 0f;
    [SerializeField] public float runOffset = 0f;
    [SerializeField] public GameObject fire;
    [SerializeField] public GameObject alarm;
    public bool panicked = false;

    //----------------------------------------------------------------------
    public string[] NPCIdleStatesW = new string[] {
        "Exercise_warmingUp_170f",
        "idle_f_1_150f",
        "idle_f_2_190f",
        "idle_phoneTalking_180f",
        "idle_selfcheck_1_300f"};

    //----------------------------------------------------------------------
    public string[] NPCIdleStatesM = new string[] {
        "Exercise_warmingUp_170f",
        "idle_phoneTalking_180f",
        "idle_m_1_200f",
        "idle_m_2_220f"};
    
    public Dictionary<string, float> NPCWalkingStates = new Dictionary<string, float>() {
        { "locom_m_basicWalk_30f", 0.5f },
        { "locom_m_phoneWalking_40f", 0.55f },
        { "locom_m_slowWalk_40f", 0.69f }
    };

    public Dictionary<string, float> NPCRunningStates = new Dictionary<string, float>() {
        { "locom_m_jogging_30f", 0.3f},
        { "locom_m_running_20f", 0.3f }
    };

    Dictionary<Vector3, Vector3[]> exitPaths = new Dictionary<Vector3, Vector3[]>() {
        {new Vector3(5f, 0f, -22.29f), new Vector3[] {
            new Vector3(-27f, 0f, -7.5f),
            new Vector3(-15.5f, 0f, -7.5f),
            new Vector3(-4f, 0f, -7.5f)
        }},
        {new Vector3(-35f, 0f, -9f), new Vector3[] {
            new Vector3(-4f, 0f, -7.5f),
            new Vector3(-15.5f, 0f, -7.5f),
            new Vector3(-27f, 0f, -7.5f)
        }},
    };
    int currentExitIndex = 0;
    Vector3 closestExit = new Vector3(0f, 0f, 0f);

    // public Vector3[] exitLocations = new Vector3[] {
    //     new Vector3(5f, 0f, -22.29f),
    //     new Vector3(-35f, 0f, -9f)
    // };
    //----------------------------------------------------------------------

    AnimatorClipInfo[] animatorInfo;
    string currentAnimation;
    private List<Location> path;
    private Vector3 lookatVector;
    private int currentPathIndex;
    private AStarMultithreaded pathfinder;

    private IEnumerator IdleCoroutine() {
        while (true) {
            int idleVar = Random.Range(-changeIdleVariance, changeIdleVariance);
            yield return new WaitForSeconds(timeChangeIdle + idleVar);

            if (isIdle && !isWalking && !isRunning) { // if it isnt idle, or is walking along a path, dont change idles
                setRandomIdle();
            }
            yield return null;
        }

    }
    private IEnumerator ChangeLocationCoroutine() {
        while (true) {
            int moveVar = Random.Range(-moveVariance, moveVariance);
            yield return new WaitForSeconds(timeBeforeMove + moveVar);

            if (isIdle && !isCurrentlyPathfinding) { // if it isnt idle, or is walking along a path, dont change idles
                moveToRandom();
            }
            yield return null;
        }
    }


    private void Start() {
        audioSource = NPCAudioSource;
        
        originalLocationsFilePath = Application.dataPath + originalLocationsFilePath;
        NPCLocationsFilePath = Application.dataPath + NPCLocationsFilePath;
        // Resets json to original
        string jsonContent = File.ReadAllText(originalLocationsFilePath);
        File.WriteAllText(NPCLocationsFilePath, jsonContent);

        animator = GetComponent<Animator>();
        path = new List<Location>();
        pathfinder = GetComponent<AStarMultithreaded>();
        setRandomIdle();

        StartCoroutine(IdleCoroutine());
        StartCoroutine(ChangeLocationCoroutine());
    }


    private void Update() {
        // Fire Detection
        //Debug.Log(this.transform.position);
        if (alarm.GetComponent<Alarm>().alarmSource.isPlaying && !panicked) {
            panicked = true;
            Debug.Log("NPC detected alarm");
            setPathToNearestExitCheckpoint();
        }

        if (fire.GetComponent<FireManager>().fireParticleSystem.isPlaying && !panicked) { // if fire is active, but NPC is not panicked
            RaycastHit fireHit; // hit info for raycast
            Debug.DrawRay(transform.position + new Vector3(0, NPCHeight/2, 0), fire.transform.position - transform.position, Color.red);
            if (Physics.CapsuleCast(transform.position + new Vector3(0, NPCHeight/2, 0), transform.position - new Vector3(0, NPCHeight/2, 0), NPCRadius, fire.transform.position - transform.position, out fireHit, Vector3.Distance(transform.position, fire.transform.position), fireDetectionLayer)) {
                if (fireHit.collider.gameObject == fire) {
                    panicked = true;
                    Debug.Log("NPC has LOS to fire");
                    setPathToNearestExitCheckpoint();
                }
            }
        }
        
        if (!isCurrentlyPathfinding && panicked) {
            if (Vector3.Distance(transform.position, closestExit) < 0.1f) {
                Destroy(gameObject);
            }
            else if (currentExitIndex >= exitPaths[closestExit].Length) { // reached the end of the path, just go to the exit
                setPathTo(closestExit);
            }
            else {
                setPathTo(exitPaths[closestExit][currentExitIndex]);
                currentExitIndex++;
            }
        }

        // Debug.Log($"{transform.position} and {transform.position + transform.forward}");
        if (pathfinder != null) {
            path = pathfinder.GetPath();
            lookatVector = pathfinder.lookatVector;
        }
        
        animatorInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (animatorInfo.Length == 1) {
            currentAnimation = animatorInfo[0].clip.name;
            // Makes sure we have to correct animation based on the state of the NPC
            if (isRunning && !(NPCRunningStates.ContainsKey(currentAnimation))) { //|| NPCRunningStates.ContainsKey(currentAnimation))) { 
                setRandomRunning();
                if (activeMouvementCoroutine != null) {StopCoroutine(activeMouvementCoroutine);}
                activeMouvementCoroutine = runCoroutine(NPCRunningStates[currentAnimation]);
                StartCoroutine(activeMouvementCoroutine);
            }
            else if (isWalking && !(NPCWalkingStates.ContainsKey(currentAnimation))) { //|| NPCWalkingStates.ContainsKey(currentAnimation))) { 
                setRandomWalking();
                if (activeMouvementCoroutine != null) {StopCoroutine(activeMouvementCoroutine);}
                activeMouvementCoroutine = walkCoroutine(NPCWalkingStates[currentAnimation]);
                StartCoroutine(activeMouvementCoroutine);
            }
            else if (isIdle && !(NPCIdleStatesM.Contains(currentAnimation) || NPCIdleStatesW.Contains(currentAnimation))) { 
                if (activeMouvementCoroutine != null) {
                    StopCoroutine(activeMouvementCoroutine);
                    activeMouvementCoroutine = null;
                    NPCAudioSource.Stop();
                }
                setRandomIdle();
            }

            if (panicked) {
                MoveAlongPath(true);
            }
            else { MoveAlongPath();}
            // rotates the NPC every time it gets to a point on the path to face the next point on the path
            // Dont need to move NPC forward since that is automatically done above (isRunning
        }
    }


// ====================================================================================================


    public void moveToRandom() {
        string jsonContent = File.ReadAllText(NPCLocationsFilePath);
        NPCLocationInfoList idleLocationsListObj = JsonUtility.FromJson<NPCLocationInfoList>(jsonContent);
        List<NPCLocationInfo> idleLocations = idleLocationsListObj.idleLocationsList;
        
        List<NPCLocationInfo> availableLocations = new List<NPCLocationInfo>();
        List<int> availableLocationsIndices = new List<int>();
        for (int i = 0; i < idleLocations.Count; i++) {
            if (!idleLocations[i].isSpotTaken) {
                availableLocations.Add(idleLocations[i]);
                availableLocationsIndices.Add(i);
            }
        }

        if (availableLocations.Count == 0) {
            Debug.LogError("WARNING: All NPC locations are taken!");
        }
        else {
            int randomIndex = Random.Range(0, availableLocations.Count - 1); // index of location within availableLocations
            tentativeNPCLocationIndex = availableLocationsIndices[randomIndex]; // index of location within idleLocations

            Vector3 newLocation = availableLocations[randomIndex].position;
            setPathTo(newLocation);
        }
    }

    public void ChangeLocationStatus() {
        string jsonContent = File.ReadAllText(NPCLocationsFilePath);
        NPCLocationInfoList idleLocationsListObj = JsonUtility.FromJson<NPCLocationInfoList>(jsonContent); // updates since pathfinding can take some time/values might change
        if (previousNPCLocationIndex != -1) { // if it is its initialized state (no previous location)
            idleLocationsListObj.idleLocationsList[previousNPCLocationIndex].isSpotTaken = false;
        }
        previousNPCLocationIndex = NPCLocationIndex;
        NPCLocationIndex = tentativeNPCLocationIndex;

        idleLocationsListObj.idleLocationsList[NPCLocationIndex].isSpotTaken = true;
        string updatedJson = JsonUtility.ToJson(idleLocationsListObj, true); // true: format
        File.WriteAllText(NPCLocationsFilePath, updatedJson); // Update the path as necessary
    }

    private void setPathToNearestExitCheckpoint() {
        List<Vector3> locationKeys = new List<Vector3>(exitPaths.Keys);
        closestExit = locationKeys[0];
        for (int i = 1; i < locationKeys.Count; i++) {
            if (Vector3.Distance(transform.position, locationKeys[i]) < Vector3.Distance(transform.position, closestExit)) {
                closestExit = locationKeys[i];
            }
        }

        Debug.Log("Closest Exit: " + closestExit);
        Vector3 closestPoint = exitPaths[closestExit][0];
        for (int i = 1; i < exitPaths[closestExit].Length; i++) {
            if (Vector3.Distance(transform.position, exitPaths[closestExit][i]) < Vector3.Distance(transform.position, closestPoint)) {
                closestPoint = exitPaths[closestExit][i];
            }
        }

        Debug.Log("Closest Point: " + closestPoint);
        currentExitIndex = new List<Vector3>(exitPaths[closestExit]).IndexOf(closestPoint);
    }
    private void setPathTo(Vector3 location) {
        //Debug.Log("Setting path to " + location);
        if (isCurrentlyPathfinding) {
            return;
        }
        isCurrentlyPathfinding = true;
        pathfinder.FindPath(transform.position, location);
        currentPathIndex = 0;
    }

    private void MoveAlongPath(bool run = false) { // default param of walking not running
        if (path == null || path.Count == 0 || currentPathIndex > path.Count - 1) {
            return;
        }

        if (!run) {
            isWalking = true;
            isIdle = false;
            isRunning = false;
        }
        else if (run) {
            isWalking = false;
            isIdle = false;
            isRunning = true;
        }

        
        Vector3 targetPosition = new Vector3(path[currentPathIndex].x, 0f, path[currentPathIndex].z); // next point along path to reach

        Vector3 direction = targetPosition - transform.position; // directional vector towards the point
        float distanceToTarget = direction.magnitude;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the target position
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * NPCTurnSpeed);

        // Move the NPC
        Vector3 movement = (direction.normalized * (isRunning ? NPCRunningSpeedMultiplier : NPCWalkSpeedMultiplier) * Time.deltaTime);
        transform.position += movement;
        
        
        // Check if the NPC is close enough to the target position
        if (distanceToTarget < 0.1f) {
            if (currentPathIndex == path.Count - 1) {
                // Smooth rotation of npc once reached the end location
                targetRotation = Quaternion.LookRotation(lookatVector - transform.position);
                StartCoroutine(RotateToFinalDirectionCoroutine(targetRotation));

                pathfinder.SetPath(null);
                isWalking = false;
                isIdle = true;
                isRunning = false;
                isCurrentlyPathfinding = false;
                return;
            }
            currentPathIndex++;
        }

        for (int i = run ? pointsBeforeOpenDoor*2:pointsBeforeOpenDoor; i >= 0; i--) {
            // Tries to open a door sum number of points on the path away, if it fails, reduces the number of point away by 1 and tried again
            // Does this until it reaches 0 which will be between the next point, and the previous point
            // Might be smart to change "pointsBeforeOpenDoor" actively with tile size, but for now it can be manual
            try {
                if (isBlockedByDoor(path[currentPathIndex + i - 1].vector + new Vector3(0, 1, 0), path[currentPathIndex + i].vector + new Vector3(0, 1, 0))) {
                    Doors door = lastDoorBlocked.GetComponent<Doors>();
                    door.OpenDoorTemporarily(timeSpentOpenByNPC);
                    break;
                }
            }
            catch {
                continue;
            }
        }
    }

    public bool isBlockedByDoor(Vector3 start, Vector3 end) {
        RaycastHit hitInfo = default(RaycastHit);
        bool hit = Physics.Linecast(start, end, out hitInfo, doorLayer);
        if (hit) {
            lastDoorBlocked = hitInfo.collider.gameObject;
        }
        return hit;
    }

    private IEnumerator RotateToFinalDirectionCoroutine(Quaternion finalTargetRotation) {
        while (true) {
            if (Quaternion.Angle(transform.rotation, finalTargetRotation) < 0.1f) {
                yield break;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, finalTargetRotation, Time.deltaTime * NPCTurnSpeed);
            yield return null; // Wait for the end of the frame
        }
    }


    public void changeState(string state) {
        if (!NPCStateNames.Contains(state)) { throw new UnityException("animation provided not found"); }
        animator.CrossFadeInFixedTime(state, crossFadeDuration, 0, Random.value * getAnimation(state).length);
        currentAnimation = state;
    }

    public void setRandomIdle() {
        if (isMan) {
            changeState(NPCIdleStatesM[Random.Range(0, NPCIdleStatesM.Length)]);
        }
        else {
            changeState(NPCIdleStatesW[Random.Range(0, NPCIdleStatesW.Length)]);
        }
    }

    public void setRandomWalking() {
        if (isMan) {
            changeState(NPCWalkingStates.Keys.ToList()[Random.Range(0, NPCWalkingStates.Count)]);
        }
        else {
            changeState(NPCWalkingStates.Keys.ToList()[Random.Range(0, NPCWalkingStates.Count)]);
        }
    }

    public void setRandomRunning() {
        if (isMan) {
            changeState(NPCRunningStates.Keys.ToList()[Random.Range(0, NPCRunningStates.Count)]);
        }
        else {
            changeState(NPCRunningStates.Keys.ToList()[Random.Range(0, NPCRunningStates.Count)]);
        }
    }

    private AnimationClip getAnimation(string name) {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == name) {
                return clip;
            }
        }
        throw new UnityException("Animation provided not found");
    }
    private IEnumerator walkCoroutine(float walkDelay) {
        yield return new WaitForSeconds(walkOffset);
        NPCAudioSource.clip = walkAudio;
        while (true) {
            yield return new WaitForSeconds(walkDelay);
            NPCAudioSource.Play();
        }
    }

    private IEnumerator runCoroutine(float runDelay) {
        yield return new WaitForSeconds(runOffset);
        NPCAudioSource.clip = runAudio;
        while (true) {
            yield return new WaitForSeconds(runDelay);
            NPCAudioSource.Play();
        }
    }
}