using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gamesense.Overwatch.Models
{
	[DataContract]
	public class RGBA
	{
		[DataMember(Name = "red")]
		public int Red;
		[DataMember(Name = "green")]
		public int Green;
		[DataMember(Name = "blue")]
		public int Blue;
		[IgnoreDataMember]
		public int Alpha;

		public static RGBA FromUint(uint c)
		{
			var b = BitConverter.GetBytes(c);
			return new RGBA { Red = b[0], Green = b[1], Blue = b[2], Alpha = b[3] };
		}

		public static List<RGBA> FromUintArray(uint[] c)
		{
			var colors = new List<RGBA>();
			for (int i = 0; i < c.Length; i++)
			{
				colors.Add(FromUint(c[i]));
			}

			return colors;
		}

		public override string ToString() { return $"R: {Red}, G: {Green}, B: {Blue}, A: {Alpha}"; }
	}

	[DataContract]
	public class RGB
	{
		[DataMember(Name = "red")]
		public int Red;
		[DataMember(Name = "green")]
		public int Green;
		[DataMember(Name = "blue")]
		public int Blue;

		public RGB() : this(0, 0, 0) { }
		public RGB(int r, int g, int b)
		{
			Red = r;
			Green = g;
			Blue = g;
		}

		public static RGB FromRGBA(RGBA c)
		{
			return new RGB(c.Red, c.Green, c.Blue);
		}

		public override string ToString() { return $"R: {Red}, G: {Green}, B: {Blue}"; }
	}
}
