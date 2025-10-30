//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public enum TileOwner { None, Player1, Player2 }

//[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
//public class Tile : MonoBehaviour
//{
//    Å¸ÀÏ ¼ÒÀ¯ Á¤º¸
//    public TileOwner owner = TileOwner.None;               // ÇöÀç Å¸ÀÏ ¼ÒÀ¯ÇÑ ÇÃ·¹ÀÌ¾î
//    public TileOwner territoryOwner = TileOwner.None;      // Å¸ÀÏ ¿ø·¡ ¼Ò¼Ó

//    Å¸ÀÏ »óÅÂ
//    public bool isAttackTile = false;                      // °ø°Ý Å¸ÀÏ ¿©ºÎ
//    public bool isDefenseTile = false;                     // ¹æ¾î Å¸ÀÏ ¿©ºÎ

//    ¼º°ø Ç¥½Ã¿ë ½ºÇÁ¶óÀÌÆ®
//    public Sprite attackSuccessSprite;
//    public Sprite defenseSuccessSprite;

//    ¹æ¾î Å¸ÀÏ UI
//    public GameObject defenseTimerUIPrefab;                // ¹æ¾î Å¸ÀÏ Å¸ÀÌ¸Ó UI ÇÁ¸®ÆÕ
//    private GameObject activeDefenseUI;                    // Ç¥½Ã ÁßÀÎ ¹æ¾î UI ÀÎ½ºÅÏ½º
//    private TextMeshPro defenseTimerText;                  // UI ³»ºÎ Å¸ÀÌ¸Ó ÅØ½ºÆ®

//    ÄÄÆ÷³ÍÆ® ¹× Å¸ÀÌ¸Ó
//    private SpriteRenderer rend;                           // Å¸ÀÏ »ö»ó Ç¥½Ã¿ë SpriteRenderer
//    private Coroutine attackTimeoutCoroutine;
//    private Coroutine defenseCoroutine;

//    public Sprite defaultSprite;
//    public Sprite player1Sprite;
//    public Sprite player2Sprite;
//    public Sprite attackSprite;
//    public Sprite defenseSprite;

//    private void Awake()
//    {
//        rend = GetComponent<SpriteRenderer>();
//        UpdateSprite();
//    }

//    ¼ÒÀ¯ÀÚ¸¦ º¯°æÇÏ°í »ö»ó ¾÷µ¥ÀÌÆ®
//    public void SetOwner(TileOwner newOwner)
//    {
//        owner = newOwner;
//        UpdateSprite();
//    }

//    ÀÌ Å¸ÀÏÀ» °ø°Ý Å¸ÀÏ·Î ¼³Á¤ÇÏ°í ÀÚµ¿ ÇØÁ¦ Å¸ÀÌ¸Ó ½ÃÀÛ
//    public void SetAsAttackTile(TileOwner attacker)
//    {
//        isAttackTile = true;
//        UpdateSprite();
//        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(attacker));
//    }

//    °ø°Ý Å¸ÀÏ ÇØÁ¦ ¹× Å¸ÀÌ¸Ó Á¾·á
//    public void ClearAttackTile()
//    {
//        isAttackTile = false;
//        if (attackTimeoutCoroutine != null)
//        {
//            StopCoroutine(attackTimeoutCoroutine);
//            attackTimeoutCoroutine = null;
//        }
//        UpdateSprite();
//    }

//    ¹æ¾î Å¸ÀÏ·Î ¼³Á¤
//    public void SetAsDefenseTile(TileOwner defender)
//    {
//        isDefenseTile = true;
//        UpdateSprite();
//    }

//    ¹æ¾î Å¸ÀÏ ÇØÁ¦ ¹× UI Á¦°Å
//    public void ClearDefenseTile()
//    {
//        isDefenseTile = false;
//        HideDefenseTimerUI();
//        UpdateSprite();
//    }

//    ÀÏÁ¤ ½Ã°£ ÈÄ °ø°Ý Å¸ÀÏ ÀÚµ¿ Á¦°Å
//    private IEnumerator AttackTimeout(TileOwner attacker)
//    {
//        yield return new WaitForSeconds(5f);
//        if (isAttackTile)
//        {
//            ClearAttackTile();
//            if (!GameManager.Instance.IsFeverTime)
//            {
//                GameManager.Instance.SetNewAttackTileForPlayer(attacker, this);
//            }
//        }
//    }
//    public void ShowDefenseTimerUI()
//    {
//        if (defenseTimerUIPrefab != null && activeDefenseUI == null)
//        {
//            UI »ý¼º(Å¸ÀÏ ÀÚ½ÄÀ¸·Î ºÙÀÌ±â)
//            activeDefenseUI = Instantiate(defenseTimerUIPrefab, transform);

//            ±âº» À§Ä¡ ¼³Á¤
//            activeDefenseUI.transform.localPosition = new Vector3(2.25f, -0.1f, 0);
//            activeDefenseUI.transform.localRotation = Quaternion.identity;

