using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
	[SerializeField] private GameObject separatorPrefab;
	[SerializeField] private CanvasGroup root;
	[SerializeField] private Transform pointer;
	[SerializeField] private Camera mainCamera;
	[SerializeField] private Cannon cannon;
	[Space]
	[SerializeField] private GameObject iconPrefab;
	[SerializeField] private float selectionScaleDelta;
	[SerializeField] private float selectionScaleDuration;

	private bool isInitialised;
	private bool isOpen;
	private Vector2 screenCenter;
	private float itemSpacingAngle;
	private List<Image> iconInstances = new();
	private int currentlySelected = -1;
	private int currentlyLoaded = (int)ProjectileType.Sapling;

	#region MonoBehaviour

	private void Awake()
	{
		Close();
		Init();
	}

	private void Start()
	{
		screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
	}

	private void Update()
	{
		if (!isInitialised) 
			return;

		// Close menu if game ends while menu is open
		if (!ScoreManager.Instance.LevelOngoing && isOpen)
		{
			Close();
			return;
		}

		if (!ScoreManager.Instance.LevelOngoing)
			return;

		if (Input.GetMouseButton(1))
		{
			if (!isOpen)
				Open();

			var dir = ((Vector2)Input.mousePosition - screenCenter).normalized;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			var rotation = new Vector3(0, 0, angle - 90);
			pointer.rotation = Quaternion.Euler(rotation);

			var signedAngle = Vector2.SignedAngle(Vector2.up, dir);

			float actualAngle;
			if (signedAngle < 0)
				actualAngle = Mathf.Abs(signedAngle);
			else
			{
				actualAngle = 180 + (180 - signedAngle);
			}

			int selectedItem = Mathf.FloorToInt(actualAngle / itemSpacingAngle);
			if (selectedItem != currentlySelected)
			{
				AnimateSelection(currentlySelected, selectedItem);
				currentlySelected = selectedItem;
			}
		}
		else if (isOpen)
		{
			Close();

			if (currentlySelected != currentlyLoaded)
			{
				cannon.StartLoadRoutine((ProjectileType)currentlySelected);
				currentlyLoaded = currentlySelected;
			}
		}
	}

	#endregion

	public void Init()
	{
		var projectileDatabase = Resources.Load<ProjectileDatabase>("ProjectileDatabase");
		var itemNum = projectileDatabase.Count;
		itemSpacingAngle = 360 / itemNum;
		
		for (int i = 0; i < itemNum; i++)
		{
			var separator = Instantiate(separatorPrefab, transform);
			separator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -itemSpacingAngle * i));

			var icon = Instantiate(iconPrefab, transform);
			var iconRotation = -itemSpacingAngle * i - (itemSpacingAngle / 2);
			icon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, iconRotation));

			var type = (ProjectileType)i;
			var image = icon.GetComponentInChildren<Image>();
			image.sprite = projectileDatabase.GetProjectile(type).Icon;
			image.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
			iconInstances.Add(image);
		}

		isInitialised = true;
	}

	private void AnimateSelection(int previous, int selected)
	{
		if (previous > -1)
		{
			LeanTween.value(selectionScaleDelta, 1, selectionScaleDuration)
				.setOnUpdate(v => iconInstances[previous].transform.localScale = Vector3.one * v)
				.setEase(LeanTweenType.easeOutQuart);
		}

		LeanTween.value(1, selectionScaleDelta, selectionScaleDuration)
			.setOnUpdate(v => iconInstances[selected].transform.localScale = Vector3.one * v)
			.setEase(LeanTweenType.easeOutBack);
	}

	public void Open()
	{
		SetVisible(true);
		isOpen = true;
	}

	public void Close()
	{
		SetVisible(false);
		isOpen = false;
	}

	private void SetVisible(bool visible) => root.alpha = visible ? 1.0f : 0.0f;
}
