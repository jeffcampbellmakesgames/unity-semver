# JCMG.SemVer
## Overview
JCMG.SemVer adds a 2.0.0 SemVer implementation (see [here](https://semver.org/spec/v2.0.0.html) for more information) via the `SemVersion` class. `SemVersion` is capable of Unity serialization as well as supporting custom serialization. It has a custom PropertyDrawer to make it easier to view and modify in the inspector; this includes Regex validation of some fields (`PreRelease` and `Build`).

![SemVersion in Unity Inspector](/Documentation/Inspector.png)

## Using JCMG.SemVer
Using this library in your project can be done in two ways:
* **Releases:** The latest release can be found [here]() as a UnityPackage file that can be downloaded and imported directly into your project's Assets folder.
* **Package:** Using the native Unity Package Manager introduced in 2017.2, you can add this library as a package by modifying your `manifest.json` file found at `/ProjectName/Packages/manifest.json` to include it as a dependency. See the example below on how to reference it.

```
{
	"dependencies": {
		...
		"JCMG.SemVer" : "https://github.com/jeffcampbellmakesgames/unity-semver.git#release-stable"
		...
	}
}
```

## License
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