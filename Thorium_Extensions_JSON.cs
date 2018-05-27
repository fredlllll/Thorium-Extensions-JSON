using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Thorium.Reflection;

public static class Thorium_Extensions_JSON
{
    public static T Get<T>(this JObject jobj, string key, T def = default(T))
    {
        var token = jobj[key];
        if(token != null && !token.IsNull())
        {
            return token.Value<T>();
        }
        return def;
    }

    /// <summary>
    /// non generic version of https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/Extensions.cs#L255
    /// also renamed it to Value as it makes more sense
    /// 
    /// only works for primitive types
    /// </summary>
    /// <param name="token"></param>
    /// <param name="outputType"></param>
    /// <returns></returns>
    public static object Value(this JToken token, Type outputType)
    {
        if(token == null)
        {
            return ReflectionHelper.GetDefault(outputType);
        }
        if(outputType.IsAssignableFrom(token.GetType())
            // don't want to cast JValue to its interfaces, want to get the internal value
            && outputType != typeof(IComparable) && outputType != typeof(IFormattable))
        {
            // HACK
            return token;
        }
        JValue jValue = token as JValue;
        if(jValue == null)
        {
            throw new InvalidCastException(String.Format(CultureInfo.InvariantCulture, "Cannot cast {0} to {1}.", token.GetType(), outputType));
        }
        if(outputType.IsAssignableFrom(jValue.Value.GetType()))
        {
            return jValue.Value;
        }
        if(ReflectionHelper.IsNullableType(outputType))
        {
            if(jValue.Value == null)
            {
                return ReflectionHelper.GetDefault(outputType);
            }
            outputType = Nullable.GetUnderlyingType(outputType);
        }
        return Convert.ChangeType(jValue.Value, outputType, CultureInfo.InvariantCulture);
    }

    public static object Get(this JObject jobj, Type t, string key, object def = null)
    {
        var token = jobj[key];

        if(token != null && !token.IsNull())
        {
            return token.Value(t);
        }
        return def;
    }

    public static bool HasValue(this JObject jobj, string key)
    {
        var token = jobj[key];
        return token != null;
    }

    public static bool IsNull(this JToken token)
    {
        return token.Type == JTokenType.Null;
    }

    public static bool IsNullOrEmpty(this JToken token)
    {
        return (token.Type == JTokenType.Array && !token.HasValues) ||
               (token.Type == JTokenType.Object && !token.HasValues) ||
               (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
               (token.Type == JTokenType.Null);
    }
}
