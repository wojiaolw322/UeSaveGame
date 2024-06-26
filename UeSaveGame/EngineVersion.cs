﻿using UeSaveGame.Util;

namespace UeSaveGame
{
	/// <summary>
	/// Represents a version of the engine
	/// </summary>
	public struct EngineVersion
	{
		public short Major;
		public short Minor;
		public short Patch;
		public int Build;
		public FString BuildId;

		/// <summary>
		/// The latest engine version this library has been tested with.
		/// </summary>
		public static readonly EngineVersion LatestTested;

		static EngineVersion()
		{
			LatestTested = new EngineVersion()
			{
				Major = 5,
				Minor = 3,
				Patch = 0,
				Build = 0,
				BuildId = new("UE5")
			};
		}

		internal static EngineVersion Deserialize(BinaryReader reader)
		{
			EngineVersion version = new();

			version.Major = reader.ReadInt16();
			version.Minor = reader.ReadInt16();
			version.Patch = reader.ReadInt16();
			version.Build = reader.ReadInt32();
			version.BuildId = reader.ReadUnrealString()!;

			return version;
		}

		internal readonly void Serialize(BinaryWriter writer)
		{
			writer.Write(Major);
			writer.Write(Minor);
			writer.Write(Patch);
			writer.Write(Build);
			writer.WriteUnrealString(BuildId);
		}

		public override readonly string ToString()
		{
			return $"{Major}.{Minor}.{Patch}.{Build} ({BuildId})";
		}
	}
}
