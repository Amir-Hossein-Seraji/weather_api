# Technical Answers

---

### How much time did you spend on this task?

I spent time on this task over the span of two days, totaling approximately 7-8 hours. This time was divided between:
* Researching the OpenWeatherMap API documentation.
* Implementing the core API logic and all required endpoints.
* Refactoring the code to follow Clean Code principles (separating logic into a service, using DTOs, etc.).
* Improving the project with production-ready features like caching and structured logging.
* Writing the unit test and `README.md` file.
* Preparing the answers for these technical questions.

---

### If you had more time, what improvements or additions would you make?

The current solution meets the challenge requirements and already includes key features like **in-memory caching** and **structured logging**. If I had more time to make it a fully production-grade application, I would focus on these key areas:

1.  **Resilience & Reliability:** The app currently fails on a single network timeout. I would implement a resilience policy using a library like **Polly**. This would add **automatic retries** (e.g., retry 3 times with exponential backoff) for transient network errors. I would also add a **Circuit Breaker** to stop the app from hammering the external API if it's clearly down, allowing it to recover.

2.  **Security & Scalability:**
    * **Rate Limiting:** To protect the public endpoint from abuse or DDoS attacks, I would use .NET's built-in rate-limiting middleware. This would prevent a single IP from overwhelming the service.
    * **Distributed Cache:** The current `IMemoryCache` is per-server. If the app were scaled to run on multiple servers, I would replace this with a **distributed cache (like Redis)** to ensure all instances share one consistent cache.

3.  **Maintainability (Configuration):** I would improve configuration management for different environments. This would involve adding environment-specific files like `appsettings.Development.json` and `appsettings.Production.json` to manage different API keys, logging levels, or cache durations for "dev," "staging," and "production."

**A Note on Parallelization:**
One performance improvement I investigated was parallelizing the external API calls using `Task.WhenAll`. However, I discovered that the Air Pollution API call **depends on the coordinates** returned from the Current Weather API call. Because of this data dependency, the calls must be executed sequentially. If the calls were independent, parallelization would have been a high-priority addition.

---
### What is the most useful feature recently added to your favorite programming language? Please include a code snippet to demonstrate how you use it.

My favorite programming language is Python. The most useful feature added recently is **Structural Pattern Matching** (the `match`/`case` statement), which was introduced in Python 3.10.

Before this feature, if you had a function that needed to handle many different conditions or types of data, you would end up with a very long and hard-to-read chain of `if`/`elif`/`else` statements.

The `match`/`case` statement cleans this up beautifully. It's like a "super-powered `switch` statement" that can not only check for a value but also check the *structure* of the data (like a dictionary's keys or an object's properties).

This is incredibly useful in backend development, for example, when processing JSON data from an API or a message queue.

### Code Snippet

Here is a simple "before and after" to show how I use it.

#### **Before: The Old `if/elif` Way**
Imagine you are processing different "event" dictionaries. The code is clunky and you have to manually pull data out of the dictionary.

```python
def process_event_old(event):
    if event['type'] == 'create_user':
        # Manually get the username
        username = event.get('username')
        if username:
            print(f"Creating user {username}...")
    
    elif event['type'] == 'delete_user':
        # Manually get the user_id
        user_id = event.get('user_id')
        if user_id:
            print(f"Deleting user {user_id}...")
    
    else:
        print("Unknown event type")
```
#### **after: The new `mach/case` Way**
The code is much cleaner. The match statement can check the dictionary's structure and unpack the values into variables (username, user_id) all in one step.
```python
def process_event_new(event):
    match event:
        # Match a dict with 'type' and 'username' keys
        case {'type': 'create_user', 'username': username}:
            print(f"Creating user {username}...")
        
        # Match a dict with 'type' and 'user_id' keys
        case {'type': 'delete_user', 'user_id': user_id}:
            print(f"Deleting user {user_id}...")
        
        # The default case
        case _:
            print("Unknown event type")
```
### How do you identify and diagnose a performance issue in a production environment? Have you done this before?

While I have not yet had the chance to diagnose a performance issue in a live production environment, I have a clear theoretical understanding of the process I would follow. My approach would be in two phases: **Identify** and **Diagnose**.

**1. Identify the Problem (Is something slow?)**

First, I need to know a problem is happening. This is usually discovered in two ways:
* **Proactive (Alerts):** The best-case scenario. A monitoring dashboard (like a Grafana/Prometheus stack) would show a spike in a key metric and trigger an alert.
* **Reactive (User Reports):** Users would start to complain that "the site is slow."

