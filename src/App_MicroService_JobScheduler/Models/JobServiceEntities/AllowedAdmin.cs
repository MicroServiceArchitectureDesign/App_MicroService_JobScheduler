using System.ComponentModel.DataAnnotations;

namespace App_MicroService_JobScheduler.Models.JobServiceEntities;

public record AllowedAdmin(
    [property: Key] long Id,
    string Email,
    string Password,
    DateTime LastLogin,
    string FirstName,
    string LastName);