//            TextMeshPro(3D ÅØ½ºÆ®) °¡Á®¿À±â
//          defenseTimerText = activeDefenseUI.GetComponentInChildren<TextMeshPro>();

//            if (defenseTimerText != null)
//            {
//                ÇÇ¹þ Áß¾Ó º¸Á¤
//                defenseTimerText.alignment = TextAlignmentOptions.Center;

//                Sorting Layer ¹× Order ¼³Á¤(SpriteRenderer À§¿¡ Ç¥½Ã)
//                MeshRenderer mr = defenseTimerText.GetComponent<MeshRenderer>();
//                if (mr != null)
//                {
//                    mr.sortingLayerName = "UI"; // ÇÊ¿ä ½Ã »õ·Î »ý¼º
//                    mr.sortingOrder = 100;       // Å« ¼ýÀÚ ¡æ Ç×»ó À§
//                }

//                È¸Àü Àû¿ë
//                float zRotation = (owner == TileOwner.Player1) ? 270f : -270f;
//                defenseTimerText.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

//                À§Ä¡ ÃÊ±âÈ­(È¸Àü ÈÄ ¹Ð¸² º¸Á¤)
//                defenseTimerText.transform.localPosition = Vector3.zero;
//            }
//        }
//    }
//    ¹æ¾î Å¸ÀÌ¸Ó ½ÃÀÛ
//    public void StartDefenseTimer(TileOwner attacker)
//    {
//        isDefenseTile = true;
//        UI 1È¸ »ý¼º
//        if (activeDefenseUI == null)
//            ShowDefenseTimerUI();

//        ÀÌÀü ÄÚ·çÆ¾ Á¾·á
//        if (defenseCoroutine != null)
//            StopCoroutine(defenseCoroutine);

//        defenseCoroutine = StartCoroutine(DefenseTileTimerCoroutine(attacker));
//    }

//    ¹æ¾î Å¸ÀÌ¸Ó ÄÚ·çÆ¾
//    private IEnumerator DefenseTileTimerCoroutine(TileOwner attacker)
//    {
//        float timer = GameManager.Instance.defenseTileLifetime;

//        while (timer > 0f)
//        {
//            if (!isDefenseTile)
//            {
//                HideDefenseTimerUI();
//                yield break;
//            }

//            if (defenseTimerText != null)
//                defenseTimerText.text = Mathf.CeilToInt(timer).ToString();

//            timer -= Time.deltaTime;
//            yield return null; // ¸Å ÇÁ·¹ÀÓ °»½Å
//        }

//        Á¾·á ½Ã Ã³¸®
//       isDefenseTile = false;

//        ¹æ¾î ½ÇÆÐ »ç¿îµå Àç»ý Ãß°¡
//        if (GameManager.Instance != null)
//            GameManager.Instance.PlayDefenseFailSound();

//        SetOwner(attacker);
//        GameManager.Instance.AddScore(attacker);
//        HideDefenseTimerUI();
//    }

//    ¹æ¾î Å¸ÀÏ UI °»½Å
//    public void UpdateDefenseTimerUI(int seconds)
//    {
//        if (defenseTimerText != null)
//            defenseTimerText.text = seconds.ToString();
//    }

//    ¹æ¾î Å¸ÀÏ UI Á¦°Å
//    public void HideDefenseTimerUI()
//    {
//        if (activeDefenseUI != null)
//        {
//            Destroy(activeDefenseUI);
//            activeDefenseUI = null;
//            defenseTimerText = null;
//        }
//    }

//    public void UpdateSprite() // Å¸ÀÏ ½ºÇÁ¶óÀÌÆ® º¯°æ
//    {
//        if (isAttackTile)
//        {
//            rend.sprite = attackSprite;
//        }
//        else if (isDefenseTile)
//        {
//            rend.sprite = defenseSprite;
//        }
//        else
//        {
//            switch (owner)
//            {
//                case TileOwner.Player1:
//                    rend.sprite = player1Sprite;
//                    break;
//                case TileOwner.Player2:
//                    rend.sprite = player2Sprite;
//                    break;
//                default:
//                    rend.sprite = defaultSprite;
//                    break;
//            }
//        }
//    }


//    private void OnTriggerEnter(Collider other)
//    {

//        XR È¯°æ¿¡¼­´Â À§Ä¡·Î ¼ÒÀ¯ÀÚ ÃßÁ¤(ÇÃ·¹ÀÌ¾î À§Ä¡ ±âÁØ)
//        TileOwner assumedOwner = other.transform.position.x < 0 ? TileOwner.Player1 : TileOwner.Player2;
//        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;


//        Debug.Log($"Å¸ÀÏ {gameObject.name}¿¡ {other.name} ´êÀ½ (ÃßÁ¤ ¼ÒÀ¯ÀÚ: {assumedOwner})");

//        ¹æ¾î Å¸ÀÏ Ã³¸®
//        if (isDefenseTile && owner == assumedOwner)
//        {
//            ClearDefenseTile();
//            SetOwner(owner); // ¹æ¾î ¼º°ø ½Ã »ö À¯Áö
//            return;
//        }

