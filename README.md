# Swashbuckle.AspNetCore.EnumExtensions
Enum extension for swagger model

Example configyration

```csharp
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Info
    {
        Title = "Title",
        Version = "v1",
        Description = "Description",
    });

    var xmlFirst = new XPathDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "First.xml"));
    var xmlSecond = new XPathDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Second.xml"));
    var xmlThird = new XPathDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Third.xml"));

    options.IncludeXmlComments(() => xmlFirst);
    options.IncludeXmlComments(() => xmlSecond);
    options.IncludeXmlComments(() => xmlThird);

    options.DocumentFilter<EnumDocumentFilter>(new object[] { new[] { xmlFirst, xmlSecond, xmlThird } });
});
```
