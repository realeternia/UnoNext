using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private Image targetImage;
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(1f, 1f, 1f, 0.5f);
    private float timer = 0f;
    private bool isFadingIn = true;

    public bool isOnTurn;
    public TMP_Text playerNameText;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
        // if (targetImage != null)
        // {
        //     targetImage.color = startColor;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnTurn)
        {
            if (targetImage != null)
            {
                timer += Time.deltaTime;
                // 使用正弦函数计算插值因子，范围在 0 到 1 之间
                float t = (Mathf.Sin((timer / blinkDuration) * Mathf.PI * 2f) + 1f) / 2f;
                // 根据插值因子在 startColor 和 endColor 之间做差值
                targetImage.color = Color.Lerp(startColor, endColor, t);
                // 重置计时器，让其循环
                timer %= blinkDuration;
            }
        }
        else
        {
            if(targetImage != null)
            {
                if(targetImage.color != new Color(1f, 1f, 1f, 0.5f))
                {
                    targetImage.color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
        }
    }
}
