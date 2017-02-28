
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ./  (_(__(S)   |___*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WanzyeeStudio.Json{

	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>System.Collections.Generic.Dictionary</c>.
	/// </summary>
	public class DictionaryConverter : JsonConverter{
		
		/// <summary>
		/// Determine if the type is <c>System.Collections.Generic.Dictionary</c>.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns><c>true</c> if this can convert the specified type; otherwise, <c>false</c>.</returns>
		public override bool CanConvert(Type objectType){

			if(!objectType.IsGenericType) return false;

			var _t = objectType.GetGenericTypeDefinition();

			return typeof(Dictionary<,>) == _t || typeof(IDictionary<,>) == _t;

		}

		/// <summary>
		/// Read as <c>System.Collections.Generic.KeyValuePair</c> array to rebuild a dictionary.
		/// </summary>
		/// <returns>The object value.</returns>
		/// <param name="reader">The <c>Newtonsoft.Json.JsonReader</c> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override object ReadJson(
			JsonReader reader,
			Type objectType,
			object existingValue,
			JsonSerializer serializer
		){

			if(JsonToken.Null == reader.TokenType) return null;

			var _d = (IDictionary)Activator.CreateInstance(objectType);
			var _a = objectType.GetGenericArguments();

			foreach(JObject _v in JArray.Load(reader)){

				var _k = _v["Key"].ToObject(_a[0], serializer);

				if(!_d.Contains(_k)) _d.Add(_k, _v["Value"].ToObject(_a[1], serializer));

				else Debug.LogWarningFormat("Ignore pair with repeat key: {0}", _v.ToString(Formatting.None));

			}

			return _d;

		}

		/// <summary>
		/// Write as <c>System.Collections.Generic.KeyValuePair</c> array.
		/// </summary>
		/// <param name="writer">The <c>Newtonsoft.Json.JsonWriter</c> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
			serializer.Serialize(writer, ((IDictionary)value).Cast<object>());
		}

	}

}
