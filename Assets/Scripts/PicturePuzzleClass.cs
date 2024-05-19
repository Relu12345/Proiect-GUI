using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PictureData
{
    public Sprite spriteEmoji;
    public Sprite spritePicture;
    public int number;
}

public class PicturePuzzleClass : MonoBehaviour
{
    // Define pictures data for each enum value
    public List<PictureData> pictureDataList = new List<PictureData>();

    // Static method to get sprite and number for a given picture
    public static void GetSpriteAndNumber(int index, out Sprite spriteEmoji, out Sprite spriteImage, out int number)
    {
        spriteEmoji = null;
        spriteImage = null;
        number = 0;

        PicturePuzzleClass instance = FindObjectOfType<PicturePuzzleClass>();

        if (instance != null && index >= 0 && index < instance.pictureDataList.Count)
        {
            spriteEmoji = instance.pictureDataList[index].spriteEmoji;
            spriteImage = instance.pictureDataList[index].spritePicture;
            number = instance.pictureDataList[index].number;
        }
    }

    public static void AssignRandomPicturesAndNumbers()
    {
        PicturePuzzleClass instance = FindObjectOfType<PicturePuzzleClass>();

        if (instance != null && instance.pictureDataList.Count >= 3)
        {
            // Shuffle pictureDataList
            for (int i = 0; i < instance.pictureDataList.Count; i++)
            {
                PictureData temp = instance.pictureDataList[i];
                int randomIndex = UnityEngine.Random.Range(i, instance.pictureDataList.Count);
                instance.pictureDataList[i] = instance.pictureDataList[randomIndex];
                instance.pictureDataList[randomIndex] = temp;
            }
        }
    }
}
