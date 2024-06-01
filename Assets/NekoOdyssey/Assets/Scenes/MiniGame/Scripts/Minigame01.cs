using System.Collections;
using UnityEngine;

namespace NekoOdyssey.Assets.Scenes.MiniGame.Scripts
{
 public class Minigame01 : MonoBehaviour
 {
     [Header("Progress")]
     [Range(0f, 1f)]
     [SerializeField] float progress;
     [SerializeField] bool isFinish, isCaught, canGrab;
     [SerializeField] bool finish, caughtd, isGrab;
     [SerializeField] bool game_start;
     [SerializeField] int player_HP;
     [Header("UI")]
     [SerializeField] GameObject UI;
     [SerializeField] SpriteRenderer space, spacePress;
     [SerializeField] GameObject ready, go, clear, fail;
     [SerializeField] GameObject effect1, effect2;
     [SerializeField] GameObject heart1, heart2, heart3;
     [SerializeField] GameObject heartLost1, heartLost2, heartLost3;
     [SerializeField] GameObject zzz, silent, alert;
     [Header("Hand")]
     [SerializeField] GameObject hand;
     [SerializeField] SpriteRenderer handwait, handHit, handGrap;
     [SerializeField] Vector3 handDistance = Vector3.zero;
     [SerializeField] Vector3 handOriginPose = Vector3.zero;
     [Header("Object")]
     [SerializeField] GameObject bag1;
     [SerializeField] GameObject bag2;
     [Header("CatPart")]
     [SerializeField] GameObject tail;
     [SerializeField] GameObject arm1, arm2, arm3, arm4;
     [SerializeField] GameObject body1, body2, body3, body4;
     [Header("CatAlert")]
     [Range(0f, 1f)]
     [SerializeField] float catAwareness;
     [SerializeField] int catState = 0;
     [Header("Audio")]
     [SerializeField] Minigame01_AudioManager audioManager;
 
     void Start()
     {
         //UI
         UI = GameObject.Find("UIs");
         space = UI.transform.Find("Space").GetComponent<SpriteRenderer>();
         spacePress = UI.transform.Find("SpacePress").GetComponent<SpriteRenderer>();
         ready = UI.transform.Find("Ready").gameObject; ready.SetActive(false);
         go = UI.transform.Find("Go").gameObject; go.SetActive(false);
         clear = UI.transform.Find("Clear").gameObject; clear.SetActive(false);
         fail = UI.transform.Find("Fail").gameObject; fail.SetActive(false);
         effect1 = GameObject.Find("Effect"); effect1.SetActive(false);
         effect2 = GameObject.Find("FocusLine"); effect2.SetActive(false);
         heart1 = GameObject.Find("Hearts1");
         heart2 = GameObject.Find("Hearts2");
         heart3 = GameObject.Find("Hearts3");
         heartLost1 = GameObject.Find("HeartLost1"); heartLost1.SetActive(false);
         heartLost2 = GameObject.Find("HeartLost2"); heartLost2.SetActive(false);
         heartLost3 = GameObject.Find("HeartLost3"); heartLost3.SetActive(false);
         zzz = GameObject.Find("ZZZ1");
         silent = GameObject.Find("Silent"); silent.SetActive(false);
         alert = GameObject.Find("Alert"); alert.SetActive(false);
 
         //Hand-Player
         hand = GameObject.Find("Hand");
         handwait = hand.transform.Find("Hand1").GetComponent<SpriteRenderer>();
         handHit = GameObject.Find("Hand2").GetComponent<SpriteRenderer>();
         handGrap = hand.transform.Find("Hand3").GetComponent<SpriteRenderer>();
 
         //Object
         bag1 = GameObject.Find("Bag01");
         bag2 = GameObject.Find("Bag03"); bag2.SetActive(false);
 
         //Cat
         tail = GameObject.Find("Tail");
         arm1 = GameObject.Find("Arm01");
         arm2 = GameObject.Find("Arm02"); arm2.SetActive(false);
         arm3 = GameObject.Find("Arm03"); arm3.SetActive(false);
         arm4 = GameObject.Find("Arm04"); arm4.SetActive(false);
         body1 = GameObject.Find("Body01");
         body2 = GameObject.Find("Body02"); body2.SetActive(false);
         body3 = GameObject.Find("Body03"); body3.SetActive(false);
         body4 = GameObject.Find("Body04"); body4.SetActive(false);
 
         handOriginPose = hand.transform.position;
         player_HP = 3;
 
         audioManager = GetComponent<Minigame01_AudioManager>();
 
         StartCoroutine(GameStartSequnce());
 
     }
 
