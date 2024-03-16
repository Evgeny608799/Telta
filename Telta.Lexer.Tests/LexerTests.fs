module LexerTests

open System.Collections.Generic
open System.IO
open Telta.Lexer
open Telta.Lexer.Tests
open Telta.Syntax
open Xunit

let private testTokenization (source:string) testFunc =
    use stream = new StringReader(source)
    let sourceFile = FakeSource(stream)
    let lexer = Lexer(sourceFile)
    let tokens = lexer.Tokenization
    testFunc tokens

[<Fact>]
let PrimaryTest () =
    let mutable source = """int32 sigma =10 ; // ignored text
int32 a= 5;
string greetMessage = "Hello, World!";
if (sigma>a)
{
    print(greetMessage);
    
    if (a >=2) {
            a |+;
            +| sigma;
    }
}

decimal realNum = 4,5;
decimal realNum2 = 4.5;
decimal realNum3 = 4.;

realNum3 += (decimal)sigma;//"""
    use stream = new StringReader(source)
    let sourceFile = FakeSource(stream)
    let lexer = Lexer(sourceFile)
    let tokens = lexer.Tokenization
    Assert.True(true)
    
let AssignNewVariablesTest (tokens:TokenStream) (definedType:TokenType) (value:TokenType) =
    Assert.Equal(6, tokens.Length)
    let expectedTokens = [|
        definedType; TokenType.Identifier; TokenType.Assign; value; TokenType.Semicolon; TokenType.End
    |]
    Assert.Equivalent(expectedTokens, (tokens :> IEnumerable<Token>) |> Seq.map (_.Type))
    
[<Theory>]
[<InlineData("int32 num = 10;")>]
[<InlineData("int32 NUM= 10 ;")>]
[<InlineData("int32 NUM_= 10;")>]
[<InlineData("int32 _num=10;")>]
[<InlineData("int32 my_num =  10 ; // myNum")>]
let IntegerLiteralAssignNewVariablesTest (source:string) =
    testTokenization source (fun tokens ->
        AssignNewVariablesTest tokens TokenType.KeywordInt TokenType.IntegerLiteral)
    
[<Theory>]
[<InlineData("//ignored")>]
[<InlineData("// ignored")>]
[<InlineData("//")>]
let IgnoreCommentLexemesTest (source:string) =
    testTokenization source Assert.Single