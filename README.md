# DoToday
Why put off to tomorrow what you can do today? "Do Today" is just a simple task/checklist app where
line items are grouped into lists. The main intent is to be able to organize line items that can be
checked/unchecked so that important things don't get lost into the ether or delayed forever.

The original inspiration for this project was a take home exercise but I also wanted a low-barrier
task/shopping list app (ie. just go to the URL and click, no auth, no permissions, etc.) that I
could use since I'm terrible at remembering what I needed to buy and end up buying everything else
when I go shopping. For scaleability I'm intending this to just be a container image that can be
deployed and if anyone ends up wanting this functionality they can manage their own instance.

The notes in this doc serve as an explanation to reviewers of some of my thought processes, some of
the assumptions I made and reasoning behind the opinions that shaped this code. Because this is a
public repo, I also plan to use it as an educational tool, as well as a learning tool in my
personal community of developer friends as a visual tool/discussion points, especially as some of
our more junior friends skill up.

"DoToday" because I'm terrible at branding and naming.

## Design goals
* Backend API using .NET Core
* Persistent storage using SQLite
* Front end written with React
* A lightweight, simple to build and deploy application (potentially in a container that can run on
  a Raspberry Pi)

## Architecture
This solution is comprised of the following projects:

### DoToday.Server
The background API project is comprised of 3 layers:
- Controllers - API endpoints, routing
- Services - Business logic, validation
- Repositories - Data Access (Entity Framework and SQLite)

### dotoday.client
The front end is written in React using TypeScript and Vite, leveraging:
- React Query for data fetching, caching and mutations
- Material UI for easy theming and components
- Vitest + MSW for unit testing and mocking service responses

### DoToday.Server.Tests
Unit test project using XUnit and Moq.

