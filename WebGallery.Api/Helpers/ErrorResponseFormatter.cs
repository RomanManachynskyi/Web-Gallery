using System.Net;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;

namespace WebGallery.Api.Helpers;

public interface IErrorResponseFormatter
{
    (HttpStatusCode, ErrorResponse) GetErrorResponse(Exception exc);
}

public class ErrorResponseFormatter : IErrorResponseFormatter
{
    public (HttpStatusCode, ErrorResponse) GetErrorResponse(Exception exc)
    {
        var (key, description, httpStatusCode) = exc switch
        {
            SpecificationBuilderException sbExc => (string.Empty, sbExc.Description, HttpStatusCode.BadRequest),
            NotFoundException => (string.Empty, "Couldn't find an entity", HttpStatusCode.NotFound),
            AlreadyExistsException => (string.Empty, "Entity already exists", HttpStatusCode.Conflict),
            _ => (exc.GetType().Name, exc.InnerException?.Message ?? string.Empty, HttpStatusCode.InternalServerError)
        };

        var response = new ErrorResponse(key, exc.Message, description);

        return (httpStatusCode, response);
    }
}