//        Á¡·É Ã³¸®
//        if (owner != assumedOwner && !isDefenseTile)
//        {
//            if (owner == territoryOwner)
//                SetOwner(assumedOwner);
//        }

//        °ø°Ý Å¸ÀÏ Ã³¸®
//        if (isAttackTile)
//        {

//            °ø°Ý ¹ßÆÇ ¹â¾ÒÀ» ¶§ »ç¿îµå Àç»ý Ãß°¡
//            if (GameManager.Instance != null)
//                GameManager.Instance.PlayAttackTileSound();

//            bool isFever = GameManager.Instance.IsFeverTime;

//            ClearAttackTile();

//            if (isFever)
//            {
//                GameManager.Instance.AddScore(assumedOwner);
//                GameManager.Instance.RespawnFeverAttackTile(assumedOwner, this);
//            }
//            else
//            {
//                Tile[] allTiles = FindObjectsOfType<Tile>();
//                List<Tile> enemyTiles = new List<Tile>();

//                foreach (Tile t in allTiles)
//                {
//                    if (t.owner == enemy && t.territoryOwner == enemy && !t.isAttackTile && !t.isDefenseTile)
//                        enemyTiles.Add(t);
//                }

//                if (enemyTiles.Count > 0)
//                {
//                    Tile targetTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
//                    targetTile.SetAsDefenseTile(enemy);
//                    targetTile.StartDefenseTimer(assumedOwner);  // ¼öÁ¤µÊ
//                }

//            }
//            GameManager.Instance.SetNewAttackTileForPlayer(assumedOwner, this);
//        }
//    }
//}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public enum TileOwner { None, Player1, Player2 }

//[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
//public class Tile : MonoBehaviour
//{
//    // Å¸ÀÏ ¼ÒÀ¯ Á¤º¸
//    public TileOwner owner = TileOwner.None;
//    public TileOwner territoryOwner = TileOwner.None;

//    // Å¸ÀÏ »óÅÂ
//    public bool isAttackTile = false;
//    public bool isDefenseTile = false;

//    // ¼º°ø Ç¥½Ã¿ë ½ºÇÁ¶óÀÌÆ®
//    public Sprite attackSuccessSprite;
//    public Sprite defenseSuccessSprite;

//    // ¹æ¾î Å¸ÀÏ UI
//    public GameObject defenseTimerUIPrefab;
//    private GameObject activeDefenseUI;
//    private TextMeshPro defenseTimerText;

//    // ÄÄÆ÷³ÍÆ® ¹× Å¸ÀÌ¸Ó
//    private SpriteRenderer rend;
//    private Coroutine attackTimeoutCoroutine;
//    private Coroutine defenseCoroutine;

//    public Sprite defaultSprite;
//    public Sprite player1Sprite;
//    public Sprite player2Sprite;
//    public Sprite attackSprite;
//    public Sprite defenseSprite;

//    private void Awake()
//    {
//        rend = GetComponent<SpriteRenderer>();
//        UpdateSprite();
//    }

//    // ¼ÒÀ¯ÀÚ¸¦ º¯°æÇÏ°í »ö»ó ¾÷µ¥ÀÌÆ®
//    public void SetOwner(TileOwner newOwner)
//    {
//        owner = newOwner;
//        UpdateSprite();
//    }

//    // ÀÌ Å¸ÀÏÀ» °ø°Ý Å¸ÀÏ·Î ¼³Á¤ÇÏ°í ÀÚµ¿ ÇØÁ¦ Å¸ÀÌ¸Ó ½ÃÀÛ
//    public void SetAsAttackTile(TileOwner attacker)
//    {
//        isAttackTile = true;
//        UpdateSprite();
//        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(attacker));
//    }

//    // °ø°Ý Å¸ÀÏ ÇØÁ¦ ¹× Å¸ÀÌ¸Ó Á¾·á
//    public void ClearAttackTile()
//    {
//        isAttackTile = false;
//        if (attackTimeoutCoroutine != null)
//        {
//            StopCoroutine(attackTimeoutCoroutine);
//            attackTimeoutCoroutine = null;
//        }
//        UpdateSprite();
//    }

//    // ¹æ¾î Å¸ÀÏ·Î ¼³Á¤
//    public void SetAsDefenseTile(TileOwner defender)
//    {
//        isDefenseTile = true;
//        UpdateSprite();
//    }

//    // ¹æ¾î Å¸ÀÏ ÇØÁ¦ ¹× UI Á¦°Å
//    public void ClearDefenseTile()
//    {
//        isDefenseTile = false;
//        HideDefenseTimerUI();
//        UpdateSprite();
//    }

