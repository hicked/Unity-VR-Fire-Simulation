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
    [SerializeField] public bool man;
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

        timeSinceIdleChange += Time.deltaTime;
        timeSinceMoved += Time.deltaTime;

        animatorInfo = animator.GetCurrentAnimatorClipInfo(0);
        currentAnimation = animatorInfo[0].clip.name;


        if (NPCWalkingStatesM.Contains(currentAnimation) || NPCWalkingStatesW.Contains(currentAnimation)) {
            transform.position +=  transform.forward * Time.deltaTime * NPCWalkSpeedMultiplier;
        }
        else if (NPCRunningStatesM.Contains(currentAnimation) || NPCRunningStatesW.Contains(currentAnimation)) {
            transform.position +=  transform.forward * Time.deltaTime * NPCRunningSpeedMultiplier;
        }

        if (timeSinceIdleChange > timeBeforeChangeIdle + Random.Range(-changeIdleVariance, changeIdleVariance)) {
            Debug.Log("changing Idle");
            setRandomIdle();
            timeSinceIdleChange = 0f;
        }
        else if (timeSinceMoved> timeBeforeMove + Random.Range(-moveVariance, moveVariance)) {
            moveTo(new Vector3(0,0,0));
            timeSinceMoved = 0f;
        }
    }


    public void changeState(string state) {
        if (!NPCStateNames.Contains(state)) {throw new UnityException("animation provided not found");}
        
        animator.CrossFadeInFixedTime(state, crossFadeDuration, 0, Random.value * findAnimation(state).length);
    }

    public void setRandomIdle() {
        if (man) {
            changeState(NPCIdleStatesM[Random.Range(0, NPCIdleStatesM.Length)]);
        }
        else {
            changeState(NPCIdleStatesW[Random.Range(0, NPCIdleStatesW.Length)]);
        }
    }

    private AnimationClip findAnimation (string name) {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == name) {
                return clip;
            }
        }
    throw new UnityException("animation provided not found");
    }
    public void moveTo(Vector3 location) {
        // ai pathfinding
    }
}