using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Range(30, 1000)]
    public int fps;

    public static GameManager instance;
    public GameObject stopButton;
    public GameObject ddPanel;
    public Animator anim;
    public Way startWay;
    public List<PlayerMovement> players;
    public List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    public int currentPlayer;
    public int diceSide = 0;

    public GameObject isTwoPlayerPanel;

    public GameObject twoPlayerPanel;

    public float rotationDuration = 2f;
    public float moveDuration = 2f;

    private Transform cameraTarget;
    public Camera camera;

    public TextMeshProUGUI[] characterNameTexts;
    public TextMeshPro[] characterNameTextsTwo;

    public int skipTwoTurnsCount = 2;

    public GameObject greenCard;
    public GameObject blueCard;
    public GameObject bomb;
    public GameObject bomb5stepback;

    public GameObject p1turtle;
    public GameObject p2turtle;
    public GameObject p3turtle;
    public GameObject p4turtle;

    public GameObject p1redcard;
    public GameObject p2redcard;
    public GameObject p3redcard;
    public GameObject p4redcard;

    public GameObject diceValuePanel;
    public GameObject diceValuePanelTwo;
    public GameObject diceValuePanelThree;
    public GameObject diceValuePanelFour;

    public TextMeshProUGUI diceValueText;
    public TextMeshProUGUI diceValueTextTwo;
    public TextMeshProUGUI diceValueTextThree;
    public TextMeshProUGUI diceValueTextFour;

    public GameObject p1jail;
    public GameObject p2jail;
    public GameObject p3jail;
    public GameObject p4jail;

    public GameObject buttonText;
    public GameObject dice;
    public GameObject button;

    public Button diceButton;

    public AudioSource panelSound;
    public AudioSource buttonSound;

    public List<PlayerMovement> twoPlayers = new List<PlayerMovement>();
    public int[] diceValues = new int[2];
    public enum CardType
    {
        None, Green, Blue, Red, Turtle, Jail, Hospital, Hospital1, Hospital2, Hospital3, Hospital4, Hospital5, Bomb, Bomb5Steps, TeleportBlue, TeleportRed
    }
    private void Awake()
    {
        instance = this;
    }

    public void DisableRaycastOnButton()
    {
        GraphicRaycaster graphicRaycaster = button.GetComponentInParent<GraphicRaycaster>();

        if (graphicRaycaster != null)
        {
            graphicRaycaster.enabled = false;
        }
    }

        private void Start()
    {
        Application.targetFrameRate = fps;

        SetCameraTarget(currentPlayer);

        PlayersAnim();
        for (int i = 0; i < characterNameTexts.Length; i++)
        {
            string savedName = PlayerPrefs.GetString(i.ToString());
            characterNameTexts[i].text = savedName;
        }

        for (int i = 0; i < characterNameTextsTwo.Length; i++)
        {
            string savedName = PlayerPrefs.GetString(i.ToString());
            characterNameTextsTwo[i].text = savedName;
        }

        NickOnOff.Instance.currentPl();
    }

    public void OffButton()
    {
        diceButton.interactable = false;
    }

    public void EnableButton()
    {
        diceButton.interactable = true;
    }

    private void Update()
    {

    }

    public void OpenPanel()
    {
        anim.SetTrigger("openPanel");
    }

    public void ClosePanel()
    {
        anim.SetTrigger("closePanel");
    }

    public void PlayButtonSound()
    {
        buttonSound.Play();
    }
    public void SetCameraTarget(int cameraIndex)
    {
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            cam.Priority = 0;
        }
        cameras[cameraIndex].Priority = 1;

        diceButton.interactable = true;
    }

    public void StartNextTurn()
    {
        players[currentPlayer].gameObject.GetComponent<PlayerAnimator>().StopAnim();

        dice.SetActive(false);
        buttonText.SetActive(true);

    retry:
        currentPlayer = (currentPlayer + 1) % players.Count;
        SetCameraTarget(currentPlayer);
        PlayersAnim();
        if (players[currentPlayer].skipTurn == true)
        {
            players[currentPlayer].skipTurn = false;
            goto retry;
        }

        if (players[currentPlayer].skipTwoTurns == true && skipTwoTurnsCount == 0)
        {
            players[currentPlayer].skipTwoTurns = false;
        }

        else if (players[currentPlayer].skipTwoTurns == true && skipTwoTurnsCount != 0)
        {
            skipTwoTurnsCount--;
            goto retry;
        }

        PlayersAnim();
        NickOnOff.Instance.currentPl();

        if (players[currentPlayer].isSuspended == true)
        {
            players[currentPlayer].isSuspended = false;
            CardEvent.instance.BackComeGreenCard();

        }
        if (players[currentPlayer].haveCard == true)
        {
            CardEvent.instance.WhenComeBack();
        }
        if (currentPlayer == CheckCards.instance.currentPlayerr)
        {

        }

        PlayersAnim();

    }

    public void PlayersAnim()
    {
        players[currentPlayer].gameObject.GetComponent<PlayerAnimator>().StartAnim();

    }

    public void TwoPlayersAnim()
    {
        players[Way.instance.twoPlayerOne].gameObject.GetComponent<PlayerAnimator>().StartAnim();
    }

    public void TwoPlayersAnimSecond()
    {
        players[Way.instance.twoPlayerOne].gameObject.GetComponent<PlayerAnimator>().StopAnim();
        players[Way.instance.twoPlayerTwo].gameObject.GetComponent<PlayerAnimator>().StartAnim();
    }

    public void TwoPlayersAnimSecondStop()
    {
        players[Way.instance.twoPlayerTwo].gameObject.GetComponent<PlayerAnimator>().StopAnim();
    }

    public void SetDiceSide(int side)
    {
        diceSide = side;
    }

    public int GetDiceSide()
    {
        return diceSide;
    }
    int diceValuesIndex = 1;
    public void OnDiceRolled(float result)
    {
        PlayersAnim();
        diceSide = (int)result;
        if (players[currentPlayer].isTwoPlayer == true)
        {
            PlayersAnim();
            diceValues[diceValuesIndex] = (int)result;
            diceValuesIndex--;
            currentPlayer = twoPlayers[0].playerIndex;
            if (diceValuesIndex < 0)
            {

                if (diceValues[0] < diceValues[1] && twoPlayers[0].haveArmor)
                {
                    CardEvent.instance.ComeBackCard(players[currentPlayer].cardTypeee.gameObject);
                }

                else if (diceValues[0] < diceValues[1])
                {
                    twoPlayers[0].GoToHospital();
                    twoPlayers.Clear();

                }

                else if (diceValues[0] > diceValues[1] && twoPlayers[1].haveArmor)
                {
                    CardEvent.instance.ComeBackCard(players[currentPlayer].cardTypeee.gameObject);
                }

                else if (diceValues[0] > diceValues[1])
                {
                    twoPlayers[1].GoToHospital();
                    twoPlayers.Clear();
                }

                diceValuesIndex = 1;
                currentPlayer = twoPlayers[1].playerIndex;
                twoPlayers[0].isTwoPlayer = false;
                twoPlayers[1].isTwoPlayer = false;
                twoPlayers.Clear();
            }
        }

        else if (players[currentPlayer].onTurtle == true)
        {

            result = result / 2;
            result = Mathf.Ceil(result);
            players[currentPlayer].onTurtle = false;
            players[currentPlayer].moveCountt = (int)result;
            players[currentPlayer].MoveForward(result);
        }

        else
        {
            players[currentPlayer].moveCountt = (int)result;
            players[currentPlayer].MoveForward(result);
        }

    }

    public void IfTwoPlayer()
    {
        players[currentPlayer].GetComponent<PlayerAnimator>().StopAnim();
    }

    public void TurtlePanel()
    {

        if (players[0].onTurtle == true)
        {
            p1turtle.SetActive(true);
        }

        else if (players[0].onTurtle == false)
        {
            p1turtle.SetActive(false);
        }

        if (players[1].onTurtle == true)
        {
            p2turtle.SetActive(true);
        }

        else if (players[1].onTurtle == false)
        {
            p2turtle.SetActive(false);
        }

        if (players[2].onTurtle == true)
        {
            p3turtle.SetActive(true);
        }

        else if (players[2].onTurtle == false)
        {
            p3turtle.SetActive(false);
        }

        if (players[3].onTurtle == true)
        {
            p4turtle.SetActive(true);
        }

        else if (players[3].onTurtle == false)
        {
            p4turtle.SetActive(false);
        }
    }

    public void ActiveTwoPanel()
    {

        panelSound.Play();
        isTwoPlayerPanel.SetActive(true);

        isTwoPlayerPanel.GetComponent<Animator>().Play("panelAnimator");

    }

    public void TwoPlayersSameBlock(List<PlayerMovement> players)
    {
        ActiveTwoPanel();
        PlayersAnim();

        for (int i = 0; i < players.Count; i++)
        {
            twoPlayers.Add(players[i]);
        }

        if (twoPlayers[0].hasShieldForFight == true)
        {
            CardEvent.instance.ComeBackCardGreen(twoPlayers[0].shieldForFightCard);
        }

        else if (twoPlayers[1].hasShieldForFight == true)
        {
            CardEvent.instance.ComeBackCardGreen(twoPlayers[1].shieldForFightCard);
        }

        else
        {

        }

    }

    public TextMeshPro dice1;
    public TextMeshPro dice2;

    public void TwoPlayersCheck()
    {
        if (players[0].isTwoPlayer == true)
        {
            twoPlayers.Add(players[0]);
        }

        if (players[1].isTwoPlayer == true)
        {
            twoPlayers.Add(players[1]);
        }

        if (players[2].isTwoPlayer == true)
        {
            twoPlayers.Add(players[2]);
        }
        if (players[3].isTwoPlayer == true)
        {
            twoPlayers.Add(players[3]);
        }
    }

    public void CancelEscapeCard()
    {
        panelSound.Play();
        isTwoPlayerPanel.SetActive(true);

        isTwoPlayerPanel.transform.position = new Vector3(isTwoPlayerPanel.transform.position.x, Screen.height, isTwoPlayerPanel.transform.position.z);

        isTwoPlayerPanel.transform.localScale = Vector3.zero;

        LeanTween.moveY(isTwoPlayerPanel, Screen.height * 0.5f, 0.5f)
            .setEaseOutBounce()
            .setOnComplete(() =>
            {
               
            });

        LeanTween.scale(isTwoPlayerPanel, Vector3.one, 0.5f)
            .setEase(LeanTweenType.easeOutCubic);
    }

    void SelectedPlayerChange(int playerIndex)
    {
        for (int i = 0; i < players.Count; i++)
        {
            LeanTween.cancel(players[i].gameObject);
            players[i].transform.localScale = Vector3.one * 0.5f;
        }
        LeanTween.scale(players[playerIndex].gameObject, Vector3.one * 0.633135736f, 0.5f).setLoopPingPong();
    }
    public bool isChallenge = false;

    public void PrintDiceValues()
    {
        if (twoPlayers[0] == players[0] || twoPlayers[1] == players[0])
        {
            diceValuePanel.SetActive(true);
        }

        if (twoPlayers[0] == players[1] || twoPlayers[1] == players[1])
        {
            diceValuePanelTwo.SetActive(true);
        }

        if (twoPlayers[0] == players[2] || twoPlayers[1] == players[2])
        {
            diceValuePanelThree.SetActive(true);
        }

        if (twoPlayers[0] == players[3] || twoPlayers[1] == players[3])
        {
            diceValuePanelFour.SetActive(true);
        }
    }

    public int victimOneDice;
    public int victimTwoDice;

    public void TwoPlayersDices()
    {
        if (victimOneDice < victimTwoDice)
        {
            GameManager.instance.twoPlayers[0].GoToHospital();
            twoPlayers.Remove(twoPlayers[1]);
            twoPlayers.Remove(twoPlayers[0]);
            Way.instance.playerMovement.Clear();
        }

        else if (victimOneDice > victimTwoDice)
        {
            GameManager.instance.twoPlayers[1].GoToHospital();
            twoPlayers.Remove(twoPlayers[1]);
            twoPlayers.Remove(twoPlayers[0]);
            Way.instance.playerMovement.Clear();
        }

        else if (victimOneDice == victimTwoDice)
        {
            Diced.Instance.TwoDeneme();
        }

    }

    public void SameBlockDiceValue(int diceValue)
    {
        Debug.Log("Dice rolled: " + diceValue);
        challengeDiceValues.Add(diceValue);
        SelectedPlayerChange(twoPlayers[1].playerIndex);

        if (twoPlayers.Count >= 2)
        {

            if (twoPlayers[0].playerIndex == 0)
            {
                diceValueText.text = challengeDiceValues[0].ToString();
            }

            else if (twoPlayers[1].playerIndex == 0 && challengeDiceValues.Count >= 2)
            {
                diceValueText.text = challengeDiceValues[1].ToString();
            }

            if (twoPlayers[0].playerIndex == 1)
            {
                diceValueTextTwo.text = challengeDiceValues[0].ToString();
            }

            else if (twoPlayers[1].playerIndex == 1 && challengeDiceValues.Count >= 2)
            {
                diceValueTextTwo.text = challengeDiceValues[1].ToString();
            }

            if (twoPlayers[0].playerIndex == 2)
            {
                diceValueTextThree.text = challengeDiceValues[0].ToString();
            }

            else if (twoPlayers[1].playerIndex == 2 && challengeDiceValues.Count >= 2)
            {
                diceValueTextThree.text = challengeDiceValues[1].ToString();
            }

            if (twoPlayers[0].playerIndex == 3)
            {
                diceValueTextFour.text = challengeDiceValues[0].ToString();
            }

            else if (twoPlayers[1].playerIndex == 3 && challengeDiceValues.Count >= 2)
            {
                diceValueTextFour.text = challengeDiceValues[1].ToString();
            }
        }

        if (challengeDiceValues.Count >= 2)
        {

            for (int i = 0; i < twoPlayers.Count; i++)
            {
                twoPlayers[i].isTwoPlayer = false;
            }
            if (challengeDiceValues[0] == challengeDiceValues[1])
            {
                challengeDiceValues.RemoveAt(1);
                StartCoroutine(Diced.Instance.RollDiceforTwoPlayer());
            }
            else
            {
                if (challengeDiceValues[0] > challengeDiceValues[1])
                {
                    twoPlayers[1].GoToHospital();
                    twoPlayers.Remove(twoPlayers[1]);
                    twoPlayers.Remove(twoPlayers[0]);
                }
                else if (challengeDiceValues[0] < challengeDiceValues[1])
                {
                    twoPlayers[0].GoToHospital();
                    twoPlayers.Remove(twoPlayers[0]);
                    twoPlayers.Remove(twoPlayers[1]);
                }

                challengeDiceValues.Clear();
                twoPlayers.Clear();
                StartNextTurn();

                Invoke("CloseDiceValues", 2f);
            }

        }
    }
    void CloseDiceValues()
    {
        diceValueText.text = "";
        diceValueTextTwo.text = "";
        diceValueTextThree.text = "";
        diceValueTextFour.text = "";

        diceValuePanel.SetActive(false);
        diceValuePanelTwo.SetActive(false);
        diceValuePanelThree.SetActive(false);
        diceValuePanelFour.SetActive(false);

    }
    public List<int> challengeDiceValues = new List<int>();
    public void ChallengeDiceValue(int diceValue)
    {
        Debug.Log("Dice rolled: " + diceValue);
        challengeDiceValues.Add(diceValue);
        NickOnOff.Instance.currentPlayerChallenge(CardEvent.instance.selectedPlayers[1].playerIndex);
        SetCameraTarget(CardEvent.instance.selectedPlayers[1].playerIndex);
        SelectedPlayerChange(CardEvent.instance.selectedPlayers[1].playerIndex);
        if (challengeDiceValues.Count >= 2)
        {

            if (challengeDiceValues[0] == challengeDiceValues[1])
            {
                challengeDiceValues.RemoveAt(1);
                StartCoroutine(Diced.Instance.ChallengeRollDice());
            }
            else
            {
                if (challengeDiceValues[0] > challengeDiceValues[1])
                {
                    CardEvent.instance.selectedPlayers[1].BckMove(challengeDiceValues[1]);
                }
                else if (challengeDiceValues[0] < challengeDiceValues[1])
                {
                    CardEvent.instance.selectedPlayers[0].BckMove(challengeDiceValues[0]);
                }

                CardEvent.instance.selectedPlayers.Clear();
            }

        }
    }

    public void RollDiceButton()
    {
        buttonText.SetActive(false);
        dice.SetActive(true);
        if (isChallenge)
        {
            StartCoroutine(Diced.Instance.ChallengeRollDice());
        }
        else if (twoPlayers.Count >= 2)
        {
            StartCoroutine(Diced.Instance.RollDiceforTwoPlayer());
        }
        else
            Diced.Instance.StartWithClick();
    }

}