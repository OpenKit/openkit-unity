using System.Collections;
using System.Collections.Generic;
using System;

namespace OpenKit
{
	public class FormPair
	{

		public string Name { get; private set; }
		public object Value { get; private set; }
		public FormPair(string name, object val) {
			Name = name;
			Value = val;
		}

		public static System.Collections.IEnumerable Each(Dictionary<string, object> parameters, string runningName = "")
		{
			foreach (var p1 in parameters) {
				var v = p1.Value;
				var k = p1.Key;

				// Naming format we are going for is: "person[address][city]"
				string name = (runningName.Length == 0) ? k : String.Format("{0}[{1}]", runningName, k);

				if (v.GetType() == typeof(Dictionary<string, object>)) {
					Dictionary<string, object> nestedParams = (Dictionary<string, object>)v;
					foreach(FormPair pair in Each(nestedParams, name)) {
						yield return pair;
					}
				} else {
					yield return new FormPair(name, v);
				}
			}
		}
	}
}
