using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class NPCAI : MonoBehaviour {
    [SerializeField] private float timeBeforeChangeIdle = 15f;
    [SerializeField] private float changeIdleVariance = 3f;
    [SerializeField] private float timeBeforeMove = 30f;
    [SerializeField] private float moveVariance = 5f;

    [SerializeField] private float NPCWalkSpeedMultiplier = 1.5f;
    [SerializeField] private float NPCRunningSpeedMultiplier = 2.5f;
    [SerializeField] private float crossFadeDuration = 1f;
    [SerializeField] public bool onPhone;
    [SerializeField] public bool isMan;
    [SerializeField] public bool isIdle = true;
    [SerializeField] public bool isWalking = false;
    [SerializeField] public bool isRunning = false;
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
    private float timeSinceIdleChange = 0f;
    private float timeSinceMoved = 0f;
    private AnimationClip[] clips;
    private Animator animator;
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
    
    private void Start() {
        animator = GetComponent<Animator>();
        setRandomIdle();
    } 

    private void Update() {
        // Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        // Vector2Int goal = new Vector2Int(-24, -8);
        // int[,] grid = new int[,] {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        //                     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        //                     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        //                     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        //                     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        //                     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}};

        // List<Vector2Int> path = AStarPathfinding.FindPath(start, goal, grid);
        // if (path != null)
        // {
        //     foreach (Vector2Int point in path)
        //     {
        //         Debug.Log("Path point: " + point);
        //         transform.position = new Vector3(point.x, transform.position.y, point.y);
        //     }
        // }
        // else
        // {
        //     Debug.Log("No path found");
        // }

        // Gets information about the animator
        animator = GetComponent<Animator>();
        animatorInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (animatorInfo.Length == 1) {
            currentAnimation = animatorInfo[0].clip.name;
            // Makes sure we have to correct animation based on the state of the NPC
            if (isRunning && !(NPCRunningStatesM.Contains(currentAnimation) || NPCRunningStatesW.Contains(currentAnimation))) {setRandomRunning();}
            else if (isWalking && !(NPCWalkingStatesM.Contains(currentAnimation) || NPCWalkingStatesW.Contains(currentAnimation))) {setRandomWalking();}
            else if (isIdle && !(NPCIdleStatesM.Contains(currentAnimation) || NPCIdleStatesW.Contains(currentAnimation))) {setRandomIdle();}

            if (isRunning) {
                transform.position +=  transform.forward * Time.deltaTime * NPCRunningSpeedMultiplier;
            }
            else if (isWalking) {
                transform.position +=  transform.forward * Time.deltaTime * NPCWalkSpeedMultiplier;
            }

            // Switches Idle animation after set amount of time
            else if (isIdle) {
                timeSinceIdleChange += Time.deltaTime;
                timeSinceMoved += Time.deltaTime;

                if (timeSinceIdleChange > timeBeforeChangeIdle + Random.Range(-changeIdleVariance, changeIdleVariance)) {
                    setRandomIdle();
                    timeSinceIdleChange = 0f;
                }
                if (timeSinceMoved> timeBeforeMove + Random.Range(-moveVariance, moveVariance)) {
                    //moveToRandom(new Vector3(0,0,0));
                    timeSinceMoved = 0f;
                }
            }
        }
    }


    public void changeState(string state) {
        if (!(NPCStateNames.Contains(state))) {throw new UnityException("animation provided not found");}
        animator.CrossFadeInFixedTime(state, crossFadeDuration, 0, Random.value * getAnimation(state).length);
        Debug.Log(state);
    }

    public void setRandomIdle() {
        if (isMan) {
            changeState(NPCIdleStatesM[Random.Range(0, NPCIdleStatesM.Length-1)]);
        }
        else {
            changeState(NPCIdleStatesW[Random.Range(0, NPCIdleStatesW.Length-1)]);
        }
    }

    public void setRandomWalking() {
        if (isMan) {
            changeState(NPCWalkingStatesM[Random.Range(0, NPCWalkingStatesM.Length-1)]);
        }
        else {
            changeState(NPCWalkingStatesW[Random.Range(0, NPCWalkingStatesW.Length-1)]);
        }
    }

    public void setRandomRunning() {
        if (isMan) {
            changeState(NPCRunningStatesM[Random.Range(0, NPCRunningStatesM.Length-1)]);
        }
        else {
            changeState(NPCRunningStatesW[Random.Range(0, NPCRunningStatesW.Length-1)]);
        }
    }

    private AnimationClip getAnimation (string name) {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == name) {
                return clip;
            }
        }
    throw new UnityException("animation provided not found");
    }

    public void moveToRandom() {
    }

    public void moveTo(Vector3 location) {

    }

    //  public bool isBlocked(Vector3 dir) {
    //     RaycastHit hitInfo = default(RaycastHit);
    //         bool hit = Physics.BoxCast(
    //             new Vector3(
    //                 transform.position.x, 
    //                 transform.position.y - playerHitBox.size.y + (playerHitBox.size.y - minHeightBeforeCollisions)/2f + minHeightBeforeCollisions, 
    //                 transform.position.z), 
    //             new Vector3((playerHitBox.size.x - maxDistance)/2f, (playerHitBox.size.y - minHeightBeforeCollisions)/2f, (playerHitBox.size.z - maxDistance)/2f), // IMOPRTANT: set the y to negative, that way its starts from the top down instead of bottom up
    //             dir, 
    //             out hitInfo,
    //             transform.rotation, 
    //             maxDistance,
    //             layersForCollisions);
        
    //     Debug.DrawRay(transform.position, dir * maxDistance, hit ? Color.red : Color.green);
    //     if (hit) {
    //         lastBlockedByType = LayerMask.LayerToName(hitInfo.collider.gameObject.layer);
    //         lastBlockedByObj = hitInfo.collider.gameObject;
    //         lastBlockedLocation = hitInfo.point;
    //     }
    //     return hit;
    // }
}