using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Divine.SDK.Localization
{
    public static class LocalizationHelper
    {
        private static string ToEnumString<T>(this T type)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            return enumMemberAttribute.Value;
        }

        public static string Localize(Loc localization, params object[] objects)
        {
            var text = localization.ToEnumString();

            var match = Regex.Matches(text, @"(?<!\{)\{([0-9]+).*?\}(?!})").Cast<Match>().ToList();
            if (match.Any())
            {
                var count = match.Max(m => int.Parse(m.Groups[1].Value)) + 1;
                if (count != objects.Length)
                {
                    throw new ArgumentException($"Given {objects.Length} arguments, but {count} arguments were expected");
                }

                if (count > 0)
                {
                    text = string.Format(text, objects);
                }
            }

            return GameManager.GetLocalize(text);
        }

        public static string LocalizeAbilityName(string name)
        {
            return Localize(Loc.DOTA_Tooltip_Ability_STRING, name);
        }

        public static string LocalizeName(AbilityId id)
        {
            return LocalizeAbilityName(id.ToString());
        }

        public static string LocalizeName(string name)
        {
            return GameManager.GetLocalize(name);
        }

        public static string LocalizeName(HeroId id)
        {
            return LocalizeName(id.ToString());
        }

        public static string LocalizeName(Ability ability)
        {
            return LocalizeAbilityName(ability.Name);
        }

        public static string LocalizeName(Entity entity)
        {
            return LocalizeName(entity.Name);
        }
    }
}