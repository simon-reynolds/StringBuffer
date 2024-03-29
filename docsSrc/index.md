# StringBuffer

---

## What is StringBuffer?

StringBuffer is an F# computation expression for being able to easily write formatted text.

It is built with the goal of being able to easily write code within an F# program.

## Why use StringBuffer?

As part of working on other projects I found myself needing to generate F# code within a program.

This was particularly problematic considering F#, like many languages, is whitespace sensitive, using indentation to declare scopes.

I found myself needing a way to create code with correct indentation while still being able to break apart large chunks of generated code into smaller sections that could be built separately and then put together to create the final file.

---

# Installing StringBuffer

Simply add it from nuget, paket, however you source your packages. The module automatically opens and will be available throughout your project.

Find the latest version at

The module provides two computation expressions:

* `stringBuffer`
    * This is the one you want to start off with
* `indent` 
    * Have a part of a `stringBuffer` you want to indent? Use this

# Samples

## Basic build

```fsharp
let result = stringBuffer {
    "My first line"
    "My second line"
    sprintf "This line has a number in it: %i" 3
}

```

Output

```
My first line
My second line
This line has a number in it: 3
```

## Using variables and loops
```fsharp
let firstLine = "Line 1"
let moreLines = [2 .. 5]

let result = stringBuffer {
    firstLine
    moreLines |> Seq.map(fun i -> sprintf "Line %d" i)
}
```
Output
```
Line 1
Line 2
Line 3
Line 4
Line 5
```

## Use multiple lines, even other instances of stringBuffer
```fsharp
let firstTwoLines = seq {
    "Line 1"
    "Line 2"
}

let thirdLine = "Line 3"

let result = stringBuffer {
    firstTwoLines
    thirdLine
    "Line 4"

    stringBuffer {
        "Line 5"
    }

}
```

Output
```
Line 1
Line 2
Line 3
Line 4
Line 5
```

## Indenting
```fsharp
let result = stringBuffer {
    "let square x ="
    indent {
        "x * x"
    }
}
```

Output
```
let square x =
    x * x
```
