using Xunit;

namespace RestMock
{
    public class ConcurrentMocksTests
    {
        [Fact]
        public void RunTwoServersSimultaneously()
        {
            var mock1 = RestMockBuilder.New();
            mock1.Get("/foo").ReturnsOk();

            var mock2 = RestMockBuilder.New();
            mock2.Get("/bar").ReturnsOk();

            using (var server1 = mock1.Create())
            using (var server2 = mock2.Create())
            using (var client1 = server1.CreateHttpClient())
            using (var client2 = server2.CreateHttpClient())
            {
                Assert.NotEqual(server1.ListenUrl.ToString(), server2.ListenUrl.ToString());

                Assert.Equal(200, (int)client1.GetAsync("/foo").Result.StatusCode);
                Assert.Equal(200, (int)client2.GetAsync("/bar").Result.StatusCode);
                Assert.NotEqual(200, (int)client1.GetAsync("/bar").Result.StatusCode);
                Assert.NotEqual(200, (int)client2.GetAsync("/foo").Result.StatusCode);
            }
        }
    }
}