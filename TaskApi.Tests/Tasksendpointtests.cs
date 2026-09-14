using System.Net;
using System.Net.Http.Json;
using Xunit;

public class TasksEndpointTests : IClassFixture<TaskApiFactory>
{
    private readonly HttpClient _client;

    public TasksEndpointTests(TaskApiFactory factory)
    {
        _client = factory.CreateClient();
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
        var created = await postResponse.Content.ReadFromJsonAsync<TaskItem>();

        // Act
        var getResponse = await _client.GetAsync($"/tasks/{created!.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<TaskItem>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Check GET after POST", fetched!.Title);
    }
}