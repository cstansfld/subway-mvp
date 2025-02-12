# subway-mvp

## Description

```
This is a MVP for a subway rest api. It is a simple solution for organizing features using Clean Architecture techniques.
These are lessons learned from many courses participated in to [Elevate Your .NET Skills To The Next Level](https://www.milanjovanovic.tech/).
```
### Subway Mvp - Fresh Menu Meal of The Day
#### Build an Api Endpoint
1.	This endpoint will return the meal of the day list (Endpoint all)
	- Monday: Cold Cut Combo
	- Tuesday: All-Pro Sweet Onion Chicken Teriyaki
	- Wednesday: Meatball Marinara
	- Thursday: All-New Baja Chipotle Chicken
	- Friday: Tuna
	- Saturday: The Ultimate B.M.T.
	- Sunday: The Philly
2.	This endpoint will be queried by a pipeline but could also be available if other uses are determined (Endpoint mealoftheday)
	- Inputs optional DateTime(Type string) and optional Meal(Type string)
	- When Meal is provided it must be 1 of the meals in the above list
	- Meal input can be case insensitive
	- Date Formats 2004-09-16T23:59:58.75565, 2025-09-16
	- Invalid 2004-09 or 2004
3.  Late addition:

4. The marketing department came up with a few ideas:
	- Make sure we support the following for the mealoftheday endpoint
		1. Promotion: on your birthday what was the mealoftheday is it the featured mealoftheday
		2. Promotion: for some day in the future what would the mealoftheday be
		3. 	The Meal of the day dto should be enough to handle all the requirements (I memory for this exercise - should be docker with database)

5. Notes:
    ##### Since the data for the meal of the day list doesn’t change it should be cached
	1. An embedded RavenDb has been added
	2. A hosted service has been added to seed the data and cache (Hybrid .net9 pre-release) the data on first setup
	3. What is nice about this is a redis cache canb be added as well and use l1 local and l2 redius cache
	4. This service shuts down after seeding and caching the data
	5. An application lifetime service service has been added to capture shutdown events
    ##### Output caching: 
	1. The caching behavior is configurable on the server.
	2. The client doesn't override the caching behavior that you configure on the server. (Default headers in some browser max-age 0)
    3. Resource locking ensures that all requests for a given response wait for the first request to populate the cache.
	4. Cache revalidation minimizes bandwidth usage (Bonus)
	#### Api Versioning
    #### Brotli Compression / GZIP Compression content (br favored)
	#### HealthChecks - HttpStatus and Error Tracking
	#### Unhandled Error Handling
	#### UnitTests Supporting use cases 

