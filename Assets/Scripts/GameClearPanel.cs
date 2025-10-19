using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{

    [Header("UI")]
    public Image imageGameClearStatus;
    public Sprite spriteSuccess; // 게임 성공 이미지 
    public Sprite spriteFail; // 게임 실패 이미지\

    private DOTweenAnimation DOTWeen;

    public void Start()
    {
        gameObject.SetActive(false);
        DOTWeen = GetComponentInChildren<DOTweenAnimation>();
    }

    // Start is called before the first frame update
    public void ShowGameClearPanel(bool result)
    {
        // true면 성공, fail이면 실패 패널
        if (result)
        {
            imageGameClearStatus.sprite = spriteSuccess;
        } else
        {
            imageGameClearStatus.sprite = spriteFail;
        }

        gameObject.SetActive(true);
        DOTWeen.DORestart();
    }
}
