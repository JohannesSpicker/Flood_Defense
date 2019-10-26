using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UiController : MonoBehaviour
{
	[SerializeField] private GameObject resourceObject;
	private TextMeshProUGUI resourceText;
	[SerializeField] private GameObject villagesSavedObject;
	private TextMeshProUGUI villagesSavedText;
	private PlayerController player;

	[SerializeField] private GameObject panelUI;
	[SerializeField] private GameObject panelOutroLose;
	[SerializeField] private GameObject panelOutroWin;

	// Start is called before the first frame update
	void Start()
	{
		resourceText = resourceObject?.GetComponent<TextMeshProUGUI>();
		villagesSavedText = villagesSavedObject?.GetComponent<TextMeshProUGUI>();
		player = FindObjectOfType<PlayerController>();

		//resourceObject.SetActive(false);
		SetPanelUI();
	}

	// Update is called once per frame
	void Update()
	{
		SetResourceUI();
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(0);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void SetResourceUI()
	{
		if (resourceText.isActiveAndEnabled)
			resourceText.text = player.resources.ToString();
	}

	private void SetVillageUI()
	{
		if (villagesSavedText.isActiveAndEnabled)
			villagesSavedText.text = player.villages.ToString();
	}

	public void SetPanelUI()
	{
		panelUI.SetActive(true);
		panelOutroLose.SetActive(false);
		panelOutroWin.SetActive(false);
	}

	public void SetPanelWin()
	{
		panelUI.SetActive(false);
		panelOutroLose.SetActive(false);
		panelOutroWin.SetActive(true);

		SetVillageUI();
	}

	public void SetPanelLose()
	{
		panelUI.SetActive(false);
		panelOutroLose.SetActive(true);
		panelOutroWin.SetActive(false);
	}
}
