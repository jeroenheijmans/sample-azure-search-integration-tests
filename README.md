# Sample Azure Search Integration Tests

Repository to show how you could set up integration tests for Azure Search using xUnit and .NET Core.

## Getting started

> ⚠ If you use a _free_ tier search service, or a _basic tier with > 1 replica_, you need to adjust `WaitForIndexing` (see comments in method).

To run this solution:

- Set up an Azure Search Service in your Azure Portal, note an Admin API Key
- Create a file `AzureSearchExample.Tests/appsettings.dev.json` (it is in the `.gitignore` file) containing the `apiKey` and the `searchServiceName`
- Build
- Run the tests

All tests should be green.
If they're not, see the above warning, or debug things yourself...
