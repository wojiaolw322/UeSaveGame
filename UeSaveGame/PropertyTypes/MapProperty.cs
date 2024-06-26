﻿// Copyright 2022 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class MapProperty : UProperty<IList<KeyValuePair<UProperty, UProperty>>>
    {
        private int mRemovedCount;

        public FString? KeyType { get; set; }

        public FString? ValueType { get; set; }

		public MapProperty(FString name)
			: this(name, new(nameof(MapProperty)))
		{
		}

		public MapProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, PackageVersion packageVersion)
        {
            if (includeHeader)
            {
                KeyType = reader.ReadUnrealString();
                ValueType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            if (KeyType == null || ValueType == null) throw new InvalidOperationException("Unknown map type cannot be read.");

            mRemovedCount = reader.ReadInt32();
            if (mRemovedCount != 0)
            {
                // Maps share some serialization code with Sets. Sets can store items to be removed as well as items to be added.
                // Not sure if such a feature exists for maps, but it has not yet been encountered if it does.
                throw new NotImplementedException();
            }

            int count = reader.ReadInt32();
            
            // Check isFGuid as Dictionary key , it with out header and struct , just 16bytes value
            // in C++ Like  TMap<struct FGuid, struct F.....> Field; 
            // in C# Like    Dictionary<Guid,F.....> Field; 
            // In GvasFile  [16bytes][Valuebytes]
            bool isFGuid = false;
            if (KeyType == "StructProperty")
            {
                long position = reader.BaseStream.Position;
                int length = reader.ReadInt32();
                // This is not rigorous,hope the value of bit A of the Guid is not within this range
                if (length < 0 | length > 255)
                {
                    isFGuid = true;
                }
                reader.BaseStream.Position = position;
            }
            Value = new List<KeyValuePair<UProperty, UProperty>>(count);
            
            for (int i = 0; i < count; ++i)
            {
                UProperty? key;
                {
                    Type type = ResolveType(KeyType);
                    FString StructName = FString.Empty;
                    if (isFGuid)
                    {
                        StructName = new FString("FGuid");
                    }
                    key = (UProperty?)Activator.CreateInstance(type, StructName, KeyType);
                    if (key == null) throw new FormatException("Error reading map key");
                    key.Deserialize(reader, 0, false, packageVersion);
                }
                UProperty? value;
                {
                    Type type = ResolveType(ValueType);
                    value = (UProperty?)Activator.CreateInstance(type, Name, ValueType);
                    if (value == null) throw new FormatException("Error reading map value");
                    value.Deserialize(reader, 0, false, packageVersion);
                }
                Value.Add(new KeyValuePair<UProperty, UProperty>(key, value));
            }

            tempSize = size;
        }

        long tempSize;

        public override long Serialize(BinaryWriter writer, bool includeHeader, PackageVersion packageVersion)
        {
            if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

            if (includeHeader)
            {
                writer.WriteUnrealString(KeyType);
                writer.WriteUnrealString(ValueType);
                writer.Write((byte)0);
            }

            long startPosition = writer.BaseStream.Position;

            writer.Write(mRemovedCount);

            writer.Write(Value.Count);
            foreach (var pair in Value)
            {
                pair.Key.Serialize(writer, false, packageVersion);
                pair.Value.Serialize(writer, false, packageVersion);
            }

            return writer.BaseStream.Position - startPosition;
        }

        public override string ToString()
        {
            return Value == null ? base.ToString() : $"{Name} [{nameof(MapProperty)}<{KeyType},{ValueType}>] Count = {Value.Count}";
        }
    }
}
