using UnityEngine;
using Rewired;

[CreateAssetMenu(fileName = "New Control", menuName = "Data Containers/Action Name")]
public class ActionName : ScriptableObject {
	[ActionIdProperty(typeof(RewiredConsts.Action))]
	public int actionId;
	public AxisRange axisRange = AxisRange.Full;
}
