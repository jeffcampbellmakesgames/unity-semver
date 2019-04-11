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
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace JCMG.SemVer
{
	/// <summary>
	/// <see cref="SemVersion"/> represents a semantic version that conforms to the 2.0.0 SemVer standard
	/// described here (https://semver.org/spec/v2.0.0.html).
	/// </summary>
	[Serializable]
	public sealed class SemVersion :
		IComparable<SemVersion>,
		IComparable,
		ISerializable
	{
		/// <summary>
		/// Gets the major version.
		/// </summary>
		public int Major
		{
			get { return _major; }
			set { _major = value; }
		}

		/// <summary>
		/// Gets the minor version.
		/// </summary>
		public int Minor
		{
			get { return _minor; }
			set { _minor = value; }
		}

		/// <summary>
		/// Gets the patch version.
		/// </summary>
		/// <value>
		/// The patch version.
		/// </value>
		public int Patch
		{
			get { return _patch; }
			set { _patch = value; }
		}

		/// <summary>
		/// Gets the pre-release version.
		/// </summary>
		public string Prerelease
		{
			get { return _prerelease; }
			set { _prerelease = value; }
		}

		/// <summary>
		/// Gets the build version.
		/// </summary>
		public string Build
		{
			get { return _build; }
			set { _build = value; }
		}

		[SerializeField]
		private int _major;

		[SerializeField]
		private int _minor;

		[SerializeField]
		private int _patch;

		[SerializeField]
		private string _prerelease;

		[SerializeField]
		private string _build;

		private static readonly StringBuilder _versionStringBuilder;

		static SemVersion()
		{
			_versionStringBuilder = new StringBuilder();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SemVersion" /> class.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <exception cref="ArgumentException"></exception>
		private SemVersion(SerializationInfo info, StreamingContext context)
		{
			Assert.IsNotNull(info);

			var semVersion = Parse(info.GetString(RuntimeConstants.SemVersionClassName));
			_major = semVersion.Major;
			_minor = semVersion.Minor;
			_patch = semVersion.Patch;
			_prerelease = semVersion.Prerelease;
			_build = semVersion.Build;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SemVersion" /> class.
		/// </summary>
		public SemVersion()
		{
			_major = 0;
			_minor = 0;
			_patch = 0;

			_prerelease = string.Empty;
			_build = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SemVersion" /> class.
		/// </summary>
		/// <param name="major">The major version.</param>
		/// <param name="minor">The minor version.</param>
		/// <param name="patch">The patch version.</param>
		/// <param name="prerelease">The prerelease version (eg. "alpha").</param>
		/// <param name="build">The build eg ("nightly.232").</param>
		public SemVersion(
			int major,
			int minor = 0,
			int patch = 0,
			string prerelease = null,
			string build = null)
		{
			_major = major;
			_minor = minor;
			_patch = patch;

			_prerelease = string.IsNullOrEmpty(prerelease) ? string.Empty : prerelease;
			_build = string.IsNullOrEmpty(build) ? string.Empty : build;
		}

		/// <summary>
		/// Copy-constructor that performs a deep-copy of <see cref="Version"/>  <paramref name="version"/>.
		/// </summary>
		/// <param name="version">The <see cref="Version"/> that is used to initialize
		/// the Major, Minor, Patch and Build properties.</param>
		public SemVersion(Version version)
		{
			Assert.IsNotNull(version);

			_major = version.Major;
			_minor = version.Minor;

			if (version.Revision >= 0)
			{
				_patch = version.Revision;
			}

			_prerelease = string.Empty;

			_build = version.Build > 0 ? version.Build.ToString() : string.Empty;
		}

		/// <summary>
		/// Parses the specified string to a semantic version.
		/// </summary>
		/// <param name="version">The version string.</param>
		/// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
		/// <returns>The SemVersion object.</returns>
		/// <exception cref="ArgumentException">When a invalid version string is passed.</exception>
		/// <exception cref="InvalidOperationException">When a invalid version string is passed.</exception>
		public static SemVersion Parse(string version, bool strict = false)
		{
			var match = RuntimeConstants.ParseRegEx.Match(version);
			if (!match.Success)
			{
				throw new ArgumentException(RuntimeConstants.InvalidVersionString);
			}

			var major = int.Parse(match.Groups[RuntimeConstants.MajorVersionField].Value, CultureInfo.InvariantCulture);

			var minorMatch = match.Groups[RuntimeConstants.MinorVersionField];
			var minor = 0;
			if (minorMatch.Success)
			{
				minor = int.Parse(minorMatch.Value, CultureInfo.InvariantCulture);
			}
			else if (strict)
			{
				throw new InvalidOperationException(RuntimeConstants.InvalidVersionNoMinorValue);
			}

			var patchMatch = match.Groups[RuntimeConstants.PatchVersionField];
			var patch = 0;
			if (patchMatch.Success)
			{
				patch = int.Parse(patchMatch.Value, CultureInfo.InvariantCulture);
			}
			else if (strict)
			{
				throw new InvalidOperationException(RuntimeConstants.InvalidVersionNoPatchValue);
			}

			var prerelease = match.Groups[RuntimeConstants.PreReleaseVersionField].Value;
			var build = match.Groups[RuntimeConstants.BuildVersionField].Value;

			return new SemVersion(major, minor, patch, prerelease, build);
		}

		/// <summary>
		/// Parses the specified string to a semantic version.
		/// </summary>
		/// <param name="version">The version string.</param>
		/// <param name="semver">When the method returns, contains a SemVersion instance equivalent
		/// to the version string passed in, if the version string was valid, or <c>null</c> if the
		/// version string was not valid.</param>
		/// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
		/// <returns><c>False</c> when a invalid version string is passed, otherwise <c>true</c>.</returns>
		public static bool TryParse(string version, out SemVersion semver, bool strict = false)
		{
			try
			{
				semver = Parse(version, strict);
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat(RuntimeConstants.VersionParseErrorFormat, ex);
				semver = null;
				return false;
			}
		}

		/// <summary>
		/// Tests the specified versions for equality.
		/// </summary>
		/// <param name="versionA">The first version.</param>
		/// <param name="versionB">The second version.</param>
		/// <returns>If versionA is equal to versionB <c>true</c>, else <c>false</c>.</returns>
		public static bool Equals(SemVersion versionA, SemVersion versionB)
		{
			return ReferenceEquals(versionA, null)
				? ReferenceEquals(versionB, null)
				: versionA.Equals(versionB);
		}

		/// <summary>
		/// Compares the specified versions.
		/// </summary>
		/// <param name="versionA">The version to compare to.</param>
		/// <param name="versionB">The version to compare against.</param>
		/// <returns>If versionA &lt; versionB <c>&lt; 0</c>, if versionA &gt; versionB <c>&gt; 0</c>,
		/// if versionA is equal to versionB <c>0</c>.</returns>
		public static int Compare(SemVersion versionA, SemVersion versionB)
		{
			if (ReferenceEquals(versionA, null))
			{
				return ReferenceEquals(versionB, null) ? 0 : -1;
			}

			return versionA.CompareTo(versionB);
		}

		/// <summary>
		/// Make a copy of the current instance with optional altered fields.
		/// </summary>
		/// <param name="major">The major version.</param>
		/// <param name="minor">The minor version.</param>
		/// <param name="patch">The patch version.</param>
		/// <param name="prerelease">The prerelease text.</param>
		/// <param name="build">The build text.</param>
		/// <returns>The new version object.</returns>
		public SemVersion Change(
			int? major = null,
			int? minor = null,
			int? patch = null,
			string prerelease = null,
			string build = null)
		{
			return new SemVersion(
				major ?? Major,
				minor ?? Minor,
				patch ?? Patch,
				prerelease ?? Prerelease,
				build ?? Build);
		}

		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		public override string ToString()
		{
			_versionStringBuilder.Length = 0;
			_versionStringBuilder.Append(string.Format(
				RuntimeConstants.SimpleVersionFormat,
				_major,
				_minor,
				_patch));

			if (!string.IsNullOrEmpty(_prerelease))
			{
				_versionStringBuilder.Append(RuntimeConstants.PreReleasePrefix);
				_versionStringBuilder.Append(_prerelease);
			}

			if (!string.IsNullOrEmpty(_build))
			{
				_versionStringBuilder.Append(RuntimeConstants.BuildPrefix);
				_versionStringBuilder.Append(_build);
			}

			return _versionStringBuilder.ToString();
		}

		/// <summary>
		/// Determines whether the specified <see cref="object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		public override bool Equals(object obj)
		{
			if(ReferenceEquals(this, obj))
			{
				return true;
			}

			var otherVersion = obj as SemVersion;
			if (ReferenceEquals(otherVersion, null))
			{
				return false;
			}

			return _major == otherVersion.Major &&
			       _minor == otherVersion.Minor &&
			       _patch == otherVersion.Patch &&
			       string.Equals(_prerelease, otherVersion.Prerelease, StringComparison.Ordinal) &&
			       string.Equals(_build, otherVersion.Build, StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var result = _major.GetHashCode();
				result = result * 31 + _minor.GetHashCode();
				result = result * 31 + _patch.GetHashCode();
				result = result * 31 + _prerelease.GetHashCode();
				result = result * 31 + _build.GetHashCode();
				return result;
			}
		}

		#region IComparable

		/// <summary>
		/// Compares the current instance with another object of the same type and returns an integer that indicates
		/// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
		/// other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared.
		/// The return value has these meanings: Value Meaning Less than zero
		///  This instance precedes <paramref name="obj" /> in the sort order.
		///  Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. i
		///  Greater than zero This instance follows <paramref name="obj" /> in the sort order.
		/// </returns>
		public int CompareTo(object obj)
		{
			return CompareTo((SemVersion)obj);
		}

		#endregion

		#region IComparable<SemVersion>

		/// <summary>
		/// Compares the current instance with another object of the same type and returns an integer that indicates
		/// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
		/// other object.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared.
		/// The return value has these meanings: Value Meaning Less than zero
		///  This instance precedes <paramref name="other" /> in the sort order.
		///  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. i
		///  Greater than zero This instance follows <paramref name="other" /> in the sort order.
		/// </returns>
		public int CompareTo(SemVersion other)
		{
			if (ReferenceEquals(other, null))
			{
				return 1;
			}

			var r = CompareByPrecedence(other);
			if (r != 0)
			{
				return r;
			}

			r = CompareComponent(Build, other.Build);
			return r;
		}

		/// <summary>
		/// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
		/// </summary>
		/// <param name="other">The semantic version.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared.
		/// The return value has these meanings: Value Meaning Less than zero
		///  This instance precedes <paramref name="other" /> in the version precedence.
		///  Zero This instance has the same precedence as <paramref name="other" />. i
		///  Greater than zero This instance has creator precedence as <paramref name="other" />.
		/// </returns>
		private int CompareByPrecedence(SemVersion other)
		{
			if (ReferenceEquals(other, null))
			{
				return 1;
			}

			var r = _major.CompareTo(other.Major);
			if (r != 0)
			{
				return r;
			}

			r = _minor.CompareTo(other.Minor);
			if (r != 0)
			{
				return r;
			}

			r = _patch.CompareTo(other.Patch);
			if (r != 0)
			{
				return r;
			}

			r = CompareComponent(_prerelease, other.Prerelease, true);
			return r;
		}

		private static int CompareComponent(string a, string b, bool lower = false)
		{
			var aEmpty = string.IsNullOrEmpty(a);
			var bEmpty = string.IsNullOrEmpty(b);
			if (aEmpty && bEmpty)
			{
				return 0;
			}

			if (aEmpty)
			{
				return lower ? 1 : -1;
			}

			if (bEmpty)
			{
				return lower ? -1 : 1;
			}

			var aComps = a.Split(RuntimeConstants.VersionFieldDelimiter);
			var bComps = b.Split(RuntimeConstants.VersionFieldDelimiter);
			var minLen = Math.Min(aComps.Length, bComps.Length);
			for (var i = 0; i < minLen; i++)
			{
				var ac = aComps[i];
				var bc = bComps[i];
				int anum, bnum;
				var isanum = int.TryParse(ac, out anum);
				var isbnum = int.TryParse(bc, out bnum);
				int r;
				if (isanum && isbnum)
				{
					r = anum.CompareTo(bnum);
					if (r != 0)
					{
						return anum.CompareTo(bnum);
					}
				}
				else
				{
					if (isanum)
					{
						return -1;
					}

					if (isbnum)
					{
						return 1;
					}

					r = string.CompareOrdinal(ac, bc);
					if (r != 0)
					{
						return r;
					}
				}
			}

			return aComps.Length.CompareTo(bComps.Length);
		}

		#endregion

		#region ISerializable

		/// <summary>
		/// Adds the serialization info for this instance to <see cref="SerializationInfo"/>
		/// <paramref name="info"/>.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <exception cref="ArgumentNullException">When <see cref="SerializationInfo"/> <paramref name="info"/>
		/// is null.</exception>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Assert.IsNotNull(info);

			info.AddValue(RuntimeConstants.SemVersionClassName, ToString());
		}

		#endregion

		#region Operators

		/// <summary>
		/// Implicit conversion from string to SemVersion.
		/// </summary>
		/// <param name="version">The semantic version.</param>
		public static implicit operator SemVersion(string version)
		{
			return Parse(version);
		}

		/// <summary>
		/// The override of the equals operator.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public static bool operator ==(SemVersion left, SemVersion right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// The override of the un-equal operator.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public static bool operator !=(SemVersion left, SemVersion right)
		{
			return !Equals(left, right);
		}

		/// <summary>
		/// The override of the greater operator.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public static bool operator >(SemVersion left, SemVersion right)
		{
			return Compare(left, right) > 0;
		}

		/// <summary>
		/// The override of the greater than or equal operator.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public static bool operator >=(SemVersion left, SemVersion right)
		{
			return left == right || left > right;
		}

		/// <summary>
		/// The override of the less operator.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public static bool operator <(SemVersion left, SemVersion right)
		{
			return Compare(left, right) < 0;
		}

		/// <summary>
		/// The override of the less than or equal operator.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public static bool operator <=(SemVersion left, SemVersion right)
		{
			return left == right || left < right;
		}

		#endregion
	}
}