## Getting Started/Installation

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Node.js](https://nodejs.org/) (LTS recommended)

### Option 1: Visual Studio (Recommended)
1. Open `DoToday.sln` in Visual Studio
2. Press **F5** or click the green play button to run

Visual Studio should automatically perform the following:
- Restore NuGet packages for the .NET backend
- Run `npm install` for the React frontend
- Start both the backend API and frontend dev server
- Open your browser to the application

If there are errors in the output window you may have to manually restore NuGet packages and
install the client dependencies.

### Option 2: Command Line
1. Navigate to the project root and restore backend dependencies:
   ```bash
   dotnet restore
   ```

2. Install frontend dependencies:
   ```bash
   cd dotoday.client
   npm install
   cd ..
   ```

3. Start the backend API server (keep this terminal open):
   ```bash
   dotnet run --project DoToday.Server
   ```
   The API will start on `https://localhost:7088`.

4. Open a **new terminal window**, navigate to the frontend folder, and start the frontend
   dev server:
   ```bash
   cd dotoday.client
   npm run dev
   ```
   The frontend will start on `https://localhost:56604` (or another port shown in the terminal).

5. Open your browser to the frontend URL (e.g., `https://localhost:56604`). The app should
   now be running with the frontend connecting to the backend API.

### Option 3: Docker
1. Ensure [Docker](https://www.docker.com/get-started) is installed and running

2. Build and start the container:
   ```bash
   docker-compose up --build
   ```
   Or run in detached mode:
   ```bash
   docker-compose up --build -d
   ```

3. Open your browser to `http://localhost:8080`

The container uses a Docker volume (`dotoday-data`) to persist the SQLite database between
restarts.

To stop the container:
```bash
docker-compose down
```

### Running Tests

**Backend tests:**
Select `Run All Tests` from the `Test` menu in Visual Studio or:

```bash
dotnet test
```

**Frontend tests:**
```bash
cd dotoday.client
npm test
```

## Developer Notes
The goal of this project was to create a small to-do task management app, but I decided that I
could use a little more organization in my own life, so I added a layer that was not part of the
initial problem statement: organization of tasks into uniquely named lists. This makes this a more
usable for my personal life, and I have it in a container deployed onto my network behind a Caddy
reverse proxy (behind OAuth) so I can access it on the go.

On the server side, this is a pretty straightforward .NET Core WebAPI project leveraging the Entity
Framework provider for SQLite. I typically build services with the following layers:

- Controllers: API endpoints/routing
- Services: Business logic
- Repositories: Data access

The controllers should be lightweight, business logic shouldn't happen at this level. They really
just exist to expose the endpoints and transport the DTOs to the caller.

Similarly, the repositories should be just simple data access: map the model to a data store type 
(if necessary) and make the appropriate call.

The services are where the magic happens - business logic should be implemented at this level. Any
additional validation (ie. uniqueness of the list names) would happen here. This project is pretty
light, but if there was complex business rules, they would be implemented here.

The front-end should be as lightweight as possible, aside from validation, there should be little
to no logic in this client.

The client is a mobile friendly, React TypeScript using Material UI since it provides a declarative
and expressive (enough) which lets me get a "good enough" looking user interface up quickly. In the
past, Material has proven cumbersome for highly custom UIs and there are some pain points where the
theming feels like it's fighting the defaults (hence the "!important" on the margin-top reset for
the MuiButtonBase, but for this project it's a small cost for a presentable UI.

### Trade offs and Assumptions
#### Database/Data Storage
SQLite and Entity framework are sufficient for a quick prototype project but certainly are one of
the first areas I would look at if this were intended to be a production MVP, I would look at
using SQL Server or MySQL and migrate the EF code to leverage sprocs that are versioned and
controlled/reviewable in source control. Connection pooling and read replicas are also things to
consider if the application is expected to get real load/traffic.

#### API Hardening
As traffic and visibility grows, rate limiting on the APIs... similarly optimistic updates (and
disabling controls during mutation) could be easily added since React Query supports it, though
the code is pretty verbose and, for the scale of this project, it doesn't feel necessary yet.

If the APIs are going to change, it would be a good idea to introduce API versioning  
(ie. /api/v1/lists) to allow for backwards compatibility to protect against breaking changes,
though we'd also want to define an SLA for how long/how many versions back will be supported.

#### Observability
Adding logging/reporting and telemetry/analytics, especially to a central log aggregator is a
must-have for servers running in production. Adding a health check endpoint, especially if this
is going to scale to a server farm behind a load balancer, but these can be useful for custom
dashboards/monitors as well. Also, this implementation is currently very light on error handling
and there is certainly an area that could use some attention.

#### Infrastructure
While there are no real "secrets" (ie. passwords, etc.) in this project currently, secrets
management is something to consider; the hard coded Data Source in the API project is a pretty
strong code smell. Similarly there are really only development environments configured at the
moment, as I containerize this project that will probably change, but as of now that still needs
to be done, so this is really only "dev ready" at the moment. I'd also like to set up CI/CD but,
as there are no environments yet, there is nowhere to deploy to continuously and I don't have a
build agent set up anywhere. Integration tests and maybe some end-to-end automation would also
be nice to have.

### AI (Claude) Usage
I used this as an opportunity to experiment with Anthropics Claude AI assistant, though in a very
limited capacity. Previously I've used webpack and jest pretty extensively, this was my first
project using Vite (it's great) but I figured since I was Googling everything (ie. setting up
vitest, etc.) I'd see how well Claude did on creating the client tests so I prompted it to test
the pages and AddItem control. Honestly, I'm pretty impressed with the result of the tests though
it took some finessing to get it to where it's at. It should be no big surprise, but the generated
setup and server code is all essentially identical to what I found online and the handlers follow
the same model from the blog post I was referencing when manually setting this up at TypeOnce.dev
(link in References below.)

Personally, I prefer as much of the test code to live with each test as possible; in my opinion,
tests should be as fully self documenting as possible, even if it ends up violating DRY, but I
like seeing the setup, the inputs and the expected files in one place; scrolling back and forth
even in one large file to see what a test is doing is usually a code smell for me that the test
might be doing too much, and the mock service worker (server.ts and handlers.ts) being in
separate files feels like it's toeingthat line.

I ended up having a small bug because the OpenAPI spec wasn't including the 201 response type,
which caused NSwag to treat 201s as errors. I recognized this from having run into it before but
I prompted Claude to investigate and while it did correctly identify the root problem, its
approach to solving the issue was more of a hack. The first suggestion it had was to modify the
generated client code; then it wanted to update the returned response code from the create
endpoints to 200 instead of using 201. By the time it suggested what I consider the "correct"
fix: adding the ProducesResponseType attribute to the endpoints so that the spec would include
the 201 response code, I had already just done it manually.

### Entity Framework (ORMs)
My typical usage of EF (or any ORMs) is more of a placeholder - I like the speed for getting a
project set up and working but, more often than not, on more complicated projects the loss of
control over the SQL ends up being a pain. I've found EF and other ORMs typically work great...
until they don't. For the scale of the initial build of this project it's probably sufficient,
but this may be one of the first areas that gets refactored, especially since migrations as
features are added get kind of scary. Sprocs end up feeling better for me as the schema is very
intentional and changes are generally proper diffs instead of drop/creates. ORMs are not an
excuse to not know SQL.

### Outstanding Questions/Issues
- List names are unique, should tasks be unique per list?
- List deletion is currently permanent, is there a use case for a soft delete/hide?
- I couldn't decide if I needed to add a layer of abstraction around the API client calls in
  the React client... these are simple CRUD calls but if we ever need to combine API calls or
  do anything more complex than we're doing now this might be a necessarily change, I don't
  necessarily need this so I'm going to keep it simple; it's a simple, stateless, lightweight
  client so we don't need to complicate things with making it a singleton or anything either
  and we get a lot of things like caching and optimistic updates through the hooks in React
  Query.

### Possible v.Next work
The following features I either haven't had an immediate demand for or didn't have time to
implement yet. These features aren't necessarily guaranteed to be implemented, but this is kind
of a brainstorming area.
- List cloning
- Soft delete/hiding lists/tasks
- More thorough validation - mainly leveraging Data Annotations, sufficient and simple for now
- More thorough testing - the server side testing is essentially just a set of simple success/fail
  tests for each endpoint/method and I'm sure there are boundary tests that are missing like long
  names and null/empty names (though those are blocked at the controller level by Data Annotations,
  the service could still test for those values) but in the interest of time and brevity, those
  tests have been omitted for now.
- Auth/Users? - if we intend to support a multi-user scenario, I'm personally handling auth through
  oauth in my Caddy reverse proxy on my deployment but if we want per user lists we'll need to
  integrate.
- Audit logging - (Who added what? Who said something was done?)
- Details view - if we add more fields, ie. a description, to tasks/lists, we'll need a view for it

### TO DO
[x] - Containerize
[x] - Real time updates

## References
This is not a comprehensive list of web resources, I looked up syntax for things like Docker and
Docker compose syntax, Material UI references, etc. but those were more of refreshers/reference
lookups, this is more to track the interesting reads for this project for myself, for future
reference:

- React Query - https://tanstack.com/query/latest
- Vite/ViTest - https://www.typeonce.dev/course/effect-beginners-complete-getting-started/testing-with-services/vitest-and-msw-testing-setup
