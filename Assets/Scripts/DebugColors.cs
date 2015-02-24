using UnityEngine;
using System.Collections;

public class DebugColors {
	public static Color AIMLINE  = Color.blue;
	public static Color FIRELINE = Color.green;

	public static Color TrailColor(float speed){
		return new Color (speed / 90000f, 0, 0);
	}
	
	public static Color LEADLINE = Color.magenta;
}
