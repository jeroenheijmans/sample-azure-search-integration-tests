using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AzureSearchExample
{
    [SerializePropertyNamesAsCamelCase]
    public class PersonDto
    {
        [Key]
        [IsFilterable]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [IsSortable]
        [IsSearchable]
        public string FirstName { get; set; }

        [IsSortable]
        [IsSearchable]
        public string LastName { get; set; }

        [IsSortable]
        [IsSearchable]
        public string Email { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? IsActive { get; set; }

        [IsSortable]
        [IsFilterable]
        [IsFacetable]
        public DateTimeOffset? RegisteredAtUtc { get; set; }
    }
}
