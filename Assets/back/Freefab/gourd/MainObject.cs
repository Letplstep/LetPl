using UnityEngine;

public class MainObject : MonoBehaviour
{
    public int objectNumber;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        // 생성될 때 자동으로 Box 태그 지정
        gameObject.tag = "Box";

        // SpriteRenderer 가져오기 / 없으면 자동 추가
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    public void ReactAndDestroy()
    {
        Debug.Log($"Main Object {objectNumber} Reacted and Destroyed!");
        Destroy(gameObject);
    }
}
