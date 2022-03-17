using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerStats : PersistentObject {
	HashSet<Ability> unlockedAbilities;

	string baseDamage = "baseDamage";

	protected override void SetDefaults() {
		SetDefault(baseDamage, 1);
		SetDefault("currentHealth", 20);
		SetDefault("maxHealth", 20);
		SetDefault("currentEnergy", 10);
		SetDefault("maxEnergy", 10);
		SetDefault(nameof(unlockedAbilities), new List<string>());

		// store enums as text
		unlockedAbilities = new HashSet<Ability>(
			GetProperty<List<string>>(nameof(unlockedAbilities))
			.Select(x => (Ability) System.Enum.Parse(typeof(Ability), x))
		);
	}

	void UpdateAbilitiesProperty() {
		SetProperty(
			nameof(unlockedAbilities),
			unlockedAbilities.Select(x => x.ToString())
		);
	}

	public void SetBaseDamage(int dmg) {
		SetProperty("baseDamage", dmg);
	}

	public void SetHealth(int cur, int max) {
		SetProperty("currentHealth", cur);
		SetProperty("maxHealth", max);
	}

	public void SetEnergy(int cur, int max) {
		SetProperty("currentEnergy", cur);
		SetProperty("maxEnergy", max);
	}

	public void UnlockAbility(Ability ability) {
		unlockedAbilities.Add(ability);
		UpdateAbilitiesProperty();
	}

	public int GetBaseDamage() {
		return GetProperty<int>(baseDamage);
	}

	public int GetCurrentHealth() {
		return GetProperty<int>("currentHealth");
	}

	public int GetMaxHealth() {
		return GetProperty<int>("maxHealth");
	}

	public int GetCurrentEnergy() {
		return GetProperty<int>("currentEnergy");
	}

	public int GetMaxEnergy() {
		return GetProperty<int>("maxEnergy");
	}

	public bool HasAbility(Ability a) {
		return unlockedAbilities.Contains(a);
	}
}
