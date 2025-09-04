using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anicontroller : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            anim.SetTrigger("Air");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim.SetTrigger("Bell");
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            anim.SetTrigger("HighBell");
        }
    }
}
