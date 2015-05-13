using UnityEngine;
using System.Collections;
using System.IO;

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

		public static string ToJSONArray(IJSONable[] jsonables){
			string buffer = "[";
			for( int i = 0; i < jsonables.Length; i++){
				buffer += jsonables[i].ToJSON();
				if(i < jsonables.Length - 1){
					buffer += ", ";
				}
			}
			buffer += "]";
			return buffer;
		}
	}

	public class FileIO{
		public static string Load(string filename){
			if(File.Exists(filename)){
				return File.ReadAllText(filename);
			}
			else{
				Debug.Log("FileIO: Could not read from file [" + filename + "]");
				return "";
			}
		}

		public static void Write(string filename, string text, bool overwrite){
			if(File.Exists(filename)){
				Debug.Log ("FileIO: File with given name exists. [" + filename + "]");
				if(!overwrite){
					return;
				}
			}
			File.WriteAllText(filename, text);
		}
	}

}
