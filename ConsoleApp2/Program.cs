using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Viagogo

{   

    public class Solution
    {
       // private const string CacheKey = "closestCity";         
        private static Dictionary<string, int> _distanceCache = new Dictionary<string, int>();
        private static Dictionary<string, List<Event>> _cachedDictClosestCity = new Dictionary<string, List<Event>>();
        
        static void Main(string[] args)
        {

            var events = new List<Event>{
                        new Event{ Name = "Phantom of the Opera", City = "New York"},
                        new Event{ Name = "Metallica", City = "Los Angeles"},
                        new Event{ Name = "Metallica", City = "New York"},
                        new Event{ Name = "Metallica", City = "Boston"},
                        new Event{ Name = "LadyGaGa", City = "New York"},
                        new Event{ Name = "LadyGaGa", City = "Boston"},
                        new Event{ Name = "LadyGaGa", City = "Chicago"},
                        new Event{ Name = "LadyGaGa", City = "San Francisco"},
                        new Event{ Name = "LadyGaGa", City = "Washington"}
                        };

            //1. find out all events that are in cities of customer
            // then add to email.
            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            //----------------------------------------------------------------------------------------------------------------

            // 1. TASK    //query to get all events that matches the city
            var query = events.Where(x => x.City.Contains(customer.City)).ToList();            
            //send email
            query.ForEach(x=>AddToEmail(customer,x));

            //Or 
            //Task 1 Simplified answer 
            events.Where(x => x.City.Contains(customer.City))?.ToList()?.ForEach(x => AddToEmail(customer, x));

            //----------------------------------------------------------------------------------------------------------------

            //2. TASK
            //query to get 5 closest city to customer
            var closestCity =events.OrderBy(x=> GetDistance(customer.City,x.City))?.Take(5)?.ToList();
            //send email
            closestCity.ForEach(x => AddToEmail(customer, x));

            //Or 
            //Task 2 Simplified answer
            events.OrderBy(x => GetDistance(customer.City, x.City))?.Take(5)?.ToList()?.ForEach(x => AddToEmail(customer, x));

            //----------------------------------------------------------------------------------------------------------------
            //Task 3
            //If the GetDistance method is an API call which could fail or is too expensive, how will u improve the code written in 2 ? Write the code.
            //Solution is to cache the closest city result and call it anytime i want 
            var cachedClosestCity = new List<Event>();
            string CacheKey = customer.City;

            if (_cachedDictClosestCity.ContainsKey(CacheKey))
                cachedClosestCity = _cachedDictClosestCity?.FirstOrDefault(x => x.Key == CacheKey).Value;
            else
                _cachedDictClosestCity.Add(CacheKey, cachedClosestCity=events.OrderBy(x => GetDistance(customer.City, x.City))?.Take(5)?.ToList());

            //send your email
            cachedClosestCity.ForEach(x => AddToEmail(customer, x));


            //----------------------------------------------------------------------------------------------------------------
            //Task 4
            // If the GetDistance method can fail, we don't want the process to fail. What can be done?    Code it.
            //Solution : store each distance call in a cache dictionary and reuse  
            //Use fromcity to city as key 
            int distance = 0;
            
                events.ForEach(x => distance
                = _distanceCache.ContainsKey(x.City + customer?.City) == true
                ? _distanceCache.FirstOrDefault(y => y.Key == x.City + customer?.City).Value
                : GetDistance(customer.City, x.City)
                );
           

            //----------------------------------------------------------------------------------------------------------------
            //Task 5
            //order by price 
            cachedClosestCity.OrderBy(x => GetPrice(x));


        }

        // You do not need to know how these methods work
        static void AddToEmail(Customer c, Event e, int? price = null)
        {

            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }

        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }

        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {

            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }

            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }

            return result;

        }

    }

}


public class Event
{
    public string Name { get; set; }
    public string City { get; set; }
}

public class Customer
{
    public string Name { get; set; }
    public string City { get; set; }
}
