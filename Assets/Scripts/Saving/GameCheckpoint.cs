using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Runtime/Game Checkpoint")]
public class GameCheckpoint : ScriptableObject {
	public List<GameCheckpoint> composites;

	public List<GameState> gameStates;
	public List<GameFlag> gameFlags;
	public List<Item> items;

	public HashSet<GameState> GetGameStates() {
		HashSet<GameState> states = new HashSet<GameState>(gameStates);
		foreach (GameCheckpoint checkpoint in composites) {
			states.UnionWith(checkpoint.GetGameStates());
		}
		return states;
	}

	public HashSet<GameFlag> GetGameFlags() {
		HashSet<GameFlag> flags = new HashSet<GameFlag>(gameFlags);
		foreach (GameCheckpoint checkpoint in composites) {
			flags.UnionWith(checkpoint.GetGameFlags());
		}
		return flags;
	}

	public HashSet<Item> GetItems() {
		HashSet<Item> itemSet = new HashSet<Item>(items);
		foreach (GameCheckpoint checkpoint in composites) {
			itemSet.UnionWith(checkpoint.GetItems());
		}
		return itemSet;
	}

	public void Import() {
		foreach (GameState gameState in GetGameStates()) {
			SaveManager.AddState(gameState);
		}

		foreach (GameFlag flag in GetGameFlags()) {
			SaveManager.AddGameFlag(flag);
		}

		foreach (Item item in GetItems()) {
			GlobalController.AddItem(new StoredItem(item), quiet:true);
		}
	}

}
