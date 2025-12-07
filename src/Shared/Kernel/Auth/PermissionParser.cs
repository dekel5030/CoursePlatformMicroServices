using Kernel.Auth.AuthTypes;

namespace Kernel.Auth;

public static class PermissionParser
{
    public static bool TryParseEffect(
        string effectSegment,
        out EffectType result)
    {
        return TryParseEnum(effectSegment, out result);
    }
    public static bool TryParseAction(
        string actionSegment,
        out ActionType result)
    {
        if (actionSegment == "*")
        {
            result = ActionType.Wildcard;
            return true;
        }

        return TryParseEnum(actionSegment, out result);
    }

    public static bool TryParseResource(
        string resourceSegment,
        out ResourceType result)
    {
        if (resourceSegment == "*")
        {
            result = ResourceType.Wildcard;
            return true;
        }

        return TryParseEnum(resourceSegment, out result);
    }

    private static bool TryParseEnum<TEnum>(string value, out TEnum result)
        where TEnum : struct, Enum
    {
        return Enum.TryParse(value, ignoreCase: true, out result);
    }
}