     // Update is called once per frame
     void Update()
     {
         if (game_start)
         {
             if (isFinish)
             {
                 GameFinish();
             }
             else if (isCaught && !caughtd)
             {
                 GameOver();
                 caughtd = true;
             }
             else if (!isFinish && !isCaught)
             {
                 CheckInput();
                 CheckProgress();
                 CheckAlert();
             }
         }
 
         if (Time.time - timer >= 0.75f && !isCaught)
         {
             spacePress.enabled = false;
             space.enabled = true;
         }
     }
     float timer;
     public void CheckInput()
     {
         if (!canGrab)
         {
             if (Input.GetKeyDown(KeyCode.Space))
             {
                 spacePress.enabled = true;
                 space.enabled = false;
                 timer = Time.time;
                 progress += 0.04f;
                 progress = Mathf.Clamp(progress, 0f, 1f);
             }
         }
 
         if (Input.GetKeyDown(KeyCode.Space) && canGrab)
         {
             handwait.enabled = false;
             handGrap.enabled = true;
             if (Input.GetKeyDown(KeyCode.Space) && isGrab)
             {
                 if (catState == 2)
                 {
                     isCaught = true;
                 }
                 else
                 {
                     isFinish = true;
                 }
             }
             isGrab = true;
         }
 
 
         if (Time.time - timer >= 2f && !isGrab)
         {
             progress -= 0.08f * Time.deltaTime;
             progress = Mathf.Clamp(progress, 0f, 1f);
         }
     }
 
     public void CheckProgress()
     {
         handDistance = handOriginPose + new Vector3(0, progress / 3.25f, 0);
         hand.transform.position = handDistance;
 
         if (progress == 1f)
         {
             canGrab = true;
         }
         if (progress < 1f)
         {
             canGrab = false;
         }
     }
 
     public void GameFinish()
     {
         hand.transform.position = hand.transform.position + new Vector3(0, -0.025f, 0);
         bag1.transform.position = bag1.transform.position + new Vector3(0, -0.025f, 0);
         if (!finish)
         {
             finish = true;
             clear.SetActive(true);
             audioManager.StopBGM();
             audioManager.PlaySFX("SFX_Clear", false);
         }
     }
     public void GameOver()
     {
         StartCoroutine(GetCaught());
     }
 
     IEnumerator GameStartSequnce()
     {
         audioManager.PlayBGM("BGM Minigame [Dim Sound]", true);
 
         ready.SetActive(true);
         audioManager.PlaySFX("VOC_Ready", false);
         yield return new WaitForSeconds(2f);
         ready.SetActive(false);
         go.SetActive(true);
         audioManager.PlaySFX("VOC_Go", false);
         game_start = true;
         yield return new WaitForSeconds(.7f);
         go.SetActive(false);
     }
 
     public void CheckAlert()
     {
         catAwareness = Mathf.Clamp(catAwareness, 0f, 1f);
 
         switch (catState)
         {
             case 0:
                 Cat_ZZZ(); break;
             case 1:
                 Cat_Silent(); break;
             case 2:
                 Cat_Alert(); break;
         }
 
         if (catAwareness == 1f)
         {
             isCaught = true;
         }
     }
 
