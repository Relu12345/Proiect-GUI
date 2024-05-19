using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer1;
    private SpriteRenderer spriteRenderer2;
    private SpriteRenderer spriteRenderer3;

    private SpriteRenderer spriteRendererImg1;
    private SpriteRenderer spriteRendererImg2;
    private SpriteRenderer spriteRendererImg3;

    public List<Material> materials = new List<Material>();

    public static List<int> numbers = new List<int>();

    private void Start()
    {
        PicturePuzzleClass.AssignRandomPicturesAndNumbers();

        spriteRenderer1 = transform.Find("Visuals/Root/Image1").GetComponent<SpriteRenderer>();
        spriteRenderer2 = transform.Find("Visuals/Root/Image2").GetComponent<SpriteRenderer>();
        spriteRenderer3 = transform.Find("Visuals/Root/Image3").GetComponent<SpriteRenderer>();

        spriteRendererImg1 = GameObject.Find("Image1(Clone)").GetComponent<SpriteRenderer>();
        spriteRendererImg2 = GameObject.Find("Image2(Clone)").GetComponent<SpriteRenderer>();
        spriteRendererImg3 = GameObject.Find("Image3(Clone)").GetComponent<SpriteRenderer>();

        int number1;
        Sprite sprite1, spriteImg1;
        PicturePuzzleClass.GetSpriteAndNumber(0, out sprite1, out spriteImg1, out number1);

        int number2;
        Sprite sprite2, spriteImg2;
        PicturePuzzleClass.GetSpriteAndNumber(1, out sprite2, out spriteImg2, out number2);

        int number3;
        Sprite sprite3, spriteImg3;
        PicturePuzzleClass.GetSpriteAndNumber(2, out sprite3, out spriteImg3, out number3);

        spriteRenderer1.sprite = sprite1;
        spriteRenderer2.sprite = sprite2;
        spriteRenderer3.sprite = sprite3;

        spriteRendererImg1.sprite = spriteImg1;
        spriteRendererImg2.sprite = spriteImg2;
        spriteRendererImg3.sprite = spriteImg3;

        spriteRenderer1.SetMaterials(materials);
        spriteRenderer2.SetMaterials(materials);
        spriteRenderer3.SetMaterials(materials);

        numbers.Add(number1);
        numbers.Add(number2);
        numbers.Add(number3);
    }
}
