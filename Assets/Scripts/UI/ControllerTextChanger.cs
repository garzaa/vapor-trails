using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ControllerTextChanger : MonoBehaviour {

	// (kbd|xbox|ps4)
	static string pattern = @"\(\w+\|\w+\|\w+\)";
	static string textLastFrame;
	static MatchEvaluator evaluator;

	void OnEnable() {
		evaluator = new MatchEvaluator(ButtonExtractor);
	}

	public static string ReplaceText(string target) {
		return Regex.Replace(
			target,
			pattern,
			evaluator
		);
	}

	string ButtonExtractor(Match match) {
		string[] matchedString = match.Value.Replace("(","").Replace(")","").Split('|');
		if (!GlobalController.xboxController && !GlobalController.playstationController) {
			return matchedString[0];
		} else if (GlobalController.xboxController) {
			return matchedString[1];
		} else {
			return matchedString[2];
		}
	}

}
