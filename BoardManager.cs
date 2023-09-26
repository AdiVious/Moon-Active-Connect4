using MoonActive.Connect4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class BoardManager : MonoBehaviour
{

    [SerializeField] GameManager gameManager;

    // how many pieces in a row will you need for a win. 
    [SerializeField] private int NumForWin = 4;

    //how many total slots are there
    [SerializeField] private int TotalSlots = 42;

    //Column Hieght Allowed
    public int ColumnNum = 7;

    //Rows Lengh Allowed
    public int RowsNum = 6;





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Clears all Disks droped as well as clears the variables for all of the colliders.
    public void ClearBoard()
    {
        GameObject[] Disks = GameObject.FindGameObjectsWithTag("Disk");
        foreach (GameObject disk in Disks) { Destroy(disk); }

        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");
        foreach (GameObject Collider in Colliders)
        {

            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();
            boardCollider.full = false;
            boardCollider.playerPiece = 0; // not really necessery but for good practice
        }

    }

    //Set the grid as a two dimentional array, this would normally be set in the board set up but I have no access to the api code.
    public void setBoardColliders()
    {
        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");

        // one number less since we are starting from 0
        int rowCounter = 5;
        int colCounter = 0;

        foreach (GameObject Collider in Colliders)
        {


            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();
            boardCollider.col = colCounter;
            boardCollider.row = rowCounter;

            colCounter++;

            if (colCounter == 7)
            {
                colCounter = 0;
                rowCounter--;
            }

        }

    }


    //fill a collider and set its player piece value by using the current turn
    public void fillCollider(int col, int row, int turn)
    {
        
        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");

        foreach (GameObject Collider in Colliders)
        {
            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();
            if (col == boardCollider.col && row == boardCollider.row)
            {
                boardCollider.full = true;
                boardCollider.playerPiece = turn;
            }
        }
    }

    //Check for the current hight of a given column - how many Disks are in this column
    public int checkForColumnHeight(int col)
    {
        int counter = 0;

        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");

        foreach (GameObject Collider in Colliders)
        {
            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();
            if (boardCollider.col == col && boardCollider.full)
            {
                counter++;
            }

        }

        return counter;

    }

    //return true if the chosen col can fit another Disk in it.
    public bool isMoveAllowed(int col)
    {
        if (checkForColumnHeight(col) < ColumnNum)
        {
            return true;
        }

        else return false;
    }

    //get the value of the which player's this collider belongs to.
    private int getFullColliderPlayerNumber(int col, int row)
    {
        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");

        foreach (GameObject Collider in Colliders)
        {
            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();

            if (boardCollider.col == col && boardCollider.row == row && boardCollider.full)
            {
                return boardCollider.playerPiece;
            }

        }
        return 999; // just an invalid value 
    }





    /* 
     * return: 
     * 0 - Game Over Player 0 Won
     * 1 - Game Over Player 1 Won
     * 2 - Game Over No More Moves - Tie
     * 3 - Game Not Over
     */


    public int checkForWin(int col, int row, int turn)
    {
        //Debug.Log("vert: " + getVerticalLength(col, row));
        //Debug.Log("hori: " + getHorizontalLength(col, row));
        if (getVerticalLength(col, row, turn) >= NumForWin || getHorizontalLength(col, row , turn) >= NumForWin || getUpRightLength(col, row, turn) >= NumForWin || getDownRightLength(col, row, turn) >= NumForWin)
        {

            Debug.Log("we won!!");
            return (gameManager.playerTurn);
        }


        int filledCounter = 0;

        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");

        foreach (GameObject Collider in Colliders)
        {

            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();

            if (boardCollider.full)
            {
                filledCounter++;
            }

        }

        if (filledCounter >= TotalSlots)
        {
            return 2;
        }


        return 3;
    }

/*
    public void SetDisksStatic()
    {
        GameObject[] Disks = GameObject.FindGameObjectsWithTag("Disk");

        foreach (GameObject Disk in Disks)
        {
            Disk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    public void DisableColliderCollision(int col, int row)
    {

        GameObject Collider = PickCollider(col, row);
        Collider.GetComponent<Collider2D>().enabled = false;

    }

*/

    private GameObject PickCollider(int col, int row)
    {

        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");

        foreach (GameObject Collider in Colliders)
        {
            BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();

            if (boardCollider.col == col && boardCollider.row == row)
            {
                return Collider;
            }

        }
        return null;
    }

    private int getVerticalLength(int col, int row, int turn)
    {
        int minCol = col;
        int minRow = row;
        int maxCol = col;
        int maxRow = row;

        while (minRow > 0 && getFullColliderPlayerNumber(minCol, minRow - 1) == turn)
        {
            minRow--;
        }

        while (maxRow < (RowsNum) && getFullColliderPlayerNumber(maxCol, maxRow + 1) == turn)
        {
            maxRow++;
        }



        return (maxRow - minRow + 1);// + 1 is for counting the current dropped disk
    }


    private int getHorizontalLength(int col, int row, int turn)
    {

        int minCol = col;
        int minRow = row;
        int maxCol = col;
        int maxRow = row;



        while (minCol > 0 && getFullColliderPlayerNumber(minCol - 1, minRow) == turn)
        {
            minCol--;
        }

        while (maxCol < (ColumnNum) && getFullColliderPlayerNumber(maxCol + 1, maxRow) == turn)
        {
            maxCol++;
        }
        return (maxCol - minCol + 1);
    }




    private int getUpRightLength(int col, int row, int turn)
    {
        int minCol = col;
        int minRow = row;
        int maxCol = col;
        int maxRow = row;


        while (minCol > 0 && minRow > 0 && getFullColliderPlayerNumber(minCol - 1, minRow - 1) == turn)
        {
            minCol--;
            minRow--;
        }

        while (maxCol < (ColumnNum) && maxRow < RowsNum && getFullColliderPlayerNumber(maxCol + 1, maxRow + 1) == turn)
        {
            maxCol++;
            maxRow++;
        }
        return maxRow - minRow + 1;
    }

    private int getDownRightLength(int col, int row, int turn)
    {
        int minCol = col;
        int minRow = row;
        int maxCol = col;
        int maxRow = row;


        while (minCol > 0 && minRow > 0 && getFullColliderPlayerNumber(minCol + 1, minRow - 1) == turn)
        {
            minCol++;
            minRow--;
        }

        while (maxCol < (ColumnNum) && maxRow < RowsNum && getFullColliderPlayerNumber(maxCol - 1, maxRow + 1) == turn)
        {
            maxCol--;
            maxRow++;
        }
        return maxRow - minRow + 1;
    }


    private void undoMove(int col , int row)
    {
        GameObject Collider = PickCollider(col , row);
        BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();
        boardCollider.full = false;
        boardCollider.playerPiece = 0; 
    }

    //currently not being used
    // This is just a solution to pick the Disk located on a collider since i only have access for the IDisk from the api.
    public GameObject PickDiskOnCollider(int col , int row)
    {
        
        GameObject Collider = PickCollider(col, row);
        GameObject[] Disks = GameObject.FindGameObjectsWithTag("Disk");
        GameObject DiskToReturn = Disks[0];
        

        if(DiskToReturn != null)
        {
            float distance = Vector3.Distance(Collider.transform.position, DiskToReturn.transform.position);

            foreach (GameObject Disk in Disks)
            {
                float TempDistance = Vector3.Distance(Collider.transform.position, Disk.transform.position);

                if (TempDistance < distance)
                {
                    distance = TempDistance;
                    DiskToReturn = Disk;
                }

            }
        }
            
        return DiskToReturn;

    }

    public GameObject PickDisk(int col , int row)
    {
        GameObject[] Disks = GameObject.FindGameObjectsWithTag("Disk");

        foreach (GameObject Disk in Disks)
        {
            
              DiskVars diskVars = Disk.GetComponent<DiskVars>();
              
              if(diskVars.row == row && diskVars.col == col)

                {
                return Disk; 
                }

        }
        return null;

    }

    // returns a list of all the columns that still have remaining space
    private List<int> getNonFullColumns()
    {
        List<int> freeCols = new List<int>();

        for(int i = 0; i <= ColumnNum ; i++)
        {
           
            if(checkForColumnHeight(i) < RowsNum - 1)
            {
                Debug.Log(checkForColumnHeight(i));
                freeCols.Add(i);
            }
        }

        return freeCols;

    }
    /*
     * This function picks a move for the Easy bot difficulty, given more time (Yom kippur was in the way)
     * I would make two additional function for medium and hard difficulty, both would use the same MinMax algorithm with diffrent depths to calculate the best move by looking at possible future moves.         
     *for now the function first looks for a winning move and if there is one picks it, than looks for a move that blocks a winning move for the opponent, if none of those exist just picks a random valid move by looking for non empty columns.
     */

    public int ComputerTurnEasy()
    {
        GameObject[] Colliders = GameObject.FindGameObjectsWithTag("Collider");
        GameObject[] Disks = GameObject.FindGameObjectsWithTag("Disk");

        //Check for a winning move
        for(int i = 0; i <= ColumnNum ; i++)
        {
            int freeRow = RowsNum - checkForColumnHeight(i) - 2;           
            
            if (freeRow <= RowsNum)
            {                
                GameObject Collider = PickCollider(i, freeRow);

               if(Collider != null)
                {
                    BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();

                    if (!boardCollider.full)
                    {
                        // fill collider, than check if the move is winning and undo the move. 
                        boardCollider.full = true;
                        boardCollider.row = freeRow;
                        boardCollider.playerPiece = gameManager.playerTurn;
                        boardCollider.col = i;

                        int checkWin = checkForWin(i, freeRow , boardCollider.playerPiece);

                        if (checkWin == gameManager.playerTurn)
                        {
                            undoMove(i, freeRow);
                            return i;
                        }
                        else
                        {
                            undoMove(i, freeRow);
                        }
                    }

                    
                }
                                         
            }
        }
        //if there is no winning move search for a move to block the oponent's win.
        for(int i = 0; i <= ColumnNum ; i++)
        {
            
            int freeRow = RowsNum - checkForColumnHeight(i) -2;           
            if (freeRow <= RowsNum)
            {

                GameObject Collider = PickCollider(i, freeRow);

                if (Collider != null)
                {
                    BoardCollider boardCollider = Collider.GetComponent<BoardCollider>();

                    if (!boardCollider.full)
                    {
                        //fill collider, This time with the opponet Disk, if the move is winning return the column to block the win. 
                        boardCollider.full = true;
                        boardCollider.row = freeRow;
                        boardCollider.playerPiece = (gameManager.playerTurn == 0) ? 1 : 0;
                        boardCollider.col = i;
                        

                        int checkWin = checkForWin(i, freeRow , boardCollider.playerPiece);                       

                        if (checkWin != 3) // if there is a win or a tie this is the best move to do so we do it. 
                        {
                            
                            undoMove(i, freeRow);
                            return i;
                        }
                        else
                        {
                            undoMove(i, freeRow);
                        }
                    }
                      

                }
                
            }

        }
        //if we got here there is no winning move and no win-blocking move so just pick a random move
        List<int> freeCols = getNonFullColumns();

        foreach (int col in freeCols)
        {
            Debug.Log("Element in freeCols: " + col);
        }

        if (freeCols.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, freeCols.Count);
            Debug.Log(freeCols[randomIndex]);
            return freeCols[randomIndex];
        }

        else
        {
            Debug.LogError("No Free Column for computer to pick from");
            return 0;
        }
    }


    /////////////////////////////////////////

    //private int MinMaxAlgorithm( int depth , heuristic , int maximizingPlayer , int lastIndex , bool returnCol)

}


