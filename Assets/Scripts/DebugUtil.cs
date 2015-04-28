using UnityEngine;
using System.Collections;

public class DebugUtil {

	public class Colors{
		public static Color AIMLINE  = Color.blue;
		public static Color FIRELINE = Color.green;

		public static Color TrailColor(float speed){
			return new Color (speed / 90000f, 0, 0);
		}
		
		public static Color LEADLINE = Color.magenta;
	}

	public class Logging{
		public static string VectorJSON(Vector3 vec){
			return string.Format("[{0}, {1}, {2}]", vec.x, vec.y, vec.z);
		}
	}
}
