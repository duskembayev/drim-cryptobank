using System.Net;
using System.Text.Json;
using FastEndpoints;
using JetBrains.Annotations;
using Shouldly;

namespace cryptobank.api.tests.extensions;

public static class TestResultExtensions
{
    [ContractAnnotation("res:null => halt")]
    public static void ShouldBeOk<TResult>(this TestResult<TResult> res)
    {
        ShouldBeWithStatus(res, HttpStatusCode.OK);
    }

    public static void ShouldBeProblem(this TestResult<ProblemDetails> res, HttpStatusCode statusCode, string code)
    {
        ShouldBeWithStatus(res, statusCode);

        res.Result.ShouldNotBeNull();

        var element = (JsonElement?) res.Result.Extensions["code"];
        var actualCode = element?.GetString();

        actualCode.ShouldBe(code);
    }
    
    public static void ShouldBeValidationProblem(this TestResult<ProblemDetails> res, HttpStatusCode statusCode, string property, string code)
    {
        ShouldBeWithStatus(res, statusCode);

        res.Result.ShouldNotBeNull();

        var element = (JsonElement?) res.Result.Extensions["errors"];
        var actualCode = element?.EnumerateArray()
            .Single(e =>
            {
                var eProperty = e.GetProperty("property").GetString();
                return eProperty != null && eProperty.Equals(property, StringComparison.OrdinalIgnoreCase);
            })
            .GetProperty("code")
            .GetString(); 

        actualCode.ShouldBe(code);
    }

    [ContractAnnotation("res:null => halt")]
    public static void ShouldBeWithStatus<TResult>(this TestResult<TResult> res, HttpStatusCode statusCode)
    {
        res.ShouldNotBeNull();
        res.Response.StatusCode.ShouldBe(statusCode);
    }
}