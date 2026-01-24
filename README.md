# DoToday
Why wait until tomorrow what you can do today? "Do Today" is just a simple task/checklist app where
line items are grouped into lists. The main intent is to be able to organize line items that can be
checked/unchecked so that important things don't get lost into the ether or delayed forever.

The original inspiration for this project was a take home exercise but I also wanted a low-barrier
task/shopping list app (ie. just go to the URL and click, no auth, no permissions, etc.) since I'm
terrible at remembering what I needed to buy and end up buying everything else when I go shopping.
For scaleability I'm intending this to just be a container image that can be deployed and if anyone
ends up wanting this functionality they can manage their own instance.

"DoToday" because I'm terrible at branding and naming.

## Architecture
This solution is comprised of two separate projects:
- .NET Core backend, written in C#, DoToday.Server
- React front end, written in TypeScript, dotoday.client

The intent is to have a containerizable lightweight application that is easily deployed and
maintained (ie. on a Raspberry Pi)

### DoToday Server
This is a pretty typical .NET Core WebAPI project leveraging the Entity Framework provider for
SQLite.

- Controller: API endpoints/routing layer
- Services: Business logic layer
- Repositories: Data access layer

### DoToday Client
Mobile friendly, React backed by TypeScript. The goal is to keep this as lightweight as possible,
aside from validation, there should be little to no business logic at this layer.

## Getting Started/Installation

## Developer Notes
### Entity Framework (ORMs)
My typical usage of EF (or any ORMs) is more of a placeholder - I like the speed for getting a
project set up and working but, more often than not, on more complicated projects the loss of
control over the SQL ends up being a pain. I've found EF and other ORMs typically work great...
until they don't. For the scale of the initial build of this project it's probably sufficient,
but this may be one of the first areas that gets refactored, especially since migrations as
features are added get kind of scary. Sprocs end up feeling better for me as the schema is very
intentional and changes are generally proper diffs instead of drop/creates.

### Outstanding Questions
- List names are unique, should tasks be unique per list?
- List deletion is currently permanent, is there a use case for a soft delete/hide?
- I couldn't decide if I needed to add a layer of abstraction around the API client calls in
  the React client... these are simple CRUD calls but if we ever need to combine API calls or
  want to add client side caching or optimistic updates then it might make some sense to do it
  but, for now, I don't necessarily need this so I'm going to keep it simple; it's a simple,
  stateless, lightweight client so we don't need to complicate things with making it a singleton
  or anything either.

### Up Next
The following features I either haven't had an immediate demand for or didn't have time to
implement yet. These features aren't necessarily guaranteed to be implemented, but this is kind
of a brainstorming area.
- List cloning
- More thorough validation - mainly leveraging Data Annotations, sufficient for simple validation
- More thorough testing 
- Auth/Users? - if we intend to support a multi-user scenario
- Details - if we add more fields, ie. a description, to tasks/lists, we'll need a view for it