Once alerted, I would immediately look at high-level monitoring dashboards to confirm the problem. The key metrics I'd check are:
* **Latency (Response Time):** Are API response times spiking? Which endpoints are affected?
* **Error Rate:** Are we seeing a rise in HTTP 500s or 4xxs?
* **Saturation (System Load):** Is the server's CPU at 100%? Is it running out of RAM or disk I/O?
* **Traffic (Throughput):** Is there a sudden, abnormal flood of requests (like a DDoS attack)?

**2. Diagnose the Root Cause (Why is it slow?)**

Once I've identified *what* is slow (e.g., "the `GET /weather` endpoint is taking 8 seconds"), my next step is to find *why*. This is where the **structured logging** I implemented in this project becomes critical.

* **Check the Logs:** I would filter the logs (using a tool like Splunk, ELK, or just `grep`) for the specific endpoint or time frame. The logs would show me the "story" of the request.
* **Isolate the Bottleneck:** I would look at the timestamps in the logs to see where the time is being spent.
    * **Is it our app?** Is a specific function or loop taking a long time?
    * **Is it the database?** Is a database query hanging?
    * **Is it an external API?** In our project, I would check if the log shows that the OpenWeatherMap API call is the part that's taking 8 seconds.

If the logs aren't specific enough, the final step would be to attach a **profiler** or use an **Application Performance Management (APM)** tool (like DataDog or New Relic) to get a line-by-line breakdown of where every millisecond is being spent in the code.

### What's the last technical book you read or technical conference you attended? What did you learn from it?

I am currently reading the **"OWASP Web Security Testing Guide (WSTG)"**.

I started reading it because I believe that building secure, reliable applications is a fundamental responsibility for a backend developer. It's not just an afterthought. I'm personally interested in understanding the *why* behind security best practices, rather than just "copying and pasting" a security solution.

The most important thing I've learned from it so far is the "why" behind common advice. For example:

* **Input Validation:** I learned it's not just about checking string length (like I did in this project). It's about actively preventing major attacks like **Cross-Site Scripting (XSS)** (by sanitizing output) and **SQL Injection** (by using parameterized queries, which EF Core does automatically).
* **Broken Access Control:** It's given me a framework for thinking about security beyond just logging in. For example, ensuring that a user who is logged in and asks for `.../api/data/123` is actually *authorized* to see item `123`, and can't just change the URL to `.../api/data/124` to see someone else's data.

Reading it has confirmed my "security-first" mindset and given me practical ways to be a more responsible backend developer.
### What's your opinion about this technical test?

I had a very positive opinion of this technical test.

I found the coding exercise to be excellent. It was a practical, real-world task that accurately reflects a backend developer's daily work—consuming a 3rd-party API, mapping data, and exposing it through a clean, new API. The requirements for "Clean Code principles" and a "unit test" showed me that the company values high-quality, maintainable code, not just a quick-and-dirty solution.

The technical questions were also a great addition. Questions like "if you had more time" or "how do you diagnose a performance issue" allowed me to demonstrate my thought process and my understanding of production-level concerns (like caching, logging, and security) that go beyond what's possible to code in a short time.

Overall, it was a fair and insightful challenge that I felt did a great job of evaluating a developer's practical skills and problem-solving mindset.
### Please describe yourself using JSON format.

```json
{
  "personalInfo": {
    "firstName": "Amir Hossein",
    "lastName": "Seraji"
  },
  "summary": "A Computer Engineering student with a foundational passion for coding, starting with Python in middle school. Eager to apply my problem-solving skills and grow as a backend developer.",
  "education": {
    "degree": "Bachelor of Science",
    "major": "Computer Engineering",
    "status": "In-Progress",
    "currentTerm": 5
  },
  "technicalSkills": {
    "primaryLanguage": "Python",
    "proficiencies": [
      "Django",
      "MySQL",
      "OOP",
      "Web Scraping",
      "Network",
      "BeautifulSoup4",
      "OpenCV",
      "Rubber Ducky Debugging"
    ],
    "learning": [
      "C#",
      ".NET"
    ],
    "interests": [
      "Backend Development",
      "API Design",
      "Web Security"
    ]
  },
  "goals": {
    "objective": "To secure a junior backend developer position where I can contribute to challenging projects, learn from experienced mentors, and develop high-quality, secure software."
  },
  "hobbies": [
    "Playing Music",
    "Exercising",
    "Tennis"
  ]
}
```