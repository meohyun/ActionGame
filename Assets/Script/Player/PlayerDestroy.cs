using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDestroy : MonoBehaviour
{
    public static PlayerDestroy s_instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (s_instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
