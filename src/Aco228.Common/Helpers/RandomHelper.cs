using MongoDB.Bson;

namespace Aco228.Common.Helpers;

public static class RandomHelper
{
    private static readonly Random _rng = new Random();

    public static T? RandomChance<T>(params Func<T>?[] choices)
    {
        // 1. Guard against null or empty input
        if (choices == null || choices.Length == 0)
            return default;

        // 2. Pick the index using our shared random instance
        int index = _rng.Next(0, choices.Length);

        // 3. Use the null-conditional operator for cleaner code
        // This executes the function if it's not null, otherwise returns default
        return choices[index] != null ? choices[index]!() : default;
    }
    
    /// <summary>
    /// var result = WeightedRandom(
    // (() => "You found a copper coin!", 0.8),  // 80% chance
    // (() => "You found a silver coin!", 0.15), // 15% chance
    // (() => "You found a GOLD coin!!",  0.05)  // 5% chance
    // );
    /// </summary>
    /// <param name="choices"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? WeightedRandom<T>(params (Func<T>? action, double weight)[] choices)
    {
        if (choices == null || choices.Length == 0) return default;

        double totalWeight = 0;
        foreach (var choice in choices) totalWeight += choice.weight;

        // Standard practice: Use a static Random or pass one in to avoid seed issues
        double roll = _rng.NextDouble() * totalWeight;

        double cumulative = 0;
        foreach (var choice in choices)
        {
            cumulative += choice.weight;
            if (roll <= cumulative) // Use <= to catch the edge case of 0 weight
            {
                // If action is null, return default(T), otherwise invoke it
                return choice.action != null ? choice.action() : default;
            }
        }

        return default; 
    }
}