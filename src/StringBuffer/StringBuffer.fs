[<AutoOpen>]
module rec StringBuffer

open System
open System.Text
open System.IO

type private Indenter(s: IndentedStringBuilder) =

    do
        s.IncrementIndent()
        |> ignore

    interface IDisposable with

        member this.Dispose() =
            s.DecrementIndent()
            |> ignore


type IndentedStringBuilder() =

    let indentSize = 4

    let mutable _indent = 0
    let mutable _indentPending = true

    let _stringBuilder = StringBuilder()

    let doIndent () =
        if
            _indentPending
            && _indent > 0
        then
            _stringBuilder.Append(
                ' ',
                _indent
                * indentSize
            )
            |> ignore

        _indentPending <- false

    member this.IndentCount = _indent
    member this.Length = _stringBuilder.Length

    member this.Clear() =
        _stringBuilder.Clear()
        |> ignore

        _indent <- 0
        this

    member this.IncrementIndent() =
        _indent <-
            _indent
            + 1

        this

    member this.DecrementIndent() =
        _indent <-
            Math.Max(
                _indent
                - 1,
                0
            )

        this

    member this.Indent() = new Indenter(this) :> IDisposable

    member this.Append(value: string) =
        doIndent ()

        _stringBuilder.Append(value)
        |> ignore

        this

    member this.Append(value: FormattableString) =
        doIndent ()

        _stringBuilder.Append(value)
        |> ignore

        this

    member this.Append(value: char) =
        doIndent ()

        _stringBuilder.Append(value)
        |> ignore

        this

    member this.Append(values: string seq) =
        doIndent ()

        values
        |> Seq.iter (
            _stringBuilder.Append
            >> ignore
        )

        this

    member this.Append(values: char seq) =
        doIndent ()

        values
        |> Seq.iter (
            _stringBuilder.Append
            >> ignore
        )

        this

    member this.AppendLine(value: string) =
        if
            value.Length
            <> 0
        then
            doIndent ()

        _stringBuilder.AppendLine(value)
        |> ignore

        _indentPending <- true

        this

    member this.AppendLine(value: FormattableString) =
        doIndent ()

        _stringBuilder.Append(value).AppendLine()
        |> ignore

        _indentPending <- true

        this

    member this.AppendLine() =
        doIndent ()

        this.AppendLine(String.Empty)
        |> ignore

        this

    member this.AppendLines(value: string, ?skipFinalNewLine: bool) =

        let skipFinalNewLine = defaultArg skipFinalNewLine false

        use reader = new StringReader(value)

        Seq.initInfinite (fun _ -> reader.ReadLine())
        |> Seq.takeWhile (
            isNull
            >> not
        )
        |> Seq.iteri (fun idx line ->
            if idx <> 0 then
                this.AppendLine()
                |> ignore

            if line.Length > 0 then
                this.Append line
                |> ignore
        )

        if not skipFinalNewLine then
            this.AppendLine()
            |> ignore

        this

    member this.AppendLines(values: string seq) =
        doIndent ()

        values
        |> Seq.iter (
            _stringBuilder.AppendLine
            >> ignore
        )

        this

    member this.AppendLines(values: char seq) =
        doIndent ()

        values
        |> Seq.iter (
            string
            >> _stringBuilder.AppendLine
            >> ignore
        )

        this

    member this.AppendJoin(values: string seq, ?separator: string) =

        let separator = defaultArg separator ", "

        doIndent ()

        _stringBuilder.AppendJoin(separator, values)
        |> ignore

        this

    member this.AppendJoin(separator: string, [<ParamArray>] values: string[]) =

        doIndent ()

        _stringBuilder.AppendJoin(separator, values)
        |> ignore

        this

    override this.ToString() = _stringBuilder.ToString()

type StringBuffer = IndentedStringBuilder -> unit

type private IndentationLevel =
    | NoIndent
    | Indent

let private writeStringBuffer (f: StringBuffer) (indent: IndentationLevel) =
    let b = IndentedStringBuilder()

    match indent with
    | NoIndent -> ()
    | Indent ->
        b.IncrementIndent()
        |> ignore

    do f b

    match indent with
    | NoIndent -> ()
    | Indent ->
        b.DecrementIndent()
        |> ignore

    b.ToString()

let writeNamespaces (namespaces: string seq) =
    let strings =
        namespaces
        |> Seq.map (fun n ->
            "open "
            + n
        )

    String.Join(Environment.NewLine, strings)

type StringBufferBuilder() =

    member __.Yield(txt: string) =
        fun (b: IndentedStringBuilder) ->
            Printf.kprintf
                (b.AppendLines
                 >> ignore)
                "%s"
                txt

    member __.Yield(c: char) =
        fun (b: IndentedStringBuilder) ->
            Printf.kprintf
                (b.AppendLine
                 >> ignore)
                "%c"
                c

    member __.YieldFrom(f: StringBuffer) = f

    member __.Combine(f, g) =
        fun (b: IndentedStringBuilder) ->
            f b
            g b

    member __.Delay f =
        fun (b: IndentedStringBuilder) -> (f ()) b

    member __.Zero() = ignore

    member __.For(xs: 'a seq, f: 'a -> StringBuffer) =
        fun (b: IndentedStringBuilder) ->
            use e = xs.GetEnumerator()

            while e.MoveNext() do
                (f e.Current) b

    member __.While(p: unit -> bool, f: StringBuffer) =
        fun (b: IndentedStringBuilder) ->
            while p () do
                f b

    abstract member Run: StringBuffer -> string
    default this.Run(f: StringBuffer) = writeStringBuffer f NoIndent

    member _.Yield(lines: string seq) =
        fun (b: IndentedStringBuilder) ->
            lines
            |> Seq.iter (fun txt ->
                Printf.kprintf
                    (b.AppendLines
                     >> ignore)
                    "%s"
                    txt
            )

    member _.Yield(lines: char seq) =
        fun (b: IndentedStringBuilder) ->
            lines
            |> Seq.iter (fun txt ->
                Printf.kprintf
                    (b.AppendLines
                     >> ignore)
                    "%c"
                    txt
            )

    member __.Yield(txt: string option) =
        fun (b: IndentedStringBuilder) ->
            match txt with
            | Some t ->
                Printf.kprintf
                    (b.AppendLines
                     >> ignore)
                    "%s"
                    t
            | None -> ()

    member __.Yield(txt: string option seq) =
        fun (b: IndentedStringBuilder) ->
            for tt in txt do
                match tt with
                | Some t ->
                    Printf.kprintf
                        (b.AppendLines
                         >> ignore)
                        "%s"
                        t
                | None -> ()


type IndentStringBufferBuilder() =
    inherit StringBufferBuilder()

    override _.Run(f: StringBuffer) = writeStringBuffer f Indent

let stringBuffer = new StringBufferBuilder()
let indent = new IndentStringBufferBuilder()
