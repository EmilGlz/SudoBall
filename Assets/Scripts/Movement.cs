using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    private const int GROUNDLAYER = 6;
    private const int WALLFREELAYER = 7;
    private const int WALLLAYER = 8;
    private const int RINGLAYER = 9;
    private const int DIAMONDLAYER = 10;
    private const int FINISHLAYER = 11;
    private const int RINGFREELAYER = 12;

    private const int ringSoundIndex = 2;
    private const int crystalSoundIndex = 3;
    private const int comboPowerSoundIndex = 5;
    private const int wallCrashSoundIndex = 6;
    //private const int squishSoundBeginIndex = 13;
    //private const int squishSoundCount = 4;
    private const float destroyTimeConst = 10f;

    [SerializeField] private GameObject winGO;

    [SerializeField] private Vector3 gravity;
    [SerializeField] private float jumpForce;

    private Rigidbody rB;

    private System.Random random;

    private bool gameStart = false;
    private bool isMoving = false;
    private bool endGame = false;
    private bool stop = false;

    #region LevelInfo
    [SerializeField] private Image fillImage;
    private float levelLength;
    #endregion

    #region BallScaleAnim
    [SerializeField] private Transform renderBall;
    [SerializeField] AnimationCurve animCurveXZ;
    [SerializeField] AnimationCurve animCurveY;

    private float maxHeight;
    private float scaleXZ, scaleY;
    private bool goingUp;
    #endregion

    #region Forward move
    private const float OBSTACLEDELTAPOSITION = 10.80002f;
    [SerializeField] private float force;
    [SerializeField] private float velocity;
    [SerializeField] private float cameraOffset;
    private Camera mainCam;
    [SerializeField] private Transform ground;
    [SerializeField] private GameObject groundPart;
    [SerializeField] private float firstPartPoint;
    #endregion

    #region Controller with Joystick
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private float forwardForceOnPlay;
    [SerializeField] private float forwardForceOnComboPower;
    private float forwardForce;
    [SerializeField] private float sensitivity;
    [SerializeField] private float sideLimit;
    private float curHor, lastHor = 0;
    private float delta;
    private float targetPoint;
    #endregion

    #region Features Instantiation
    //Ring
    [SerializeField] private GameObject ring;
    private GameObject tempRing;
    private readonly int ringProbability = 65;
    [SerializeField] private float ringWidth;
    [SerializeField] private float ringOffset = -1.5f;
    [SerializeField] private GameObject ringExplodePS;
    private bool miss = true;
    //

    //Diamond
    [SerializeField] private GameObject diamondPS;
    //

    //Finish
    [SerializeField] private GameObject finishLine;
    [SerializeField] private float finishLineOffset;
    //

    //Wall
    [SerializeField] private GameObject wall;
    private GameObject tempWall;
    [SerializeField] private float wallOffset;

    [SerializeField] private GameObject[] stains;
    [SerializeField] private GameObject stainPS;
    //private GameObject tempStain;


    private bool calculated = false;
    private float secondZ;
    private float obstacleDeltaZ;
    #endregion

    #region Features Effects
    [SerializeField] private Material skyboxMat;
    [SerializeField] private Color32[] skyboxColors;
    [SerializeField] private Color32[] fogColors;
    [SerializeField] private GameObject perfectText;
    [SerializeField] private GameObject goodText;
    [SerializeField] private float perfectOffset = .6f;

    private int randColorIndex;

    [SerializeField] private int powerComboActionNum = 3;
    #endregion

    [SerializeField] private float loseGameBackForce;

    //Combo and PowerActionCombo variables
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject powerComboPS;
    [SerializeField] private Image comboFill;
    [SerializeField] private RectTransform comboFireRect;
    [SerializeField] private float powerComboHeight = 3.5f;
    [SerializeField] private int powerComboObsCount = 5;
    private int comboCount = 0;
    private int powerComboCount = 0;
    private int maxCombo;
    private bool isPowerCombo = false;
    //

    private float lastTimeTemp;
    private float tempCheck;

    private void Start()
    {
        random = new System.Random();
        randColorIndex = random.Next(skyboxColors.Length);
        skyboxMat.SetColor("_Tint", skyboxColors[randColorIndex]);
        RenderSettings.fogColor = fogColors[randColorIndex];
        mainCam = Camera.main;
        rB = GetComponent<Rigidbody>();
        Physics.gravity = gravity;
        maxHeight = (jumpForce * jumpForce) / (2 * Mathf.Abs(gravity.y));
        joystick.SetBackground();
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        int randomObstacleCount = random.Next(30, 35);
        int probabilityNum;
        secondZ = rB.position.z + 2 * OBSTACLEDELTAPOSITION;
        for (int i = 0; i < randomObstacleCount; i++)
        {
            probabilityNum = random.Next(100);
            if (probabilityNum < ringProbability)
            {
                Instantiate(ring, new Vector3(Random.Range(-(sideLimit - ringWidth / 2), sideLimit - ringWidth / 2), 1.1f, secondZ + ringOffset), Quaternion.identity);
            }
            else
            {
                tempWall = Instantiate(wall, new Vector3(0, 0, secondZ + wallOffset), Quaternion.identity);
                tempWall.GetComponent<WallObstacle>().walls[random.Next(3)].SetActive(false);
            }
            secondZ += OBSTACLEDELTAPOSITION;
        }
        levelLength = secondZ + finishLineOffset;
        Instantiate(finishLine,new Vector3(0, 2.7f, levelLength), Quaternion.identity);
        secondZ += 2*OBSTACLEDELTAPOSITION;
        for (int i = 0; i < 10; i++)
        {
            probabilityNum = random.Next(100);
            if (probabilityNum < ringProbability)
            {
                tempRing = Instantiate(ring, new Vector3(0, 0, secondZ + ringOffset), Quaternion.identity);
                tempRing.GetComponent<Ring>().circle.transform.localPosition = new Vector3(Random.Range(-(sideLimit - ringWidth / 2), sideLimit - ringWidth / 2), 0, 0);
            }
            else
            {
                tempWall = Instantiate(wall, new Vector3(0, 0, secondZ + wallOffset), Quaternion.identity);
                tempWall.GetComponent<WallObstacle>().walls[random.Next(3)].SetActive(false);
            }
            secondZ += OBSTACLEDELTAPOSITION;
        }
        CommonDataAndPros.Instance.LoadingComplete();
    }

    private void FixedUpdate()
    {
        if (goingUp && rB.velocity.y < 0) goingUp = false;

        scaleXZ = animCurveXZ.Evaluate((goingUp ? 1 : 0) + (rB.position.y - 0.5f) / maxHeight);
        scaleY = animCurveY.Evaluate((goingUp ? 1 : 0) + (rB.position.y - 0.5f) / maxHeight);

        renderBall.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);

        if (isPowerCombo)
        {
            rB.MovePosition(new Vector3(rB.position.x, powerComboHeight, rB.position.z + forwardForce));
        }
        else
        {
            rB.MovePosition(new Vector3(rB.position.x, rB.position.y, rB.position.z + forwardForce));
        }
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, rB.position.z - cameraOffset);

        if(gameStart)
        {
            fillImage.fillAmount = rB.position.z / levelLength;
        }
    }

    public void JoystickOnDrag()
    {
        curHor = joystick.Horizontal;

        delta = curHor - lastHor;

        targetPoint = rB.position.x + delta * sensitivity;
        if (Mathf.Abs(targetPoint) <= sideLimit)
        {
            rB.MovePosition(new Vector3(rB.position.x + delta * sensitivity, rB.position.y, rB.position.z));
        }

        lastHor = curHor;
    }

    public void JoystickPointerUp()
    {
        lastHor = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == GROUNDLAYER && !goingUp)
        {
            if (!isMoving && gameStart)
            {
                forwardForce = forwardForceOnPlay;
                isMoving = true;
            }
            AudioManager.Instance.Play(random.Next(2));
            if (endGame)
            {
                if (!stop) stop = true;
                else rB.velocity = Vector3.zero;
            }
            rB.AddForce(rB.transform.up * jumpForce, ForceMode.Impulse);
            goingUp = true;
            Destroy(Instantiate(stains[random.Next(2)], new Vector3(collision.contacts[0].point.x, 0.01f, collision.contacts[0].point.z), Quaternion.Euler(90, random.Next(360), 0)), .5f);
            Destroy(Instantiate(stainPS, new Vector3(collision.contacts[0].point.x, 0.01f, collision.contacts[0].point.z), Quaternion.identity), .5f);
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == WALLLAYER)
        {
            if (isPowerCombo)
            {
                AudioManager.Instance.Play(wallCrashSoundIndex);
                other.gameObject.GetComponent<WallInfo>().myPS.Play();
                other.gameObject.SetActive(false);
            }
            else
            {
                forwardForce = 0;
                rB.AddForce(-1 * loseGameBackForce * rB.transform.forward, ForceMode.Impulse);
                CommonDataAndPros.Instance.LoseGame();
                gameStart = false;
                endGame = true;
            }
        }
        else if (other.gameObject.layer == RINGFREELAYER)
        {
            if (miss)
            {
                comboCount = 0;
                powerComboCount = 0;
                comboFill.fillAmount = 0;
                CommonDataAndPros.Instance.MissedShot();
            }
            miss = true;
        }
        else if (other.gameObject.layer == RINGLAYER)
        {
            AudioManager.Instance.Play(ringSoundIndex);
            if (Mathf.Abs(other.gameObject.transform.position.x - transform.position.x) <= perfectOffset)
            {
                if(!isPowerCombo && ++powerComboCount == powerComboActionNum) // Use Combo Power
                {
                    rB.useGravity = false;
                    //TODO Make 3.5f Serialize field height of Power combo
                    rB.MovePosition(new Vector3(rB.position.x, powerComboHeight, rB.position.z));
                    powerComboPS.SetActive(true);
                    trailRenderer.enabled = false;

                    AudioManager.Instance.Play(comboPowerSoundIndex);
                    forwardForce = forwardForceOnComboPower;
                    LeanTween.scale(comboFireRect, Vector2.one * 3f, 1f).setEasePunch();
                    isPowerCombo = true;
                    powerComboCount = 0;
                    StartCoroutine(ComboPowerUpdate(rB.position.z));
                }
                else
                {
                    comboFill.fillAmount = (float)powerComboCount / powerComboActionNum;
                }
                CommonDataAndPros.Instance.PerfectShot(++comboCount);
            }
            else
            {
                CommonDataAndPros.Instance.GoodShot();
                comboCount = 0;
                powerComboCount = 0;
                comboFill.fillAmount = 0;
            }
            LeanTween.scale(other.gameObject, new Vector3(2, 1, 2), .1f).setOnComplete(() => {
                Destroy(Instantiate(ringExplodePS, other.gameObject.transform.position, Quaternion.identity), 2f);
                Destroy(other.gameObject);
                });
            miss = false;
            //StartCoroutine(RingExplode(other.gameObject));
        }
        else if(other.gameObject.layer == DIAMONDLAYER)
        {
            AudioManager.Instance.Play(crystalSoundIndex);
            CommonDataAndPros.Instance.AddDiamond(1, mainCam.WorldToScreenPoint(other.transform.position));
            other.gameObject.SetActive(false);
            Destroy(Instantiate(diamondPS, other.gameObject.transform.position, Quaternion.identity),1f);
        }
        else if(other.gameObject.layer == FINISHLAYER)
        {
            forwardForce = 0f;
            other.gameObject.GetComponent<FinishLine>().FinalParticle();
            //joystick.gameObject.SetActive(false);
            if (isPowerCombo)
            {
                rB.useGravity = true;
                powerComboPS.SetActive(false);
                trailRenderer.enabled = true;

                isPowerCombo = false;
            }
            CommonDataAndPros.Instance.EndGame();
        }
        //else if (other.gameObject.layer == WALLFREELAYER)
        //{
        //    other.gameObject.GetComponent<WallObstacle>().ExplodeAll();
        //}
    }

    private IEnumerator ComboPowerUpdate(float startZPos)
    {
        float powerModeLength = powerComboObsCount * OBSTACLEDELTAPOSITION;
        while (true)
        {
            comboFill.fillAmount = 1 - (rB.position.z - startZPos) / powerModeLength;
            if (rB.position.z >= (startZPos + powerModeLength))
            {
                rB.useGravity = true;
                powerComboPS.SetActive(false);
                trailRenderer.enabled = true;

                forwardForce = forwardForceOnPlay;
                isPowerCombo = false;
                powerComboCount = 0;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator RingExplode(GameObject ring)
    {
        yield return new WaitForSeconds(.1f);
        LeanTween.scale(ring, new Vector3(2, 1, 2), .1f).setOnComplete(() => Destroy(ring));
    }

    public void OnClick_StartGame()
    {
        gameStart = true;
    }

    public void OnClick_RestartOrNextLevel()
    {
        SceneManager.LoadScene(0);
    }

}
