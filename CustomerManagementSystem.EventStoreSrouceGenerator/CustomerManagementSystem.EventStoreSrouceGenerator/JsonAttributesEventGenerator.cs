using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CustomerManagementSystem.EventStoreSrouceGenerator;

[Generator]
public class JsonAttributesEventGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var declarations = context.SyntaxProvider
            // .ForAttributeWithMetadataName()// 👈 use the new API
            .CreateSyntaxProvider(
                // 👇 Runs for _every_ syntax node, on _every_ key press!
                predicate: static (s, _) => IsCandidateForGeneratingJsonAttributes(s),
                // 👇 Runs for _every_ node selected by the predicate, on _every_ key press!
                transform: static (ctx, stoppingToken) => GetEventsMarkedWithIEventInterface(ctx, stoppingToken))
            .Where(static symbol => symbol is not null);

        // 👇 Runs for every _new_ declarations value returned by the syntax provider
        //    (* as long as declarations is value equatable)
        var allEventsDeclarations = declarations.Collect();

        // 👇 Runs for every _new_ declarations value returned by the syntax provider
        //    (* as long as declarations is value equatable)
        context.RegisterSourceOutput(allEventsDeclarations, GenerateSource!);
    }


    /// <summary>
    /// This method should be as fast as possible and as selective as possible, as usual, "Trade-off" ¯\_(ツ)_/¯ 
    /// </summary>
    /// <param name="syntaxNode"></param>
    /// <returns></returns>
    private static bool IsCandidateForGeneratingJsonAttributes(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not RecordDeclarationSyntax && syntaxNode is not ClassDeclarationSyntax)
            return false;

        var baseListTypes = (syntaxNode as TypeDeclarationSyntax)!.BaseList?.Types.ToImmutableArray() ?? [];

        var iEventSpan = "IEvent<".AsSpan();

        foreach (var type in baseListTypes)
        {
            if (type.GetText().ToString().AsSpan().StartsWith(iEventSpan))
                return true;
        }

        return false;
    }

    /// <summary>
    /// The key is to extract all the details you need from the SyntaxNode and SemanticModel in the syntax provider transform,
    /// and store these in a data model that is equatable so that caching works correctly.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static InterfaceWithEvents? GetEventsMarkedWithIEventInterface(GeneratorSyntaxContext context, CancellationToken cancellationToken = default)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol symbol)
            return null;

        var @interface = symbol.Interfaces.FirstOrDefault(i =>
            i.ToDisplayString().StartsWith("CustomerManagementSystem.Domain.IEvent<"));

        if (@interface is null)
            return null;
        
        cancellationToken.ThrowIfCancellationRequested();

        var interfaceName = @interface.Name;
        var interfaceNamespace = @interface.ContainingNamespace.ToDisplayString();
        var name = symbol.Name;
        var @namespace = symbol.ContainingNamespace.ToDisplayString();

        return new InterfaceWithEvents(name, @namespace, interfaceName, interfaceNamespace);
    }

    private static void GenerateSource(SourceProductionContext context, ImmutableArray<InterfaceWithEvents?> events)
    {
        var nonNullEventTypes = events.Where(d => d.HasValue).Select(d => d!.Value).ToImmutableArray();

        var eventInterface = nonNullEventTypes.Select(e => new { e.InterfaceName, e.InterfaceNamespace }).First();

        var allEventsNamespaces = nonNullEventTypes
            .Select(t => $"using {t.Namespace};")
            .Where(ns => ns != $"using {eventInterface.InterfaceNamespace};")
            .Distinct();

        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine();
        sb.AppendLine("using System.Text.Json.Serialization;");
        foreach (var ns in allEventsNamespaces)
        {
            sb.AppendLine(ns);
        }

        sb.AppendLine();
        sb.AppendLine($"namespace {eventInterface.InterfaceNamespace};");
        sb.AppendLine();
        sb.AppendLine("[JsonPolymorphic(IgnoreUnrecognizedTypeDiscriminators = true)]");

        foreach (var eventType in nonNullEventTypes)
        {
            sb.AppendLine($"[JsonDerivedType(typeof({eventType.Name}), nameof({eventType.Name}))]");
        }

        sb.Append($"public partial interface {eventInterface.InterfaceName};");
        sb.AppendLine();

        context.AddSource("EventsJsonAttributes.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}

internal readonly record struct InterfaceWithEvents
{
    public readonly string InterfaceName;
    public readonly string InterfaceNamespace;
    public readonly string Name;
    public readonly string Namespace;

    internal InterfaceWithEvents(string name, string @namespace, string interfaceName, string interfaceNamespace)
    {
        Name = name;
        Namespace = @namespace;

        InterfaceName = interfaceName;
        InterfaceNamespace = interfaceNamespace;
    }
}


/*
https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/
         ██████████████    ██  ██  ██  ██  ████      ██  ████        ████    ██  ██████  ██████      ██████████████
   ██          ██  ██  ████  ████  ██████    ████████  ██  ████  ████  ██  ████  ██  ██████    ██          ██
   ██  ██████  ██      ████  ██    ████    ██████    ██  ██  ██  ██████  ██    ██        ██    ██  ██████  ██
   ██  ██████  ██  ██████  ██████████  ██    ██    ██████  ██    ████████████████████████  ██  ██  ██████  ██
   ██  ██████  ██  ██  ██  ████████████      ██    ██████████              ██  ██  ██  ██      ██  ██████  ██
   ██          ██        ██  ████    ██        ██  ██      ██████        ████████  ██  ██      ██          ██
   ██████████████  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██  ██████████████
                   ██  ████    ████  ████████    ████      ████  ██                ██████  ██
     ██  ████████  ██  ██████████      ████    ██  ████████████████    ██████  ██  ██████  ██████  ████  ██
     ████    ██  ████  ██  ████████  ██        ████████    ████████████████████████  ████  ██████  ██████
   ██  ████    ██    ██  ██████  ██      ████████████  ██        ████  ██  ████    ██    ██  ████  ██  ██
     ██    ████  ██  ████    ██    ████████████            ██  ██  ██  ██      ██████████    ██  ██  ██  ████
     ██    ██  ██      ██  ████            ██████  ██████      ██  ██  ██  ██  ██████  ████████    ████
   ██████  ████  ████  ████    ████  ████  ██  ██    ██  ████████  ██    ████  ████  ████    ████    ██    ██
     ████████  ██    ██████████████  ████  ████        ██        ████  ██  ████████████  ████████████      ██
       ██    ██  ██████        ██  ████  ████      ██  ████    ██      ████  ██      ██████  ████    ██████
     ██    ██  ████  ████  ████        ██      ██        ██      ████████  ██          ██    ██  ████      ██
     ██    ██    ██████████            ██  ████  ██████  ██  ████  ████████████  ████  ██    ████      ██
   ████  ██████████  ██  ████  ██      ████████          ██  ██  ██████    ████    ██    ████      ██  ██  ██
                 ██████████████    ████  ████████  ██      ██  ████  ██    ██      ████      ████      ██  ██
   ██  ████    ████        ██████  ████  ██  ██████  ████    ████    ██████  ████  ██  ████████    ██  ██████
     ████    ██      ██  ██          ██  ██  ██    ████  ██    ██    ████    ██  ████████████████    ████  ██
         ████████    ██    ████  ██    ████      ██  ████  ██  ████  ████      ██  ██      ████████████████
   ████  ██████        ██    ████  ██████      ████            ██  ████  ██              ████  ██    ██  ██
         ████████████  ██  ██      ██████████████████████████  ██  ████    ██  ██  ██  ████████████████
   ████    ██      ██    ██      ██  ██  ██    ██████      ██  ██    ██        ██  ██████  ██      ██      ██
     ████  ██  ██  ██  ██        ████        ██  ████  ██  ████      ██    ██████  ██      ██  ██  ██████  ██
   ██    ████      ████████  ██████            ██  ██      ████  ████████      ██      ██  ██      ████  ████
   ████    ████████████████      ██████  ██    ██████████████      ██████  ██    ██    ██  ██████████  ██  ██
           ████  ██  ██    ████          ████  ██    ██  ██    ████  ████  ████    ██████    ██    ██  ██
   ██  ██    ████████      ██      ██████  ██    ██  ██  ████  ████████      ██    ██  ██████  ██  ██████  ██
     ████  ████    ██  ██  ████  ██        ██  ██        ██  ██      ████          ██          ████    ██
   ██    ████████      ██  ██████  ██    ████  ██    ████            ██  ████      ██    ██            ██████
   ████    ██        ██████  ██          ██                ██  ████  ██████████████    ██  ██████  ██  ██
     ████████  ████  ██  ██  ██    ██    ██        ██    ██  ██████  ████        ██  ██              ██
         ██      ████    ██████████  ██      ██      ██  ██  ██  ██████  ██    ██      ████████  ██  ██    ██
   ██████  ██  ██████████    ██  ████    ████████    ██    ██████  ████    ██  ██  ████████          ██    ██
       ██  ██    ████  ██████████  ████      ████      ██  ██        ██  ████████████  ██    ██  ████████████
   ████  ██    ██          ██████████  ████    ████    ██  ██    ██  ██  ██  ████              ██  ██      ██
   ██    ██████        ██    ██  ██  ██  ██  ██████████    ██  ██    ████      ██      ████  ██  ████  ██████
     ██  ████  ██      ████  ██      ██  ██    ████    ██████  ████████    ██    ██  ██    ████    ██    ██
       ████████  ██  ██████████      ██████████          ██    ██████████    ██  ██    ██████████████████
   ████  ████████████████████  ██  ██    ██  ██████      ██    ████████    ████      ██        ██  ██    ████
     ████        ██  ████    ██  ██    ██  ██  ██    ██    ██    ██  ████              ██      ██  ██  ██████
         ██    ████  ██  ██      ██    ████  ████  ██████████████    ██  ████████        ████████████  ██  ██
                   ██  ██  ██████  ██  ██    ████████      ██    ██  ██  ████  ██    ████  ██      ████
   ██████████████      ██  ██    ████    ██  ████████  ██  ████  ██  ██    ██      ██      ██  ██  ██
   ██          ██  ██      ██████  ████    ████    ██      ████  ██████████  ██      ████  ██      ████    ██
   ██  ██████  ██  ████  ████  ██████      ██    ████████████  ██  ██  ██  ██  ██  ████  ██████████████
   ██  ██████  ██  ██      ████  ██████  ██  ██  ██████    ████    ████        ██████████  ██    ██    ██████
   ██  ██████  ██      ██        ██  ██          ██████      ██          ██    ██  ██    ██    ████████    ██
   ██          ██  ████  ██████    ████  ██████  ██████  ██              ████  ██      ██████████    ████  ██
   ██████████████      ████████████████      ██    ██  ██      ██████    ██████████        ██████
 */
