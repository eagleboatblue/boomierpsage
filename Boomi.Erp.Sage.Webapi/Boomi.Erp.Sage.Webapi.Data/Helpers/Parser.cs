using System;
using System.Collections.Generic;
using System.Linq;

namespace Boomi.Erp.Sage.Webapi.Data.Helpers
{
    public static class Parser
    {
        public static List<string> SplitSentence(string sentence, int limit = 40)
        {
            var result = new List<string>();

            if (String.IsNullOrEmpty(sentence))
            {
                return result;
            }

            if (sentence.Length <= limit)
            {
                result.Add(sentence);

                return result;
            }

            string[] arr = sentence.Split(' ');
            var currentSentence = String.Empty;
            // reservation for whitespace
            var sentenceLimit = limit - 1;
            var lastIndex = arr.Length - 1;

            for (var i = 0; i < arr.Length; i ++)
            {
                var word = arr[i];
                var isLastWord = i == lastIndex;
                var exceedsLimit = (currentSentence.Length + word.Length) > sentenceLimit;

                // If sum of line and word together is less then a given limit
                // Then just concat them
                if (!exceedsLimit)
                {
                    if (!String.IsNullOrWhiteSpace(currentSentence))
                    {
                        currentSentence += " ";
                    }
                    
                    currentSentence += word;
                } else
                {
                    // If the sum is greater or equal
                    // Add current line to the result list
                    if (!String.IsNullOrWhiteSpace(currentSentence))
                    {
                        result.Add(currentSentence);
                    }

                    // If the word is less then the limit
                    // use it as a new current line
                    if (word.Length <= limit)
                    {
                        currentSentence = word;
                    }
                    else
                    {
                        // Otherwise reset the current line
                        currentSentence = "";

                        result.AddRange(Parser.SplitWord(word));
                    }
                }

                if (isLastWord)
                {
                    if (!String.IsNullOrWhiteSpace(currentSentence))
                    {
                        result.Add(currentSentence);
                    }
                }
            }

            return result;
        }

        public static List<string> SplitWord(string word, int limit = 40) {
            var result = new List<string>();

            if (String.IsNullOrEmpty(word))
            {
                return result;
            }

            if (word.Length < limit)
            {
                result.Add(word);

                return result;
            }

            var endRange = Math.Round(
                Math.Ceiling(Convert.ToDecimal(word.Length) / Convert.ToDecimal(limit))
            );

            for (var i = 0; i < endRange; i++)
            {
                var startIndex = i * limit;
                var endIndex = (i + 1) * limit;
                string phrase;

                if (word.Length > endIndex)
                {
                    phrase = word.Substring(startIndex, (endIndex - startIndex));
                } else
                {
                    phrase = word.Substring(startIndex);
                }

                result.Add(phrase);
            }

            return result;
        }
    }
}
