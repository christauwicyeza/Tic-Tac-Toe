using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace tic_toe
{
    public enum STRIKETYPE
    {
        LEFT_VERTICAL,
    MIDDLE_VERTICAL,
    RIGHT_VERTICAL,
    UP_HORIZONTAL,
    MIDDLE_HORIZONTAL,
    DOWN_HORIZONTAL,
    RIGHT_CROSS,
    LEFT_CROSS
    };

    public enum TICK
    {
        CIRCLE,
        CROSS,
        EMPTY
    };

    public enum PlayTime
    {
        PLAYER,
        OPPONENT,
        AI
    };

    public class TicTacToeManager : MonoBehaviour
    {

        [FormerlySerializedAs("allButtons")] public List<Button> AllButtons = new();

        [SerializeField] private List<GameObject> AllStrikes = new();
        [SerializeField] [Range(1f, 10f)] private float GrowFactor = 5.0f;

 
        public TICK currentTick;


        [SerializeField] private TextMeshProUGUI PlayText;


        private readonly String[] TICKS = { "O", "X", "" };


        private readonly String[] PLAY = { "Player O Move", "Player X Move", "Player VS AI" };
 
        public bool AI = false;

        public GameObject ResetButton;

        private int NumberOfMoves = 0;

        private TextMeshProUGUI TextHold;
        private bool canGrow = false;
        private float MaxGrowSize = 160.0f;
        private const float MinGrowSize = 20.0f;

        private int[] Board = new int[]{ 0, 0, 0 , 0, 0, 0 , 0, 0, 0 };
        private int current = 0;

        private bool CheckAvailableMove()
        {
            if (NumberOfMoves <= 8)
            {
                return true;
            }
            return false;
        }

        private int currentMove = 0;
        private int GenerateRandomPosition()
{
    int move = -1;
    List<int> availableMoves = new List<int>();

    for (int i = 0; i < AllButtons.Count; i++)
    {
        if (AllButtons[i].interactable)
        {
            availableMoves.Add(i);
        }
    }

    if (availableMoves.Count > 0)
    {
        int randomIndex = Random.Range(0, availableMoves.Count);
        move = availableMoves[randomIndex];
    }

    return move;
}


        private void AIPlays()
        {
            if(CheckAvailableMove())
            {
                int position = GenerateRandomPosition();
                if (position < 0)
                {
                    // end the game
                    ResetButton.SetActive(true);
                    CheckBoardPlayer();
                    CheckCurrentBoard();
                    return;
                }
                AI_Move(position, (int)TICK.CROSS);
            }
        }
        
        
    public void CheckBoardPlayer()
    {
    Check_Template(0, 1, 2, (int)STRIKETYPE.UP_HORIZONTAL);
    Check_Template(3, 4, 5, (int)STRIKETYPE.MIDDLE_HORIZONTAL);
    Check_Template(6, 7, 8, (int)STRIKETYPE.DOWN_HORIZONTAL);
    
    Check_Template(0, 3, 6, (int)STRIKETYPE.LEFT_VERTICAL);
    Check_Template(1, 4, 7, (int)STRIKETYPE.MIDDLE_VERTICAL);
    Check_Template(2, 5, 8, (int)STRIKETYPE.RIGHT_VERTICAL);
    
    Check_Template(0, 4, 8, (int)STRIKETYPE.LEFT_CROSS);
    Check_Template(2, 4, 6, (int)STRIKETYPE.RIGHT_CROSS);
}


    
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="DrawIndex"></param>
        private void Check_Template(int first, int second, int third, int DrawIndex)
        {
            if (Board[first] == 1 && Board[second] == 1 && Board[third] == 1)
            {
                StrikeLine(DrawIndex);
                if (AI)
                {
                    PlayText.text = "<color=red>AI Wins</color>";
                }
                else
                {
                    PlayText.text = "<color=green>X Player Wins</color>";
                }
                return;
            }
            
            if (Board[first] == 2 && Board[second] == 2 && Board[third] == 2)
            {
                StrikeLine(DrawIndex);
                // call winner here
                if (AI)
                {
                    PlayText.text = "<color=green>Human Player Wins</color>";
                }
                else
                {
                    PlayText.text = "<color=green>O Player Wins</color>";
                }
                return;
            }
        }


    public void CheckCurrentBoard()
    {
        for (int i = 0; i < AllButtons.Count; i++)
        {
            Board[i] =  (AllButtons[i].interactable) ? 0 : 1;
            Board[i] = PlayerSelectionType(AllButtons[i]);
        }
    }

    public int PlayerSelectionType(Button B)
    {
        TextMeshProUGUI current = B.GetComponentInChildren<TextMeshProUGUI>();
        if (current.text == "X")
        {
            return 1;
        }

        if (current.text == "O")
        {
            return 2;
        }

        return 0;
    }
    
    
    private void Start()
    {
        currentTick = TICK.CROSS;
        NumberOfMoves = 0;
    }


    private void Update()
    {
        AnimateGrowth();
        CountMoveNumber();
        UpdatePlayerState();
    }

    private void UpdatePlayerState()
    {
        if (Time.frameCount % 50 == 0)
        {
            CheckBoardPlayer();
            CheckCurrentBoard();
        }
    }

    private void CountMoveNumber()
    {
        if (NumberOfMoves == 9)
        {
            ResetButton.SetActive(true);
            NumberOfMoves++;
        }
    }

    private TextMeshProUGUI primaryHold;
    private void AnimateGrowth()
    {
        if (canGrow && !AI)
        {
            primaryHold = TextHold;
            primaryHold.fontSize += Time.time * GrowFactor;
            
            
            if (primaryHold.fontSize >= 150)
            {
                canGrow = false;
            }
        }
    }


    private void OnEnable()
    {
        AllButtons[0].onClick.AddListener(PlayerMove00);
        AllButtons[1].onClick.AddListener(PlayerMove01);
        AllButtons[2].onClick.AddListener(PlayerMove02);
        
        AllButtons[3].onClick.AddListener(PlayerMove10);
        AllButtons[4].onClick.AddListener(PlayerMove11);
        AllButtons[5].onClick.AddListener(PlayerMove12);
        
        AllButtons[6].onClick.AddListener(PlayerMove20);
        AllButtons[7].onClick.AddListener(PlayerMove21);
        AllButtons[8].onClick.AddListener(PlayerMove22);
    }


    public void StrikeLine(int index)
    {
        AllStrikes[index].SetActive(true);
        foreach (Button B in AllButtons)
        {
            B.interactable = false;
        }
        
        ResetButton.SetActive(true);
    }

    public void UpdatePlayType(int index)
    {
        PlayText.text = PLAY[index];
    }

    private void PlayerMove00()
    {
        Update_Button(AllButtons[0],(int)currentTick);
        SwapingTurns();
    }

    private void SwapingTurns()
    {
        if (!AI)
        {
            UpdatePlayType(current);
            current = Math.Abs(current + (-1));
            if (current == 0)
            {
                currentTick = TICK.CROSS;
            }
            else
            {
                currentTick = TICK.CIRCLE;
            }

            NumberOfMoves++;
           
        }
        else
        {
            currentTick = TICK.CIRCLE;
            NumberOfMoves++;
            
            if(CheckAvailableMove())
            {
                AIPlays();
            }
        }
    }

    private void PlayerMove01()
    {
        Update_Button(AllButtons[1], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove02()
    {
        Update_Button(AllButtons[2], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove10()
    {
        Update_Button(AllButtons[3], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove11()
    {
        Update_Button(AllButtons[4], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove12()
    {
        Update_Button(AllButtons[5], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove20()
    {
        Update_Button(AllButtons[6], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove21()
    {
        Update_Button(AllButtons[7], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove22()
    {
        Update_Button(AllButtons[8], (int)currentTick);
        SwapingTurns();
    }

    public void AI_Move(int position, int index)
    {
        Update_Button_AI(AllButtons[position], index);
        AllButtons[position].interactable = false;
        NumberOfMoves++;
    }
    
    private void Update_Button(Button _b,int index)
    {
        
        TextHold = _b.GetComponentInChildren<TextMeshProUGUI>();
        TextHold.fontSize = !AI ? MinGrowSize : MaxGrowSize;
        _b.interactable = false;
        TextHold.text = TICKS[index];
        canGrow = true;
    }
    
    private void Update_Button_AI(Button _b,int index)
    {
        
        TextHold = _b.GetComponentInChildren<TextMeshProUGUI>();
        TextHold.fontSize = MaxGrowSize;
        _b.interactable = false;
        TextHold.text = TICKS[index];
    }

    private void OnDisable()
    {
        AllButtons[0].onClick.RemoveListener(PlayerMove00);
        AllButtons[1].onClick.RemoveListener(PlayerMove01);
        AllButtons[2].onClick.RemoveListener(PlayerMove02);
        
        AllButtons[3].onClick.RemoveListener(PlayerMove10);
        AllButtons[4].onClick.RemoveListener(PlayerMove11);
        AllButtons[5].onClick.RemoveListener(PlayerMove12);
        
        AllButtons[6].onClick.RemoveListener(PlayerMove20);
        AllButtons[7].onClick.RemoveListener(PlayerMove21);
        AllButtons[8].onClick.RemoveListener(PlayerMove22);
    }
    
    }

}
