# Welcome to the Team confidence workshop

After discussing about modernization strategy you are now ready to get back the control of the legacy code.

## Exercise 1 : Discover the external dependencies

### Step 1 : logging http calls

Identify in the test setup where the HttpClient is injected to setup logging.

### Step 2 : stub http calls

1. Open the file ```Subscription.Tests\SubscriberControllerTests.http.cs```
2. Go to ```Give_CreatedSubscriber_when_email_is_safe``` 
3. Use code coverage to find where the code stop working. 
4. Identify the stub data you need to make the code run properly.
5. Enrich the provided test data builder to generate the appropriate response from the external API.
6. Explore the code to find new tests cases to increase coverage

## Exercise 2 : Mock-it or fake-it

Choose the first strategy you would like to apply: using mocks or a fake in memory database.

### 2.a : Mocking the database

#### Step 1 : setup EF mocking

As this setup is quite time consuming, a dedicated WebApplicationFactory as been provided with the setup.

1. Open the file ```TestMockWebApplicationFactory.cs```
2. Compare with previous WebApplicationFactory you customized in previous exercise to identify the required setup for mocking.

#### Step 2 : Update existing tests

1. Open file ```Subscription.Tests/SubscriberControllerTests.mock.cs```
2. Run the test
3. Reuse test data builders from previous exercise to improve readability.
4. How is the result give you confidence the code is working? What should you assert to validate the code behavior?

5. Create a new test to validate the behavior of the code when a subscriber already exists. You need to be sure that the existing subscriber in database is returned. 

### 2.b : Faking the database

#### Step 1 : Setup the in memory database

1. Open ```test factory```
2. Identify how the database is configured

#### Step 2 : Update existing tests

1. Open test ```first test```
2. Run the test
3. Reuse test data builders from previous exercise to improve readability.
4. What is the result? Are you sure the code is behaving as expected? Which assertion in the test could you add to ensure a subscriber was persisted in database?

5. Create a new test to validate the behavior of the code when a subscriber already exists. You need to be sure that the existing subscriber in database is returned. 

### Reflect

With the properties from the test desiderata in mind, think about the trade off of the different approaches. Which constraints could push towards one decision over the other.
