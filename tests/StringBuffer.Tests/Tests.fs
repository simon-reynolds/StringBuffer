namespace StringBuffer.Tests

open System
open Expecto

module Tests =

    [<Tests>]
    let indentedStringBuilderTests =
        testList "IndentedStringBuilder Tests" [
            testCase "Test IncrementIndent and DecrementIndent"
            <| fun _ ->
                let subject =
                    IndentedStringBuilder()
                        .AppendLine("Line 1")
                        .IncrementIndent()
                        .AppendLine("Line 2")
                        .DecrementIndent()
                        .Append("Line 3")
                        .ToString()

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "    Line 2"
                    + Environment.NewLine
                    + "Line 3"

                Expect.equal subject expected "Works as expected"

            testCase "Test Properties"
            <| fun _ ->
                let subject = IndentedStringBuilder()

                Expect.equal subject.Length 0 "Initial Length is zero"
                Expect.equal subject.IndentCount 0 "Initial IndentCount is zero"

                subject.Append 'c'
                |> ignore

                Expect.equal subject.Length 1 "Initial Length is one"

                subject.Clear()
                |> ignore

                Expect.equal subject.Length 0 "Initial Length is zero"

            testCase "Indenter Disposable works as expected"
            <| fun _ ->
                let subject = IndentedStringBuilder().AppendLine("Line 1")

                use indent = subject.Indent()

                subject.AppendLine("Line 2")
                |> ignore

                indent.Dispose()

                subject.Append("Line 3")
                |> ignore

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "    Line 2"
                    + Environment.NewLine
                    + "Line 3"

                Expect.equal (subject.ToString()) expected "Works as expected"

            testCase "Append - FormattableString works as expected"
            <| fun _ ->

                let s: FormattableString = $"Test"

                let subect = IndentedStringBuilder().Append(s).ToString()

                let expected = "Test"

                Expect.equal subect expected "Works as expected"

            testCase "AppendLine - FormattableString works as expected"
            <| fun _ ->

                let s: FormattableString = $"Test"

                let subect = IndentedStringBuilder().AppendLine(s).ToString()

                let expected =
                    "Test"
                    + Environment.NewLine

                Expect.equal subect expected "Works as expected"

            testCase "Append - String seq works as expected"
            <| fun _ ->

                let s =
                    seq {
                        "Line 1"
                        "Line 2"
                    }

                let subect = IndentedStringBuilder().Append(s).ToString()

                let expected = "Line 1Line 2"

                Expect.equal subect expected "Works as expected"

            testCase "Append - Char seq works as expected"
            <| fun _ ->

                let s =
                    seq {
                        'a'
                        'b'
                    }

                let subect = IndentedStringBuilder().Append(s).ToString()

                let expected = "ab"

                Expect.equal subect expected "Works as expected"

            testCase "AppendLines - String seq works as expected"
            <| fun _ ->

                let s =
                    seq {
                        "Line 1"
                        "Line 2"
                    }

                let subect = IndentedStringBuilder().AppendLines(s).ToString()

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine

                Expect.equal subect expected "Works as expected"

            testCase "AppendLines - Char seq works as expected"
            <| fun _ ->

                let s =
                    seq {
                        'a'
                        'b'
                    }

                let subect = IndentedStringBuilder().AppendLines(s).ToString()

                let expected =
                    "a"
                    + Environment.NewLine
                    + "b"
                    + Environment.NewLine

                Expect.equal subect expected "Works as expected"

            testCase "AppendLines - skipFinalNewLine works as expected"
            <| fun _ ->

                let initial =
                    "a"
                    + Environment.NewLine
                    + "b"
                    + Environment.NewLine
                    + ""
                    + Environment.NewLine
                    + "c"


                let subect = IndentedStringBuilder().AppendLines(initial, false).ToString()

                let expected =
                    "a"
                    + Environment.NewLine
                    + "b"
                    + Environment.NewLine
                    + ""
                    + Environment.NewLine
                    + "c"
                    + Environment.NewLine

                Expect.equal subect expected "Works as expected"

                let subect = IndentedStringBuilder().AppendLines(initial, true).ToString()

                let expected =
                    "a"
                    + Environment.NewLine
                    + "b"
                    + Environment.NewLine
                    + ""
                    + Environment.NewLine
                    + "c"

                Expect.equal subect expected "Works as expected"

            testCase "AppendJoin - String seq works as expected"
            <| fun _ ->
                let values =
                    seq {
                        "Line 1"
                        "Line 2"
                    }

                let defaultSeparator = IndentedStringBuilder().AppendJoin(values).ToString()

                Expect.equal defaultSeparator "Line 1, Line 2" "Works as expected"

                let customSeparator = IndentedStringBuilder().AppendJoin(values, "|").ToString()

                Expect.equal customSeparator "Line 1|Line 2" "Works as expected"

            testCase "AppendJoin - ParamArray works as expected"
            <| fun _ ->
                let values = [|
                    "Line 1"
                    "Line 2"
                |]

                let asArray = IndentedStringBuilder().AppendJoin("|", values).ToString()

                Expect.equal asArray "Line 1|Line 2" "Works as expected"

                let asParamArray =
                    IndentedStringBuilder().AppendJoin("|", "Line 1", "Line 2").ToString()

                Expect.equal asParamArray "Line 1|Line 2" "Works as expected"

        ]

    [<Tests>]
    let stringBufferTests =
        testList "StringBuffer Tests" [
            testCase "Two lines"
            <| fun _ ->
                let subject =
                    stringBuffer {
                        "Line 1"
                        "Line 2"
                    }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"
            testCase "Three lines, last one indented"
            <| fun _ ->
                let subject =
                    stringBuffer {
                        "Line 1"
                        "Line 2"
                        indent { "Line 3" }
                    }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine
                    + "    Line 3"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"
            testCase "Three lines, middle one indented"
            <| fun _ ->
                let subject =
                    stringBuffer {
                        "Line 1"
                        indent { "Line 2" }
                        "Line 3"
                    }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "    Line 2"
                    + Environment.NewLine
                    + "Line 3"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "Different types work as expected"
            <| fun _ ->

                let string = "String"

                let stringSeq =
                    seq {
                        "Seq1"
                        "Seq2"
                    }

                let char = 'c'

                let charSeq =
                    seq {
                        'd'
                        'e'
                    }

                let subject =
                    stringBuffer {
                        string
                        stringSeq
                        char
                        charSeq
                    }

                let expected =
                    "String"
                    + Environment.NewLine
                    + "Seq1"
                    + Environment.NewLine
                    + "Seq2"
                    + Environment.NewLine
                    + "c"
                    + Environment.NewLine
                    + "d"
                    + Environment.NewLine
                    + "e"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "Writes namespaces as expected"
            <| fun _ ->
                let namespaces = [
                    "Some.Namespace"
                    "Another.Namespace"
                ]

                let subject = stringBuffer { writeNamespaces namespaces }

                let expected =
                    "open Some.Namespace"
                    + Environment.NewLine
                    + "open Another.Namespace"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "String seq works as expected"
            <| fun _ ->
                let strings =
                    seq {
                        "Line 1"
                        "Line 2"
                    }

                let subject = stringBuffer { strings }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "char seq works as expected"
            <| fun _ ->
                let chars =
                    seq {
                        'a'
                        'b'
                    }

                let subject = stringBuffer { chars }

                let expected =
                    "a"
                    + Environment.NewLine
                    + "b"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "String option works as expected"
            <| fun _ ->

                let subject =
                    stringBuffer {
                        Some "Line 1"
                        None
                        Some "Line 2"
                    }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "String option seq works as expected"
            <| fun _ ->

                let s =
                    seq {
                        Some "Line 1"
                        None
                        Some "Line 2"
                    }

                let subject = stringBuffer { s }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "For works as expected"
            <| fun _ ->
                let s = seq { 1..3 }

                let subject =
                    stringBuffer {
                        for i in s do
                            sprintf "Line %d" i
                    }

                let expected =
                    "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine
                    + "Line 3"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "While works as expected"
            <| fun _ ->
                let s = seq { 1..3 }

                let subject =
                    stringBuffer {
                        let mutable i = 0

                        while i < 3 do
                            sprintf "Line %d" i
                            i <- i + 1
                    }

                let expected =
                    "Line 0"
                    + Environment.NewLine
                    + "Line 1"
                    + Environment.NewLine
                    + "Line 2"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "YieldFrom works as expected"
            <| fun _ ->
                let s =
                    fun (sb: IndentedStringBuilder) ->
                        sb.AppendLine "1"
                        |> ignore

                        sb.AppendLine "2"
                        |> ignore

                        sb.AppendLine "3"
                        |> ignore

                let subject = stringBuffer { yield! s }

                let expected =
                    "1"
                    + Environment.NewLine
                    + "2"
                    + Environment.NewLine
                    + "3"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

            testCase "Using Seq.map works as expected"
            <| fun _ ->

                let s =
                    seq {
                        "Line 1"
                        "Line 2"
                    }

                let subject =
                    stringBuffer {
                        s
                        |> Seq.map (fun s ->
                            s
                            + " mapped"
                        )
                    }

                let expected =
                    "Line 1 mapped"
                    + Environment.NewLine
                    + "Line 2 mapped"
                    + Environment.NewLine

                Expect.equal subject expected "Works as expected"

        ]
