using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class donotdestory : MonoBehaviour
{
    public static donotdestory s_Instance = null;

    // Start is called before the first frame update
    void Start()
    {
        //if (s_Instance)
        //{
        //    DestroyImmediate(this.gameObject);
        //    return;
        //}
        s_Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
}
