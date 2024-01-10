using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GG.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the description assigned to the enum value throguh system.componentmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value)
        {
            CheckIsEnum<T>(false);
            string name = Enum.GetName(typeof(T), value);
            if (name != null)
            {
                FieldInfo field = typeof(T).GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// Returns the description assigned to the enum value throguh system.componentmodel,
        /// Use this when passing a generic object though thats an enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionFromObject(this object o)
        {
            if (!o.GetType().IsEnum)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", o.GetType()));
            }
            string name = Enum.GetName(o.GetType(), o);
            if (name != null)
            {
                FieldInfo field = o.GetType().GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }
        
        public static IEnumerable<string> GetDescriptions<T>()
        {
            List<string> descs = new List<string>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                descs.Add(GetDescription(item));
            }
            return descs;
        }
        
        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                .SelectMany(f => f.GetCustomAttributes(
                    typeof(DescriptionAttribute), false), (
                    f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((DescriptionAttribute)a.Att)
                                                                              .Description == description);
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }

        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(true);
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }

        public static T SetFlags<T>(this T value, T flags, bool on) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }

            return (T) Enum.ToObject(typeof(T), lValue);
        }

        public static T JoinFlags<T>(this T value, T flags) where T : struct => value.SetFlags<T>(flags, true);

        public static T SetFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, true);
        }

        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, false);
        }

        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = 0;
            foreach (T flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }

            return (T) Enum.ToObject(typeof(T), lValue);
        }

        public static T CycleEnum<T>(this T enumerable) where T : struct, IConvertible
        {
            int enumLength = Enum.GetValues(typeof(T)).Length;

            int val = (int) (IConvertible) enumerable;
            val++;

            if (val == enumLength)
            {
                val = 0;
            }

            T returnVal = (T) (IConvertible) val;

            return returnVal;
        }
        
        public static List<string> AsList<T>() where T : struct, Enum
        {
            return Enum.GetNames(typeof(T)).ToList();
        }
    }
}
