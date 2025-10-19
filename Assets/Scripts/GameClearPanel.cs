using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{

    [Header("UI")]
    public Image panelGameClear;
    public Sprite spriteSuccess; // 게임 성공 이미지 
    public Sprite spriteFail; // 게임 실패 이미지

    // Start is called before the first frame update
    public void ShowGameClearPanel(bool result)
    {
        // true면 성공, fail이면 실패 패널
        // 성공 실패 기준 알아서
        panelGameClear.gameObject.SetActive(result);
    }
}
