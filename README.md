# SimpleResults
It is just simple implementation of results pattern which in last year I was using a lot. 
Below couple of words how to use this library, and some practicies I follow when using it 

## Failure usage
Create failure object to describe error or warnning. Constrcutor needs: 

- Code - a string to indentify the error. Ideally should be unique in code base. 
- FailureType - enum to identify severity of the error. It can be either critical (error) 
or non-critical warning but it is up to the developer to decide how to handle the error. 
- Message - text with description of the failure.
- Description - (optional) more detailed description, it can be good place to put the callstack 
of exception, or steps to mitigate the failure.   

Example of failure initialization: 
```csharp
public static class ParsingFailures
{
	public static Failure FailureWhenParsingTag(string text) => 
		new Failure(
			"012", 
			FailureType.Error, 
			"Tag text is null or empty.",
			$"Tag text:{text} is null or empty.");
}
```

Some practicies regarding failures: 
group failures in one static class (for simple projects) or if needed make it more granular: 
per project, per layer, per operation. Just remember too much granularity in this case can make it hard to maintain.
Grouping them this way give couple of advantages:
- Failure codes are easier to maintain in particular when they are assigned manually. 
- Code is cleaner due to just reference instead of whole object creation in the operation.
- Same failure can be reused in multiple places and with IDE usage can be easy tracked. 
- Error handling logic is more readible without multiple hardcoded strings (no "magic string"):

```csharp
if(result.Errors.Contains(ParsingFailures.FailureWhenParsingTag)	
	SomeHandlingLogic();
```

From disadvantages: 
- more code and celebration around errors. 

## Result usage

There are two different result types: 
- Result - represents result from void function when there is no return value just 
information that operaation completed sucessfully or not. 
- ObjectResult\<T> - represents result from function with returning value. T is type of the returning value. 

Both result types works similarly, differences are: additional Value property in ObjectResult, 
optional value override when merging two ObjectResult\<T> with MergeIn method and couple of differences with 
factory methods used for initialization.

### Result

Properties:
- Errors - collection of failures with ```FailureType``` set to ```FailureType.Error```
- Warnnings - collection of failures with ```FailureType``` set to ```FailureType.Warning```
- IsSuccess - is true only when there is no failure in errors AND warnings. 
- IsFailure - !IsSuccess, true if there is any failure in errors OR warnings.
- HasWarnings - is true when Warnings contains any failure.  
- HasErrors - is true when Errors contains any failure.

Methods: 
- ```AddFailure(params Failure[] failures)``` - method to add multiple failures, 
it automatically distinguishes between warnings and errors. Returns this object. 
- ```GetFailures()``` - returns array of failures consisting from bot Erros and Warnings. 
- ```Result MergeIn(Result externalResult)``` - combines warnings and errors from externalResult
to the caller and returns it. 

Factory methods for Result:
- ```Result Success()``` - returns Result without any errors. 
- ```Result Empty()``` - returns Result without any errors. 
- ```Result Fail(params Failure[] failures)``` - return Result with puplated Errors and Warnings props based 
on provided failures parameters. Throws if failures array is empty.

There are also implicit conversion from `Failure` and `Failure[]` to Result with provided failures.

### ObjectResult\<T>

Properties:
- Errors - collection of failures with ```FailureType``` set to ```FailureType.Error```
- Warnnings - collection of failures with ```FailureType``` set to ```FailureType.Warning```
- IsSuccess - is true only when there is no failure in errors AND warnings. 
- IsFailure - !IsSuccess, true if there is any failure in errors OR warnings.
- HasWarnings - is true when Warnings contains any failure.  
- HasErrors - is true when Errors contains any failure.
- Value - property of type T, can be null. 

Methods: 
- ```AddFailure(params Failure[] failures)``` - method to add multiple failures, 
it automatically distinguishes between warnings and errors. Returns this object. 
- ```GetFailures()``` - returns array of failures consisting from bot Erros and Warnings. 
- ```ObjectResul<T> MergeIn(ObjectResul<T> externalResult, bool overrideValue = false)``` - combines warnings 
and errors from externalResult to the caller. Optionally if overrideValue is set to true caller 
value will be overriden by the value from externalResult.

Factory methods for Result:
- ```ObjectResult<T> Success(T value)``` - returns ObjectResult\<T> without any failures and assign value to Value property. 
- ```ObjectResult<T> Empty(T? value = default)``` - returns ObjectResult\<T> without any failures and optionally 
assign value to Value property. 
- ```ObjectResult<T> Fail(params Failure[] failures)``` - return ObjectResult\<T> with puplated Errors and Warnings props based 
on provided failures parameters. Throws if failures array is empty.

There are also implicit conversion from `Failure` and `Failure[]` to ObjectResult\<T> with provided failures, and 
from type T to ObjectResult\<T> without any errors and warnings. 