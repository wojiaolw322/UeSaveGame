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

using UeSaveGame.DataTypes;

namespace UeSaveGame.StructData
{
	public class DateTimeStruct : BaseStructData
    {
        public FDateTime DateTime { get; set; }

        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "DateTime";
            }
        }

        public override void Deserialize(BinaryReader reader, long size, PackageVersion packageVersion)
        {
            FDateTime dateTime = new FDateTime();
            dateTime.Ticks = reader.ReadInt64();
            DateTime = dateTime;
        }

        public override long Serialize(BinaryWriter writer, PackageVersion packageVersion)
        {
            writer.Write(DateTime.Ticks);

            return 8;
        }

        public override string ToString()
        {
            return DateTime.ToString();
        }
    }
}