     public void Cat_ZZZ()
     {
         zzz.SetActive(true);
         silent.SetActive(false);
         alert.SetActive(false);
 
         if (Time.time - timer >= 0.75f)
         {
             catAwareness -= 0.1f * Time.deltaTime;
         }
 
         if ((Input.GetKeyDown(KeyCode.Space)))
         {
             catAwareness += Random.Range(0.03f, 0.05f);
         }
 
         if (catAwareness >= 0.6f)
         {
             catState = 1;
         }
 
     }
     public void Cat_Silent()
     {
         zzz.SetActive(false);
         silent.SetActive(true);
         alert.SetActive(false);
         audioManager.StopLoopedSFX("SFX_CatAlert");
 
         body1.SetActive(true); body2.SetActive(false);
 
         if (Time.time - timer >= 0.75f)
         {
             catAwareness -= 0.05f * Time.deltaTime;
         }
 
         if ((Input.GetKeyDown(KeyCode.Space)))
         {
             catAwareness += Random.Range(0.05f, 0.08f);
         }
 
         if (catAwareness >= 0.8f)
         {
             catState = 2;
         }
         else if (catAwareness <= 0.5f)
         {
             catState = 0;
         }
 
     }
 
     public void Cat_Alert()
     {
         zzz.SetActive(false);
         silent.SetActive(false);
         alert.SetActive(true);
         audioManager.PlaySFX("SFX_CatAlert", true);
 
 
         body1.SetActive(false); body2.SetActive(true);
 
         if (Time.time - timer >= 0.75f)
         {
             catAwareness -= 0.02f * Time.deltaTime;
         }
         if ((Input.GetKeyDown(KeyCode.Space)))
         {
             catAwareness += 1f;
         }
 
         if (catAwareness <= 0.7f)
         {
             catState = 1;
         }
     }
 
     IEnumerator GetCaught()
     {
         alert.SetActive(false); 
         body2.SetActive(false); body3.SetActive(true);
         arm1.SetActive(false); arm3.SetActive(true);
         tail.SetActive(false);
         bag1.SetActive(false); bag2.SetActive(true);
         handwait.enabled = false;
         handGrap.enabled = false;
         handHit.enabled = true;
 
         space.enabled = false;
         spacePress.enabled = false;
 
         effect1.SetActive(true); effect2.SetActive(true);
 
         audioManager.StopLoopedSFX("SFX_CatAlert");
         audioManager.PlaySFX("SFX_CatHit", false);
 
         yield return new WaitForSeconds(1.75f);
 
         space.enabled = false;
 
         body3.SetActive(false); body4.SetActive(true);
         arm3.SetActive(false); arm4.SetActive(true);
 
         effect1.SetActive(false); effect2.SetActive(false);
         audioManager.PlaySFX("SFX_CatHiss", false);
 
         yield return new WaitForSeconds(0.75f);
 
         switch (player_HP)
         {
             case 0:
                 Fail(); break;
             case 1:
                 heart1.SetActive(false); heartLost1.SetActive(true);
                 yield return new WaitForSeconds(1.2f); Fail(); player_HP--;
                 break;
             case 2:
                 heart2.SetActive(false); heartLost2.SetActive(true);
                 yield return new WaitForSeconds(1.2f); StageRestart(); player_HP--;
                 audioManager.PlaySFX("SFX_TimeOut", false);
                 yield return new WaitForSeconds(0.2f);
                 audioManager.PlayBGM("BGM Minigame speedup", true);
                 break;
             case 3:
                 heart3.SetActive(false); heartLost3.SetActive(true);
                 yield return new WaitForSeconds(1.2f); StageRestart(); player_HP--;
                 break;
         }
     }
 
     public void Fail()
     {
         fail.SetActive(true);
     }
 
     public void StageRestart()
     {
         body4.SetActive(false); body3.SetActive(false); body1.SetActive(true);
         arm4.SetActive(false); arm3.SetActive(false); arm1.SetActive(true);
         tail.SetActive(true);
 
         bag2.SetActive(false); bag1.SetActive(true);
         handHit.enabled = false;
         handwait.enabled = true;
 
         space.enabled = true;
         spacePress.enabled = false;
 
         progress = 0f;
         catAwareness = 0f;
 
         isCaught = false; caughtd = false; isGrab = false; canGrab = false;
     }
 }   
}
