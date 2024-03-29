# StringBuffer

An F# computation expression for writing code with code

---

## Builds

GitHub Actions |
:---: |
[![GitHub Actions](https://github.com/simon-reynolds/StringBuffer/actions/workflows/build.yml/badge.svg)](https://github.com/simon-reynolds/StringBuffer/actions/workflows/build.yml)
[![Build History](https://buildstats.info/github/chart/simon-reynolds/StringBuffer)](https://github.com/simon-reynolds/StringBuffer/actions?query=branch%3Amain) |

## NuGet

Package | Stable | Prerelease
--- | --- | ---
StringBuffer | [![NuGet Badge](https://buildstats.info/nuget/StringBuffer)](https://www.nuget.org/packages/StringBuffer/) | [![NuGet Badge](https://buildstats.info/nuget/StringBuffer?includePreReleases=true)](https://www.nuget.org/packages/StringBuffer/)

---

### Using

Install the latest version from [https://www.nuget.org/packages/StringBuffer/](https://www.nuget.org/packages/StringBuffer/)

Then use `stringBuffer` and `indent` to write your code that writes code.

```fsharp
stringBuffer {
    "let square x ="
    indent {
        "x * x"
    }
}
```

`stringBuffer` and `indent` can both be nested in each other and will correctly produce output indented as expected.

```fsharp
let namespaces = seq {
    "System"
    "System.IO"
}

let formattedCode = stringBuffer {
    namespaces |> Seq.map (fun ns -> "open " + ns)

    ""
    "module MyModule"
    indent {
        "let LifeUniverseAndEverything () ="
        indent {
            "42"
        }
    }
}
```

```fsharp
open System
open System.IO

module MyModule
    let LifeUniverseAndEverything () =
        42
```

### Developing

Make sure the following **requirements** are installed on your system:

- [dotnet SDK](https://www.microsoft.com/net/download/core) 6.0 or higher

or

- [VSCode Dev Container](https://code.visualstudio.com/docs/remote/containers)

