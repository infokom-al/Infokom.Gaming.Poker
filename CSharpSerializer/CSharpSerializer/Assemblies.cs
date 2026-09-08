using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpSerializer;

internal static class Assemblies
{
	public static IEnumerable<Assembly> GetRelevantAssembliesSlow()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		yield return executingAssembly;
		string rootNameSpace = executingAssembly.GetRootNamespace().GetFirstNamespaceWord();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (assembly.GetName().Name.StartsWith(rootNameSpace))
			{
				yield return assembly;
			}
		}
	}

	public static string GetRootNamespace(this Assembly assembly)
	{
		return (from ns in (from t in assembly.GetTypes()
				select t.Namespace into ns
				where !string.IsNullOrEmpty(ns)
				select ns).Distinct()
			orderby ns.Length
			select ns).FirstOrDefault() ?? throw new InvalidOperationException("Failed to get root namespace");
	}

	public static string GetAssemblyName(this Assembly assembly)
	{
		return assembly.GetName().Name ?? throw new InvalidOperationException("No assembly name");
	}

	private static string GetFirstNamespaceWord(this string @namespace)
	{
		int num = @namespace.IndexOf('.');
		if (num == -1)
		{
			return @namespace;
		}
		return @namespace.Substring(0, num);
	}
}
