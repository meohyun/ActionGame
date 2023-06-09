using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isFight;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    public int enemyCntE;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject clearPanel;
    public GameObject gameOverPanel;
    public GameObject portal;

    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text finalScoreTxt;

    public Text playerLifeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyA;
    public Text enemyB;
    public Text enemyC;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public RectTransform playerHealthBar;


    // Start is called before the first frame update
    void Start()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

    }

    public void GameStart()
    {
        menuCam.SetActive(false);

        if (!player.isRestart)
            gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFight)
            playTime += Time.deltaTime;


        if (enemyCntA + enemyCntB + enemyCntC + enemyCntD + enemyCntE== 0 && portal != null)
        {
            portal.SetActive(true);
        }
    }

    //update가 끝난후 실행
    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        //게임 시간
        int hour = (int) (playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}", hour) +":" + string.Format("{0:00}",min) + ":"+ string.Format("{0:00}",sec);

        playerLifeTxt.text = "X" + player.Life.ToString();
        playerHealthTxt.text = player.Health + " / " + player.MaxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.Coin);

        playerHealthBar.localScale = new Vector3((float) player.Health/player.MaxHealth,1,1);

        if (player.equipWeapon == null)
            playerAmmoTxt.text = "_ / " + player.Ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "_ / " + player.Ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.Ammo;

        // 무기 보유에 따른 투명도 설정
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapon[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapon[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapon[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.HasGrenades > 0 ? 1 : 0);

        enemyA.text = "X " + enemyCntA.ToString();
        enemyB.text = "X " + enemyCntB.ToString();
        enemyC.text = "X " + enemyCntC.ToString();


        // 보스 UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHp / boss.maxHp, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }

        // GameClear

        if (stage == 4 && boss == null && enemyCntA + enemyCntB + enemyCntC + enemyCntD + enemyCntE == 0)
        {
            gamePanel.SetActive(false);
            clearPanel.SetActive(true);
            finalScoreTxt.text = "Point: " + scoreTxt.text;
        }

    }

    // 게임 오버 or 클리어시 재 시작   
    public void reStart(bool isClear)
    {
        int maxScore = PlayerPrefs.GetInt("MaxScore");

        if (player.score > maxScore)
        {
            PlayerPrefs.SetInt("MaxScore", player.score);
        }

        player.isRestart = true;
        player.isDie = false;
        //player.isDamage = false;

        player.transform.gameObject.SetActive(true);
        player.grenadeGroup.SetActive(true);
        player.transform.position = new Vector3(0, 0, 0);
        player.rb.velocity = Vector3.zero;

        if (isClear)
            clearPanel.SetActive(false);
        else
            gameOverPanel.SetActive(false);

        gamePanel.SetActive(true);

        initPlayer();

        SceneManager.LoadScene("Stage_0");

    }

    public void initPlayer()
    {
        for (int i =0; i < 3; i++)
            player.hasWeapon[i] = false;

        // 무기가 없다
        if (player.equipWeapon != null)
            player.equipWeapon.gameObject.SetActive(false);
        player.equipWeapon = null;
        player.isFireReady = true;

        for (int i = 0; i <4; i++)
            player.grenades[i].SetActive(false);

        player.HasGrenades = 0;

        player.Health = 100;
        player.Coin = 10000;
        player.Ammo = 300;
        player.Life = 5;
        player.score = 0;

    }

    public IEnumerator GameOver()
    {
        player.gameObject.layer = 11;
        player.anim.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);
        player.gameObject.SetActive(false);
        player.grenadeGroup.SetActive(false);

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        finalScoreTxt.text = "Point: " + scoreTxt.text;
    }
}
