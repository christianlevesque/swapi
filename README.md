# Star Wars App

The requested Star Wars API application contains two .NET projects: the app itself, and unit tests.

## Overall development philosophy

The app is, needless to say, a little over-engineered. I did this on purpose. This app is designed in such a way that it could easily be integrated into a larger application, such as an ASP.NET app. Many parts of the app are arranged so there's room to grow in the application. The Service layer classes, for example, use a local cache instead of a full repository - but just a little bit of work could transition the app over to using persistent storage. I had originally written a simple repository system to abstract the data layer, but that seemed like overkill (considering the application is going to be deleted in a week anyway) so I decided to implement a simple local cache instead. I tried to strike a balance between over-engineering to "allow for growth" and simple, pragmatic choices betraying this application's pending demise.

## Sorting justification

As discussed, the sorting rules you defined can't *strictly* be applied to the application, so some decisions had to be made.

### Planet sorting

I decided to sort based on homeworld. I did this because while we know Naboo appears in AotC, and we know Captain Typho is from Naboo **and** appears in AotC, we don't know programmatically that Captain Typho ever appears on Naboo in AotC. This might be a fair assumption based on his homeworld, but he also appears on Coruscant in AotC and there's no reason we might make that connection based on the API itself.

#### Unknown planet

If the API doesn't have homeworld data on a character, it returns `unknown`. For simplicity, I sorted `unknown` with the rest of the planets alphabetically.

### Age sorting

Because each character is only meant to appear once, but many characters appear in multiple movies (and have multiple valid ages), I decided to sort based on absolute age rather than movie-relative age. I chose an arbitrary date in the future (4000 ABY) and generated a timespan relative to that date.

You did not specify whether to sort by age descending or ascending, so age is sorted ascending. In my submission, the ages were sorted descending, but I changed this for consistency.

#### Unknown age

If the age of a character is unknown, their age is considered 0. These characters will always appear first in age sorting. 

## Design decisions

Here, I justify why I made certain decisions as opposed to others when faced with multiple **valid** options.

### Location of API calls

There are several valid locations for third-party API calls. Many applications will put third-party API calls in the Data layer as long as those APIs are called solely for data purposes. This is the case here - the SWAPI is essentially another data repository - but I decided to put the API calls in the Service layer so more complex logic could be used in response to the results of the API calls. In my opinion, a repository should only return the requested entity or `null`, but since more can happen with API calls than simply not finding the entity, I needed more flexibility with the logic. Logic belongs in the Service layer.

### Data models

The data pulled in from the API is simple, so it would be pretty trivial to strip off the data we need as we access it instead of creating classes to which to deserialize the JSON. However, I chose to deserialize to simple classes for three reasons:

1. In any app larger than this one, access to the API will be more frequent. Manually pulling in the data instead of deserializing to a class quickly becomes cumbersome.
2. This app is not intended to grow, but any **real** app needs to either assume growth or be written so it can accommodate growth. Creating data models for deserialization does both.
3. Simple classes are cheap and have virtually no impact on runtime performance in this case, so the improved developer experience is well-justified.

### No data layer

While I wanted to demonstrate how I might go about designing code in a larger app, I also had to keep things practical. I originally wrote a simple Repository pattern wrapper around the data stored in the Service layer, but in the moment it just seemed silly to write a wrapper around a `Dictionary` - even for demonstration - instead of just using a `Dictionary` in the first place. So I decided to scrap it.

### No base `CacheableService` class

The amount of code that would have been saved by creating a base class for `FilmService`, `PlanetService`, and `PeopleService` is minimal. Furthermore, one of my guiding purposes in the design of this app was to allow it to grow more easily. Creating a base `CacheableService` class would have been counterproductive in my opinion, because if the app had grown any further, I would have created a datalayer instead of using a `Dictionary<string, TEntity>` anyway - and once the datalayer was introduced, the `CacheableService` class would have to be refactored out of the code.

### `____Service` classes don't extend `SWAPIService`

There are several strategies that I could have taken in regards to the relationship between the API endpoint service classes. These could have extended a `CacheableService`, which I already rejected. These could also have extended a single interface shared between them, which I chose not to do because in a larger app, each service would likely have its own methods other than wrappers around `ISWAPIService.Fetch()`. I could have had a base interface and extended it for each service, but I decided not to simply because it wasn't needed and at the time I didn't feel it would demonstrate much about how I code (especially since I can just explain here why I didn't do it).

Another option that seems obvious is to have each endpoint service inherit from `SWAPIService`. However, I didn't do this for a couple reasons. Firstly, `SWAPIService` defines valid endpoint actions like GET - each endpoint service only *consumes* those actions. Little would be gained from inheriting from `SWAPIService` directly, and I feel this would violate Separation of Concerns. Secondly, by having `SWAPIService` independent from each endpoint service, this would allow me to register `SWAPIService` as a Typed Client in the DI container (if I had been using a true DI container, that is), a pattern I've grown fond of over the last year or so.

## Testing

The app includes unit tests with 95% line coverage and 93% branch coverage. An HTML coverage report is generated by running the `coverage.ps1` script found in the repo root. I decided not to do any integration testing because the app itself only does a single thing - the proof is in the pudding, as it were, and it either works or it doesn't.

### Uncovered lines

The application has 338 coverable lines. 15 of those are listed as uncovered by Cobertura, but 10 of them are actually completely covered by tests. Coverlet has always been a little buggy, and it doesn't seem to handle switch expressions well - in `StarWarsApp.Services.FilmService`, a method that translates between API film numbers and episodic film numbers returns from a switch expression, accounting for 10 "uncovered" lines.

The last 5 lines are truly uncovered. In `StarWarsApp.Services.SWAPIService`, a `catch` block is uncovered because API results are deserialized with `System.Text.Json.JsonSerializer`, which is static. I was unable to stimulate any exceptions other than a `JsonException` so the general `Exception` block is untested. It's simple and I'm confident that it would work as expected.

### Uncovered branches

The application has 79 branches. 5 of those are uncovered, but only one branch is genuinely uncovered due to well-known bugs in Coverlet regarding branch coverage, especially in regards to `async` code.

In `StarWarsApp.Program`, there are two uncovered branches. One is the `async` branching bug in Coverlet (line 35) and the other is in the `InitApp` method, which only exists to allow unit testing of `StarWarsApp.Program` anyway - so I didn't feel the need to pursue further coverage. This second branch is the only genuinely uncovered branch in the program.

In `StarWarsApp.Services.FilmService`, two uncovered branches also coincide with the 10 uncovered lines a few paragraphs ago. Since it's a `switch` statement, the uncovered branches are for the valid case and the default case. Both of these are actually covered in unit tests.

In `StarWarsApp.Services.CSVWriterService`, one branch is uncovered due to the `async` branching bug in Coverlet.