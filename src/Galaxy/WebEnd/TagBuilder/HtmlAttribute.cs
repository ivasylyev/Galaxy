﻿using System.IO;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public abstract class HtmlAttribute
    {
        public const string Class = "class";
        public const string For = "for";
        public const string Type = "type";

        public readonly string Name;

        public HtmlAttribute(string name)
        {
            Name = name;
        }

        public static readonly HtmlAttribute InputNumber = new SimpleAttribute<string>(Type, "number");

        public static readonly HtmlAttribute InputText = new SimpleAttribute<string>(Type, "text");

        public static readonly HtmlAttribute InputPassword = new SimpleAttribute<string>(Type, "password");

        public static readonly HtmlAttribute InputCheckbox = new SimpleAttribute<string>(Type, "checkbox");

        public static readonly HtmlAttribute Checked = new SimpleAttribute<string>("checked", "checked");

        public void WriteTo(TextWriter writer)
        {
            writer.Write(Name);
            writer.Write('=');
            writer.Write('"');

            WriteValue(writer);

            writer.Write('"');
        }

        protected abstract void WriteValue(TextWriter writer);

        public static HtmlAttribute CreateName<TValue>(TValue value)
        {
            return new SimpleAttribute<TValue>("name", value);
        }

        public static HtmlAttribute CreateId<TValue>(TValue value)
        {
            return new SimpleAttribute<TValue>("id", value);
        }

        public static HtmlAttribute CreatePlaceholder<TValue>(TValue value)
        {
            return new SimpleAttribute<TValue>("placeholder", value);
        }

        public static HtmlAttribute CreateValue<TValue>(TValue value)
        {
            return new SimpleAttribute<TValue>("value", value);
        }

        public static HtmlAttribute CreateRows<TValue>(TValue value)
        {
            return new SimpleAttribute<TValue>("rows", value);
        }
    }
}