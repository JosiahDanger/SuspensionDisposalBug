using System.Text.Json.Serialization;

namespace SuspensionDisposalBug.Models
{
	[JsonSerializable(typeof(AppModel))]
	[JsonSourceGenerationOptions(WriteIndented = true)]
	internal partial class AppModelSerialiserContext : JsonSerializerContext
	{
	}
}
