using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CustomerManagementSystem.Analyzers.MaybeSemanticAnalyzer,
    CustomerManagementSystem.Analyzers.MaybeCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CustomerManagementSystem.Analyzers.Tests;

public class MaybeCodeFixProviderTests
{
    [Fact]
    public async Task Suggests_code_fix_for_throwing_exception_when_method_returns_Maybe_T()
    {
        const string source = """
                              using System;
                              using CustomerManagementSystem.Api.Shared.Fx;

                              public class Program
                              {
                                  public Maybe<int> GetValue(string number)
                                  {
                                      throw new InvalidOperationException("Could not parse the number");
                                  }
                              }
                              """;

        const string newSource = """
                                 using System;
                                 using CustomerManagementSystem.Api.Shared.Fx;

                                 public class Program
                                 {
                                     public Maybe<int> GetValue(string number)
                                     {
                                         return CustomerManagementSystem.Api.Shared.Fx.Maybe.None;
                                     }
                                 }
                                 """;

        var expected = Verifier.Diagnostic()
            .WithLocation(8, 9)
            .WithMessage("Use Maybe.None instead of throw exception");

        var codeFixTester = DiagnosticTestUtilities
            .GetCodeFixAnalyzerForOption<MaybeSemanticAnalyzer, MaybeCodeFixProvider, DefaultVerifier>(
                source, [expected], newSource);

        await codeFixTester.RunAsync();
    }
}
