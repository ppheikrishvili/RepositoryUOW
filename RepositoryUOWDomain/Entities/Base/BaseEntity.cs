using RepositoryUOWDomain.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryUOWDomain.Entities.Base;


public class BaseEntity : IBaseEntity
{
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //public int Id { get; set; }
    [NotMapped]
    public string? AttributeError { get; set; }

    //[NotMapped]
    //public int Id { get; set; }

    public bool IsValid(object? value) => string.IsNullOrWhiteSpace(this[$"{value}"]);

    [NotMapped]
    public string? this[string columnName]
    {
        get
        {
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(GetType().GetProperty(columnName)?.GetValue(this),
                new ValidationContext(this) { MemberName = columnName }, results);

            return valid ? null : results[0].ErrorMessage;
        }
    }

    public async Task<bool> IsValidAsync()
    {
        return await Task.Run(() =>
        {
            var validationContext = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(this, validationContext, results, true)) return true;
            AttributeError = string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage).ToArray());
            return false;
        }).ConfigureAwait(false);
    }


    public bool IsValid()
    {
        var validationContext = new ValidationContext(this, null, null);
        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(this, validationContext, results, true)) return true;
        AttributeError = string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage).ToArray());
        return false;
    }
}