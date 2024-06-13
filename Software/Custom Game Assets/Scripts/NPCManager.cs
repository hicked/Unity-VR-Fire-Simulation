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
    public Vector3 lookat;
    public bool isSpotTaken;
}

[System.Serializable]
public class NPCLocationInfoList {
    public List<NPCLocationInfo> idleLocationsList;
}

public class NPCManager : MonoBehaviour {
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] public float NPCHeight = 1.75f;
    [SerializeField] public float NPCRadius = 0.3f;

    [SerializeField] private int timeChangeIdle = 15;
    [SerializeField] private int changeIdleVariance = 3;
    [SerializeField] private int timeBeforeMove = 60;
    [SerializeField] private int moveVariance = 30;

    [SerializeField] private float NPCWalkSpeedMultiplier = 1.5f;
    [SerializeField] private float NPCRunningSpeedMultiplier = 2.5f;
    [SerializeField] private float NPCTurnSpeed = 6f;

    [SerializeField] private int pointsBeforeOpenDoor = 4; // how many tiles away will the door open on an NPCs path
    [SerializeField] private float timeSpentOpenByNPC = 2f;

    [SerializeField] private float crossFadeDuration = 1f;

    [SerializeField] public bool onPhone;
    [SerializeField] public bool isMan;

    public bool isIdle = true;
    public bool isWalking = false;
    public bool isRunning = false;
    private GameObject lastDoorBlocked;


    [SerializeField] private TextAsset locationsJSON;

    [SerializeField] private static NPCLocationInfoList idleLocations = new NPCLocationInfoList();/* = new NPCLocationInfo[] {
        // Hallway
        new NPCLocationInfo(new Vector3(-36f, 0, -9.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-36f, 0, -6.1f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-28.69f, 0, -9.56f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-28.51f, 0, -6f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-13.9f, 0, -6f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-13.9f, 0, -9.6f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-5.3f, 0, -9.7f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-5.3f, 0, -6f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(1.4f, 0, -9.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(6f, 0, -5.9f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(2.75f, 0, -10.9f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(2.8f, 0, -23.5f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(6.3f, 0, -23.5f), new Vector3(0f, 0f, 0f), false),

        // Rooms 1-3
        new NPCLocationInfo(new Vector3(-25.3f, 0, 4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-22f, 0, 4.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-21f, 0, 2.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-21.2f, 0, -2.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-25.3f, 0, -4.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-27.8f, 0, 1.5f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-27.8f, 0, -1.4f), new Vector3(0f, 0f, 0f), false),

        new NPCLocationInfo(new Vector3(-14.3f, 0, 4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-11f, 0, 4.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-10f, 0, 2.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-10.2f, 0, -2.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-14.3f, 0, -4.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-16.8f, 0, 1.5f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-16.8f, 0, -1.4f), new Vector3(0f, 0f, 0f), false),

        new NPCLocationInfo(new Vector3(-3.3f, 0, 4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(0f, 0, 4.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(1f, 0, 2.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(0.8f, 0, -2.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-3.3f, 0, -4.2f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-5.8f, 0, 1.5f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-5.8f, 0, -1.4f), new Vector3(0f, 0f, 0f), false),

        // Rooms 4-6
        new NPCLocationInfo(new Vector3(-32.8f, 0, -13.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-28.5f, 0, -11.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-26f, 0, -17f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-28.7f, 0, -19.8f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-32f, 0, -19.9f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-33f, 0, -18.1f), new Vector3(0f, 0f, 0f), false),

        new NPCLocationInfo(new Vector3(-21.8f, 0, -13.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-17.5f, 0, -11.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-15f, 0, -17f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-17.7f, 0, -19.8f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-21f, 0, -19.9f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-22f, 0, -18.1f), new Vector3(0f, 0f, 0f), false),

        new NPCLocationInfo(new Vector3(-10.8f, 0, -13.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-6.5f, 0, -11.4f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-4f, 0, -17f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-6.7f, 0, -19.8f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-10f, 0, -19.9f), new Vector3(0f, 0f, 0f), false),
        new NPCLocationInfo(new Vector3(-11f, 0, -18.1f), new Vector3(0f, 0f, 0f), false)
    };*/

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
    private Vector3 lookatVector;

    //----------------------------------------------------------------------
    public string[] NPCIdleStatesW = new string[] {
                                        "Exercise_warmingUp_170f",
                                        "idle_f_1_150f",
                                        "idle_f_2_190f",
                                        "idle_phoneTalking_180f",
                                        "idle_selfcheck_1_300f"};
    public string[] NPCWalkingStatesW = new string[] {
                                        "locom_m_basicWalk_30f",
                                        "locom_f_phoneWalking_40f",
                                        "locom_f_slowWalk_40f"};
    public string[] NPCRunningStatesW = new string[] {
                                        "locom_m_jogging_30f",
                                        "locom_f_running_20f"};

    //----------------------------------------------------------------------
    public string[] NPCIdleStatesM = new string[] {
                                        "Exercise_warmingUp_170f",
                                        "idle_phoneTalking_180f",
                                        "idle_m_1_200f",
                                        "idle_m_2_220f"};
    public string[] NPCWalkingStatesM = new string[] {
                                        "locom_m_basicWalk_30f",
                                        "locom_m_phoneWalking_40f",
                                        "locom_m_slowWalk_40f"};

    public string[] NPCRunningStatesM = new string[] {
                                        "locom_m_jogging_30f",
                                        "locom_m_running_20f"};
    //----------------------------------------------------------------------

    AnimatorClipInfo[] animatorInfo;
    string currentAnimation;
    private List<Location> path;
    private int currentPathIndex;
    private AStarPathfinder pathfinder;
    private int locationIndex = 0;


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

            if (!isWalking && !isRunning && !pathfinder.isPathfinding) {
                Debug.Log("Changing location");
                moveToRandom();
            }
            yield return null;
        }
    }


    private void Start() {
        List<Vector3> firstIndexesList = new List<Vector3>();
        animator = GetComponent<Animator>();
        path = new List<Location>();
        pathfinder = GetComponent<AStarPathfinder>();
        setRandomIdle();

        StartCoroutine(IdleCoroutine());
        StartCoroutine(ChangeLocationCoroutine());
    }


    private void Update() {
        if (pathfinder != null) {
            path = pathfinder.GetPath();
        }

        if (path != null) {
            isWalking = true;
            isIdle = false;
            isRunning = false;
        }
        else if (path == null) {
            isWalking = false;
            isIdle = true;
            isRunning = false;
        }
        
        animatorInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (animatorInfo.Length == 1) {
            currentAnimation = animatorInfo[0].clip.name;
            // Makes sure we have to correct animation based on the state of the NPC

            if (isRunning && !(NPCRunningStatesM.Contains(currentAnimation) || NPCRunningStatesW.Contains(currentAnimation))) { setRandomRunning(); }
            else if (isWalking && !(NPCWalkingStatesM.Contains(currentAnimation) || NPCWalkingStatesW.Contains(currentAnimation))) { setRandomWalking(); }
            else if (isIdle && !(NPCIdleStatesM.Contains(currentAnimation) || NPCIdleStatesW.Contains(currentAnimation))) { setRandomIdle(); }

            MoveAlongPath(); // rotates the NPC every time it gets to a point on the path to face the next point on the path
            // Dont need to move NPC forward since that is automatically done above (isRunning
        }
    }


    public void changeState(string state) {
        if (!NPCStateNames.Contains(state)) { throw new UnityException("animation provided not found"); }
        animator.CrossFadeInFixedTime(state, crossFadeDuration, 0, Random.value * getAnimation(state).length);
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
            changeState(NPCWalkingStatesM[Random.Range(0, NPCWalkingStatesM.Length)]);
        }
        else {
            changeState(NPCWalkingStatesW[Random.Range(0, NPCWalkingStatesW.Length)]);
        }
    }

    public void setRandomRunning() {
        if (isMan) {
            changeState(NPCRunningStatesM[Random.Range(0, NPCRunningStatesM.Length)]);
        }
        else {
            changeState(NPCRunningStatesW[Random.Range(0, NPCRunningStatesW.Length)]);
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

    public void moveToRandom() {
        idleLocations = JsonUtility.FromJson<NPCLocationInfoList>(locationsJSON.text);
        int locationIndex = Random.Range(0, idleLocations.idleLocationsList.Count - 1);
        while (idleLocations.idleLocationsList[locationIndex].isSpotTaken) {
            Debug.Log("Spot taken, checking another");
            locationIndex = Random.Range(0, idleLocations.idleLocationsList.Count - 1);
        }

        Vector3 newLocation = idleLocations.idleLocationsList[locationIndex].position;
        lookatVector = idleLocations.idleLocationsList[locationIndex].lookat;

        setPathTo(newLocation);
    }


    private void setPathTo(Vector3 location) {
        StartCoroutine(pathfinder.FindPathCoroutine(transform.position, location));
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

        Vector3 targetPosition = new Vector3(path[currentPathIndex].x, transform.position.y, path[currentPathIndex].z); // next point along path to reach

        Vector3 direction = targetPosition - transform.position; // directional vector towards the point
        float distanceToTarget = direction.magnitude;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the target position
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * NPCTurnSpeed);


        // Move the NPC
        transform.position += direction.normalized * Time.deltaTime * (isRunning ? NPCRunningSpeedMultiplier : NPCWalkSpeedMultiplier);

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
                return;
            }
            currentPathIndex++;
        }

        for (int i = pointsBeforeOpenDoor; i >= 0; i--) {
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
}