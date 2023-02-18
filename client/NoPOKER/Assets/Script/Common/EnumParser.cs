using System;
public class EnumParser<T>
{
	public static T toEnum(string str)
	{
		return (T)Enum.Parse(typeof(T), str);
	}

	public static string toString(T t) {
		return t.ToString();
	} 
}

