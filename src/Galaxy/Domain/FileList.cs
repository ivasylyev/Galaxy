﻿using System;
using System.Linq;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Domain
{
    public class FileList
    {
        public static readonly FileList Empty = new FileList(new string[0]);
        [JsonProperty]
        private readonly string[] _patterns;

        [JsonConstructor]
        public FileList(string[] patterns)
        {
            _patterns = patterns;
        }

        public override string ToString()
        {
            if (_patterns == null || _patterns.Length == 0)
            {
                return string.Empty;
            }
            return string.Join(Environment.NewLine, _patterns);
        }

        public FileList Clone()
        {
            return new FileList(_patterns);
        }

        public bool IsMatched(string entry)
        {
            for (int patternIndex = 0; patternIndex < _patterns.Length; patternIndex++)
            {
                var pattern = _patterns[patternIndex];
                if (entry.EndsWith(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static explicit operator FileList(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return new FileList(new string[0]);
            }

            var patterns = candidate
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

            return new FileList(patterns);
        }
    }
}