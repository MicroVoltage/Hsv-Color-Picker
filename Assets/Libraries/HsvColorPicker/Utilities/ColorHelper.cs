﻿using UnityEngine;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HsvColorPicker {
	public static class ColorHelper {
		const string ColorHexRegex = "^#?(?:[0-9a-fA-F]{3,4}){1,2}$";

		public static float Clump (float value, bool sliding = false) {
			if (sliding) return value < 0 ? value + 1 : (value >= 1 ? value - 1 : value);//value < 0 ? value - (int)(value - 1) : value - (int)value;
		else return value < 0 ? 0 : (value > 1 ? 1 : value);
		}

		public static Color Invert (Color color) {
			return new Color(1 - color.r, 1 - color.g, 1 - color.b);
		}

		public static HsvColor Invert (HsvColor color) {
			return new HsvColor(Clump(360 - color.h, true), color.s, color.v);
		}

		// http://www.rapidtables.com/convert/color/rgb-to-hsv.htm
		public static HsvColor Rgb2Hsv (float r, float b, float g) {
			float h = 0, s, cmin, cmax, delta;

			cmin = Mathf.Min(Mathf.Min(r, g), b);
			cmax = Mathf.Max(Mathf.Max(r, g), b);
			delta = cmax - cmin;

			if (delta == 0)
				h = 6;
			else if (cmax == r)
				h = (g - b) / delta;
			else if (cmax == g)
				h = (b - r) / delta + 2;
			else if (cmax == b)
				h = (r - g) / delta + 4;
			h = h * 60f / 360f;
			if (h <= 0) h += 1f;
			h = 1f - h;

			if (cmax == 0) s = 0;
			else s = delta / cmax;

			return new HsvColor(Clump(h, true), Clump(s), Clump(cmax));
		}

		// http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
		public static Color Hsv2Rgb (float h, float s, float v) {
			h *= 360;
			float r, g, b;

			var c = v * s;
			var x = c * (1 - Mathf.Abs((h / 60) % 2 - 1));
			var m = v - c;

			switch ((int)(h / 60)) {
			case 0: // 0 <= h < 60
				r = c;
				g = x;
				b = 0;
				break;
			case 1: // 60 <= h < 120
				r = x;
				g = c;
				b = 0;
				break;
			case 2: // 120 <= h < 180
				r = 0;
				g = c;
				b = x;
				break;
			case 3: // 180 <= h < 240
				r = 0;
				g = x;
				b = c;
				break;
			case 4: // 240 <= h < 300
				r = x;
				g = 0;
				b = c;
				break;
			case 5: // 300 <= h < 360
				r = c;
				g = 0;
				b = x;
				break;
			default:
				throw new ArithmeticException(h + " is not in the range of 0 <= h < 360");
			}

			r += m;
			g += m;
			b += m;

			return new Color(Clump(r), Clump(g), Clump(b), 1);
		}

		public static string Color2Hex (Color32 color, bool displayAlpha = false) {
			if (displayAlpha) return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
			else return string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
		}

		public static bool Hex2Color (string hex, out Color32 color) {
			// Check if this is a valid hex string (# is optional)
			if (Regex.IsMatch(hex, ColorHexRegex)) {
				int startIndex = hex.StartsWith("#") ? 1 : 0;

				if (hex.Length == startIndex + 8) { //#RRGGBBAA
					color = new Color32(byte.Parse(hex.Substring(startIndex, 2), NumberStyles.AllowHexSpecifier),
						byte.Parse(hex.Substring(startIndex + 2, 2), NumberStyles.AllowHexSpecifier),
						byte.Parse(hex.Substring(startIndex + 4, 2), NumberStyles.AllowHexSpecifier),
						byte.Parse(hex.Substring(startIndex + 6, 2), NumberStyles.AllowHexSpecifier));
				} else if (hex.Length == startIndex + 6) {  //#RRGGBB
					color = new Color32(byte.Parse(hex.Substring(startIndex, 2), NumberStyles.AllowHexSpecifier),
						byte.Parse(hex.Substring(startIndex + 2, 2), NumberStyles.AllowHexSpecifier),
						byte.Parse(hex.Substring(startIndex + 4, 2), NumberStyles.AllowHexSpecifier),
						255);
				} else if (hex.Length == startIndex + 4) { //#RGBA
					color = new Color32(byte.Parse("" + hex[startIndex] + hex[startIndex], NumberStyles.AllowHexSpecifier),
						byte.Parse("" + hex[startIndex + 1] + hex[startIndex + 1], NumberStyles.AllowHexSpecifier),
						byte.Parse("" + hex[startIndex + 2] + hex[startIndex + 2], NumberStyles.AllowHexSpecifier),
						byte.Parse("" + hex[startIndex + 3] + hex[startIndex + 3], NumberStyles.AllowHexSpecifier));
				} else {  //#RGB
					color = new Color32(byte.Parse("" + hex[startIndex] + hex[startIndex], NumberStyles.AllowHexSpecifier),
						byte.Parse("" + hex[startIndex + 1] + hex[startIndex + 1], NumberStyles.AllowHexSpecifier),
						byte.Parse("" + hex[startIndex + 2] + hex[startIndex + 2], NumberStyles.AllowHexSpecifier),
						255);
				}
				return true;
			} else {
				color = new Color32();
				return false;
			}
		}
	}
}