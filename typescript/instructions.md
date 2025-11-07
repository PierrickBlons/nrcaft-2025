# Welcome to the Team confidence workshop

After discussing about modernization strategy you are now ready to get back the control of the legacy code.

## Exercise 1 : Discover the external dependencies

### Step 1 : setup MSW

1. Open the file `/src/subscription/subscriber-controller-01-msw.test.ts `
2. Add MSW handlers in tests by using the `setupServer` function from `msw/node`
3. Run the tests
4. Check in the logs the http calls made by the code

### Step 2 : stub http calls

1. Ignore calls made by supertest on localhost
2. Add a passthrough handler for localhost

```
http.all('http://127.0.0.1*', () => passthrough())
```

3. Run the test and check the output to see which calls needs to be intercepted
4. Add a specific handler for the call to be intercepted
5. Run code coverage to identify where the code fail, to identify the data required for the stub
6. Update the handler
7. Use test data builders to improve test readability
8. Explore the code to find new tests cases

## Exercise 2 : Mock-it or fake-it

Choose the first strategy you would like to apply: using mocks or a fake in memory database.

### 2.a : Mocking the database

1. Create a mock of the PrismaClient using `mockDeep` from `vitest-mock-extended`
2. Inject the mock in the nest.js setup
3. Ensure mocks are reset after each test
4. Update the test to validate the behavior and usage of the prisma create function on the subscribers table.

5. Create a new test to validate the behavior of the code when a subscriber already exists. You need to be sure that the existing subscriber in database is returned.

### 2.b : Faking the database

A global setup of pglite is already done in the solution. The global prismaClient is accessible from the test through `global.prisma`

1. Inject the global prisma fake database in the test.
2. Run the tests
3. What is the result? Are you sure the code is behaving as expected? Which assertion in the test could you add to ensure a subscriber was persisted in database?

4. Create a new test to validate the behavior of the code when a subscriber already exists. You need to be sure that the existing subscriber in database is returned.

### Reflect

With the properties from the test desiderata in mind, think about the trade off of the different approaches. Which constraints could push towards one decision over the other.
