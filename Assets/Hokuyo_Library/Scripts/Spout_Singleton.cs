using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SpoutSender가 들어간 오브젝트를 싱글톤으로 설정
public class Spout_Singleton : MonoBehaviour
{
    static Spout_Singleton instance;

    private void Awake()
    {
        if (instance == null) //존재하고 있지 않을때
        {
            instance = this; //다시 최신화
            DontDestroyOnLoad(gameObject); //씬변경시 파괴 x
        }
        else
        {
            if (instance != this) //중복으로 존재할시에는 파괴! 
                Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject); //씬변경시 파괴 x
    }
}
