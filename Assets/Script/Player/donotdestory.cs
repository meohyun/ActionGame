using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class donotdestory : MonoBehaviour
{
    public static donotdestory s_Instance = null;

    // Start is called before the first frame update
    void Start()
    {

        s_Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
}
