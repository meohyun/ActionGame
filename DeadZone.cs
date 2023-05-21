using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour

{
    public Player player;
    Animator anim;

    void Start()
    {
        anim = player.GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            anim.SetTrigger("isDie");

            Invoke("revival",1f);

        }
    }

    void revival()
    {
        SceneManager.LoadScene("Main");
    }
}
