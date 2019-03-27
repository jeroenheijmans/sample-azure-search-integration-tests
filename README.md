# Sample Azure Search Integration Tests

Repository to show how you could set up integration tests for Azure Search using xUnit and .NET Core.

## Getting started

To run this solution:

- Set up an Azure Search Service (free tier is fine) in your Azure Portal, note an Admin API Key
- Create a file `AzureSearchExcample.Tests/ApiKey.txt` containing that key (it is in the `.gitignore` file)
- Build
- Run the tests

All tests should be green.
