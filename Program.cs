using System;
using CacheLib;
using System.Collections.Generic;
using static System.String;

// Requires C# 7.0.

class Program
{
    static void Main(string[] args)
    {
        Example.Instance.Test();
    }
}

internal sealed class Example
{
    private static readonly Example _instance = new Example();

    static Example()
    {
    }

    public static Example Instance
    {
        get
        {
            return _instance;
        }
    }

    // string = address
    // decimal = sales tax rate
    private readonly Cache<string, decimal> _cache = new Cache<string, decimal>(3); 

    // this is supposed to contain billions of addresses...
    private readonly IDictionary<string, decimal> _fakeData = new Dictionary<String, Decimal>();
    
    private Example()
    { 
    }

    /// <summary>
    /// Returns the tax rate for a given address.
    /// </summary>
    /// <param name="address">The street address.</param>
    /// <returns>The tax rate.</returns>
    public decimal Sales_tax_lookup(string address)
    {
        if (IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or whitespace only.", nameof(address));
        if (_fakeData.TryGetValue(address.Trim(), out decimal taxrate))
            return taxrate;
        else
            throw new ArgumentOutOfRangeException(nameof(address));
    }


    /// <summary>
    /// Returns the tax rate for a given address (cached version).
    /// </summary>
    /// <param name="address">The street address.</param>
    /// <returns>The tax rate.</returns>
    public decimal Fast_rate_lookup(string address)
    {
        // if cache has the desired address / tax rate, return it; otherwise call the slow function and add value to cache. 
        if (!_cache.TryGetValue(address, out decimal taxrate))
        {
            taxrate = Sales_tax_lookup(address);
            _cache.Add(address, taxrate);
            Console.WriteLine("slow call and addition to cache");
        }
        return taxrate;
    }


    public void Test()
    {
        for (decimal i = 1; i <= 10; i++)
        {
            _fakeData.Add($"{i} Main Street Seattle, WA", i / 100m);
        }
        foreach (var address in _fakeData)
        {
            Console.WriteLine(address);
        }

        var tests = new string[] { "3 Main Street Seattle, WA", "4 Main Street Seattle, WA", "5 Main Street Seattle, WA", "6 Main Street Seattle, WA", "5 Main Street Seattle, WA", "3 Main Street Seattle, WA"};

        Console.WriteLine("---------- sales_tax_lookup ------------");
        foreach (string address in tests)
        {
            Console.WriteLine("{0} --> {1}", address, Sales_tax_lookup(address));
        }
        Console.WriteLine("---------- fast_rate_lookup ------------");
        foreach (string address in tests)
        {
            Console.WriteLine("{0} --> {1}", address, Fast_rate_lookup(address));

            Snoop();
        }
        Console.ReadKey(true);
    }

    private void Snoop()
    {

        Console.WriteLine(nameof(_cache._internalStorage));
        foreach (var el in _cache._internalStorage)
        {
            Console.WriteLine("{0}: {1}", el.Key, el.Value);

        }
        Console.WriteLine(nameof(_cache._lru));
        foreach (var key in _cache._lru)
        {
            Console.WriteLine(key);
        }
        Console.WriteLine("=====================================");
    }
}