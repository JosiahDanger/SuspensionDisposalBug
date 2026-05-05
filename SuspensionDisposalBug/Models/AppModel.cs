using Avalonia.Media;
using System.Text.Json.Serialization;

namespace SuspensionDisposalBug.Models
{
	internal sealed class AppModel
	{
		[JsonPropertyName("ordinalDaySuffixPosition")]
		public byte? OrdinalDaySuffixPosition { get; set; }

		[JsonPropertyName("refreshIntervalSeconds")]
		public uint RefreshIntervalSeconds { get; set; }

		[JsonPropertyName("anchoredCornerScaledPositionX")]
		public double AnchoredCornerScaledPositionX { get; set; }

		[JsonPropertyName("anchoredCornerScaledPositionY")]
		public double AnchoredCornerScaledPositionY { get; set; }

		[JsonPropertyName("monitorReference")]
		public string? MonitorReference { get; set; }

		[JsonPropertyName("fontSize")]
		public int FontSize { get; set; }

		[JsonPropertyName("fontWeight")]
		public int FontWeight { get; set; }

		[JsonPropertyName("fontRenderingMode")]
		public TextRenderingMode FontRenderingMode { get; set; }

		[JsonPropertyName("customFontColour")]
		public uint? CustomFontColour { get; set; }

		[JsonPropertyName("customDropShadowColour")]
		public uint? CustomDropShadowColour { get; set; }

		[JsonPropertyName("settingsViewOpacity")]
		public double SettingsViewOpacity { get; set; }
	}
}
