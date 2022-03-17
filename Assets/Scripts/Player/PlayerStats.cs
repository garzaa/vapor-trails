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
			GetList<string>(nameof(unlockedAbilities))
			.Select(x => (Ability) System.Enum.Parse(typeof(Ability), x))
		);
	}

	void UpdateAbilitiesProperty() {
		SetProperty(
			nameof(unlockedAbilities),
			unlockedAbilities.Select(x => x.ToString())
			.ToList<string>()
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
		return GetInt(baseDamage);
	}

	public int GetCurrentHealth() {
		return GetInt("currentHealth");
	}

	public int GetMaxHealth() {
		return GetInt("maxHealth");
	}

	public int GetCurrentEnergy() {
		return GetInt("currentEnergy");
	}

	public int GetMaxEnergy() {
		return GetInt("maxEnergy");
	}

	public bool HasAbility(Ability a) {
		return unlockedAbilities.Contains(a);
	}
}
