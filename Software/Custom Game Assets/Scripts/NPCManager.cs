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

    [SerializeField] public float maxRandomMoveDistance = 20f;

    public int NPCLocationIndex;
    public int previousNPCLocationIndex = -1; // set the value as negative since null isnt a thing
    public int tentativeNPCLocationIndex;
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
        "idle_f_1_150f",
        "idle_f_2_190f",
        "idle_phoneTalking_180f",
        "idle_selfcheck_1_300f"};

    //----------------------------------------------------------------------
    public string[] NPCIdleStatesM = new string[] {
        "Exercise_warmingUp_170f",
        "idle_phoneTalking_180f",
        "idle_m_1_200f",
        "idle_m_2_220f",
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

    //----------------------------------------------------------------------

    private Vector3[] outsideRoomLocations = new Vector3[] {
        new Vector3(-27f, 0f, -7.5f),
        new Vector3(-15.5f, 0f, -7.5f),
        new Vector3(-4f, 0f, -7.5f)
    };
    private Vector3 closestOutsideRoomLocation;

    private List<List<Vector3>> exitPaths = new List<List<Vector3>> {
        new List<Vector3>{
            new Vector3(-27f, 0f, -7.5f),
            new Vector3(-15.5f, 0f, -7.5f),
            new Vector3(-3.5f, 0f, -7.5f),
            new Vector3(4.5f, 0f, -7.5f),
            new Vector3(4.5f, 0f, -28f)
        },
        new List<Vector3> {
            new Vector3(4.5f, 0f, -7.5f),
            new Vector3(-3.5f, 0f, -7.5f),
            new Vector3(-15.5f, 0f, -7.5f),
            new Vector3(-27f, 0f, -7.5f),
            new Vector3(-41, 0f, -7.5f),
        }
    };
    private List<Vector3> closestExitPath;

    //----------------------------------------------------------------------

    AnimatorClipInfo[] animatorInfo;
    string currentAnimation;
    [SerializeField] public List<Location> path;
    public List<List<Vector3>> pathfindingRequestsQueue = new List<List<Vector3>>(); // queue of pathfinding requests
    public Vector3 lookatVector;
    public int currentPathIndex;
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

            if (!panicked) { // if it isnt idle, or is walking along a path, dont change idles
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
        
        if (pathfindingRequestsQueue.Count > 0 && !pathfinder.isPathfinding) { // if there are pathfinding requests in the queue, and the pathfinder is not currently pathfinding
            List<Vector3> pathfindingRequest = pathfindingRequestsQueue.First();
            pathfinder.FindPath(pathfindingRequest.First(), pathfindingRequest.Last());
            pathfindingRequestsQueue.RemoveAt(0);
        }

        if (fire.GetComponent<FireManager>().fireParticleSystem.isPlaying && !panicked) { // if fire is active, but NPC is not panicked
            RaycastHit fireHit; // hit info for raycast
            if (Physics.CapsuleCast(transform.position + new Vector3(0, NPCHeight/2, 0), transform.position - new Vector3(0, NPCHeight/2, 0), NPCRadius, fire.transform.position - transform.position, out fireHit, Vector3.Distance(transform.position, fire.transform.position), fireDetectionLayer)) {
                if (fireHit.collider.gameObject == fire) {
                    panicked = true;
                    Debug.Log("NPC has LOS to fire");
                }
            }
        }

        if (alarm.GetComponent<Alarm>().isPlaying && !panicked) {
            panicked = true;
            Debug.Log("NPC detected alarm");
        }
        
         // Fire Detection
        if (panicked && closestExitPath == null) { //|| (isExiting && path == null && !isInQueue)) { // if panicked and is ending path
            closestExitPath = exitPaths.First();
            for (int i = 1; i < exitPaths.Count; i++) {
                List<Vector3> exitPath = exitPaths[i];
                if (Vector3.Distance(transform.position, exitPath.Last()) < Vector3.Distance(transform.position, closestExitPath.Last())) {
                    closestExitPath = exitPaths[i];
                }
            }
            Debug.Log(closestExitPath.Last());
            setPathOutsideRoom();

            Vector3 closestStartingPoint = closestExitPath.First();
            for (int i = 1; i < closestExitPath.Count; i++) {
                if (Vector3.Distance(closestOutsideRoomLocation, closestExitPath[i]) < Vector3.Distance(closestOutsideRoomLocation, closestStartingPoint)) {
                    closestStartingPoint = closestExitPath[i];
                }
            }
            Debug.Log("Closest starting point: " + closestStartingPoint);
            int startingIndex = closestExitPath.IndexOf(closestStartingPoint);

            setPathTo(closestExitPath[startingIndex], closestOutsideRoomLocation);
            for (int i = startingIndex; i < closestExitPath.Count - 1; i++) {
                setPathTo(closestExitPath[i+1], closestExitPath[i]);
            }
            Debug.Log("NPC is moving to exit");
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
                if (Vector3.Distance(transform.position, idleLocations[i].position) < maxRandomMoveDistance) {
                    availableLocations.Add(idleLocations[i]);
                    availableLocationsIndices.Add(i);
                }
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


    private void setPathTo(Vector3 location, Vector3? curPosition = null) {
        //Debug.Log("Setting path to " + location);
        if (pathfindingRequestsQueue.Count > 0) { // if has already queued a pathfinding request, or is currently pathfinding
            List<Vector3> lastPathfindingRequest = pathfindingRequestsQueue.Last();
            Vector3 lastPathfindingRequestEndLocation = lastPathfindingRequest.ElementAt(1);
            pathfindingRequestsQueue.Add(new List<Vector3> { lastPathfindingRequestEndLocation, location });
        }
        else if (pathfinder.isPathfinding) {
            pathfindingRequestsQueue.Add(new List<Vector3> { path.Last().vector, location });
        }
        else {
            Vector3 startPosition = curPosition ?? transform.position;
            pathfindingRequestsQueue.Add(new List<Vector3> { startPosition, location });
        }
    }

    private void MoveAlongPath(bool run = false) { // default param of walking not running
        if (path == null || path.Count == 0) {
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

        Location nextLocation = path.ElementAt(0); 
        Vector3 targetPosition = nextLocation.vector; // next point along path to reach

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
            if (path.Count == 1 && !pathfinder.isPathfinding) { // if it is the last point on the path
                // Smooth rotation of npc once reached the end location
                if (panicked) {
                    Destroy(gameObject);
                }
                // Debug.Log("lookatVector: " + lookatVector);
                
                targetRotation = Quaternion.LookRotation(lookatVector);
                StartCoroutine(RotateToFinalDirectionCoroutine(targetRotation));

                isWalking = false;
                isIdle = true;
                isRunning = false;
                path.RemoveAt(0);
            }
            else {
                path.RemoveAt(0);
            }
        }

        for (int i = run ? pointsBeforeOpenDoor*2:pointsBeforeOpenDoor; i >= 0; i--) {
            // Tries to open a door sum number of points on the path away, if it fails, reduces the number of point away by 1 and tried again
            // Does this until it reaches 0 which will be between the next point, and the previous point
            // Might be smart to change "pointsBeforeOpenDoor" actively with tile size, but for now it can be manual
            try {
                if (isBlockedByDoor(path[i - 1].vector + new Vector3(0, 1, 0), path[i].vector + new Vector3(0, 1, 0))) {
                    Doors door = lastDoorBlocked.GetComponent<Doors>();
                    door.OpenDoorTemporarily(timeSpentOpenByNPC * (run ? 1.5f : 1f));
                    break;
                }
            }
            catch {
                continue;
            }
        }
    }

    private void setPathOutsideRoom() {
        // // do raycast around the room until it finds a door that is unlocked
        // GameObject closestDoor = null;
        // RaycastHit hitInfo;
        // List<Vector3> directions = new List<Vector3> {
        //     //every 10 degrees
        //     new Vector3(0, 0, 1),
        //     new Vector3(0.1736482f, 0, 0.9848078f),
        //     new Vector3(0.3420201f, 0, 0.9396926f),
        //     new Vector3(0.5f, 0, 0.8660254f),
        //     new Vector3(0.6427876f, 0, 0.7660444f),
        //     new Vector3(0.7660444f, 0, 0.6427876f),
        //     new Vector3(0.8660254f, 0, 0.5f),
        //     new Vector3(0.9396926f, 0, 0.3420201f),
        //     new Vector3(0.9848078f, 0, 0.1736482f),
        //     new Vector3(1, 0, 0),
        //     new Vector3(0.9848078f, 0, -0.1736482f),
        //     new Vector3(0.9396926f, 0, -0.3420201f),
        //     new Vector3(0.8660254f, 0, -0.5f),
        //     new Vector3(0.7660444f, 0, -0.6427876f),
        //     new Vector3(0.6427876f, 0, -0.7660444f),
        //     new Vector3(0.5f, 0, -0.8660254f),
        //     new Vector3(0.3420201f, 0, -0.9396926f),
        //     new Vector3(0.1736482f, 0, -0.9848078f),
        //     new Vector3(0, 0, -1),
        //     new Vector3(-0.1736482f, 0, -0.9848078f),
        //     new Vector3(-0.3420201f, 0, -0.9396926f),
        //     new Vector3(-0.5f, 0, -0.8660254f),
        //     new Vector3(-0.6427876f, 0, -0.7660444f),
        //     new Vector3(-0.7660444f, 0, -0.6427876f),
        //     new Vector3(-0.8660254f, 0, -0.5f),
        //     new Vector3(-0.9396926f, 0, -0.3420201f),
        //     new Vector3(-0.9848078f, 0, -0.1736482f),
        //     new Vector3(-1, 0, 0),
        //     new Vector3(-0.9848078f, 0, 0.1736482f),
        //     new Vector3(-0.9396926f, 0, 0.3420201f),
        //     new Vector3(-0.8660254f, 0, 0.5f),
        //     new Vector3(-0.7660444f, 0, 0.6427876f),
        //     new Vector3(-0.6427876f, 0, 0.7660444f),
        //     new Vector3(-0.5f, 0, 0.8660254f),
        //     new Vector3(-0.3420201f, 0, 0.9396926f),
        //     new Vector3(-0.1736482f, 0, 0.9848078f)
        // };
        // foreach (Vector3 direction in directions) {
        //     if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), direction, out hitInfo, 100f, doorLayer)) {
        //         if (hitInfo.collider.gameObject.GetComponent<Doors>().handleScript.isLocked) {
        //             closestDoor = hitInfo.collider.gameObject;
        //             break;
        //         }
        //     }
        // }

        // if (closestDoor != null) {
        //     closestOutsideRoomLocation = outsideRoomLocations[0];
        //     for (int i = 1; i < outsideRoomLocations.Length; i++) {
        //         if (Vector3.Distance(closestDoor.transform.position, outsideRoomLocations[i]) < Vector3.Distance(closestDoor.transform.position, closestOutsideRoomLocation)) {
        //             closestOutsideRoomLocation = outsideRoomLocations[i];
        //         }
        //     }

        //     setPathTo(closestOutsideRoomLocation);
        // } else {
            // find the nearest location to the npc
            closestOutsideRoomLocation = outsideRoomLocations[0];
            for (int i = 1; i < outsideRoomLocations.Length; i++) {
                if (Vector3.Distance(transform.position, outsideRoomLocations[i]) < Vector3.Distance(transform.position, closestOutsideRoomLocation)) {
                    closestOutsideRoomLocation = outsideRoomLocations[i];
                }
            }
            
            setPathTo(closestOutsideRoomLocation);
        // }
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