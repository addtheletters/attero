using UnityEngine;
using System.Collections;

public class DebugColors {
	public static Color AIMLINE  = Color.blue;
	public static Color FIRELINE = Color.green;

	public static Color TrailColor(float speed){
		return new Color (speed / 1000f, 0, 0);
	}
}
