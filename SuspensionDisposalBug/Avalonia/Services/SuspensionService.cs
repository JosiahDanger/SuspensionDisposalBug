using ReactiveUI;
using SuspensionDisposalBug.Models;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SuspensionDisposalBug.Avalonia.Services
{
	internal sealed class SuspensionService(string filepath) : ISuspensionDriver
	{
		private readonly string _filepath =
			filepath ?? throw new ArgumentNullException(nameof(filepath));

		public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
		{
			ArgumentNullException.ThrowIfNull(typeInfo);

			return Observable.Start(() =>
			{
				string? directoryPath = Path.GetDirectoryName(_filepath);

				if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}

				try
				{
					using FileStream targetFileStream =
						File.Open(_filepath, FileMode.Create, FileAccess.Write, FileShare.None);

					JsonSerializer.Serialize(targetFileStream, state, typeInfo);

					return Unit.Default;
				}
				catch
				{
					throw new InvalidOperationException();
				}
			});
		}

		public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
		{
			ArgumentNullException.ThrowIfNull(typeInfo);

			return Observable.Start(() =>
			{
				if (!File.Exists(_filepath))
				{
					return default;
				}

				try
				{
					using FileStream targetFileStream = File.OpenRead(_filepath);

					if (typeof(T) == typeof(AppModel))
					{
						throw new InvalidOperationException();
					}

					targetFileStream.Seek(0, SeekOrigin.Begin);
					T? result = JsonSerializer.Deserialize(targetFileStream, typeInfo);

					return result;
				}
				catch
				{
					throw new InvalidOperationException();
				}
			});
		}

		public IObservable<Unit> InvalidateState()
		{
			return Observable.Start(() =>
			{
				try
				{
					File.Delete(_filepath);
				}
				catch
				{
					throw new InvalidOperationException();
				}

				return Unit.Default;
			});
		}


		#pragma warning disable IL2046, IL3051

			public IObservable<Unit> SaveState<T>(T state) =>

						/* From ReactiveUI XML documentation in the ISuspensionDriver interface:
						 * 
						 *	"This member typically relies on reflection-based serialization and is
						 *	not trimming or AOT friendly."
						 * 
						 *	"Implementations commonly use reflection-based serialization. Prefer
						 *	SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios." */

						Observable.Throw<Unit>(new InvalidOperationException());

			public IObservable<object?> LoadState() =>

						/* From ReactiveUI XML documentation in the ISuspensionDriver interface:
						 * 
						 *	"This member typically relies on reflection-based serialization and is
						 *	not trimming or AOT friendly."
						 * 
						 *	"Implementations commonly use reflection-based serialization. Prefer
						 *	LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios." */

						Observable.Throw<object?>(new InvalidOperationException());

		#pragma warning restore IL2046, IL3051
	}
}