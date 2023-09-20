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
                    """Line 1
    Line 2
Line 3"""

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
                    """Line 1
    Line 2
Line 3"""

                Expect.equal (subject.ToString()) expected "Works as expected"

        ]

    [<Tests>]
    let stringBufferTests =
        testList "StringBuffer Tests" [
            testCase "Two lines"
            <| fun _ ->
                let subject =
                    stringBuilder {
                        "Line 1"
                        "Line 2"
                    }

                let expected =
                    """Line 1
Line 2
"""

                Expect.equal subject expected "Works as expected"
            testCase "Three lines, last one indented"
            <| fun _ ->
                let subject =
                    stringBuilder {
                        "Line 1"
                        "Line 2"
                        indent { "Line 3" }
                    }

                let expected =
                    """Line 1
Line 2
    Line 3
"""

                Expect.equal subject expected "Works as expected"
            testCase "Three lines, middle one indented"
            <| fun _ ->
                let subject =
                    stringBuilder {
                        "Line 1"
                        indent { "Line 2" }
                        "Line 3"
                    }

                let expected =
                    """Line 1
    Line 2
Line 3
"""

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
                    stringBuilder {
                        string
                        stringSeq
                        char
                        charSeq
                    }

                let expected =
                    """String
Seq1
Seq2
c
d
e
"""

                Expect.equal subject expected "Works as expected"

            testCase "Writes namespaces as expected"
            <| fun _ ->
                let namespaces = [
                    "Some.Namespace"
                    "Another.Namespace"
                ]

                let subject = stringBuilder { writeNamespaces namespaces }

                let expected =
                    """open Some.Namespace
open Another.Namespace
"""

                Expect.equal subject expected "Works as expected"
        ]
