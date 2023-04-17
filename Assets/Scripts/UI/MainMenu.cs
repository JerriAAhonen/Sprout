using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Main Page")]
	[SerializeField] private CanvasGroup mainPageRoot;
	[SerializeField] private Button playButton;
	[SerializeField] private TextMeshProUGUI levelLabel;
	[SerializeField] private Button levelButtonRight;
	[SerializeField] private Button levelButtonLeft;
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button quitButton;
	[Header("Settings Page")]
	[SerializeField] private CanvasGroup settingsPageRoot;
	[SerializeField] private Slider volumeSlider;
	[SerializeField] private Button backButton;

	private int selectedLevel = 1;

	private void Start()
	{
		playButton.onClick.AddListener(OnPlay);

		levelButtonRight.onClick.AddListener(IncreaseLevel);
		levelButtonLeft.onClick.AddListener(DecreaseLevel);

		settingsButton.onClick.AddListener(OnSettings);
		quitButton.onClick.AddListener(OnQuit);

		backButton.onClick.AddListener(OnMainMenu);

		volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

		OnMainMenu();
	}

	private void IncreaseLevel()
	{
		selectedLevel++;

		if (SceneManager.sceneCountInBuildSettings <= selectedLevel)
			selectedLevel = 1;

		levelLabel.text = $"Level {selectedLevel}";
	}

	private void DecreaseLevel()
	{
		selectedLevel--;

		if (selectedLevel <= 0)
			selectedLevel = SceneManager.sceneCountInBuildSettings - 1;

		levelLabel.text = $"Level {selectedLevel}";
	}

	private void OnVolumeChanged(float newValue)
	{
		AudioManager.Instance.SetVolume(newValue);
	}

	private void OnPlay()
	{
		LevelLoader.LoadLevel(selectedLevel);
	}

	private void OnSettings()
	{
		mainPageRoot.Toggle(false);
		settingsPageRoot.Toggle(true);
	}

	private void OnMainMenu()
	{
		mainPageRoot.Toggle(true);
		settingsPageRoot.Toggle(false);
	}

	private void OnQuit()
	{
		Application.Quit();
	}
}
