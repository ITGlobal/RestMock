RestMock
========

A small library to mock HTTP services. Its primary goal is to simplify writing integration tests for RESTful microservices.

Usage
-----

First, create a new `RestMockBuilder` object:

```csharp
var mock = RestMockBuilder.New();
```

Then, configure all HTTP endpoints you need:

```csharp
mock.Verb("GET").Url("/items/{id}").Returns(context =>
{
    var id = context.GetRouteValue("id");
    var json = new { id = id, content = "This is a fake object" };
    context.Header("X-Server", "RestMock");
    context.WriteJson(200, json);
});

// You may also use helpful extension methods:
mock.Post("/items").Returns(200);
mock.Put("/items/{id}").ReturnsJson(new { msg = "It's a fake object" });
```

You may also import a [swagger](http://swagger.io/specification/) JSON file:

```csharp
var swagger = @"
   /* Put swagger content here */
";
mock.ImportSwaggerJson(swagger);
```

This code will take all pathes and operations and define corresponding mocks. Mocks will respond with JSONs generated from swagger schemes.


Finally, you may create a instance of HTTP server with configured mocks:

```csharp
using(var server = mock.Create())
{
    var url = server.ListenUrl;
    // TODO Make some HTTP requests here
}
```

License
-------

[MIT](LICENSE)