//    // ÀÏÁ¤ ½Ã°£ ÈÄ °ø°Ý Å¸ÀÏ ÀÚµ¿ Á¦°Å
//    private IEnumerator AttackTimeout(TileOwner attacker)
//    {
//        yield return new WaitForSeconds(5f);
//        if (isAttackTile)
//        {
//            ClearAttackTile();
//            if (!GameManager.Instance.IsFeverTime)
//            {
//                GameManager.Instance.SetNewAttackTileForPlayer(attacker, this);
//            }
//        }
//    }

//    // ¹æ¾î Å¸ÀÌ¸Ó ½ÃÀÛ
//    public void StartDefenseTimer(TileOwner attacker)
//    {
//        isDefenseTile = true;
//        if (activeDefenseUI == null)
//            ShowDefenseTimerUI();

//        if (defenseCoroutine != null)
//            StopCoroutine(defenseCoroutine);

//        defenseCoroutine = StartCoroutine(DefenseTileTimerCoroutine(attacker));
//    }

//    private IEnumerator DefenseTileTimerCoroutine(TileOwner attacker)
//    {
//        float timer = GameManager.Instance.defenseTileLifetime;

//        while (timer > 0f)
//        {
//            if (!isDefenseTile)
//            {
//                HideDefenseTimerUI();
//                yield break;
//            }

//            if (defenseTimerText != null)
//                defenseTimerText.text = Mathf.CeilToInt(timer).ToString();

//            timer -= Time.deltaTime;
//            yield return null;
//        }

//        isDefenseTile = false;

//        if (GameManager.Instance != null)
//            GameManager.Instance.PlayDefenseFailSound();

//        SetOwner(attacker);
//        GameManager.Instance.AddScore(attacker);
//        HideDefenseTimerUI();
//    }

//    public void ShowDefenseTimerUI()
//    {
//        if (defenseTimerUIPrefab != null && activeDefenseUI == null)
//        {
//            activeDefenseUI = Instantiate(defenseTimerUIPrefab, transform);
//            activeDefenseUI.transform.localPosition = new Vector3(2.25f, -0.1f, 0);
//            activeDefenseUI.transform.localRotation = Quaternion.identity;

//            defenseTimerText = activeDefenseUI.GetComponentInChildren<TextMeshPro>();
//            if (defenseTimerText != null)
//            {
//                defenseTimerText.alignment = TextAlignmentOptions.Center;
//                MeshRenderer mr = defenseTimerText.GetComponent<MeshRenderer>();
//                if (mr != null)
//                {
//                    mr.sortingLayerName = "UI";
//                    mr.sortingOrder = 100;
//                }

//                float zRotation = (owner == TileOwner.Player1) ? 270f : -270f;
//                defenseTimerText.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
//                defenseTimerText.transform.localPosition = Vector3.zero;
//            }
//        }
//    }

//    public void HideDefenseTimerUI()
//    {
//        if (activeDefenseUI != null)
//        {
//            Destroy(activeDefenseUI);
//            activeDefenseUI = null;
//            defenseTimerText = null;
//        }
//    }

//    public void UpdateSprite()
//    {
//        if (isAttackTile)
//            rend.sprite = attackSprite;
//        else if (isDefenseTile)
//            rend.sprite = defenseSprite;
//        else
//        {
//            switch (owner)
//            {
//                case TileOwner.Player1:
//                    rend.sprite = player1Sprite;
//                    break;
//                case TileOwner.Player2:
//                    rend.sprite = player2Sprite;
//                    break;
//                default:
//                    rend.sprite = defaultSprite;
//                    break;
//            }
//        }
//    }

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ °ø°Ý/¹æ¾î ÀÓ½Ã ½ºÇÁ¶óÀÌÆ® Ã³¸® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    private IEnumerator ShowTemporarySprite(Sprite tempSprite, float duration = 1f)
//    {
//        if (tempSprite == null)
//            yield break;

//        Sprite original = rend.sprite;
//        rend.sprite = tempSprite;

//        yield return new WaitForSeconds(duration);

//        UpdateSprite(); // ¿ø·¡ Áø¿µ»öÀ¸·Î º¹±Í
//    }

//    private IEnumerator HandleAttackTile(TileOwner assumedOwner)
//    {
//        // Àá±ñ °ø°Ý ¼º°ø ½ºÇÁ¶óÀÌÆ® Ç¥½Ã
//        yield return StartCoroutine(ShowTemporarySprite(attackSuccessSprite, 1f));

//        // °ø°Ý ¹ßÆÇ ÇØÁ¦
//        ClearAttackTile();

//        // »ç¿îµå
//        if (GameManager.Instance != null)
//            GameManager.Instance.PlayAttackTileSound();

//        bool isFever = GameManager.Instance.IsFeverTime;
//        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;

//        if (isFever)
//        {
//            GameManager.Instance.AddScore(assumedOwner);
//            GameManager.Instance.RespawnFeverAttackTile(assumedOwner, this);
//        }
//        else
//        {
//            Tile[] allTiles = FindObjectsOfType<Tile>();
//            List<Tile> enemyTiles = new List<Tile>();
//            foreach (Tile t in allTiles)
//            {
//                if (t.owner == enemy && t.territoryOwner == enemy && !t.isAttackTile && !t.isDefenseTile)
//                    enemyTiles.Add(t);
//            }

