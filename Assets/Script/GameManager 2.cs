using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isFight;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
    public GameObject gamePanel;

    public Text maxScoreTxt;

    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;

    public Text playerHeartTxt;
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

    // Start is called before the first frame update
    void Start()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFight)
            playTime += Time.deltaTime;
    }

    //update가 끝난후 실행
    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        int hour = (int) (playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);


        playTimeTxt.text = string.Format("{0:00}", hour) +":" + string.Format("{0:00}",min) + ":"+ string.Format("{0:00}",sec);

        playerHeartTxt.text = player.Heart + " / " + player.MaxHeart;
        playerCoinTxt.text = string.Format("{0:n0}", player.Coin);

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


        bossHealthBar.localScale = new Vector3((float)boss.curHp / boss.maxHp, 1, 1);

    }


    
}