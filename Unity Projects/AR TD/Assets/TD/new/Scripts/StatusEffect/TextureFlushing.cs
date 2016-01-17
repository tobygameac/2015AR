using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextureFlushing : MonoBehaviour
{
    public GameObject icon;
    private SpriteRenderer iconImg;

    public Sprite[] sprites = new Sprite[5];

    public float floatingMagnitude;
    public float floatingSpeed;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        iconImg = icon.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float input_angle = -Mathf.PI / 2 + (floatingSpeed * Time.time) % Mathf.PI;

        Vector3 newPosition = originalPosition + transform.forward * floatingMagnitude * Mathf.Sin(input_angle);
        transform.localPosition = newPosition;

        input_angle += Mathf.PI / 2;

        if (input_angle < Mathf.PI / 5) iconImg.sprite = sprites[4];
        else if (input_angle < (Mathf.PI / 5) * 2) iconImg.sprite = sprites[3];
        else if (input_angle < (Mathf.PI / 5) * 3) iconImg.sprite = sprites[2];
        else if (input_angle < (Mathf.PI / 5) * 4) iconImg.sprite = sprites[1];
        else iconImg.sprite = sprites[0];

    }

    void Disable()
    {
        transform.localPosition = originalPosition;
    }
}
