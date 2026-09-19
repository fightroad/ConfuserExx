using System;
using dnlib.DotNet;

namespace Confuser.Protections {
	/// <summary>
	///     Detects assemblies that run on CoreCLR (.NET Core / .NET 5+).
	///     Protections that poke Framework CLR internals must refuse these.
	/// </summary>
	internal static class CoreClrSupport {
		public static bool IsNetCoreApp(ModuleDef module) {
			var assembly = module?.Assembly;
			if (assembly == null)
				return false;

			foreach (var attr in assembly.CustomAttributes) {
				if (attr.TypeFullName != "System.Runtime.Versioning.TargetFrameworkAttribute")
					continue;
				if (attr.ConstructorArguments.Count == 0)
					continue;

				var moniker = attr.ConstructorArguments[0].Value?.ToString();
				if (!string.IsNullOrEmpty(moniker) &&
					moniker.StartsWith(".NETCoreApp,", StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}
	}
}
