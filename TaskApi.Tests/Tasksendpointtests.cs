using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

public class TasksEndpointTests : IClassFixture<TaskApiFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TasksEndpointTests(TaskApiFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Post_CreatesTask_ReturnsCreatedWithLocation()
    {
        // Arrange
        var newTask = new { Title = "Write tests", IsDone = false };

        // Act
        var response = await _client.PostAsJsonAsync("/tasks", newTask);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Get_MissingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/tasks/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_AfterPost_ReturnsCreatedTask()
    {
        // Arrange
        var newTask = new { Title = "Check GET after POST", IsDone = false };
        var postResponse = await _client.PostAsJsonAsync("/tasks", newTask);

        var postBody = await postResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"POST status: {postResponse.StatusCode}");
        _output.WriteLine($"POST body: {postBody}");

        var created = JsonSerializer.Deserialize<TaskItem>(postBody, JsonOptions);
        _output.WriteLine($"Parsed created.Id: {created?.Id}");

        // Act
        var getResponse = await _client.GetAsync($"/tasks/{created!.Id}");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"GET status: {getResponse.StatusCode}");
        _output.WriteLine($"GET body: {getBody}");

        var fetched = JsonSerializer.Deserialize<TaskItem>(getBody, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Check GET after POST", fetched!.Title);
    }
}