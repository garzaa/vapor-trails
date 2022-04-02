using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerStats : PersistentObject {
	HashSet<Ability> unlockedAbilities;

	string baseDamage = "baseDamage";

	protected override void SetDefaults() {
		SetDefault(baseDamage, 1);
		SetDefault("currentHealth", 16);
		SetDefault("maxHealth", 16);
		SetDefault("currentEnergy", 12);
		SetDefault("maxEnergy", 12);
		SetDefault(nameof(unlockedAbilities), new List<string>());
		SetDefault("playerFacingRight", true);

		// store enums as text
		unlockedAbilities = new HashSet<Ability>(
			GetList<string>(nameof(unlockedAbilities))
			.Select(x => (Ability) System.Enum.Parse(typeof(Ability), x))
		);
	}

	void Start() {
		bool facingRight = GetProperty<bool>("playerFacingRight");
		if (!GlobalController.pc.facingRight && facingRight) {
			GlobalController.pc.Flip();
		} else if (GlobalController.pc.facingRight && !facingRight) {
			GlobalController.pc.Flip();
		}
	}

	protected override void PrepForSave() {
		SetProperty("playerFacingRight", GlobalController.pc.facingRight);
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
