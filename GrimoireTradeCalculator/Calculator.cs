using System;
using System.Collections.Generic;

namespace GrimoireTradeCalculator
{
    internal enum ExchangeMethod
    {
        Trade,
        Recycle
    }

    internal sealed class CalculationResult
    {
        public CalculationResult(int[] counts)
        {
            Counts = counts;
        }

        public int[] Counts { get; }

        public int this[int level] => Counts[level - 1];
    }

    internal class Calculator
    {
        private const int MinLevel = 1;
        private const int MaxLevel = 10;

        public IReadOnlyList<CalculationResult> GetEquivalentCombinations(
            int[] ownedCounts,
            int requiredLevel,
            string methodName)
        {
            return GetEquivalentCombinations(ownedCounts, requiredLevel, ParseMethod(methodName));
        }

        public IReadOnlyList<CalculationResult> GetEquivalentCombinations(
            int[] ownedCounts,
            int requiredLevel,
            ExchangeMethod method)
        {
            ValidateOwnedCounts(ownedCounts);
            ValidateLevel(requiredLevel, nameof(requiredLevel));

            long[] levelValues = BuildLevelValues(method);
            long targetValue = levelValues[requiredLevel - 1];
            int[] selectedCounts = new int[MaxLevel];
            List<CalculationResult> results = new();

            SearchCombinations(
                ownedCounts,
                levelValues,
                MaxLevel,
                targetValue,
                selectedCounts,
                results);

            return results;
        }

        private static void SearchCombinations(
            int[] ownedCounts,
            long[] levelValues,
            int level,
            long remainingValue,
            int[] selectedCounts,
            List<CalculationResult> results)
        {
            if (remainingValue <= 0)
            {
                int[] resultCounts = new int[MaxLevel];
                Array.Copy(selectedCounts, resultCounts, MaxLevel);
                results.Add(new CalculationResult(resultCounts));
                return;
            }

            if (level == 0)
            {
                return;
            }

            long currentValue = levelValues[level - 1];
            int maxUsableCount = (int)Math.Min(
                ownedCounts[level - 1],
                DivideAndRoundUp(remainingValue, currentValue));

            for (int count = 0; count <= maxUsableCount; count++)
            {
                selectedCounts[level - 1] = count;
                SearchCombinations(
                    ownedCounts,
                    levelValues,
                    level - 1,
                    remainingValue - currentValue * count,
                    selectedCounts,
                    results);
            }

            selectedCounts[level - 1] = 0;
        }

        private static long DivideAndRoundUp(long dividend, long divisor)
        {
            return (dividend + divisor - 1) / divisor;
        }

        private static long[] BuildLevelValues(ExchangeMethod method)
        {
            int previousLevelMultiplier = method == ExchangeMethod.Trade ? 3 : 2;
            long[] values = new long[MaxLevel];
            values[0] = 1;

            for (int level = 2; level <= MaxLevel; level++)
            {
                long value = values[level - 2] * previousLevelMultiplier;

                for (int lowerLevel = MinLevel; lowerLevel <= level - 2; lowerLevel++)
                {
                    value += values[lowerLevel - 1];
                }

                values[level - 1] = value;
            }

            return values;
        }

        private static ExchangeMethod ParseMethod(string methodName)
        {
            return methodName switch
            {
                "トレード" => ExchangeMethod.Trade,
                "リサイクル" => ExchangeMethod.Recycle,
                _ when Enum.TryParse(methodName, true, out ExchangeMethod method) => method,
                _ => throw new ArgumentException(
                    "トレード、リサイクル、Trade、Recycle のいずれかを指定してください。",
                    nameof(methodName))
            };
        }

        private static void ValidateOwnedCounts(int[] ownedCounts)
        {
            ArgumentNullException.ThrowIfNull(ownedCounts);

            if (ownedCounts.Length != MaxLevel)
            {
                throw new ArgumentException("レベル1から10までの10個の所持数を指定してください。", nameof(ownedCounts));
            }

            for (int i = 0; i < ownedCounts.Length; i++)
            {
                if (ownedCounts[i] < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(ownedCounts),
                        "所持数には0以上の値を指定してください。");
                }
            }
        }

        private static void ValidateLevel(int level, string parameterName)
        {
            if (level is < MinLevel or > MaxLevel)
            {
                throw new ArgumentOutOfRangeException(parameterName, "レベルは1から10までを指定してください。");
            }
        }
    }
}
