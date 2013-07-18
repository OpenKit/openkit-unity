using OpenKit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenKit
{
	public class JSONObjectExt
	{
		
		public static string encode(object o)
		{
			JSONObject jsonObj = jsonObjectify(o);
			return jsonObj.print();
		}
		
		public static JSONObject decode(string encodedStr)
		{
			JSONObject jsonObj = new JSONObject(encodedStr);
			return jsonObj;
		}
		
		public static JSONObject jsonObjectify(object o)
		{
			JSONObject j = null;
			Type ot = o.GetType();
			if (ot == typeof(String)) {
				j = new JSONObject { type = JSONObject.Type.STRING, str = (string)o };
			} 
			else if (ot == typeof(ArrayList)) {
				j = new JSONObject { type = JSONObject.Type.ARRAY };
				foreach (object element in (ArrayList)o) {
					j.Add(jsonObjectify(element));
				}
			}
			else if (ot == typeof(Dictionary<string, object>)) {
				j = new JSONObject { type = JSONObject.Type.OBJECT };
				foreach (KeyValuePair<string,object> entry in (Dictionary<string, object>)o) {
					j.AddField(entry.Key, jsonObjectify(entry.Value));
				}
			}
			else if (ot.IsPrimitive) {
				if (typeof(bool) == ot) {
					j = new JSONObject { type = JSONObject.Type.BOOL, b = (bool)o};
				} else {
					double d = Convert.ToDouble(o);
					j = new JSONObject { type = JSONObject.Type.NUMBER, n = d};
				}
			}
			return j;
		}

		public static object DeJSONify(JSONObject j)
		{
			switch (j.type) {
			case JSONObject.Type.STRING:
				return j.str;
			case JSONObject.Type.NUMBER:
				return j.n;
			case JSONObject.Type.OBJECT:
				Dictionary<string, object> dict = new Dictionary<string, object>();
				for (int i = 0; i < j.list.Count; i++) {
					string key = (string)j.keys[i];
					dict.Add(key,DeJSONify((JSONObject)j.list[i]));
				}
				return dict;
			case JSONObject.Type.ARRAY:
				ArrayList retList = new ArrayList();
				foreach (JSONObject o in j.list){
					retList.Add(DeJSONify(o));
				}
				return retList;
			case JSONObject.Type.BOOL:
				return j.b;
			case JSONObject.Type.NULL:
				return null;
			default:
				// TODO: Throw error here!
				return null;
			}
		}
	}
}
