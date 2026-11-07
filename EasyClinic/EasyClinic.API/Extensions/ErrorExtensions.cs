using EasyClinic.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EasyClinic.API.Extensions;

public static class ErrorExtensions
{
    // للـ Controllers
    public static ObjectResult ToProblem(this ControllerBase controller, Error error) =>
        controller.Problem(
            statusCode: error.Type.ToStatusCode(),
            title: error.Code,
            detail: error.Description);

    // للـ Minimal APIs
    public static IResult ToProblem(this Error error) =>
        Results.Problem(
            statusCode: error.Type.ToStatusCode(),
            title: error.Code,
            detail: error.Description);
}