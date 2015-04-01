using UnityEngine;
using System.Collections;

public class carcollision : MonoBehaviour
{

    public CarcollisionManager CollisionManager;//충돌 매니저 연결.
    public float disrepair;//파손율.
    public GameObject Part;
    public bool onoff; //더이상 충돌하지 않으면 off.
    // Use this for initialization
    void Start()
    {
        if (CollisionManager == null)
        {
            print(this.gameObject.name + "CarcollisionManager == NUll");
        }
        disrepair = 100.0f;
    }
    void OnTriggerStay(Collider other)
    {
        CollisionManager.Trigger_hit(other, this.gameObject.name);
    }
}
