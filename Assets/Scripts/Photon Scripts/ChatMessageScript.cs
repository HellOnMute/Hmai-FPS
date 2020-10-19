using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMessageScript : MonoBehaviour
{
    public string Message { get; set; }
    public float CurrentOpacity { get; set; } = 1f;
    public bool IsChatting { get; set; } = false;
    public float Timer { get; set; } = 10.0f;

    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(GetComponent<TextMeshProUGUI>().text))
            text.text = Message;

        if (IsChatting)
        {
            text.color = Color.white;
        }
        else
        {
            var transparent = Color.white;
            transparent.a = CurrentOpacity;
            text.color = transparent;
        }

        if (Timer >= 0)
            Timer -= Time.deltaTime;
        if (Timer >= 0 && Timer <= 1)
            CurrentOpacity -= Time.deltaTime;
    }
}