//            if (enemyTiles.Count > 0)
//            {
//                Tile targetTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
//                targetTile.SetAsDefenseTile(enemy);
//                targetTile.StartDefenseTimer(assumedOwner);
//            }
//        }

//        GameManager.Instance.SetNewAttackTileForPlayer(assumedOwner, this);

//        // ¿ø·¡ Å¸ÀÏ ½ºÇÁ¶óÀÌÆ® º¹±Í
//        UpdateSprite();
//    }

//    private IEnumerator HandleDefenseTile(TileOwner assumedOwner)
//    {
//        // Àá±ñ ¹æ¾î ¼º°ø ½ºÇÁ¶óÀÌÆ® Ç¥½Ã
//        yield return StartCoroutine(ShowTemporarySprite(defenseSuccessSprite, 1f));

//        ClearDefenseTile();
//        SetOwner(owner); // ÀÚ±â Áø¿µ»ö À¯Áö

//        UpdateSprite();
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        TileOwner assumedOwner = other.transform.position.x < 0 ? TileOwner.Player1 : TileOwner.Player2;
//        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;

//        // ¹æ¾î Å¸ÀÏ ¹âÀ½
//        if (isDefenseTile && owner == assumedOwner)
//        {
//            StartCoroutine(HandleDefenseTile(assumedOwner));
//            return;
//        }

//        // Á¡·É Ã³¸®
//        if (owner != assumedOwner && !isDefenseTile)
//        {
//            if (owner == territoryOwner)
//                SetOwner(assumedOwner);
//        }

//        // °ø°Ý Å¸ÀÏ ¹âÀ½
//        if (isAttackTile)
//        {
//            StartCoroutine(HandleAttackTile(assumedOwner));
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public enum TileOwner { None, Player1, Player2 }

//[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
//public class Tile : MonoBehaviour
//{
//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ Å¸ÀÏ ¼ÒÀ¯ ¹× »óÅÂ ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    public TileOwner owner = TileOwner.None;
//    public TileOwner territoryOwner = TileOwner.None;

//    public bool isAttackTile = false;
//    public bool isDefenseTile = false;

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ½ºÇÁ¶óÀÌÆ® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    public Sprite defaultSprite;
//    public Sprite player1Sprite;
//    public Sprite player2Sprite;
//    public Sprite attackSprite;
//    public Sprite defenseSprite;

//    public Sprite attackSuccessSprite;
//    public Sprite defenseSuccessSprite;

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ¹æ¾î UI ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    public GameObject defenseTimerUIPrefab;
//    private GameObject activeDefenseUI;
//    private TextMeshPro defenseTimerText;

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ÄÄÆ÷³ÍÆ® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    private SpriteRenderer rend;
//    private Coroutine attackTimeoutCoroutine;
//    private Coroutine defenseCoroutine;

//    private void Awake()
//    {
//        rend = GetComponent<SpriteRenderer>();
//        UpdateSprite();
//    }

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ¼ÒÀ¯ÀÚ / ½ºÇÁ¶óÀÌÆ® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    public void SetOwner(TileOwner newOwner)
//    {
//        owner = newOwner;
//        UpdateSprite();
//    }

//    public void UpdateSprite()
//    {
//        if (isAttackTile)
//            rend.sprite = attackSprite;
//        else if (isDefenseTile)
//            rend.sprite = defenseSprite;
//        else
//        {
//            switch (owner)
//            {
//                case TileOwner.Player1: rend.sprite = player1Sprite; break;
//                case TileOwner.Player2: rend.sprite = player2Sprite; break;
//                default: rend.sprite = defaultSprite; break;
//            }
//        }
//    }

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ °ø°Ý / ¹æ¾î Å¸ÀÏ ¼³Á¤ ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    public void SetAsAttackTile(TileOwner attacker)
//    {
//        isAttackTile = true;
//        UpdateSprite();
//        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(attacker));
//    }

//    public void ClearAttackTile()
//    {
//        isAttackTile = false;
//        if (attackTimeoutCoroutine != null)
//        {
//            StopCoroutine(attackTimeoutCoroutine);
//            attackTimeoutCoroutine = null;
//        }
//        UpdateSprite();
//    }

//    public void SetAsDefenseTile(TileOwner defender)
//    {
//        isDefenseTile = true;
//        UpdateSprite();
//    }

//    public void ClearDefenseTile()
//    {
//        isDefenseTile = false;
//        HideDefenseTimerUI();
//        UpdateSprite();
//    }

//    private IEnumerator AttackTimeout(TileOwner attacker)
//    {
//        yield return new WaitForSeconds(5f);
//        if (isAttackTile)
//        {
//            ClearAttackTile();
//            if (!GameManager.Instance.IsFeverTime)
//            {
//                GameManager.Instance.SetNewAttackTileForPlayer(attacker, this);
//            }
//        }
//    }

