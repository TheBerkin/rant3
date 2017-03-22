#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;

using Rant.Core.IO;
using Rant.Localization;
using Rant.Vocabulary.Querying;

namespace Rant.Core.Compiler.Syntax
{
    [RST("quer")]
    internal class RstQuery : RST
    {
        private Query _query;

        public RstQuery(Query query, LineCol location)
            : base(location)
        {
            _query = query;
        }

        public RstQuery(LineCol location) : base(location)
        {
            // Used by serializer
        }

        public override IEnumerator<RST> Run(Sandbox sb)
        {
			return _query.Run(sb);
        }

        protected override IEnumerator<RST> Serialize(EasyWriter output)
        {
            output.Write(_query.Name);
            output.Write(_query.Subtype);
            output.Write(_query.Exclusive);
            output.Write(_query.FilterCount);
            foreach (var filter in _query.GetFilters())
                filter.Serialize(output);

            // Carrier
            if (_query.Carrier != null)
            {
                output.Write(true);
                _query.Carrier.Serialize(output);
            }
            else
                output.Write(false);

            // Complement
            yield return _query.Complement;
        }

        protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
        {
            _query = new Query();

            _query.Name = input.ReadString();
            _query.Subtype = input.ReadString();
            _query.Exclusive = input.ReadBoolean();

            // Read filters
            int filterCount = input.ReadInt32();
            for (int i = 0; i < filterCount; i++)
            {
                var filter = Filter.GetFilterInstance(input.ReadUInt16());
                if (filter == null) continue;
                filter.Deserialize(input);
                _query.AddFilter(filter);
            }

            // Carrier
            if (input.ReadBoolean())
            {
                _query.Carrier = new Carrier();
                _query.Carrier.Deserialize(input);
            }

            var complementRequest = new DeserializeRequest();
            yield return complementRequest;
            _query.Complement = complementRequest.Result;
        }

		public override string ToString()
		{
			return $"<{_query.Name}...>";
		}
	}
}