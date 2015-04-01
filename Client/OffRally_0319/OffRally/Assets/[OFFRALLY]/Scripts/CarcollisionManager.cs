using UnityEngine;
using System.Collections;

public class CarcollisionManager : MonoBehaviour
{
    public CarControllerV2 Play;
    public GameObject collision_onoff;

    public carcollision[] collision;
   


    // Update is called once per frame
    void Start()
    {
        //나중에 플레이어 자동으로 넣기 


    }
    void Update()
    {
        //충돌시 효과 및 파손율 측정 
    }

    public void Trigger_hit(Collider other, string name)
    {
        if (other.name.Length == 10)
        {
          string test =  other.name.Remove(7, 3);
          if (test == "Checker")
              return;
        }
        switch (name)
        {
            case "bumper":
                {
                    print(name);
                    print(other.name);
                    //  print(other.name.IndexOf("curve"));

                    if (Play.Speed > 10)
                    {
                        collision[0].disrepair -= 0.1f;
                    }
                    if (Play.Speed > 20)
                    {
                        collision[0].disrepair -= 0.5f;
                    }

                    Material mt = Resources.LoadAssetAtPath("Assets/Resources/Materials/test.mat", typeof(Material)) as Material;
                    // resource 폴더에서 불러온 재질을 복사
                    collision[0].Part.renderer.material = mt;
                }
                break;

            case "r_bumper":
                print(name);
                print(other.name);

                if (Play.Speed > 10)
                {
                    collision[1].disrepair -= 0.1f;
                }
                if (Play.Speed > 20)
                {
                    collision[1].disrepair -= 0.5f;
                }

                Material mt2 = Resources.LoadAssetAtPath("Assets/Resources/Materials/test.mat", typeof(Material)) as Material;
                // resource 폴더에서 불러온 재질을 복사
                collision[1].Part.renderer.material = mt2;
                break;
        }

    }
}
