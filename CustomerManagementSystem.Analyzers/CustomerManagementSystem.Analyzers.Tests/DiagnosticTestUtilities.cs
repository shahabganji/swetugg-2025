using System.Collections.Generic;
using CustomerManagementSystem.Domain.Fx;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace CustomerManagementSystem.Analyzers.Tests;

public class DiagnosticTestUtilities
{
    public static CSharpAnalyzerTest<TAnalyzer, TVerifier> GetAnalyzerForOption<TAnalyzer, TVerifier>(
        string source, IEnumerable<DiagnosticResult> expectedDiagnostics)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TVerifier : IVerifier, new()
    {
        var analyzerTest = new CSharpAnalyzerTest<TAnalyzer, TVerifier>
        {
            TestState =
            {
                Sources = { source },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(typeof(None).Assembly.Location),
                }
            }
        };
        analyzerTest.ExpectedDiagnostics.AddRange(expectedDiagnostics);

        return analyzerTest;
    }

    public static CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> GetCodeFixAnalyzerForOption<TAnalyzer, TCodeFix,
        TVerifier>(
        string source, IEnumerable<DiagnosticResult> expectedDiagnostics, string fixedCode)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TVerifier : IVerifier, new()
        where TCodeFix : CodeFixProvider, new()
    {
        var codeFixTest = new CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>
        {
            TestState =
            {
                Sources = { source },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(typeof(None).Assembly.Location),
                }
            },
            FixedCode = fixedCode
        };
        codeFixTest.ExpectedDiagnostics.AddRange(expectedDiagnostics);

        return codeFixTest;
    }
}
