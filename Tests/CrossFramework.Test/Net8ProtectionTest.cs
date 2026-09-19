using System;
using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace CrossFramework.Test {
	public class Net8ProtectionTest : TestBase {
		public Net8ProtectionTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Protection", "anti debug")]
		[Trait("TFM", "net8.0")]
		public Task AntiDebug_Safe_Net8() =>
			Run("CrossFramework.Console.Net8.dll",
				null,
				new SettingItem<Protection>("anti debug") { { "mode", "safe" } },
				outputDirSuffix: "-net8-antidebug-safe",
				checkOutput: false);

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Protection", "anti tamper")]
		[Trait("TFM", "net8.0")]
		public Task AntiTamper_Normal_Net8() =>
			Run("CrossFramework.Console.Net8.dll",
				null,
				new SettingItem<Protection>("anti tamper") { { "mode", "normal" } },
				outputDirSuffix: "-net8-antitamper-normal",
				checkOutput: false);

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("TFM", "net8.0")]
		public Task AntiDebug_Safe_AntiTamper_Normal_Constants_Net8() =>
			Run("CrossFramework.Console.Net8.dll",
				null,
				new[] {
					new SettingItem<Protection>("anti debug") { { "mode", "safe" } },
					new SettingItem<Protection>("anti tamper") { { "mode", "normal" } },
					new SettingItem<Protection>("constants")
				},
				outputDirSuffix: "-net8-safe-normal-constants",
				checkOutput: false);

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Protection", "anti debug")]
		[Trait("TFM", "net8.0")]
		public async Task AntiDebug_Antinet_Net8_IsRejected() {
			var ex = await Assert.ThrowsAnyAsync<Exception>(() =>
				Run("CrossFramework.Console.Net8.dll",
					null,
					new SettingItem<Protection>("anti debug") { { "mode", "antinet" } },
					outputDirSuffix: "-net8-antidebug-antinet",
					checkOutput: false));

			Assert.Contains("antinet", ex.ToString(), StringComparison.OrdinalIgnoreCase);
			Assert.Contains("not supported", ex.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Protection", "anti tamper")]
		[Trait("TFM", "net8.0")]
		public async Task AntiTamper_Jit_Net8_IsRejected() {
			var ex = await Assert.ThrowsAnyAsync<Exception>(() =>
				Run("CrossFramework.Console.Net8.dll",
					null,
					new SettingItem<Protection>("anti tamper") { { "mode", "jit" } },
					outputDirSuffix: "-net8-antitamper-jit",
					checkOutput: false));

			Assert.Contains("jit", ex.ToString(), StringComparison.OrdinalIgnoreCase);
			Assert.Contains("not supported", ex.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Protection", "ref proxy")]
		[Trait("TFM", "net8.0")]
		public Task RefProxy_Mild_Net8() =>
			Run("CrossFramework.Console.Net8.dll",
				null,
				new SettingItem<Protection>("ref proxy") { { "mode", "mild" } },
				outputDirSuffix: "-net8-refproxy-mild",
				checkOutput: false);

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Protection", "ref proxy")]
		[Trait("TFM", "net8.0")]
		public async Task RefProxy_Strong_Net8_IsRejected() {
			var ex = await Assert.ThrowsAnyAsync<Exception>(() =>
				Run("CrossFramework.Console.Net8.dll",
					null,
					new SettingItem<Protection>("ref proxy") { { "mode", "strong" } },
					outputDirSuffix: "-net8-refproxy-strong",
					checkOutput: false));

			Assert.Contains("strong", ex.ToString(), StringComparison.OrdinalIgnoreCase);
			Assert.Contains("not supported", ex.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		[Trait("Category", "CrossFramework")]
		[Trait("Packer", "compressor")]
		[Trait("TFM", "net8.0")]
		public Task Compressor_AutoCompat_Net8() =>
			Run("CrossFramework.Console.Net8.dll",
				null,
				NoProtections,
				outputDirSuffix: "-net8-compressor-autocompat",
				packer: new SettingItem<Packer>("compressor"),
				checkOutput: false);
	}
}
