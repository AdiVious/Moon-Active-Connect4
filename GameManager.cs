using MoonActive.Connect4;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;





public class GameManager : MonoBehaviour

    
{

    public UnityEvent onDiskStoppedFalling;

    [Header("----- Prefabs -----")]

    [SerializeField] Disk diskA;
    [SerializeField] Disk diskB;
    [SerializeField] GameObject Board;

    [SerializeField] UIManager uiManager;
    [SerializeField] AudioManager audioManager;


    private ConnectGameGrid grid;
    private BoardManager boardManager;


    [SerializeField] private int gameState = 0;


    /*
     * modes: 
     * 0 = player Vs player
     * 1 = player Vs Computer
     * 2 = Computer Vs Computer
     */


    private int WinState = 3;
    public int gameMode = 0;
    [SerializeField] private bool isPlayingAgainstComputer = false;

    /* 
     * 0 = Easy
     * 1 = Medium
     * 0 = Hard
     */
    [SerializeField] private int Difficulty = 0;
    [SerializeField] private int TurnCounter = 0;


    private int currentPlayedCol;
    private int currentPlayedRow;

    public int playerToStart = 1; // the player we want to start, there is also the option for random choosing in the UI manager but the requiement were for the blue to always start.
   
    public int playerTurn = 0; // between 0 and 1 , computer turn is also turn 0
                               //private bool isOnComputerTurn = false;

    private bool canDrop = true;

    //to fix a bug where we would get unwanted collisions(since we are using rigid body) 
    private bool areCollisionsAllowed = true;





    // Start is called before the first frame update
    void Start()
    {

        
        grid = Board.GetComponent<ConnectGameGrid>();
        boardManager = Board.GetComponent<BoardManager>();

        // Subscribe to events       
        grid.ColumnClicked += OnColumnClicked;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ResetCollisionCoolDown()
    {
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        areCollisionsAllowed = true;
    }


    public void OnDiskStoppedFalling()
    {
        
        // Handle the event when the disk stops falling
        //Debug.Log("Disk stopped falling!");

        //disable the collision for the collider as we wont be needing it anymore.
        //boardManager.DisableColliderCollision(currentPlayedCol, currentPlayedRow);
        if (areCollisionsAllowed)
        {

            areCollisionsAllowed = false;
            StartCoroutine(ResetCollisionCoolDown());

            /*
            GameObject DiskGameObject = boardManager.PickDiskOnCollider(currentPlayedCol, currentPlayedRow);
            if (DiskGameObject != null)
            {
                DiskVars diskVars = DiskGameObject.GetComponent<DiskVars>();
                diskVars.row = currentPlayedRow;
                diskVars.col = currentPlayedCol;
                diskVars.playerPiece = playerTurn;
            }
            */
            
            WinState = boardManager.checkForWin(currentPlayedCol, currentPlayedRow, playerTurn);

            if (WinState != 3)
            {

                uiManager.OpenWinScreen(WinState);
                audioManager.PlaySFX(audioManager.Win);

            }
            else
            {
                switchTurns();                
            }
            
         
        }
        
       
     }

    public void OnColumnClicked(int column)
    {
      
        if (boardManager.isMoveAllowed(column) && canDrop && WinState == 3)
        {
            // before dropping a new disk set all disks as static so we dont get any unwanted Disk Collision
            //boardManager.SetDisksStatic(); 

            IDisk currentDisk = grid.Spawn((playerTurn == 0) ? diskA : diskB, column, 0);            
            audioManager.PlaySFX(audioManager.DiskDrop);
            currentDisk.StoppedFalling += OnDiskStoppedFalling;
            boardManager.fillCollider(column, (boardManager.RowsNum - boardManager.checkForColumnHeight(column) - 2), playerTurn);

            
            currentPlayedCol = column;
            currentPlayedRow = boardManager.RowsNum - boardManager.checkForColumnHeight(currentPlayedCol) - 1;
            
            canDrop = false;

            

        }


                
       
    }

    //Make the move picked by the Bot on the board.
    private void makeComputerMove(int col, int row) 
    {
        if(WinState == 3)
        {
            IDisk currentDisk = grid.Spawn((playerTurn == 0) ? diskA : diskB, col, 0);
            audioManager.PlaySFX(audioManager.DiskDrop);
            currentDisk.StoppedFalling += OnDiskStoppedFalling;
            boardManager.fillCollider(col, (boardManager.RowsNum - boardManager.checkForColumnHeight(col) - 2), playerTurn);

            
            currentPlayedCol = col;
            currentPlayedRow = boardManager.RowsNum - boardManager.checkForColumnHeight(currentPlayedCol) - 1;
            canDrop = false;
        }

        else
        {
            Debug.LogError("Computer made to pick but game is over");
        }
        
    }




    //Start a new game, reset variables.
    public void StartGame(int PlayerToStart)
    {
        playerTurn = PlayerToStart;
        WinState = 3;
        canDrop = true;
        boardManager.setBoardColliders();

        if(gameMode == 2)
        {
            int computerColPick;
            computerColPick = boardManager.ComputerTurnEasy();
            makeComputerMove(computerColPick, 0);
        }

    }

    public void RestartGame()
    {
        boardManager.ClearBoard();       
        uiManager.CloseWinScreen();
        StartGame(playerToStart);


    }

    //Switch the players turns, and act according to the corrent GameMode.
    private void switchTurns()
    {
        playerTurn = (playerTurn == 0) ? 1 : 0;

        if((playerTurn == 0 && gameMode == 1) || gameMode == 2)
        {
           int computerColPick; 
           computerColPick = boardManager.ComputerTurnEasy();
            
            makeComputerMove(computerColPick, 0);

        }

        else if((playerTurn == 1 && gameMode == 1) || gameMode == 0)
        {
            canDrop = true;
        }

        
    }


  
}

