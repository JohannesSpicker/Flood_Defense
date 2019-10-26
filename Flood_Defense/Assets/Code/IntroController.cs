using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
	public Sprite[] sprites;
	public Image image;
	private int currentSprite;

    // Start is called before the first frame update
    void Start()
    {
		SetSprite(0);
		currentSprite = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void NextSprite()
	{
		SetSprite(currentSprite++);
	}

	private void SetSprite(int i)
	{
		if (i < sprites.Length)
			image.sprite = sprites[i];
		else
			SceneManager.LoadScene(1);
	}
}
