using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.EnumExtensions
{
    /// <summary>
    /// Add enum value descriptions to Swagger
    /// </summary>
    public class EnumDocumentFilter : IDocumentFilter
    {
        private readonly XPathNavigator[] _xmlNavigator;

        /// <inheritdoc />
        public EnumDocumentFilter(params XPathDocument[] xmlDoc)
        {
            if (xmlDoc == null)
                throw new ArgumentNullException(nameof(xmlDoc));

            _xmlNavigator = xmlDoc.Select(x => x.CreateNavigator()).ToArray();
        }

        /// <inheritdoc />
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var schemaDictionaryItem in swaggerDoc.Definitions)
            {
                foreach (var propertyDictionaryItem in schemaDictionaryItem.Value.Properties)
                {
                    var prop = propertyDictionaryItem.Value;
                    var propEnum = prop.Enum;
                    if (propEnum?.Any() ?? false)
                    {
                        prop.Description += AddDescribeForEnum(propEnum);
                    }
                }
            }

            foreach (var item in swaggerDoc.Paths.Values)
            {
                AddDescribeForEnumParameters(item.Parameters);

                new List<Operation> { item.Get, item.Post, item.Put }
                 .FindAll(x => x != null)
                     .ForEach(x => AddDescribeForEnumParameters(x.Parameters));
            }
        }

        private void AddDescribeForEnumParameters(IList<IParameter> parameters)
        {
            if (parameters == null)
                return;

            foreach (var param in parameters
                .OfType<NonBodyParameter>()
                .Where(w => w.Enum?.Any() ?? false))
            {
                param.Description += AddDescribeForEnum(param.Enum);
            }
        }

        private string AddDescribeForEnum(IEnumerable<object> enums)
        {
            var sb = new StringBuilder();
            Type type = null;

            foreach (var enumOption in enums)
            {
                type = type ?? enumOption.GetType();

                sb.AppendLine()
                    .Append(Convert.ChangeType(enumOption, type.GetEnumUnderlyingType()))
                    .Append(" = ")
                    .Append(Enum.GetName(type, enumOption));

                var summaryNode = _xmlNavigator
                    .Select(x => x.SelectSingleNode($"/doc/members/member[@name='F:{type.FullName}.{enumOption}']"))
                    .FirstOrDefault(f => f != null)?
                    .SelectSingleNode("summary");

                if (summaryNode != null)
                {
                    sb.Append(" (")
                        .Append(XmlCommentsTextHelper.Humanize(summaryNode.InnerXml))
                        .Append(")");
                }
            }

            return sb.ToString();
        }
    }
}
