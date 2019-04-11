/*
MIT License

Copyright (c) 2019 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Text.RegularExpressions;

namespace JCMG.SemVer
{
	/// <summary>
	/// Internal constants for the SemVer library
	/// </summary>
	internal static class RuntimeConstants
	{
		// Version fields
		public const string SemVersionClassName = "SemVersion";
		public const string MajorVersionField = "major";
		public const string MinorVersionField = "minor";
		public const string PatchVersionField = "patch";
		public const string PreReleaseVersionField = "pre";
		public const string BuildVersionField = "build";

		public const char VersionFieldDelimiter = '.';
		public const char PreReleasePrefix = '-';
		public const char BuildPrefix = '+';

		public const string SimpleVersionFormat = "{0}.{1}.{2}";

		// Logging
		public const string VersionParseErrorFormat =
			"[SemVer] An unexpected error occured when parsing the version string.\n\n{0}";

		// Exceptions
		public const string InvalidVersionString = "Invalid version.";
		public const string InvalidVersionNoMinorValue = "Invalid version (no minor version given in strict mode)";
		public const string InvalidVersionNoPatchValue = "Invalid version (no patch version given in strict mode)";

		// Regex
		public static readonly Regex ParseRegEx = new Regex(
			RegexExpression,
			RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

		private const string RegexExpression =
			@"^(?<major>\d+)" +
			@"(\.(?<minor>\d+))?" +
			@"(\.(?<patch>\d+))?" +
			@"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
			@"(\+(?<build>[0-9A-Za-z\-\.]+))?$";
	}
}