//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ¹æ¾î Å¸ÀÌ¸Ó ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    public void StartDefenseTimer(TileOwner attacker)
//    {
//        isDefenseTile = true;
//        if (activeDefenseUI == null)
//            ShowDefenseTimerUI();

//        if (defenseCoroutine != null)
//            StopCoroutine(defenseCoroutine);

//        defenseCoroutine = StartCoroutine(DefenseTileTimerCoroutine(attacker));
//    }

//    private IEnumerator DefenseTileTimerCoroutine(TileOwner attacker)
//    {
//        float timer = GameManager.Instance.defenseTileLifetime;

//        while (timer > 0f)
//        {
//            if (!isDefenseTile)
//            {
//                HideDefenseTimerUI();
//                yield break;
//            }

//            if (defenseTimerText != null)
//                defenseTimerText.text = Mathf.CeilToInt(timer).ToString();

//            timer -= Time.deltaTime;
//            yield return null;
//        }

//        isDefenseTile = false;

//        if (GameManager.Instance != null)
//            GameManager.Instance.PlayDefenseFailSound();

//        SetOwner(attacker);
//        GameManager.Instance.AddScore(attacker);
//        HideDefenseTimerUI();
//    }

//    public void ShowDefenseTimerUI()
//    {
//        if (defenseTimerUIPrefab != null && activeDefenseUI == null)
//        {
//            activeDefenseUI = Instantiate(defenseTimerUIPrefab, transform);
//            activeDefenseUI.transform.localPosition = new Vector3(2.25f, -0.1f, 0);
//            activeDefenseUI.transform.localRotation = Quaternion.identity;

//            defenseTimerText = activeDefenseUI.GetComponentInChildren<TextMeshPro>();
//            if (defenseTimerText != null)
//            {
//                defenseTimerText.alignment = TextAlignmentOptions.Center;
//                MeshRenderer mr = defenseTimerText.GetComponent<MeshRenderer>();
//                if (mr != null)
//                {
//                    mr.sortingLayerName = "UI";
//                    mr.sortingOrder = 100;
//                }

//                float zRotation = (owner == TileOwner.Player1) ? 270f : -270f;
//                defenseTimerText.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
//                defenseTimerText.transform.localPosition = Vector3.zero;
//            }
//        }
//    }

//    public void HideDefenseTimerUI()
//    {
//        if (activeDefenseUI != null)
//        {
//            Destroy(activeDefenseUI);
//            activeDefenseUI = null;
//            defenseTimerText = null;
//        }
//    }

//    private IEnumerator HandleAttackTile(TileOwner assumedOwner)
//    {
//        // ÆË ¾Ö´Ï¸ÞÀÌ¼Ç Á¦°Å, ½ºÇÁ¶óÀÌÆ®¸¸ ±³Ã¼
//        if (attackSuccessSprite != null)
//            rend.sprite = attackSuccessSprite;

//        yield return new WaitForSeconds(0.5f);

//        ClearAttackTile();

//        if (GameManager.Instance != null)
//            GameManager.Instance.PlayAttackTileSound();

//        bool isFever = GameManager.Instance.IsFeverTime;
//        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;

//        if (isFever)
//        {
//            GameManager.Instance.AddScore(assumedOwner);
//            GameManager.Instance.RespawnFeverAttackTile(assumedOwner, this);
//        }
//        else
//        {
//            Tile[] allTiles = FindObjectsOfType<Tile>();
//            List<Tile> enemyTiles = new List<Tile>();
//            foreach (Tile t in allTiles)
//            {
//                if (t.owner == enemy && t.territoryOwner == enemy && !t.isAttackTile && !t.isDefenseTile)
//                    enemyTiles.Add(t);
//            }

//            if (enemyTiles.Count > 0)
//            {
//                Tile targetTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
//                targetTile.SetAsDefenseTile(enemy);
//                targetTile.StartDefenseTimer(assumedOwner);
//            }
//        }

//        GameManager.Instance.SetNewAttackTileForPlayer(assumedOwner, this);
//        UpdateSprite();
//    }

//    private IEnumerator HandleDefenseTile(TileOwner assumedOwner)
//    {
//        // ÆË ¾Ö´Ï¸ÞÀÌ¼Ç Á¦°Å, ½ºÇÁ¶óÀÌÆ®¸¸ ±³Ã¼
//        if (defenseSuccessSprite != null)
//            rend.sprite = defenseSuccessSprite;

//        yield return new WaitForSeconds(0.5f);

//        ClearDefenseTile();
//        SetOwner(owner);
//        UpdateSprite();
//    }
//    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ Ãæµ¹ Ã³¸® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
//    private void OnTriggerEnter(Collider other)
//    {
//        TileOwner assumedOwner = other.transform.position.x < 0 ? TileOwner.Player1 : TileOwner.Player2;

