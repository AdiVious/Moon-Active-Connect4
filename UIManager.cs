using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;



public class UIManager : MonoBehaviour
{
    [Header("----- GameObjects -----")]

    [SerializeField] private GameObject Selection;

    [SerializeField] private GameObject Options;

    [SerializeField] private GameManager gameManager;
                         
    [SerializeField] private GameObject Board;

    [SerializeField] private GameObject ComputerButton;

    [SerializeField] private GameObject PlayerVsPlayerButton;

    [SerializeField] private GameObject ComputerVsComputerButton;

    [SerializeField] private GameObject WinScreen;

    [SerializeField] private AudioManager audioManager;



    [Header("----- Selection Page Settings -----")]

    [SerializeField] private float SelctionInOutTime = 1;


    [Header("----- Options Settings -----")]

    [SerializeField] private Scrollbar sfxSlider;

    private bool OptionsOpen = false;

    [SerializeField] private float OptionsInOutTime = 1;


    [Header("----- Win Screen Settings -----")]

    private bool WinScreenOpen = false;

    [SerializeField] private GameObject DiskPlayer0 ,DiskPlayer1;

    [SerializeField] private Text WinText;


    [Header("----- Buttons Tween Settings -----")]

    [SerializeField] private float buttonsTweenScale = 0.9f;
    [SerializeField] private float buttonsTweenTime = 0.1f;


    


    // Start is called before the first frame update
    void Start()
    {

        Board.SetActive(false);
        Selection.SetActive(true);
        WinScreen.SetActive(false);
        
    }


    /*
     * modes: 
     * 0 = player Vs player
     * 1 = player Vs Computer
     * 2 = Computer Vs Computer
     */

    // Update is called once per frame
    void Update()
    {

        if( gameManager.gameMode == 0)
        {
            ComputerButton.GetComponent<Outline>().enabled = false;
            PlayerVsPlayerButton.GetComponent<Outline>().enabled = true;
            ComputerVsComputerButton.GetComponent<Outline>().enabled = false;
        }
        else if( gameManager.gameMode == 1)
        {
            ComputerButton.GetComponent<Outline>().enabled = true;
            PlayerVsPlayerButton.GetComponent<Outline>().enabled = false;
            ComputerVsComputerButton.GetComponent<Outline>().enabled = false;
        }
        else
        {
            ComputerButton.GetComponent<Outline>().enabled = false;
            PlayerVsPlayerButton.GetComponent<Outline>().enabled = false;
            ComputerVsComputerButton.GetComponent<Outline>().enabled = true;
        }
    }


    public void PlayVsComputer()
    {
        gameManager.gameMode = 1;
    }

    public void PlayVsPlayer()
    {
        
        gameManager.gameMode = 0;

    }

    public void ComputerVsComputer()
    {

        gameManager.gameMode = 2;

    }


    public void HandleOptionsMenu()
    {
        if(OptionsOpen == false)
        {
            OpenOptionsMenu();
        }
        else
        {
            CloseOptionsMenu();
        }
        
    }

    private void OpenOptionsMenu()
    {
        OptionsOpen = true;
        Board.SetActive(false);
        Options.transform.DOMoveY(Screen.height/2, OptionsInOutTime).SetEase(Ease.OutBack);
        audioManager.PlaySFX(audioManager.Swish);
    }

    private void CloseOptionsMenu()
    {
        audioManager.PlaySFX(audioManager.Swish);
        Options.transform.DOMoveY(Screen.height * 2, OptionsInOutTime)
            .OnComplete(() =>
             {
                OptionsOpen = false;
                Board.SetActive(true);
             })   
            .SetEase(Ease.InBack);

    }


    public void sfxVolume()
    {
        audioManager.sfxVolume(sfxSlider.value);
    }


    public void CloseSelectionPage()
    {
        Selection.transform.DOMoveY(Screen.height * 2, SelctionInOutTime).SetEase(Ease.InBack);
        audioManager.PlaySFX(audioManager.Swish);
        Board.SetActive(true);
    }

    public void InitilaizeGame()
    {
        CloseSelectionPage();
        gameManager.StartGame(gameManager.playerToStart);
    }

    
    public void OpenWinScreen(int PlayerWon)
    {
        // Delay the activation of the WinScreen by 3 seconds
        Invoke("ActivateWinScreen", 3f);

        DiskPlayer0.SetActive(PlayerWon == 0);
        DiskPlayer1.SetActive(PlayerWon == 1);

        if (PlayerWon == 2)
        {
            WinText.text = "Tie";

            WinText.transform.position = new Vector3( Screen.width / 2f , WinText.transform.position.y , WinText.transform.position.z); 
        }
    }

    private void ActivateWinScreen()
    {
        WinScreen.SetActive(true);
    }


    public void CloseWinScreen()
    {
        WinScreen.SetActive(false);
    }

    public void CallForGameRestart()
    {
        gameManager.RestartGame();
    }


    public void ButtonClickedTween(GameObject button)
    {
        // Check if a tween is currently playing on the button's transform
        if (!DOTween.IsTweening(button.transform))
        {
            // Start the tween only if no tween is currently playing
            button.transform.DOScale(buttonsTweenScale, buttonsTweenTime).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
        }
    }

    public void KeyPressedSound()
    {
        audioManager.PlaySFX(audioManager.KeyPress);
    }


    //Not Currently being used since blue start first was a requirment 
    private int PickStartingPlayer()
    
    {
        int rnd = UnityEngine.Random.Range(0, 1);     
        return rnd;
    }
}
