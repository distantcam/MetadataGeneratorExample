using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TheMetadataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var metadata = Assembly.GetExecutingAssembly()
                .GetTypes().Where(type => typeof(Metadata).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type => (Metadata)Activator.CreateInstance(type));

            foreach (var item in metadata)
            {
                File.WriteAllText(Path.Combine("..", "..", "..", "..", "TheProject", "Schema", item.Name + ".sql"), GenerateDatabaseSchema(item));
                File.WriteAllText(Path.Combine("..", "..", "..", "..", "TheProject", "Entities", item.Name + ".cs"), GenerateEntity(item));
                File.WriteAllText(Path.Combine("..", "..", "..", "..", "TheProject", "Validation", item.Name + "Validator.cs"), GenerateValidator(item));
            }
        }

        static string GenerateDatabaseSchema(Metadata metadata)
        {
            return Generator(metadata,
                m => @$"CREATE TABLE [dbo].[{m.Name}](
    [Id] [int] IDENTITY(1,1) NOT NULL,
",
                m => @$"
 CONSTRAINT [PK_{m.Name}] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)
)
GO",
                new Dictionary<string, Func<Field, string>>
                {
                    { "string", field => $"    [{field.Name}] [varchar](MAX) {(field.Required ? "NOT NULL" : "NULL")}," },
                    { "email", field => $"    [{field.Name}] [varchar](250) {(field.Required ? "NOT NULL" : "NULL")}," },
                    { "creditcard", field => $"    [{field.Name}] [char](8) {(field.Required ? "NOT NULL" : "NULL")}," },
                });
        }

        static string GenerateEntity(Metadata metadata)
        {
            return Generator(metadata,
                m => @$"
namespace TheProject.Entities {{
    public class {m.Name} : Entity {{",
                m => @"
    }
}",
                new Dictionary<string, Func<Field, string>>
                {
                    { "string", field => $"    public string {field.Name} {{ get; set; }}" },
                    { "email", field => $"    public string {field.Name} {{ get; set; }}" },
                    { "creditcard", field => $"    public string {field.Name} {{ get; set; }}" },
                });
        }

        static string GenerateValidator(Metadata metadata)
        {
            return Generator(metadata,
                m => @$"using FluentValidation;
using TheProject.Entities;

namespace TheProject.Validation {{
    public class {m.Name}Validator : AbstractValidator<{m.Name}> {{
        public {m.Name}Validator() {{",
                m => @"
        }
    }
}",
                new Dictionary<string, Func<Field, string>>
                {
                    { "string", field => $"            RuleFor(entity => entity.{field.Name}){(field.Required ? ".NotNull()" : "")};" },
                    { "email", field => $"            RuleFor(entity => entity.{field.Name}).EmailAddress(){(field.Required ? ".NotNull()" : "")};" },
                    { "creditcard", field => $"            RuleFor(entity => entity.{field.Name}).CreditCard(){(field.Required ? ".NotNull()" : "")};" },
                });
        }

        static string Generator(Metadata metadata, Func<Metadata, string> header, Func<Metadata, string> footer, Dictionary<string, Func<Field, string>> fieldGenerators)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(header(metadata));

            foreach (var field in metadata)
            {
                var constraints = field.Value.Split(' ');

                var fieldMeta = new Field
                {
                    Name = field.Key,
                    Type = constraints[0]
                };
                if (constraints.Contains("required"))
                {
                    fieldMeta.Required = true;
                }

                var found = false;
                foreach (var gen in fieldGenerators)
                {
                    if (fieldMeta.Type == gen.Key)
                    {
                        stringBuilder.AppendLine(gen.Value(fieldMeta));
                        found = true;
                    }
                }
                if (!found)
                {
                    throw new Exception($"Unknown field type from constraints '{field.Value}'");
                }
            }

            stringBuilder.AppendLine(footer(metadata));

            return stringBuilder.ToString();
        }

        class Field
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool Required { get; set; }
        }
    }
}
