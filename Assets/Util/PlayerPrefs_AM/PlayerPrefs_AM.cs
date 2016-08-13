using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("PlayerPrefs_AM")]
public sealed class PlayerPrefs_AM {

	// Never forget that this solution is not thread safe. Because Unity is mono threaded, we keep it like this but it may cause problem.
	// For more info : http://jlambert.developpez.com/tutoriels/dotnet/implementation-pattern-singleton-csharp/
	private static PlayerPrefs_AM Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new PlayerPrefs_AM();
				_instance.floats = new SerializableDictionary<string, float>();
				_instance.ints = new SerializableDictionary<string, int>();
				_instance.bools = new SerializableDictionary<string, bool>();
				_instance.strings = new SerializableDictionary<string, string>();
				_instance.vector3XML = new SerializableDictionary<string, Vector3XML>();
			}

			return _instance;
		}
	}

	public class Vector3XML
	{
		public float x;
		public float y;
		public float z;

		// Default constructor to make it serializable.
		public Vector3XML()
		{
		}

		public Vector3XML(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	enum ArrayType { Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color }

	public static string fileName = "AM Show Calibration data.xml";

	public SerializableDictionary<string, float> floats;
	public SerializableDictionary<string, int> ints;
	public SerializableDictionary<string, bool> bools;
	public SerializableDictionary<string, string> strings;
	public SerializableDictionary<string, Vector3XML> vector3XML;
	
	private static PlayerPrefs_AM _instance; // Never use this instance. Just use Instance (that ensure unicity).

	// Used for coverting a vector3 to bytes.
	static private int endianDiff1;
	static private int endianDiff2;
	static private int idx;
	static private byte[] byteBlock;
	
	#region Static functions
	/*****************************************************/
	/*****************************************************/
	public static void SetFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
		Instance.AddToDictionnary(Instance.floats, key, value);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		float result = PlayerPrefs.GetFloat(key, defaultValue);
		Instance.AddToDictionnary(Instance.floats, key, result);
		return result;
	}
	/*****************************************************/
	/*****************************************************/
	public static void SetInt(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
		Instance.AddToDictionnary(Instance.ints, key, value);
	}
	
	public static int GetInt(string key, int defaultValue)
	{
		int result = PlayerPrefs.GetInt(key, defaultValue);
		Instance.AddToDictionnary(Instance.ints, key, result);
		return result;
	}
	/*****************************************************/
	/*****************************************************/
	public static void SetString(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		Instance.AddToDictionnary(Instance.strings, key, value);
	}
	
	public static string GetString(string key, string defaultValue)
	{
		string result = PlayerPrefs.GetString(key, defaultValue);
		Instance.AddToDictionnary(Instance.strings, key, result);
		return result;
	}
	/*****************************************************/
	/*****************************************************/
	public static void SetVector3(string key, Vector3 value, bool broadcastUpdate = false)
	{
		Instance.AddToDictionnary(Instance.vector3XML,key,new Vector3XML(value.x, value.y, value.z));
		SetFloatArray(key, new float[] { value.x, value.y, value.z });

        if(broadcastUpdate)
        {
            PlayerPrefUpdateBroadcast.Instance.UpdatePlayerPrefs(key);
        }

	}

	public static Vector3 GetVector3(string key, Vector3 defaultValue = default(Vector3))
	{
		if (PlayerPrefs.HasKey(key))
		{
			var floatArray = GetFloatArray(key);
			if (floatArray.Length < 3)
			{
				return Vector3.zero;
			}

			Vector3 result = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
			Instance.AddToDictionnary(Instance.vector3XML,key, new Vector3XML(result.x, result.y, result.z));
			return result;
		}
		return defaultValue;
	}

    public static void DeleteKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
	/*****************************************************/
	/*****************************************************/
	public static bool SetBool(string key, bool value)
	{
		try
		{
			int boolToInt = value ? 1 : 0;
			PlayerPrefs.SetInt(key, boolToInt);
			Instance.AddToDictionnary(Instance.bools, key, value);
		}
		catch
		{
			return false;
		}
		return true;
	}
	
	public static bool GetBool(string key)
	{
		bool result = PlayerPrefs.GetInt(key) == 1;
		Instance.AddToDictionnary(Instance.bools, key, result);
		return result;
	}

	public static void ExportDataToXML()
	{
		Instance.SaveToXML();
	}

	// Load every XML parameters and store it in PlayerPrefs.
	public static void LoadDataFromXML()
	{
		string path = Path.Combine(Application.dataPath,fileName);

		if(!File.Exists(path))
		{
			Debug.LogError("Try to load data from XML but file doesn't exist. Try to load : "+path);
			return;
		}

		var serializer = new XmlSerializer(typeof(PlayerPrefs_AM));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			PlayerPrefs_AM data = serializer.Deserialize(stream) as PlayerPrefs_AM;
			// Set all int stored in the xml.
			foreach(KeyValuePair<string, int> entry in data.ints)
			{
				SetInt(entry.Key, entry.Value);
			}
			// Set all floats stored in the xml.
			foreach(KeyValuePair<string, float> entry in data.floats)
			{
				SetFloat(entry.Key, entry.Value);
			}
			// Set all strings stored in the xml.
			foreach(KeyValuePair<string, string> entry in data.strings)
			{
				SetString(entry.Key, entry.Value);
			}
			// Set all bools stored in the xml.
			foreach(KeyValuePair<string, bool> entry in data.bools)
			{
				SetBool(entry.Key, entry.Value);
			}
			// Set all Vector3 stored in the xml.
			foreach(KeyValuePair<string, Vector3XML> entry in data.vector3XML)
			{
				SetVector3(entry.Key, new Vector3(entry.Value.x,entry.Value.y,entry.Value.z));
			}
		}
	}

	private static bool SetFloatArray(System.String key, float[] floatArray)
	{
		return SetValue(key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
	}

	private static float[] GetFloatArray(System.String key)
	{
		var floatList = new List<float>();
		GetValue(key, floatList, ArrayType.Float, 1, ConvertToFloat);
		return floatList.ToArray();
	}
	
	private static bool SetValue<T>(System.String key, T array, ArrayType arrayType, int vectorNumber, System.Action<T, byte[], int> convert) where T : IList
	{
		var bytes = new byte[(4 * array.Count) * vectorNumber + 1];
		bytes[0] = System.Convert.ToByte(arrayType);	// Identifier
		Initialize();
		
		for (var i = 0; i < array.Count; i++)
		{
			convert(array, bytes, i);
		}
		return SaveBytes(key, bytes);
	}

	private static void GetValue<T>(System.String key, T list, ArrayType arrayType, int vectorNumber, System.Action<T, byte[]> convert) where T : IList
	{
		if (PlayerPrefs.HasKey(key))
		{
			var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
			if ((bytes.Length - 1) % (vectorNumber * 4) != 0)
			{
				Debug.LogError("Corrupt preference file for " + key);
				return;
			}
			if ((ArrayType)bytes[0] != arrayType)
			{
				Debug.LogError(key + " is not a " + arrayType.ToString() + " array");
				return;
			}
			Initialize();
			
			var end = (bytes.Length - 1) / (vectorNumber * 4);
			for (var i = 0; i < end; i++)
			{
				convert(list, bytes);
			}
		}
	}
	
	private static void Initialize()
	{
		if (System.BitConverter.IsLittleEndian)
		{
			endianDiff1 = 0;
			endianDiff2 = 0;
		}
		else
		{
			endianDiff1 = 3;
			endianDiff2 = 1;
		}
		if (byteBlock == null)
		{
			byteBlock = new byte[4];
		}
		idx = 1;
	}
	
	private static bool SaveBytes(System.String key, byte[] bytes)
	{
		try
		{
			PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes));
		}
		catch
		{
			return false;
		}
		return true;
	}
	
	private static void ConvertFromFloat(float[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes(array[i], bytes);
	}
	private static void ConvertFloatToBytes(float f, byte[] bytes)
	{
		byteBlock = System.BitConverter.GetBytes(f);
		ConvertTo4Bytes(bytes);
	}
	private static void ConvertTo4Bytes(byte[] bytes)
	{
		bytes[idx] = byteBlock[endianDiff1];
		bytes[idx + 1] = byteBlock[1 + endianDiff2];
		bytes[idx + 2] = byteBlock[2 - endianDiff2];
		bytes[idx + 3] = byteBlock[3 - endianDiff1];
		idx += 4;
	}

	private static void ConvertToFloat(List<float> list, byte[] bytes)
	{
		list.Add(ConvertBytesToFloat(bytes));
	}
	private static float ConvertBytesToFloat(byte[] bytes)
	{
		ConvertFrom4Bytes(bytes);
		return System.BitConverter.ToSingle(byteBlock, 0);
	}
	private static void ConvertFrom4Bytes(byte[] bytes)
	{
		byteBlock[endianDiff1] = bytes[idx];
		byteBlock[1 + endianDiff2] = bytes[idx + 1];
		byteBlock[2 - endianDiff2] = bytes[idx + 2];
		byteBlock[3 - endianDiff1] = bytes[idx + 3];
		idx += 4;
	}
	
	#endregion
	
	public void SaveToXML()
	{
		string path = Path.Combine(Application.dataPath,fileName);

		XmlSerializer serializer = new XmlSerializer(typeof(PlayerPrefs_AM));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}

	void AddToDictionnary<T>(SerializableDictionary<string, T> dic, string key, T value)
	{
		if(dic.ContainsKey(key))
		{
			dic.Remove(key);
		}
		dic.Add(key, value);
	}
}
