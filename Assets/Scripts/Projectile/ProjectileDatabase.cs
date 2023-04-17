using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
	Bomb, Sapling, Water, Fertilizer
}

[CreateAssetMenu(menuName = "Projectile Database")]
public class ProjectileDatabase : ScriptableObject
{
	[Serializable]
	public class ProjectileMap
	{
		public ProjectileType Type;
		public GameObject Prefab;
		public Sprite Icon;
	}

	[SerializeField] private List<ProjectileMap> projectiles;

	public int Count => projectiles.Count;
	public ProjectileMap GetProjectile(ProjectileType type) => projectiles.Find(x => x.Type == type);
}
