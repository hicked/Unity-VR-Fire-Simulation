using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAI : MonoBehaviour {
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] public float NPCHeight = 1.75f;
    [SerializeField] public float NPCRadius = 0.3f;
    [SerializeField] private float hitDistanceDoors = 1f;
    [SerializeField] private int timeChangeIdle = 15;
    [SerializeField] private int changeIdleVariance = 3;
    [SerializeField] private int timeBeforeMove = 30;
    [SerializeField] private int moveVariance = 5;
    [SerializeField] private float NPCWalkSpeedMultiplier = 1.5f;
    [SerializeField] private float NPCRunningSpeedMultiplier = 2.5f;
    [SerializeField] private float NPCTurnSpeed = 100f;
    [SerializeField] private float crossFadeDuration = 1f;
    [SerializeField] public bool onPhone;
    [SerializeField] public bool isMan;
    [SerializeField] private int pointsBeforeOpenDoor = 3; // how many tiles away will the door open on an NPCs path
    public bool isIdle = true;
    public bool isWalking = false;
    public bool isRunning = false;
    private GameObject lastDoorBlocked;

    private class NPCLocationInfo {
        public Vector3 position;
        public Vector3 lookAt;
        public bool isSpotTaken;

        public NPCLocationInfo(Vector3 pos, Vector3 look, bool isTaken) {
            position = pos;
            lookAt = look;
            isSpotTaken = isTaken;
        }
    }

    [SerializeField] private static NPCLocationInfo[] idleLocations = new NPCLocationInfo[] { // must be static to keep track of if the spots are taken/not, shared across all instances of NPCs
            new NPCLocationInfo(new Vector3(6f, 0 ,-23.2f), new Vector3(5.35f, 0 ,-22.5f), false),
            new NPCLocationInfo(new Vector3(6f, 0, -6.3f), new Vector3(5.3f, 0, -7f), false),
            new NPCLocationInfo(new Vector3(1.14f, 0, -9.7f), new Vector3(0.6f, 0, -8.87f), false),
            new NPCLocationInfo(new Vector3(1.34f, 0, 3.92f), new Vector3(0.68f, 0, 3.17f), false),
            new NPCLocationInfo(new Vector3(-4.14f, 0, 1.45f), new Vector3(-3.45f, 0, 0.72f), false),
            new NPCLocationInfo(new Vector3(2.11f, 0, -1.8f), new Vector3(2.11f, 0, -1.8f), false),
            new NPCLocationInfo(new Vector3(-3.41f, 0, -19.43f), new Vector3(-4.26f, 0, -18.91f), false),
            new NPCLocationInfo(new Vector3(-11.12f, 0, -19.61f), new Vector3(-10.53f, 0, -18.8f), false),
            new NPCLocationInfo(new Vector3(-10.86f, 0, -11.52f), new Vector3(-10.13f, 0, -12.2f), false),
            new NPCLocationInfo(new Vector3(-8.08f, 0, -14.92f), new Vector3(-7.35f, 0, -15.6f), false),
            new NPCLocationInfo(new Vector3(-7f, 0, -16.22f), new Vector3(-7.69f, 0, -15.5f), false),
            new NPCLocationInfo(new Vector3(-5.34f, 0, -11.35f), new Vector3(-5.08f, 0, -12.32f), false),
            new NPCLocationInfo(new Vector3(-23.12f, 0, -19.61f), new Vector3(-22.53f, 0, -18.8f), false),
            new NPCLocationInfo(new Vector3(-22.86f, 0, -11.52f), new Vector3(-22.13f, 0, -12.2f), false),
            new NPCLocationInfo(new Vector3(-20.08f, 0, -14.92f), new Vector3(-19.35f, 0, -15.6f), false),
            new NPCLocationInfo(new Vector3(-19f, 0, -16.22f), new Vector3(-19.69f, 0, -15.5f), false),
            new NPCLocationInfo(new Vector3(-17.34f, 0, -11.35f), new Vector3(-17.08f, 0, -12.32f), false),
            new NPCLocationInfo(new Vector3(-14f, 0, -9.46f), new Vector3(-14f, 0, -8.48f), false),
            new NPCLocationInfo(new Vector3(-5.2f, 0, -9.66f), new Vector3(-4.93f, 0, -8.7f), false),
            new NPCLocationInfo(new Vector3(-35.55f, 0, -9.17f), new Vector3(-34.7f, 0, -8.64f), false)
    };
    
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
    private AStarPathfinding pathfinder;
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

        foreach (NPCLocationInfo location in idleLocations) {
            Debug.DrawLine(location.position, location.position + transform.forward * 0.25f, Color.yellow);
        }
        animator = GetComponent<Animator>();
        path = new List<Location>();
        pathfinder = GetComponent<AStarPathfinding>();
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
            
            if (isRunning && !(NPCRunningStatesM.Contains(currentAnimation) || NPCRunningStatesW.Contains(currentAnimation))) {setRandomRunning();}
            else if (isWalking && !(NPCWalkingStatesM.Contains(currentAnimation) || NPCWalkingStatesW.Contains(currentAnimation))) {setRandomWalking();}
            else if (isIdle && !(NPCIdleStatesM.Contains(currentAnimation) || NPCIdleStatesW.Contains(currentAnimation))) {setRandomIdle();}

            MoveAlongPath(); // rotates the NPC every time it gets to a point on the path to face the next point on the path
            // Dont need to move NPC forward since that is automatically done above (isRunning
        }
    }


    public void changeState(string state) {
        if (!NPCStateNames.Contains(state)) {throw new UnityException("animation provided not found");}
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

    private AnimationClip getAnimation (string name) {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == name) {
                return clip;
            }
        }
    throw new UnityException("Animation provided not found");
    }

    public void moveToRandom() {
        idleLocations[locationIndex].isSpotTaken = false;
        locationIndex = Random.Range(0, idleLocations.Length-1);
        while (idleLocations[locationIndex].isSpotTaken == true) {
            Debug.Log("Spot taken, checking another");
            locationIndex = Random.Range(0, idleLocations.Length-1);
        }
        idleLocations[locationIndex].isSpotTaken = true;
        Vector3 newLocation = idleLocations[locationIndex].position;
        lookatVector = idleLocations[locationIndex].lookAt;
        
        setPathTo(newLocation);
    }

    private void setPathTo(Vector3 location) {
        StartCoroutine(pathfinder.FindPathCoroutine(transform.position, location));
        currentPathIndex = 0;
    }

    private void MoveAlongPath(bool run=false) { // default param of walking not running
        if (path == null || path.Count == 0 || currentPathIndex > path.Count-1) {
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
                if (isBlockedByDoor(path[currentPathIndex+i-1].vector + new Vector3(0, 1, 0), path[currentPathIndex+i].vector + new Vector3(0, 1, 0))) {
                    Doors door = lastDoorBlocked.GetComponent<Doors>();
                    door.OpenDoorTemporarily();
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