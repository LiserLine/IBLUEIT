﻿public class Parsers
{
    public static float Float(string value) => float.Parse(value.Replace('.', ','));
}