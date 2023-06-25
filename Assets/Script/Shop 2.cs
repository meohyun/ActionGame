using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObjs;
    public int[] itemPrices;
    public Transform[] itemPos;
    public Text talktext;
    public string[] talkData;


    Player enterPlayer;

    // Start is called before the first frame update
    public void Enter(Player player)
    {
        enterPlayer = player;
        // 화면 정중앙에 오도uiGroup.anchoredPosition = Vector3.zero;
        uiGroup.anchoredPosition = Vector3.zero;

    }

    // Update is called once per frame
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void BuyWeapon(int index)
    {

        int price = itemPrices[index];
        if (price > enterPlayer.Coin)
        {
            StopCoroutine(notEnoughTalk());
            StartCoroutine(notEnoughTalk());
            return;

        }

        enterPlayer.Coin -= price;
        StopCoroutine(EnoughTalk());
        StartCoroutine(EnoughTalk());
        enterPlayer.hasWeapon[index] = true;

    }

    public void BuyItems(int index)
    {
        int price = itemPrices[index];
        if (price > enterPlayer.Coin)
        {
            StopCoroutine(notEnoughTalk());
            StartCoroutine(notEnoughTalk());
            return;

        }

        enterPlayer.Coin -= price;
        StopCoroutine(EnoughTalk());
        StartCoroutine(EnoughTalk());

        Vector3 ranVec = Vector3.forward * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3);
        Instantiate(itemObjs[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }

    IEnumerator notEnoughTalk()
    {
        talktext.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talktext.text = talkData[0];
    }

    IEnumerator EnoughTalk()
    {
        talktext.text = talkData[2];
        yield return new WaitForSeconds(2f);
        talktext.text = talkData[0];
    }

}