//        // ¹æ¾î ¹ßÆÇ ¹âÀ½
//        if (isDefenseTile && owner == assumedOwner)
//        {
//            StartCoroutine(HandleDefenseTile(assumedOwner));
//            return;
//        }

//        // Á¡·É Ã³¸®
//        if (owner != assumedOwner && !isDefenseTile)
//        {
//            if (owner == territoryOwner)
//                SetOwner(assumedOwner);
//        }

//        // °ø°Ý ¹ßÆÇ ¹âÀ½
//        if (isAttackTile)
//        {
//            StartCoroutine(HandleAttackTile(assumedOwner));
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TileOwner { None, Player1, Player2 }

[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ Å¸ÀÏ »óÅÂ ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public TileOwner owner = TileOwner.None;
    public TileOwner territoryOwner = TileOwner.None;

    public bool isAttackTile = false;
    public bool isDefenseTile = false;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ½ºÇÁ¶óÀÌÆ® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public Sprite defaultSprite;
    public Sprite player1Sprite;
    public Sprite player2Sprite;
    public Sprite attackSprite;
    public Sprite defenseSprite;

    public Sprite attackSuccessSprite;
    public Sprite defenseSuccessSprite;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ¹æ¾î UI ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public GameObject defenseTimerUIPrefab;
    private GameObject activeDefenseUI;
    private TextMeshPro defenseTimerText;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ÄÄÆ÷³ÍÆ® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private SpriteRenderer rend;
    private Coroutine attackTimeoutCoroutine;
    private Coroutine defenseCoroutine;

    // Áßº¹ ¹æÁö ÇÃ·¡±×
    private bool isBeingAttacked = false;
    private bool isBeingDefended = false;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ¼ÒÀ¯ÀÚ / ½ºÇÁ¶óÀÌÆ® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void SetOwner(TileOwner newOwner)
    {
        owner = newOwner;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (isAttackTile)
            rend.sprite = attackSprite;
        else if (isDefenseTile)
            rend.sprite = defenseSprite;
        else
        {
            switch (owner)
            {
                case TileOwner.Player1: rend.sprite = player1Sprite; break;
                case TileOwner.Player2: rend.sprite = player2Sprite; break;
                default: rend.sprite = defaultSprite; break;
            }
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ °ø°Ý / ¹æ¾î Å¸ÀÏ ¼³Á¤ ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void SetAsAttackTile(TileOwner attacker)
    {
        isAttackTile = true;
        UpdateSprite();
        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(attacker));
    }

    public void ClearAttackTile()
    {
        isAttackTile = false;
        if (attackTimeoutCoroutine != null)
        {
            StopCoroutine(attackTimeoutCoroutine);
            attackTimeoutCoroutine = null;
        }
        UpdateSprite();
    }

    public void SetAsDefenseTile(TileOwner defender)
    {
        isDefenseTile = true;
        UpdateSprite();
    }

    public void ClearDefenseTile()
    {
        isDefenseTile = false;
        HideDefenseTimerUI();
        UpdateSprite();
    }

    private IEnumerator AttackTimeout(TileOwner attacker)
    {
        yield return new WaitForSeconds(5f);
        if (isAttackTile)
        {
            ClearAttackTile();
            if (!GameManager.Instance.IsFeverTime)
            {
                GameManager.Instance.SetNewAttackTileForPlayer(attacker, this);
            }
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ¹æ¾î Å¸ÀÌ¸Ó ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void StartDefenseTimer(TileOwner attacker)
    {
        isDefenseTile = true;
        if (activeDefenseUI == null)
            ShowDefenseTimerUI();

        if (defenseCoroutine != null)
            StopCoroutine(defenseCoroutine);

        defenseCoroutine = StartCoroutine(DefenseTileTimerCoroutine(attacker));
    }

    private IEnumerator DefenseTileTimerCoroutine(TileOwner attacker)
    {
        float timer = GameManager.Instance.defenseTileLifetime;

        while (timer > 0f)
        {
            if (!isDefenseTile)
            {
                HideDefenseTimerUI();
                yield break;
            }

            if (defenseTimerText != null)
                defenseTimerText.text = Mathf.CeilToInt(timer).ToString();

            timer -= Time.deltaTime;
            yield return null;
        }

        isDefenseTile = false;

        if (GameManager.Instance != null)
            GameManager.Instance.PlayDefenseFailSound();

        SetOwner(attacker);
        GameManager.Instance.AddScore(attacker);
        HideDefenseTimerUI();
    }

    public void ShowDefenseTimerUI()
    {
        if (defenseTimerUIPrefab != null && activeDefenseUI == null)
        {
            activeDefenseUI = Instantiate(defenseTimerUIPrefab, transform);
            activeDefenseUI.transform.localPosition = new Vector3(2.25f, -0.1f, 0);
            activeDefenseUI.transform.localRotation = Quaternion.identity;

            defenseTimerText = activeDefenseUI.GetComponentInChildren<TextMeshPro>();
            if (defenseTimerText != null)
            {
                defenseTimerText.alignment = TextAlignmentOptions.Center;
                MeshRenderer mr = defenseTimerText.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.sortingLayerName = "UI";
                    mr.sortingOrder = 100;
                }

                float zRotation = (owner == TileOwner.Player1) ? 270f : -270f;
                defenseTimerText.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
                defenseTimerText.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void HideDefenseTimerUI()
    {
        if (activeDefenseUI != null)
        {
            Destroy(activeDefenseUI);
            activeDefenseUI = null;
            defenseTimerText = null;
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ ÆË ¾Ö´Ï¸ÞÀÌ¼Ç (°ø°Ý/¹æ¾î °ø¿ë) ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private IEnumerator ShowPopEffect(Sprite tempSprite, float duration = 0.5f, float scaleMultiplier = 1.3f)
    {
        if (tempSprite == null) yield break;

        Sprite originalSprite = rend.sprite;
        Vector3 originalScale = transform.localScale;

        rend.sprite = tempSprite;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float scale = Mathf.Lerp(scaleMultiplier, 1f, Mathf.SmoothStep(0, 1, t));
            transform.localScale = originalScale * scale;
            yield return null;
        }

        transform.localScale = originalScale;
        rend.sprite = originalSprite;
        UpdateSprite();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ °ø°Ý / ¹æ¾î Ã³¸® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private IEnumerator HandleAttackTile(TileOwner assumedOwner)
    {
        // ÆË ¾Ö´Ï¸ÞÀÌ¼Ç (¼º°ø ½ºÇÁ¶óÀÌÆ®)
        yield return StartCoroutine(ShowPopEffect(attackSuccessSprite, 0.4f, 1.4f));

        ClearAttackTile();
        if (GameManager.Instance != null)
            GameManager.Instance.PlayAttackTileSound();

        bool isFever = GameManager.Instance.IsFeverTime;
        TileOwner enemy = assumedOwner == TileOwner.Player1 ? TileOwner.Player2 : TileOwner.Player1;

        if (isFever)
        {
            // ÇÇ¹öÅ¸ÀÓ¿¡¼­´Â Á¡¼ö¸¸ Ãß°¡ÇÏ°í »õ Å¸ÀÏ Áï½Ã ¸®½ºÆù
            GameManager.Instance.AddScore(assumedOwner);
            GameManager.Instance.RespawnFeverAttackTile(assumedOwner, this);
        }
        else
        {
            // ÀÏ¹Ý »óÈ²: Àû Å¸ÀÏÀ» ¹æ¾î Å¸ÀÏ·Î ÀüÈ¯
            Tile[] allTiles = FindObjectsOfType<Tile>();
            List<Tile> enemyTiles = new List<Tile>();
            foreach (Tile t in allTiles)
            {
                if (t.owner == enemy && t.territoryOwner == enemy && !t.isAttackTile && !t.isDefenseTile)
                    enemyTiles.Add(t);
            }

            if (enemyTiles.Count > 0)
            {
                Tile targetTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
                targetTile.SetAsDefenseTile(enemy);
                targetTile.StartDefenseTimer(assumedOwner);
            }
        }

        GameManager.Instance.SetNewAttackTileForPlayer(assumedOwner, this);
        UpdateSprite();
    }

    private IEnumerator HandleDefenseTile(TileOwner assumedOwner)
    {
        yield return StartCoroutine(ShowPopEffect(defenseSuccessSprite, 0.4f, 1.3f));
        ClearDefenseTile();
        SetOwner(owner);
        UpdateSprite();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡ Ãæµ¹ Ã³¸® ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private void OnTriggerEnter(Collider other)
    {
        TileOwner assumedOwner = other.transform.position.x < 0 ? TileOwner.Player1 : TileOwner.Player2;

        // ¹æ¾î ¹ßÆÇ ¹âÀ½ (Áßº¹ ¹æÁö)
        if (isDefenseTile && owner == assumedOwner && !isBeingDefended)
        {
            isBeingDefended = true;
            StartCoroutine(HandleDefenseOnce(assumedOwner));
            return;
        }

        // Á¡·É Ã³¸®
        if (owner != assumedOwner && !isDefenseTile)
        {
            if (owner == territoryOwner)
                SetOwner(assumedOwner);
        }

        // °ø°Ý ¹ßÆÇ ¹âÀ½ (Áßº¹ ¹æÁö)
        if (isAttackTile && !isBeingAttacked)
        {
            isBeingAttacked = true;
            StartCoroutine(HandleAttackOnce(assumedOwner));
        }
    }

    private IEnumerator HandleAttackOnce(TileOwner assumedOwner)
    {
        yield return StartCoroutine(HandleAttackTile(assumedOwner));
        yield return new WaitForSeconds(0.1f);
        isBeingAttacked = false;
    }

    private IEnumerator HandleDefenseOnce(TileOwner assumedOwner)
    {
        yield return StartCoroutine(HandleDefenseTile(assumedOwner));
        yield return new WaitForSeconds(0.1f);
        isBeingDefended = false;
    }
}