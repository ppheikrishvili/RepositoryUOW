using System.ComponentModel;

namespace RepositoryUOWDomain.Shared.Enums;

public enum ResponseCodeEnum
{
    [Description("The operation was completed successfully")]
    Success = 0,

    [Description("System error")]
    InternalSystemError = 1,

    [Description("The requested resource was not found")]
    NotFound = 2,

    [Description("An error in data storage, the principle of uniqueness is violated")]
    NeedsReTransaction = 3,

    [Description("The requested application was not found")]
    ApplicationNotFound = 4,

    [Description("The requested organisation was not found")]
    OrganizationNotFound = 5,

    [Description("The parameters are invalid")]
    IncorrectParameters = 6,

    [Description("Verify the information or Restart the operation again")]
    NeedsReCallOrCheck = 7,
    
    [Description("The unknown Fatal Error")]
    UnknownFatalError = 8,

    [Description("The user has no appropriate rights")]
    ForbiddenAction = 9,

    [Description("The user canceled the action")]
    ActionCanceled = 10,
    
    [Description("Notification")]
    Notification = 11,

    [Description("The operation ended unsuccessfully")]
    NoSuccess = 12,

    [Description("No data selected")]
    NoData = 13,

    [Description("Data Validation error")]
    ValidationError = 14

}