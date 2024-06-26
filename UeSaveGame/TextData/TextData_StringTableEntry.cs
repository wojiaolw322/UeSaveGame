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

namespace UeSaveGame.TextData
{
	public class TextData_StringTableEntry : ITextData
    {
        public FString? Table { get; set; }
        public FString? Key { get; set; }

        public void Deserialize(BinaryReader reader, long size)
        {
            Table = reader.ReadUnrealString();
            Key = reader.ReadUnrealString();
        }

        public long Serialize(BinaryWriter writer)
        {
            if (Table == null || Key == null) throw new InvalidOperationException("Instance is not valid for serialization");

            writer.WriteUnrealString(Table);
            writer.WriteUnrealString(Key);

            return 8 + Table.SizeInBytes + Key.SizeInBytes;
        }

        public override string ToString()
        {
            return $"{Table}[{Key}]";
        }
    }
}
