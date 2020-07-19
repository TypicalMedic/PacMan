﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{

    private static int boardWight = 50;
    private static int boardHight = 50;

    private bool didStartDeath = false;
    private bool didStartConsumed = false;

    public static int playerOneLevel = 1;
    public static int playerTwoLevel = 1;

    public int totalPellets = 242;
    public int score = 0;
    public static int playerOneScore = 0;
    public static int playerTwoScore = 0;
    public static int highScore = 0;

    public static int ghostConsumedRunningScore;

    public static bool isPlayerOneUp = true;
    public bool shouldBlink = false;

    public float blinkIntervalTime = 0.1f;
    private float blinkIntervalTimer = 0;

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;
    public AudioClip backgroundAudioPacManDeath;
    public AudioClip consumedGhostAudioClip;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public Text playerText;
    public Text readyText;

    public Text highScoreText;
    public Text playerOneUp;
    public Text playerTwoUp;
    public Text playerOneScoreText;
    public Text playerTwoScoreText;
    public Image playerLives2;
    public Image playerLives3;

    public Text consumedGhostScoreText;

    public GameObject[,] board = new GameObject[boardWight, boardHight];
    GameObject[] gameObjects;

    public Image[] levelImages;

    private bool didIncrementLevel = false;

    bool didSpawnBonusItem1_player1;
    bool didSpawnBonusItem2_player1;
    bool didSpawnBonusItem1_player2;
    bool didSpawnBonusItem2_player2;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1024, 768, false);

        highScoreText.text = highScore.ToString();

        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        gameObjects = GameObject.FindGameObjectsWithTag("Pause");

        foreach (GameObject o in gameObjects)
        {
            o.SetActive(false);

        }

        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;

            if (o.name != "PacMan" && o.tag != "Ghost" && o.name != "Canvas" && o.name != "EventSystem" && o.tag != "UIElements"&&o.tag != "Pause")
            {
                if (o.GetComponent<Tile>() != null)
                {
                    if (o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isPowerPellet)
                    {
                        totalPellets++;
                    }
                }

                board[(int)pos.x, (int)pos.y] = o;

            }
            else
            {

            }
        }

        if (isPlayerOneUp)
        {

            if (playerOneLevel == 1)
            {

                GetComponent<AudioSource>().Play();

            }

        }
        else
        {

            if (playerTwoLevel == 1)
            {

                GetComponent<AudioSource>().Play();

            }

        }

        StartGame();
    }

    void Update()
    {

        UpdateUI();
        CheckPelletsConsumed();
        CheckShouldBlink();
        BonusItem();

        if (highScore < playerOneScore+10)
        {
            highScore = playerOneScore;
            highScoreText.text = playerOneScoreText.text;
        }
        else if (highScore < playerTwoScore+10)
        {
            highScore = playerTwoScore;
            highScoreText.text = playerTwoScoreText.text;
        }

        if (Input.GetKeyDown(KeyCode.Escape)&&Time.timeScale!=0)
        {
            foreach (GameObject o in gameObjects)
            {
                o.SetActive(true);

            }

            AudioListener.pause = true;
            Time.timeScale = 0;

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0)
        {
            AudioListener.pause = false;
            Time.timeScale = 1;

            foreach (GameObject o in gameObjects)
            {
                o.SetActive(false);

            }
        }
    }

    void BonusItem()
    {
        if (GameMenu.isOnePlayerGame)
        {
            //SpriteUpdate();
            SpawnBonusItemForPlayer(1);
        }
        else
        {
            if (isPlayerOneUp)
            {
                SpawnBonusItemForPlayer(1);
            }
            else
            {
                SpawnBonusItemForPlayer(2);
            }
        }

    }


    void SpriteUpdate()
    {
        if (OutFit.onePlayerIdleSprite != null)
        {

            GameObject.FindGameObjectWithTag("PacMan").transform.GetComponent<SpriteRenderer>().sprite = OutFit.onePlayerIdleSprite;

            GameObject.FindGameObjectWithTag("PacMan").GetComponent<Animator>().runtimeAnimatorController = OutFit.onePlayerChompAnimation;

            GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>().chompAnimation = OutFit.onePlayerChompAnimation;
            GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>().deathAnimation = OutFit.onePlayerDeathAnimation;
            GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>().idleSprite = OutFit.onePlayerIdleSprite;

        }
    }

    void SpriteUpdate2()
    {
        if (OutFit.twoPlayerIdleSprite != null)
        {

            GameObject.FindGameObjectWithTag("PacMan").transform.GetComponent<SpriteRenderer>().sprite = OutFit.twoPlayerIdleSprite;

            GameObject.FindGameObjectWithTag("PacMan").GetComponent<Animator>().runtimeAnimatorController = OutFit.twoPlayerChompAnimation;

            GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>().chompAnimation = OutFit.twoPlayerChompAnimation;
            GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>().deathAnimation = OutFit.twoPlayerDeathAnimation;
            GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>().idleSprite = OutFit.twoPlayerIdleSprite;
        }
    }



    void SpawnBonusItemForPlayer(int playerNum)
    {
        if (playerNum == 1)
        {
            if (GameMenu.playerOnePelletsConsumed >= 70 && GameMenu.playerOnePelletsConsumed < 170)
            {
                if (!didSpawnBonusItem1_player1)
                {
                    didSpawnBonusItem1_player1 = true;
                    SpawnBonusItemForLevel(playerOneLevel);
                }
            }
            else if (GameMenu.playerOnePelletsConsumed >= 170)
            {
                if (!didSpawnBonusItem2_player1)
                {
                    didSpawnBonusItem2_player1 = true;
                    SpawnBonusItemForLevel(playerOneLevel);
                }
            }
        }
        else
        {
            if (GameMenu.playerTwoPelletsConsumed >= 70 && GameMenu.playerTwoPelletsConsumed < 170)
            {
                if (!didSpawnBonusItem1_player2)
                {
                    didSpawnBonusItem1_player2 = true;
                    SpawnBonusItemForLevel(playerTwoLevel);
                }
            }
            else if (GameMenu.playerTwoPelletsConsumed >= 170)
            {
                if (!didSpawnBonusItem2_player2)
                {
                    didSpawnBonusItem2_player2 = true;
                    SpawnBonusItemForLevel(playerTwoLevel);
                }
            }
        }
    } 

    void SpawnBonusItemForLevel(int level)
    {
        GameObject bonusItem = null;

        if (level == 1)
        {
            bonusItem = Resources.Load("Prefabs/bonus_cherries", typeof(GameObject)) as GameObject;
        }
        else if (level == 2)
        {
            bonusItem = Resources.Load("Prefabs/bonus_strawberry", typeof(GameObject)) as GameObject;
        }
        else if (level == 3)
        {
            bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
        }
        else if (level == 4)
        {
            bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
        }
        else if (level == 5)
        {
            bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
        }
        else if (level == 6)
        {
            bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
        }
        else if (level == 7)
        {
            bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
        }
        else if (level == 8)
        {
            bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
        }
        else if (level == 9)
        {
            bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
        }
        else if (level == 10)
        {
            bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
        }
        else if (level == 11)
        {
            bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
        }
        else if(level == 12)
        {
            bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
        }
        else
        {
            bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
        }

        Instantiate(bonusItem);
    }

    void UpdateUI()
    {

        playerOneScoreText.text = playerOneScore.ToString();
        playerTwoScoreText.text = playerTwoScore.ToString();

        int currentLevel;

        if (isPlayerOneUp)
        {
            currentLevel = playerOneLevel;

            if (GameMenu.livesPlayerOne == 3)
            {

                playerLives3.enabled = true;
                playerLives2.enabled = true;

            }
            else if (GameMenu.livesPlayerOne == 2)
            {

                playerLives3.enabled = false;
                playerLives2.enabled = true;

            }
            else if (GameMenu.livesPlayerOne == 1)
            {

                playerLives3.enabled = false;
                playerLives2.enabled = false;

            }

        }
        else
        {

            currentLevel = playerTwoLevel;

            if (GameMenu.livesPlayerTwo == 3)
            {

                playerLives3.enabled = true;
                playerLives2.enabled = true;

            }
            else if (GameMenu.livesPlayerTwo == 2)
            {

                playerLives3.enabled = false;
                playerLives2.enabled = true;

            }
            else if (GameMenu.livesPlayerTwo == 1)
            {

                playerLives3.enabled = false;
                playerLives2.enabled = false;

            }
        }  

        for(int i =0; i < levelImages.Length; i++)
        {
            Image li = levelImages[i];
            li.enabled = false;
        }

        for(int i =1; i < levelImages.Length; i++)
        {
            if (currentLevel >= i)
            {
                Image li = levelImages[i - 1];
                li.enabled = true;
            }
        }
    }

    void CheckPelletsConsumed()
    {

        if (isPlayerOneUp)
        {

            if (totalPellets == GameMenu.playerOnePelletsConsumed)
            {

                PlayerWin(1);

            }

        }
        else
        {

            if (totalPellets == GameMenu.playerTwoPelletsConsumed)
            {

                PlayerWin(2);

            }

        }

    }

    void PlayerWin(int playerNum)
    {

        if (playerNum == 1) 
        {
            if (!didIncrementLevel)
            {
                didIncrementLevel = true;

                playerOneLevel++;

                StartCoroutine(ProcessWin(2));

            }

        }
        else
        {
            if (!didIncrementLevel)
            {
                didIncrementLevel = true;

                playerTwoLevel++;

                StartCoroutine(ProcessWin(2));

            }
        }
    }

    IEnumerator ProcessWin(float delay)
    {

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().canMove = false;
        pacMan.transform.GetComponent<Animator>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {

            ghost.transform.GetComponent<Ghost>().canMove = false;
            ghost.transform.GetComponent<Animator>().enabled = false;

        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2));

    }

    IEnumerator BlinkBoard(float delay)
    {

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;

        }

        shouldBlink = true;

        yield return new WaitForSeconds(delay);

        shouldBlink = false;

        StartNextLevel();

    }

    private void StartNextLevel()
    {

        StopAllCoroutines();

        if (isPlayerOneUp)
        {
            ResetPelletForPlayer(1);
            GameMenu.playerOnePelletsConsumed = 0;
            didSpawnBonusItem1_player1 = false;
            didSpawnBonusItem2_player1 = false;
        }
        else 
        {
            ResetPelletForPlayer(2);
            GameMenu.playerTwoPelletsConsumed = 0;
            didSpawnBonusItem1_player2 = false;
            didSpawnBonusItem2_player2 = false;
        }

        GameObject.Find("WhiteBoard").transform.GetComponent<SpriteRenderer>().color = Color.blue;

        didIncrementLevel = false;

        StartCoroutine(ProcessStartNextLevel(1));
    }

    IEnumerator ProcessStartNextLevel(float delay)
    {

        playerText.transform.GetComponent<Text>().enabled = true;
        readyText.transform.GetComponent<Text>().enabled = true;

        if (isPlayerOneUp)
        {
            StartCoroutine(StartBlinking(playerOneUp));
        }
        else
        {
            StartCoroutine(StartBlinking(playerTwoUp));
        }

        RedrawBoard();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestartShowObjects(1));
    }

    private void CheckShouldBlink()
    {

        if (shouldBlink)
        {

            if (blinkIntervalTimer < blinkIntervalTime)
            {

                blinkIntervalTimer += Time.deltaTime;

            }
            else
            {

                blinkIntervalTimer = 0;

                if(GameObject.Find("WhiteBoard").transform.GetComponent<SpriteRenderer>().color == Color.blue)
                {

                    GameObject.Find("WhiteBoard").transform.GetComponent<SpriteRenderer>().color = Color.white;

                }
                else
                {

                    GameObject.Find("WhiteBoard").transform.GetComponent<SpriteRenderer>().color = Color.blue;

                }

            }

        }

    }

    public void StartGame()
    {

        if (GameMenu.isOnePlayerGame)
        {

            playerTwoUp.GetComponent<Text>().enabled = false;
            playerTwoScoreText.GetComponent<Text>().enabled = false;

        }
        else
        {

            playerTwoUp.GetComponent<Text>().enabled = true;
            playerTwoScoreText.GetComponent<Text>().enabled = true;

        }

        if (isPlayerOneUp)
        {
            StartCoroutine(StartBlinking(playerOneUp));

        }
        else
        {
            StartCoroutine(StartBlinking(playerTwoUp));

        }

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;

        }

        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = false;

        StartCoroutine(ShowObjectsAfter(2.25f));
    }

    public void StartConsumed(Ghost consumedGhost)
    {

        if (!didStartConsumed)
        {

            didStartConsumed = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {

                ghost.transform.GetComponent<Ghost>().canMove = false;

            }

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            Vector2 pos = consumedGhost.transform.position;

            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            consumedGhostScoreText.text = ghostConsumedRunningScore.ToString();

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            transform.GetComponent<AudioSource>().PlayOneShot(consumedGhostAudioClip);

            StartCoroutine(ProcessConsumedAfter(0.75f, consumedGhost));           
        }
    }

    public void StartConsumedBonusItem(GameObject bonusItem, int scoreValue)
    {
        Vector2 pos = bonusItem.transform.position;
        Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

        consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
        consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

        consumedGhostScoreText.text = scoreValue.ToString();

        consumedGhostScoreText.GetComponent<Text>().enabled = true;

        Destroy(bonusItem.gameObject);

        StartCoroutine(ProcessConsumedBonusItem(0.75f));
        
    }
    
    IEnumerator ProcessConsumedBonusItem(float delay)
    {
        yield return new WaitForSeconds(delay);

        consumedGhostScoreText.GetComponent<Text>().enabled = false;
        
    }

    IEnumerator StartBlinking(Text blinkText)
    {

        yield return new WaitForSeconds(0.25f);

        blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;
        StartCoroutine(StartBlinking(blinkText));

    }

    IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost)
    {

        yield return new WaitForSeconds(delay);

        consumedGhostScoreText.GetComponent<Text>().enabled = false;

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<Ghost>().canMove = true;

        }

        pacMan.transform.GetComponent<PacMan>().canMove = true;

        transform.GetComponent<AudioSource>().Play();

        didStartConsumed = false;

    }

    IEnumerator ShowObjectsAfter(float deplay)
    {

        yield return new WaitForSeconds(deplay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;

        }

        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        playerText.transform.GetComponent<Text>().enabled = false;

        StartCoroutine(StartGameAfter(1.8f)); //sdfsdfsdfsfsdf
    }

    IEnumerator StartGameAfter(float deplay)
    {

        yield return new WaitForSeconds(deplay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<Ghost>().canMove = true;

        }

        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<PacMan>().canMove = true;

        readyText.transform.GetComponent<Text>().enabled = false;

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
    }

    public void StartDeath()
    {

        if (!didStartDeath)
        {
            StopAllCoroutines();

            if (GameMenu.isOnePlayerGame)
            {
                playerOneUp.GetComponent<Text>().enabled = true;
            }
            else
            {
                playerOneUp.GetComponent<Text>().enabled = true;
                playerTwoUp.GetComponent<Text>().enabled = true;
            }

            GameObject bonusItem = GameObject.Find("bonusItem");

            if (bonusItem)
            {
                Destroy(bonusItem.gameObject);
            }

            didStartDeath = false;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {

                ghost.transform.GetComponent<Ghost>().canMove = false;

            }

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            pacMan.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeathAfter(2));

        }

    }

    IEnumerator ProcessDeathAfter(float delay)
    {

        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;

        }

        StartCoroutine(ProcessDeathAnimation(1.9f));

    }

    IEnumerator ProcessDeathAnimation(float delay)
    {

        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.localScale = new Vector3(1, 1, 1);
        pacMan.transform.localRotation = Quaternion.Euler (0, 0, 0);

        pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().deathAnimation;
        pacMan.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));
    }

    IEnumerator ProcessRestart(float delay)
    {

        if (isPlayerOneUp)
        {
            SpriteUpdate();
            GameMenu.livesPlayerOne -= 1;
        }
        else
        {
            SpriteUpdate2();
            GameMenu.livesPlayerTwo -= 1;
        }

        if (GameMenu.livesPlayerOne == 0 && GameMenu.livesPlayerTwo == 0)
        {

            playerText.transform.GetComponent<Text>().enabled = true;

            readyText.transform.GetComponent<Text>().text = "GAME OVER!";
            readyText.transform.GetComponent<Text>().color = new Color(159, 0, 0);

            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessGameOver(2));
        }
        else if (GameMenu.livesPlayerOne == 0 || GameMenu.livesPlayerTwo == 0)
        {

            if (GameMenu.livesPlayerOne == 0)
            {

                playerText.transform.GetComponent<Text>().text = "PLAYER 1";

            }
            else if (GameMenu.livesPlayerTwo == 0) 
            {

                playerText.transform.GetComponent<Text>().text = "PLAYER 2";
            }

            readyText.transform.GetComponent<Text>().text = "GAME OVER!";
            readyText.transform.GetComponent<Text>().color = Color.red;

            readyText.transform.GetComponent<Text>().enabled = true;
            playerText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            if (!GameMenu.isOnePlayerGame)
            {
                isPlayerOneUp = !isPlayerOneUp;
            }

            if (isPlayerOneUp)
            {
                StartCoroutine(StartBlinking(playerOneUp));
            }
            else
            {
                StartCoroutine(StartBlinking(playerTwoUp));
            }

            RedrawBoard();

            if (isPlayerOneUp)
            {
                playerText.transform.GetComponent<Text>().text = "PLAYER 1";
            }
            else
            {
                playerText.transform.GetComponent<Text>().text = "PLAYER 2";
            }

            readyText.transform.GetComponent<Text>().text = "READY!";
            readyText.transform.GetComponent<Text>().color = new Color(1,1, 0.3529412f);

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(2));
        }
        else
        {

            playerText.transform.GetComponent<Text>().enabled = true;
            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            if (!GameMenu.isOnePlayerGame)
            {
                isPlayerOneUp = !isPlayerOneUp;
            }

            if (isPlayerOneUp)
            {
                StartCoroutine(StartBlinking(playerOneUp));
            }
            else
            {
                StartCoroutine(StartBlinking(playerTwoUp));
            }

            if (!GameMenu.isOnePlayerGame)
            {
                if (isPlayerOneUp)
                {
                    playerText.transform.GetComponent<Text>().text = "PLAYER 1";
                }
                else
                {
                    playerText.transform.GetComponent<Text>().text = "PLAYER 2";
                }
            }

            RedrawBoard();

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(1));

        }

    }

    IEnumerator ProcessGameOver(float delay)
    {

        yield return new WaitForSeconds(delay);

        playerOneLevel = 1;
        playerOneScore = 0;
        playerTwoLevel = 1;
        playerTwoScore = 0;
        SceneManager.LoadScene("Menu");

    }

    IEnumerator ProcessRestartShowObjects(float delay)
    {

        playerText.transform.GetComponent<Text>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {

            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Animator>().enabled = true;
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }

        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<Animator>().enabled = false;
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();

        yield return new WaitForSeconds(delay);

        Restart();

    }

    public void Restart()
    {
        int playerLevel = 0;

        if (isPlayerOneUp)
        {
            playerLevel = playerOneLevel;
        }
        else
        {
            playerLevel = playerTwoLevel;
        }

        GameObject.Find("PacMan").GetComponent<PacMan>().SetDifficultyForLevel(playerLevel);

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in objects)
        {
            ghost.transform.GetComponent<Ghost>().SetDifficultyForLevel(playerLevel);
        }

        readyText.transform.GetComponent<Text>().enabled = false;

        AudioSource clip = GameObject.Find("Game").GetComponent<AudioSource>();
        clip.clip = backgroundAudioNormal;
        clip.Play();

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().Restart();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {

            ghost.transform.GetComponent<Ghost>().Restart();

        }

    }

    void ResetPelletForPlayer(int playerNum)
    {

        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach(GameObject o in objects)
        {

            if (o.GetComponent<Tile>() != null)
            {

                if(o.GetComponent<Tile>().isPellet|| o.GetComponent<Tile>().isPowerPellet)
                {

                    if(playerNum == 1)
                    {

                        o.GetComponent<Tile>().didConsumedPlayerOne = false;

                    }
                    else
                    {

                        o.GetComponent<Tile>().didConsumedPlayerTwo = false;

                    }

                }

            }

        }
    }

    void RedrawBoard()
    {

        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject o in objects)
        {
            if (o.GetComponent<Tile>() != null)
            {
                if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isPowerPellet)
                {
                    if (isPlayerOneUp)
                    {
                        if (o.GetComponent<Tile>().didConsumedPlayerOne)
                        {
                            o.GetComponent<SpriteRenderer>().enabled = false;
                        }
                        else
                        {
                            o.GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                    else
                    {
                        if (o.GetComponent<Tile>().didConsumedPlayerTwo)
                        {
                            o.GetComponent<SpriteRenderer>().enabled = false;
                        }
                        else
                        {
                            o.GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                }
            }
        }
    }

    public void Exit()
    {

        isPlayerOneUp = true;
        playerOneLevel = 1;
        playerTwoLevel = 1;
        playerOneScore = 0;
        playerTwoScore = 0;


        AudioListener.pause = false;

        Time.timeScale = 1;

        SceneManager.LoadScene("Menu");

    }

    public void ContinueButton()
    {

        AudioListener.pause = false;
        Time.timeScale = 1;

        foreach (GameObject o in gameObjects)
        {
            o.SetActive(false);

        }

    }

}
